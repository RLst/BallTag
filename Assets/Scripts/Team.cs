
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
		public Color color = Color.blue;
		public UserType userType = UserType.CPU;
		public Strategy strategy;

		public float energy;
		public float recoveryTime;
		public Vector3 attackDirection;

		[Header("Team Objects")]
		public Field field;
		public Goal goal;
		public Fence[] fences;

		//Pool; eliminate garbage allocation
		const int startingUnitsToPool = 5;
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
				default:
					throw new ArgumentException("Invalid stance!");
			}

			//Create first before
			PrepareObjectPool();

			Reset();
		}

		void Reset()
		{
			energy = 0;
			recoveryTime = -1;
			DespawnAllUnits();
		}

		//POOLING
		void PrepareObjectPool()
		{
			if (unitPool is null)
			{
				//Create and preload a new obj pool if there wasn't one already
				unitPool = new Pool<Unit>(SpawnUnit, OnGetUnit, OnRecycleUnit);

				//Preload pool
				for (int i = 0; i < startingUnitsToPool; i++)
				{
					//Create unit, add to full unit list, push into pool
					var u = unitPool.Get();
					units.Add(u);
				}
			}
			else if (units.Count > 0 && unitPool.countAll > 0)
			{
				//If there's already a pool from a previous game then reset team parameters, colors etc
				foreach (var u in units)
				{
					u.SetTeam(this);
					u.Hide();
				}
			}
		}

		//POOLING CALLBACKS
		Unit SpawnUnit()
		{
			var unit = GameObject.Instantiate<Unit>(game.genericUnitPrefab, field.transform);
			unit.SetTeam(this);    //Sets color etc
			return unit;
		}
		void OnGetUnit(Unit unit)
		{
			unit.Show();
			//Maybe set the unit in motion?
			//Set it's AI
		}
		void OnRecycleUnit(Unit unit)
		{
			unit.Hide();
		}

		//SPAWN
		public void TrySpawnUnitAtScreenPoint(Vector2 screenPoint)
		{
			//GUARDS
			//Reject if there's not enough energy
			if (energy < strategy.spawnEnergyCost)
				return;
			//Reject user input if CPU controlled team
			if (userType == UserType.CPU)
			{
				Debug.Log("Spawn request rejected! CPU controlled team");
				return;
			}

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
					//Must have clicked on this field
					if (hit.collider == this.field.collider)
					{
						TrySpawnUnitOnField(hit.point);
					}
					else
					{
						Debug.LogWarning("Click on wrong field!");
					}
				}
			}
		}

		//SPAWN player at specific location, facing toward the opposite team
		public bool TrySpawnUnitOnField(Vector3 positionOnField)
		{
			//GUARDS
			//Must have enough energy //NOTE: This may be double checked
			if (energy < strategy.spawnEnergyCost)
				return false;
			//Point must be on field
			if (!field.isPosWithinField(positionOnField))
				return false;

			//Success; Get from pool and init
			var spawn = unitPool.Get();
			spawn.transform.SetPositionAndRotation(positionOnField, Quaternion.LookRotation(attackDirection, Vector3.up));
			energy -= strategy.spawnEnergyCost;
			return true;
		}

		//DESPAWN and put back into the object pool
		public void DespawnUnit(Unit unit)
		{
			unitPool.Recycle(unit);
		}
		public void DespawnAllUnits()
		{
			foreach (var u in units)
				DespawnUnit(u);
		}

		//DESTROY
		public void DestroyAllUnits()
		{
			//Untested
			unitPool.Clear();
			//Removing gameobjects from list in reverse to prevent indexing errors
			for (int i = units.Count; i >= 0; i--)
			{
				var deleteMe = units[i];
				units.RemoveAt(i);
				GameObject.Destroy(deleteMe);
			}
		}

		//CORE
		public void HandleDowntime()
		{
			if (recoveryTime > 0)
				recoveryTime -= Time.deltaTime;
		}
		public void HandleEnergy()
		{
			// Debug.Log($"Energy: {energy}, MaxEnergy: {game.parameters.maxEnergy}");
			if (energy < game.parameters.maxEnergy)
			{
				energy += Time.deltaTime * strategy.energyRegenRate;
				onEnergyChange.Invoke(energy);
			}
		}
	}
}