using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// The list of logic events
/// </summary>
public enum CameraEvents
{
	None=0,

	ShootOne = 10,
	ShootTwo = 11,
	ShootThree = 12,

}

public class CameraEvent : MonoBehaviour {

	/// <summary>
	/// Event handler. handle the event with basic arg
	/// </summary>
	public delegate void EventHandler(BasicArg arg);

	/// <summary>
	/// an example for the normal event
	/// </summary>
//	public static event EventHandler StartApp;
//	public static void FireStartApp(BasicArg arg){if ( StartApp != null ) StartApp(arg) ; }


	/// <summary>
	/// Handler specified for dealing with the logic events
	/// </summary>
	public delegate void CameraHandler( CameraArg arg );

	/// <summary>
	/// The list of logic events, we assum that the number of total events is less than 9999
	/// </summary>
	//	public static CameraHandler[] logicEvents = new CameraHandler[System.Enum.GetNames (typeof (CameraEvents)).Length];
	public static CameraHandler[] cameraEvents = new CameraHandler[9999];

	/// <summary>
	/// A static interface to fire the logic events, without specified type
	/// </summary>
	/// <param name="arg">Argument.</param>
	public static void FireLogicEvent( CameraArg arg )
	{
		if (arg.type != CameraEvents.None) {
			FireLogicEvent (arg.type, arg);
		}
	}
	/// <summary>
	/// a static interface to fire the logic events, with specified type
	/// </summary>
	/// <param name="type">Type.</param>
	/// <param name="arg">Argument.</param>
	public static void FireLogicEvent( CameraEvents type , CameraArg arg )
	{
		if ( cameraEvents[(int)type] != null )
		{
			arg.type = type;
			cameraEvents [(int)type] ( arg );
		}

	}

	/// <summary>
	/// Registers the event according to the event type
	/// </summary>
	/// <param name="type">Type.</param>
	/// <param name="handler">Handler.</param>
	public static void RegisterEvent( CameraEvents type , CameraHandler handler )
	{
		cameraEvents [(int)type] += handler;
	}

	/// <summary>
	/// Unregisters the event.
	/// </summary>
	/// <param name="type">Type.</param>
	/// <param name="handler">Handler.</param>
	public static void UnregisterEvent( CameraEvents type , CameraHandler handler )
	{
		cameraEvents [(int)type] -= handler;
	}

	/// <summary>
	/// Register the handler function to all events
	/// </summary>
	/// <param name="handler">Handler.</param>
	public static void RegisterAll( CameraHandler handler )
	{
		for (int i = 0; i < cameraEvents.Length; ++i)
			cameraEvents [i] += handler;
	}

	/// <summary>
	/// Unregister the handler to all events
	/// </summary>
	/// <param name="handler">Handler.</param>
	public static void UnRegisterAll ( CameraHandler handler )
	{
		for (int i = 0; i < cameraEvents.Length; ++i)
			cameraEvents [i] -= handler;
	}



}

/// <summary>
/// Logic argument.
/// </summary>
public class CameraArg : MsgArg
{
	public CameraArg(object _this):base(_this){}

	/// <summary>
	/// The type of the arg.
	/// </summary>
	public CameraEvents type;
}