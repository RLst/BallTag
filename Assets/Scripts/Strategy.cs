using System;
using UnityEngine;

namespace LeMinhHuy
{
	[Serializable]
	public class Strategy
	{
		public Stance stance = Stance.Offensive;

		//Energy
		[Tooltip("The amount of energy that gets regenerated per second")]
		public float energyRegenRate = 0.5f;
		[Tooltip("The amount of energy required to spawn a unit")]
		public float spawnCost = 2f;
		[Tooltip("Time it takes to spawn and fully activate a unit after clicking on the field")]
		public float spawnTime = 0.5f;

		//AI
		// [Tooltip("The rate at which the AI attempts to spawn a unit")]
		// public float AISpawnRate = 1f;
		[Tooltip("Chance of the AI being able to spawn")]
		public float AISpawnChance = 0.4f;

		[Tooltip("Downtime when the unit has tagged opponent or been caught by opponent")]
		public float reactivationTime = 2.5f;

		//Unit speeds
		public float baseSpeed = 5f;
		public float normalSpeedMult = 1.5f;
		public float carryingSpeedMult = 0.75f;
		public float ballSpeedMult = 1.5f;
		public float returnSpeedMult = 2f;
		public float detectionRange = 0.35f;

		//Other
		public float minSpawnDistanceFromOwnGoal = 2f;
	}
}