using UnityEngine;
using System.Collections;

[ExecuteInEditMode] // Make mirror live-update even when not in play mode
public class T_Water : MonoBehaviour
{
	#region Variables
	public bool m_DisablePixelLights = true;

	public enum Quality
	{
		High,
		Mid,
		Low,
	}
	public Quality quality = Quality.Low;
	public int m_TextureSize{
		get {
			switch (quality) {
			case Quality.High:
				return 2048;
			case Quality.Mid:
				return 1024;
			case Quality.Low:
				return 512;
			default:
				break;
			}
			return 256;
		}
	}

	public float m_ClipPlaneOffset = 0.07f;

	public LayerMask m_LayerMask = -1;

	private Hashtable m_ReflectionCameras = new Hashtable(); // Camera -> Camera table
	private Hashtable m_RefractionCamera = new Hashtable(); // Camera -> Camera table

	private RenderTexture m_ReflectionTexture = null;
	private int m_OldReflectionTextureSize = 0;

	private RenderTexture m_RefractionTexture = null;
	private int m_OldRefractionTextureSize = 0;

	private static bool s_InsideRendering = false;

	public void OnWillRenderObject()
	{
		if( !enabled || !GetComponent<Renderer>() || !GetComponent<Renderer>().sharedMaterial || !GetComponent<Renderer>().enabled )
			return;

		gameObject.layer = LayerMask.NameToLayer ("Water");

		Camera cam = Camera.current;
		if( !cam )
			return;

		// Safeguard from recursive reflections.        
		if( s_InsideRendering )
			return;
		s_InsideRendering = true;

		RenderReflectCamera (cam);
		RenderRefractionCamera (cam);

		s_InsideRendering = false;
	}


	#endregion

	void RenderRefractionCamera( Camera cam )
	{
		Camera refractionCamera;
		CreateRefractionObjects (cam, out refractionCamera);

		// Optionally disable pixel lights for reflection
		int oldPixelLightCount = QualitySettings.pixelLightCount;
		if( m_DisablePixelLights )
			QualitySettings.pixelLightCount = 0;

		UpdateCameraModes (cam, refractionCamera);

		// Render refraction
		// the refraction camera plane is the same as cam
		refractionCamera.worldToCameraMatrix = cam.worldToCameraMatrix;


		// Setup oblique projection matrix so that near plane is our reflection
		// plane. This way we clip everything below/above it for free.
//		Vector4 clipPlane = CameraSpacePlane( refractionCamera, pos, normal, 1.0f );
//		Matrix4x4 projection = cam.projectionMatrix;
//		CalculateObliqueMatrix (ref projection, clipPlane);
		refractionCamera.projectionMatrix = cam.projectionMatrix;

		refractionCamera.cullingMask = ~(1<<LayerMask.NameToLayer ("Water")) & m_LayerMask.value; // never render water layer
		refractionCamera.targetTexture = m_RefractionTexture;


//		GL.SetRevertBackfacing (true);    //obsolete
		GL.invertCulling = false;
		refractionCamera.transform.position = cam.transform.position;
//		Vector3 euler = cam.transform.eulerAngles;
//		refractionCamera.transform.eulerAngles = new Vector3(0, euler.y, euler.z);
		refractionCamera.transform.rotation = cam.transform.rotation;
		refractionCamera.Render();
//		refractionCamera.transform.position = cam.transform.position;
		//GL.invertCulling = true;        //should be used but inverts the faces of the terrain
//		GL.SetRevertBackfacing (false);   //obsolete


		// set the refraction tex to the materials
		Material[] materials = GetComponent<Renderer>().sharedMaterials;
		foreach( Material mat in materials ) {
			if( mat.HasProperty("_RefractionTex") )
				mat.SetTexture( "_RefractionTex", m_RefractionTexture );
		}


		// Restore pixel light count
		if( m_DisablePixelLights )
			QualitySettings.pixelLightCount = oldPixelLightCount;
		
	}

