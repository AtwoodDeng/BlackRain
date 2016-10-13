using UnityEngine;
using System;

namespace CF.CameraBot
{
    #region Enum
    public enum UpwardReferenceMethod
    {
        World = 0,
        Local,
		RealLocal,
        Custom
    }
    #endregion

    [Serializable]
    public class ClampAngle : ICloneable
    {
        // Availiable angle
        public Transform m_ForwardReference;
        public UpwardReferenceMethod m_UpwardReferenceMethod;
        public Transform m_UpReference;
        
		public object Clone()
		{
			return new ClampAngle()
			{
				m_ForwardReference = this.m_ForwardReference,
				m_UpwardReferenceMethod = this.m_UpwardReferenceMethod,
				m_UpReference = this.m_UpReference,
			};
		}
	}
}
