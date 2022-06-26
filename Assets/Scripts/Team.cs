
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LeMinhHuy
{
	[Serializable]
	public struct Energy
	{
		public float current;
		public float downtime;
	}

	[Serializable]
	public class Team
	{
		//Inspector
		// [field: SerializeField] public float currentEnergy { get; set; }
		// [field: SerializeField] public float currentDowntime { get; set; }

		//Inspector
		public Color color;
		public UserType userType;
		public Stance stance;
		public Energy energy;
		public Vector3 attackDirection;

		[Header("Team Objects")]
		public Field field;
		public Goal goal;
		public Fence[] fences;

		//Pool; eliminate garbage allocation
		public Pool<Player> playerPool = new Pool<Player>();

		//Stats
		public int roundsWon { get; set; }
		public int catches { get; set; }
		public int passes { get; set; }
		public int outs { get; set; }    //Team members that got caught

		//Members
		GameSettings gs;

		//Core
		void Awake()
		{
			//Cache game settings from umpire
			gs = Umpire.current.gameSettings;
		}

		public void Initialise(User user)
		{
			//Init starting team data
			energy = new Energy
			{
				current = 0,
				downtime = 0,
			};

			//Preload object pool. Create players, setup, disable and put into pool
			int initPlayersInPool = 5;
			for (int i = 0; i < initPlayersInPool; i++)
			{
				var player = GameObject.Instantiate<Player>(gs.genericPlayerPrefab, field.parent.transform);
				player.team = this;
				playerPool.Release(player);
			}
		}

		void Update()
		{
			HandleDowntime();
		}

		void HandleDowntime()
		{
			if (energy.downtime > 0)
			{
				energy.downtime -= gs.downtimeRecoveryRate * Time.deltaTime;
			}
		}

		public void StartRound(Stance stance)
		{
			//Reset all stats
			//Set stance
			//Clear all players off the field
		}

		//Spawn player at specific location, facing toward the opposite team
		public void SpawnPlayer(Vector3 location)
		{
			var spawn = playerPool.Get();
			spawn.transform.SetPositionAndRotation(location, Quaternion.LookRotation(attackDirection, Vector3.up));
		}

		//Despawn and put back into the object pool
		public void DespawnPlayer(Player player)
		{
			playerPool.Release(player);
		}
	}
}