	/// <summary>
	/// set up the reflect camera and render the reflect image 
	/// save to _ReflectionTex
	/// </summary>
	/// <param name="cam">the current cameaa.</param>
	void RenderReflectCamera( Camera cam )
	{
		Camera reflectionCamera;
		CreateMirrorObjects( cam, out reflectionCamera );

		// find out the reflection plane: position and normal in world space
		Vector3 pos = transform.position;
		Vector3 normal = transform.up;

		// Optionally disable pixel lights for reflection
		int oldPixelLightCount = QualitySettings.pixelLightCount;
		if( m_DisablePixelLights )
			QualitySettings.pixelLightCount = 0;

		UpdateCameraModes( cam, reflectionCamera );

		// Render reflection
		// Reflect camera around reflection plane
		float d = -Vector3.Dot (normal, pos) - m_ClipPlaneOffset;
		Vector4 reflectionPlane = new Vector4 (normal.x, normal.y, normal.z, d);

		Matrix4x4 reflection = Matrix4x4.zero;
		CalculateReflectionMatrix (ref reflection, reflectionPlane);
		Vector3 oldpos = cam.transform.position;
		Vector3 newpos = reflection.MultiplyPoint( oldpos );
		reflectionCamera.worldToCameraMatrix = cam.worldToCameraMatrix * reflection;

		// Setup oblique projection matrix so that near plane is our reflection
		// plane. This way we clip everything below/above it for free.
		Vector4 clipPlane = CameraSpacePlane( reflectionCamera, pos, normal, 1.0f );
		Matrix4x4 projection = cam.projectionMatrix;
		CalculateObliqueMatrix (ref projection, clipPlane);
		reflectionCamera.projectionMatrix = projection;

		reflectionCamera.cullingMask = ~(1<<LayerMask.NameToLayer ("Water")) & m_LayerMask.value; // never render water layer
		reflectionCamera.targetTexture = m_ReflectionTexture;
		//GL.invertCulling = true;        //should be used but inverts the faces of the terrain
		//		GL.SetRevertBackfacing (true);    //obsolete
		GL.invertCulling = true;
		reflectionCamera.transform.position = newpos;
		Vector3 euler = cam.transform.eulerAngles;
		reflectionCamera.transform.eulerAngles = new Vector3(0, euler.y, euler.z);
		reflectionCamera.Render();
		reflectionCamera.transform.position = oldpos;
		//GL.invertCulling = true;        //should be used but inverts the faces of the terrain
		//		GL.SetRevertBackfacing (false);   //obsolete
		GL.invertCulling = false;
		Material[] materials = GetComponent<Renderer>().sharedMaterials;
		foreach( Material mat in materials ) {
			if( mat.HasProperty("_ReflectionTex") )
				mat.SetTexture( "_ReflectionTex", m_ReflectionTexture );
		}

		// Set matrix on the shader that transforms UVs from object space into screen
		// space. We want to just project reflection texture on screen.
		Matrix4x4 scaleOffset = Matrix4x4.TRS(
			new Vector3(0.5f,0.5f,0.5f), Quaternion.identity, new Vector3(0.5f,0.5f,0.5f) );
		Vector3 scale = transform.lossyScale;
		Matrix4x4 mtx = transform.localToWorldMatrix * Matrix4x4.Scale( new Vector3(1.0f/scale.x, 1.0f/scale.y, 1.0f/scale.z) );
		mtx = scaleOffset * cam.projectionMatrix * cam.worldToCameraMatrix * mtx;
		foreach( Material mat in materials ) {
			mat.SetMatrix( "_ProjMatrix", mtx );
		}

		// Restore pixel light count
		if( m_DisablePixelLights )
			QualitySettings.pixelLightCount = oldPixelLightCount;
	}

