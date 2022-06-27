
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using LeMinhHuy.Events;

namespace LeMinhHuy
{
	[Serializable]
	public class Team
	{
		//Inspector
		// [field: SerializeField] public float currentEnergy { get; set; }
		// [field: SerializeField] public float currentDowntime { get; set; }
		const float RAYCAST_MAXDISTANCE = 100f;

		//Inspector
		public Color color;
		public UserType userType;
		public Strategy strategy;

		public float energy;
		public float recoveryTime;
		public Vector3 attackDirection;

		[Header("Team Objects")]
		public Field field;
		public Goal goal;
		public Fence[] fences;

		//Pool; eliminate garbage allocation
		int startingUnitsToPool = 5;
		Pool<Unit> unitPool;
		List<Unit> units = new List<Unit>();

		List<ARRaycastHit> arHitResults;

		//Events
		public FloatEvent onEnergyChange;

		//Stats
		public int roundsWon { get; set; }
		public int roundDraw { get; set; }
		public int roundLost { get; set; }
		public int opponentsCaught { get; set; }
		public int membersCaught { get; set; }    //Team members that got caught
		public int ballPasses { get; set; }

		//Members
		GameController game;
		ARRaycastManager arRaycastManager;


		//Initialise the team and do awake/start stuff
		public void Initialise(TeamSettings ts)
		{
			//AWAKE; Cache from monobehaviour umpire
			this.game = GameController.current;
			arRaycastManager = this.game.arRaycastManager;

			//ONENABLE; Register events
			UserInput.current.onScreenPosInput.AddListener(TrySpawnUnitAtScreenPoint);

			//Set core team parameters
			this.color = ts.color;
			this.userType = ts.userType;
			switch (ts.stance)
			{
				case Stance.Offensive:
					this.strategy = game.parameters.offensiveStrategy;
					break;
				case Stance.Defensive:
					this.strategy = game.parameters.defensiveStrategy;
					break;
			}

			//Create first before
			PrepareObjectPool();
		}

		//POOLING
		void PrepareObjectPool()
		{
			unitPool = new Pool<Unit>(SpawnUnit, OnGetUnit, OnRecycleUnit);
			//Preload pool
			for (int i = 0; i < startingUnitsToPool; i++)
			{
				//Create and add to total list
				var unit = unitPool.Get();
				units.Add(unit);
				//Recycle back into pool
				unitPool.Recycle(unit);
			}
		}

		//POOLING CALLBACKS
		Unit SpawnUnit()
		{
			var unit = GameObject.Instantiate<Unit>(game.genericUnitPrefab, field.parent.transform);
			unit.Init(this);    //Sets color etc
			return unit;
		}
		void OnGetUnit(Unit unit)
		{
			//Spend energy
			energy -= strategy.spawnEnergyCost;

			unit.Show();

			//Maybe set the unit in motion?
			//Set it's AI
		}
		void OnRecycleUnit(Unit unit)
		{
			unit.Hide();
		}

		//CORE
		void Update()
		{
			HandleEnergy();
			HandleDowntime();
		}

		void HandleDowntime()
		{
			if (recoveryTime > 0)
				recoveryTime -= Time.deltaTime;
		}
		void HandleEnergy()
		{
			if (energy < game.parameters.maxEnergy)
				energy += Time.deltaTime * strategy.energyRegenRate;
		}


		//SPAWN
		public void TrySpawnUnitAtScreenPoint(Vector2 screenPoint)
		{
			//Reject if there's not enough energy
			if (energy < strategy.spawnEnergyCost)
				return;

			//Reject user input if the team is computer controlled
			if (userType == UserType.CPU)
				return;

			//Raycast to point
			if (game.parameters.isARMode)
			{
				//AR Raycast
				if (arRaycastManager.Raycast(screenPoint, arHitResults, UnityEngine.XR.ARSubsystems.TrackableType.Planes))
				{
					TrySpawnUnitOnField(arHitResults[0].pose.position);
				}
			}
			else
			{
				//Normal raycast
				var ray = Camera.main.ScreenPointToRay(screenPoint);
				if (Physics.Raycast(ray, out RaycastHit hit, RAYCAST_MAXDISTANCE))
				{
					TrySpawnUnitOnField(hit.point);
				}
			}
		}

		//Spawn player at specific location, facing toward the opposite team
		public bool TrySpawnUnitOnField(Vector3 positionOnField)
		{
			//Reject if there's not enough energy
			if (energy < strategy.spawnEnergyCost)
				return false;

			//Point must be on field
			if (!field.isPosWithinField(positionOnField))
			{
				return false;
			}

			//Success
			//Get from pool and init
			var spawn = unitPool.Get();
			spawn.transform.SetPositionAndRotation(positionOnField, Quaternion.LookRotation(attackDirection, Vector3.up));
			return true;
		}
		public void SpawnUnitOnField(Vector3 positionOnField)
		{
			//Make sure spawn point is on the field
			if (!field.isPosWithinField(positionOnField))
			{
				Debug.LogWarning("Out of bounds!");
				return;
			}

			//Get from pool and init
			var spawn = unitPool.Get();
			spawn.transform.SetPositionAndRotation(positionOnField, Quaternion.LookRotation(attackDirection, Vector3.up));
		}

		//Despawn and put back into the object pool
		public void DespawnUnit(Unit unit)
		{
			unitPool.Recycle(unit);
		}

		public void DespawnAllUnits()
		{
			foreach (var unit in units)
				unitPool.Recycle(unit);
		}
	}
}