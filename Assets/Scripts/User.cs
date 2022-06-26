using System;
using UnityEngine;

namespace LeMinhHuy
{
	public enum UserType
	{
		Player,     //User controlled
		Computer    //AI controlled
	}

	[Serializable]
	public class User
	{
		public UserType type;
	}
}