	//<summary>
	// Cleans up all the objects that were possibly created
	//</summary>
	void OnDisable()
	{
		if( m_ReflectionTexture ) {
			DestroyImmediate( m_ReflectionTexture );
			m_ReflectionTexture = null;
		}
		foreach( DictionaryEntry kvp in m_ReflectionCameras )
			DestroyImmediate( ((Camera)kvp.Value).gameObject );
		m_ReflectionCameras.Clear();
	}

	private void UpdateCameraModes( Camera src, Camera dest )
	{
		if( dest == null )
			return;

		//sets camera to clear the same way as current camera
		dest.clearFlags = src.clearFlags;
		dest.backgroundColor = src.backgroundColor; 

		if( src.clearFlags == CameraClearFlags.Skybox )
		{
			Skybox sky = src.GetComponent(typeof(Skybox)) as Skybox;
			Skybox mysky = dest.GetComponent(typeof(Skybox)) as Skybox;
			if( !sky || !sky.material )
			{
				mysky.enabled = false;
			}

			else
			{
				mysky.enabled = true;
				mysky.material = sky.material;
			}
		}

		///<summary>
		///Updates other values to match current camera.
		///Even if camera&projection matrices are supplied, some of values are used elsewhere (e.g. skybox uses far plane)
		/// </summary>
		dest.farClipPlane = src.farClipPlane;
		dest.nearClipPlane = src.nearClipPlane;
		dest.orthographic = src.orthographic;
		dest.fieldOfView = src.fieldOfView;
		dest.aspect = src.aspect;
		dest.orthographicSize = src.orthographicSize;
	}

	//<summary>
	//Creates any objects needed on demand
	//</summary>
	private void CreateMirrorObjects( Camera currentCamera, out Camera reflectionCamera )
	{
		reflectionCamera = null;

		//Reflection render texture
		if( !m_ReflectionTexture || m_OldReflectionTextureSize != m_TextureSize )
		{
			if( m_ReflectionTexture )
				DestroyImmediate( m_ReflectionTexture );
			m_ReflectionTexture = new RenderTexture( m_TextureSize, m_TextureSize, 16 );
			m_ReflectionTexture.name = "__MirrorReflection" + GetInstanceID();
			m_ReflectionTexture.isPowerOfTwo = true;
			m_ReflectionTexture.hideFlags = HideFlags.DontSave;
			m_OldReflectionTextureSize = m_TextureSize;
		}

		//Camera for reflection
		reflectionCamera = m_ReflectionCameras[currentCamera] as Camera;
		if( !reflectionCamera ) // catch both not-in-dictionary and in-dictionary-but-deleted-GO
		{
			GameObject go = new GameObject( "Mirror Refl Camera id" + GetInstanceID() + " for " + currentCamera.GetInstanceID(), typeof(Camera), typeof(Skybox) );
			reflectionCamera = go.GetComponent<Camera>();
			reflectionCamera.enabled = false;
			reflectionCamera.transform.position = transform.position;
			reflectionCamera.transform.rotation = transform.rotation;
			reflectionCamera.depthTextureMode |= DepthTextureMode.DepthNormals;
			reflectionCamera.gameObject.AddComponent<FlareLayer>();
			go.hideFlags = HideFlags.HideAndDontSave;
			m_ReflectionCameras[currentCamera] = reflectionCamera;
		}        
	}

