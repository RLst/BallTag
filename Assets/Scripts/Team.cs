
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LeMinhHuy
{
	[Serializable]
	public class Team
	{
		public GameSettings settings;

		[field: SerializeField] public float currentEnergy { get; set; }
		[field: SerializeField] public float currentDowntime { get; set; }

		public List<Player> players = new List<Player>();

		void Update()
		{
			HandleDowntime();
		}

		void HandleDowntime()
		{
			if (currentDowntime > 0)
			{
				currentDowntime -= settings.baseDownTimeRecoveryRate * Time.deltaTime;
				// if (currentDowntime < 0) currentDowntime = 0;
			}
		}
	}
}