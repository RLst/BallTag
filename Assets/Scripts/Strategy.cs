using System;

namespace LeMinhHuy
{
	[Serializable]
	public class Strategy
	{
		public Stance stance = Stance.Offensive;

		//Energy
		public float energyRegenRate = 0.5f;
		public float spawnEnergyCost = 2f;
		public float spawnDowntime = 0.5f;      //Time between attempts at spawning
		public float AISpawnPercentage = 0.4f;

		//Downtime when unit has been caught or has caught
		public float reactivationTime = 2.5f;

		//Unit
		public float baseSpeed = 5f;
		public float normalSpeedMult = 1.5f;
		public float carryingSpeedMult = 0.75f;
		public float ballSpeedMult = 1.5f;
		public float returnSpeedMult = 2f;
		public float detectionRange = 0.35f;
	}
}