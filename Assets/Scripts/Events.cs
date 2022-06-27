using System;
using UnityEngine;
using UnityEngine.Events;

namespace LeMinhHuy.Events
{
	[Serializable] public class PointInputEvent : UnityEvent<Vector2> { }  //Passes through the tap/click location
	[Serializable] public class FloatEvent : UnityEvent<float> { }  //Passes through the tap/click location
}