	//<summary>
	// Create refraction Objects on demend
	//</summary>
	private void CreateRefractionObjects( Camera currentCamera, out Camera refractionCamera )
	{
		refractionCamera = null;

		//Reflection render texture
		if( !m_RefractionTexture || m_OldRefractionTextureSize != m_TextureSize )
		{
			if( m_RefractionTexture )
				DestroyImmediate( m_RefractionTexture );
			m_RefractionTexture = new RenderTexture( m_TextureSize, m_TextureSize, 16 );
			m_RefractionTexture.name = "__MirrorRefraction" + GetInstanceID();
			m_RefractionTexture.isPowerOfTwo = true;
			m_RefractionTexture.hideFlags = HideFlags.DontSave;
			m_OldRefractionTextureSize = m_TextureSize;
		}

		//Camera for reflection
		refractionCamera = m_RefractionCamera[currentCamera] as Camera;
		if( !refractionCamera ) // catch both not-in-dictionary and in-dictionary-but-deleted-GO
		{
			GameObject go = new GameObject( "Mirror Refra Camera id" + GetInstanceID() + " for " + currentCamera.GetInstanceID(), typeof(Camera), typeof(Skybox) );
			refractionCamera = go.GetComponent<Camera>();
			refractionCamera.enabled = false;
			refractionCamera.transform.position = transform.position;
			refractionCamera.transform.rotation = transform.rotation;
			refractionCamera.gameObject.AddComponent<FlareLayer>();
			refractionCamera.depthTextureMode |= DepthTextureMode.DepthNormals;
			go.hideFlags = HideFlags.HideAndDontSave;
			m_RefractionCamera[currentCamera] = refractionCamera;
		}        
	}

	//<summary>
	//Extended sign: returns -1, 0 or 1 based on sign of a
	//</summary>
	private static float sgn(float a)
	{
		if (a > 0.0f) return 1.0f;
		if (a < 0.0f) return -1.0f;
		return 0.0f;
	}

	//<summary>
	//Given position/normal of the plane, calculates plane in camera space.
	//</summary>
	private Vector4 CameraSpacePlane (Camera cam, Vector3 pos, Vector3 normal, float sideSign)
	{
		Vector3 offsetPos = pos + normal * m_ClipPlaneOffset;
		Matrix4x4 m = cam.worldToCameraMatrix;
		Vector3 cpos = m.MultiplyPoint( offsetPos );
		Vector3 cnormal = m.MultiplyVector( normal ).normalized * sideSign;
		return new Vector4( cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos,cnormal) );
	}

	//<summary>
	//Adjusts the given projection matrix so that near plane is the given clipPlane
	//clipPlane is given in camera space
	//</summary>
	private static void CalculateObliqueMatrix (ref Matrix4x4 projection, Vector4 clipPlane)
	{
		Vector4 q = projection.inverse * new Vector4(
			sgn(clipPlane.x),
			sgn(clipPlane.y),
			1.0f,
			1.0f
		);
		Vector4 c = clipPlane * (2.0F / (Vector4.Dot (clipPlane, q)));
		// third row = clip plane - fourth row
		projection[2] = c.x - projection[3];
		projection[6] = c.y - projection[7];
		projection[10] = c.z - projection[11];
		projection[14] = c.w - projection[15];
	}

	//<summary>
	//Calculates reflection matrix around the given plane
	//</summary>
	private static void CalculateReflectionMatrix (ref Matrix4x4 reflectionMat, Vector4 plane)
	{
		reflectionMat.m00 = (1F - 2F*plane[0]*plane[0]);
		reflectionMat.m01 = (   - 2F*plane[0]*plane[1]);
		reflectionMat.m02 = (   - 2F*plane[0]*plane[2]);
		reflectionMat.m03 = (   - 2F*plane[3]*plane[0]);

		reflectionMat.m10 = (   - 2F*plane[1]*plane[0]);
		reflectionMat.m11 = (1F - 2F*plane[1]*plane[1]);
		reflectionMat.m12 = (   - 2F*plane[1]*plane[2]);
		reflectionMat.m13 = (   - 2F*plane[3]*plane[1]);

		reflectionMat.m20 = (   - 2F*plane[2]*plane[0]);
		reflectionMat.m21 = (   - 2F*plane[2]*plane[1]);
		reflectionMat.m22 = (1F - 2F*plane[2]*plane[2]);
		reflectionMat.m23 = (   - 2F*plane[3]*plane[2]);

		reflectionMat.m30 = 0F;
		reflectionMat.m31 = 0F;
		reflectionMat.m32 = 0F;
		reflectionMat.m33 = 1F;
	}
}
