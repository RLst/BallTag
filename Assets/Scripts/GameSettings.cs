using UnityEngine;

namespace LeMinhHuy
{
	[CreateAssetMenu]
	public class GameSettings : ScriptableObject
	{
		public User playerOne;
		public User playerTwo;

		public float spawnCost = 3;
		public float energyRegenRate = 1;
		public float downtime = 4;
		public float downtimeRecoveryRate = 1;

		public float playerSuccessfulCaughtDowntime = 5f;

		public Player genericPlayerPrefab;
		
	}
}