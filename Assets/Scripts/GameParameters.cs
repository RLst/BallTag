using System;
using UnityEngine;

namespace LeMinhHuy
{
	[CreateAssetMenu]
	public class GameParameters : ScriptableObject
	{
		public bool isARMode = false;
		public float fieldWidth = 20f;

		//Round
		public int roundsPerMatch = 5;
		public float startingRoundRemainingTime = 140f;

		//Energy
		public float maxEnergy;

		[Header("Strategies")]
		public Strategy offensiveStrategy;
		public Strategy defensiveStrategy;

		[Header("Match Settings")]
		//These are the definite settings as chosen by the user has selected
		//This might allow for a cutscene to play in the background CPU vs CPU
		public TeamSettings teamOneSettings;
		public TeamSettings teamTwoSettings;
	}

	[Serializable]
	public class TeamSettings
	{
		public Color color;
		public UserType userType;
		public Stance stance;
	}
}