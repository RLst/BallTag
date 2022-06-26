using UnityEngine;

namespace LeMinhHuy
{
	[CreateAssetMenu]
	public class GameParameters : ScriptableObject
	{
		public Unit genericUnitPrefab;

		public bool isARMode = false;

		public int roundsPerMatch = 5;
		public float startRoundTime = 140f;
		public float startEnergy;

		public UnitParameters offenseParams;
		public UnitParameters defenceParams;
	}
}