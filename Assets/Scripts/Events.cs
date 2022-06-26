using System;
using UnityEngine;
using UnityEngine.Events;

namespace LeMinhHuy.Events
{
	[Serializable] public class TapEvent : UnityEvent<Vector2> { }  //Passes through the tap location
}