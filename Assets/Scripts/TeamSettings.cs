using System;
using UnityEngine;

namespace LeMinhHuy
{
	[Serializable]
	public class TeamSettings
	{
		// [HideInInspector] public Team opponent;
		public string name = "Team name";
		public Color color = Color.blue;
		public UserType userType = UserType.CPU;
		public Stance stance = Stance.Offensive;
	}
}