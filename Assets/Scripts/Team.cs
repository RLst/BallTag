
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using LeMinhHuy.Events;
using UnityEngine.Events;
using System.Linq;

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
		public string name = "Team Name";
		public Color color = Color.blue;
		public UserType userType = UserType.CPU;
		public Strategy strategy;

		public float energy;
		public Vector3 attackDirection;

		[Space]
		public Field field;
		[SerializeField] TeamObject[] teamObjects = null;
		public Goal goal => teamObjects.First(x => x is Goal) as Goal;

		//Pool; eliminate garbage allocation
		const int startingUnitsToPool = 5;
		Pool<Unit> unitPool;
		List<Unit> units = new List<Unit>();

		//Properties
		internal bool hasBall
		{
			get
			{
				foreach (var u in units)
					if (u.hasBall)
						return true;
				return false;
			}
		}

		//Events
		[Space]
		public FloatEvent onEnergyChange;
		public UnityEvent onScoreGoal;
		// public UnitEvent on

		//Properties
		int? activeUnits => unitPool?.countActive;

		//Members
		GameController gc;
		ARRaycastManager arRaycastManager;
		List<ARRaycastHit> arHitResults = null;
		internal Team opponent;

		#region Stats
		//Maybe make these into a struct
		public int goals { get; private set; }
		public int wins { get; set; }
		public int draws { get; set; }
		public int losses { get; set; }
		public int tags { get; set; }   //Opponents caught
		public int outs { get; set; }    //Team members that got caught
		public int ballPasses { get; set; }
		public int despawns { get; set; }
		void ResetStats()
		{
			//Stats
			goals = 0;
			wins = 0;
			draws = 0;
			losses = 0;
			tags = 0;
			outs = 0;
			ballPasses = 0;
			despawns = 0;

			energy = 0;

			DespawnAllUnits();
		}
		public void ScoreGoal(int amount = 1)
		{
			goals += amount;
			onScoreGoal.Invoke();   //End round etc
		}
		#endregion


		//INITS
		public void Awake()
		{
			this.gc = GameController.current;
			arRaycastManager = this.gc.arRaycastManager;
		}

		public void Initialize(TeamSettings settings)
		{
			Awake();

			//Set team parameters
			this.name = settings.name;
			this.color = settings.color;
			this.userType = settings.userType;
			SetStance();

			InitTeamObjects();

			InitUnitPool();

			ResetStats();

		}
		internal void SetStance(Stance? newStance = null)   //This is for the demo system
		{
			if (newStance is object && newStance.HasValue)
				strategy.stance = newStance.Value;

			switch (strategy.stance)
			{
				case Stance.Offensive:
					this.strategy = gc.settings.offensiveStrategy;
					break;
				case Stance.Defensive:
					this.strategy = gc.settings.defensiveStrategy;
					break;
				default:
					throw new ArgumentException("Invalid stance!");
			}
		}
		internal void InitTeamObjects() //Basically colors the objects
		{
			//Setup team objects and color
			foreach (var to in teamObjects)
				to.SetTeam(this);
		}

		#region Pooling
		void InitUnitPool()
		{
			if (unitPool is object)
				return; //Pool has already been initiated

			unitPool = new Pool<Unit>(CreateUnit, OnGetUnit, OnRecycleUnit);
		}
		Unit CreateUnit()   //THIS IS NOT SPAWNING! It's for preloading
		{
			var u = GameObject.Instantiate<Unit>(gc.unitPrefab, field.transform);
			units.Add(u);
			return u;
		}
		void OnGetUnit(Unit unit)
		{
			energy -= strategy.spawnCost;
			unit.SetTeam(this);    //Sets team, color, strategy
			unit.Spawn();               //Set spawn time so it can do it's spawn sequence
			unit.SetActive(true);
		}
		void OnRecycleUnit(Unit unit)
		{
			//Hide unit, set inactive and reset it's values
			unit.SetActive(false);
			unit.inactive = -1;
		}
		#endregion

		//CORE
		public void Update()    //Since this is not a monobehaviour we're borrowing GC's Update
		{
			handleEnergy();

			void handleEnergy()
			{
				if (energy < gc.settings.maxEnergy)
				{
					energy += Time.deltaTime * strategy.energyRegenRate;
					onEnergyChange.Invoke(energy);

					if (energy > gc.settings.maxEnergy)
						energy = gc.settings.maxEnergy;
				}
			}
		}
		internal void Tick()
		{
			HandleCPULogic();
		}

		#region AI
		void HandleCPULogic()
		{
			//Don't do anything if this team is player controlled
			if (userType == UserType.Player)
				return;
			//Need a strategy
			if (strategy is null)
				return;

			switch (strategy.stance)
			{
				case Stance.Offensive:
					{
						//Try spawning a unit every second at a random location on our home field
						if (UnityEngine.Random.value > strategy.AISpawnChance)
							SpawnUnit(this.field.GetRandomLocationOnField());
					}
					break;

				case Stance.Defensive:
					{
						//If opponent has active units
						//Spawn at a random point on a ray going from our goal to a random opponent attacker
						if (opponent.activeUnits.HasValue && opponent.activeUnits > 0 && opponent.TryGetRandomActiveUnit(out Unit attacker))
						{
							//Find random attacker ^^^^

							//Find ray going from goal to random attacker
							// Debug.Log("Goal: " + goal.name, goal);
							var ray = new Ray(goal.target.position, attacker.transform.position - goal.target.position);
							Debug.DrawRay(ray.origin, ray.direction * 40, Color.red, 10f);

							//Choose a random location that's not too close to our goal and between the attacker
							var randomSpawnPoint = ray.GetPoint(UnityEngine.Random.Range(strategy.minSpawnDistanceFromOwnGoal, Vector3.Distance(goal.target.position, attacker.transform.position)));

							//Clamp within our field
							randomSpawnPoint = this.field.collider.ClosestPoint(randomSpawnPoint);

							//Spawn
							SpawnUnit(randomSpawnPoint);
						}
						//else spawn at a random location based on specific chance
						else
						{
							if (UnityEngine.Random.value > strategy.AISpawnChance)
								SpawnUnit(this.field.GetRandomLocationOnField());
						}
					}
					break;
			}
		}


		//INFO
		bool TryGetRandomActiveUnit(out Unit randomActiveUnit)
		{
			//Exit if there aren't any active units to prevent freeze
			if (unitPool.countActive == 0 || units.Count == 0)
			{
				randomActiveUnit = null;
				return false;
			}

			//Keep trying to find a random active attacker
			do
			{
				randomActiveUnit = units[UnityEngine.Random.Range(0, units.Count)];
			} while (randomActiveUnit is null);

			return (randomActiveUnit is object);
		}
		#endregion

		#region Spawning
		/// <summary>
		/// Spawns a unit at the specified click/touch point on screen
		/// </summary>
		/// <param name="screenPoint">A point on the screen to spawn</param>
		/// <returns>Returns false if unit cannot be spawned or invalid spawn location</returns>
		public void Void_SpawnUnitAtScreenPoint(Vector2 screenPoint)
			=> SpawnUnitAtScreenPoint(screenPoint);
		public bool SpawnUnitAtScreenPoint(Vector2 screenPoint)
		{
			//GUARDS
			//Reject if there's not enough energy; Check early for slight optimization
			if (energy < strategy.spawnCost)
				return false;
			//Reject user input if CPU controlled team
			if (userType == UserType.CPU)
			{
				Debug.Log("Spawn request rejected! CPU controlled team");
				return false;
			}

			//Raycast to point
			if (gc.isARMode)
			{
				//AR Raycast
				if (arRaycastManager.Raycast(screenPoint, arHitResults, UnityEngine.XR.ARSubsystems.TrackableType.Planes))
				{
					return SpawnUnit(arHitResults[0].pose.position);
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
						return SpawnUnit(hit.point);
					}

					//Probably clicked on your opponent's field
					// Debug.LogWarning("Click on wrong field!");
				}
			}
			return false;
		}

		//SPAWN player at specific location, facing toward the opposite team
		public bool SpawnUnit(Vector3 point)
		{
			//GUARDS
			//Must have enough energy //NOTE: This may be double checked
			if (energy < strategy.spawnCost)
				return false;
			//Point must be on field
			if (!field.isPointWithinField(point))
				return false;

			//Access granted!
			//Get and position unit
			var spawn = unitPool.Get();
			spawn.transform.SetPositionAndRotation(point, Quaternion.LookRotation(attackDirection, Vector3.up));

			return true;
		}

		public void DeactivateAllUnits(bool indefinite)
		{
			foreach (var u in units)
				u.Deactivate(indefinite);
		}

		//DESPAWN and put back into the object pool
		public void DespawnUnit(Unit u)
		{
			unitPool.Recycle(u);
			despawns++;
			// onDespawnUnit.Invoke(u);
		}
		public void DespawnAllUnits()
		{
			foreach (var u in units)
				DespawnUnit(u);
		}

		//DESTROY; might be redundant
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
		#endregion
	}
}