using System;

namespace LeMinhHuy
{
	[Serializable]
	public class UnitParameters
	{
		public float energyRegenRate = 0.5f;
		public float AISpawnRate = 0.4f;
		public float energyCost = 2f;
		public float spawnTime = 0.5f;
		public float reactivationTime = 2.5f;
		public float normalSpeed = 1.5f;
		public float carryingSpeed = 0.75f;
		public float ballSpeed = 1.5f;
		public float returnSpeed = 2f;
		public float detectionRange = 0.35f;
	}
}