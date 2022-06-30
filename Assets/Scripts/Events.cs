using System;
using UnityEngine;
using UnityEngine.Events;
using static LeMinhHuy.Unit;

namespace LeMinhHuy.Events
{
	[Serializable] public class PointInputEvent : UnityEvent<Vector2> { }  //Passes through the tap/click location
	[Serializable] public class FloatEvent : UnityEvent<float> { }  //Passes through the tap/click location
	[Serializable] public class IntEvent : UnityEvent<int> { }  //Passes through the tap/click location
	[Serializable] public class StateEvent : UnityEvent<State> { }  //Passes through the tap/click location
}
