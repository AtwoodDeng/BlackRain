using System;
using UnityEngine;

namespace CF.CameraBot
{
    [Serializable]
    public class Zoom : ICloneable
    {
		public object Clone()
		{
			return new Zoom();
		}
	}
}
