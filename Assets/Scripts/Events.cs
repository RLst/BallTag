using System;
using UnityEngine;
using UnityEngine.Events;
using static LeMinhHuy.Unit;

namespace LeMinhHuy.Events
{
	[Serializable] public class PointInputEvent : UnityEvent<Vector2> { }  //Passes through the tap/click location
	[Serializable] public class FloatEvent : UnityEvent<float> { }
	[Serializable] public class IntEvent : UnityEvent<int> { }
	[Serializable] public class StateEvent : UnityEvent<State> { }
	[Serializable] public class ResultEvent : UnityEvent<(Team, Result)> { }    //Results of a round
}
