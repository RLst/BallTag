
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using LeMinhHuy.Events;
using UnityEngine.Events;

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

		//Pool; eliminate garbage allocation
		const int startingUnitsToPool = 5;
		Pool<Unit> unitPool;
		List<Unit> units = new List<Unit>();

		List<ARRaycastHit> arHitResults = null;

		//Events
		[Space]
		public FloatEvent onEnergyChange;
		UnityEvent onScoreGoal = null;

		//Properties
		int? activeUnits => unitPool?.countActive;

		//Members
		GameController game;
		ARRaycastManager arRaycastManager;
		internal Team opponent;

		#region Stats
		//Maybe make these into a struct
		public int goals { get; private set; }
		public int roundsWon { get; set; }
		public int roundDraw { get; set; }
		public int roundLost { get; set; }
		public int tags { get; set; }   //Opponents caught
		public int outs { get; set; }    //Team members that got caught
		public int ballPasses { get; set; }
		public int despawns { get; set; }
		void ResetStats()
		{
			//Stats
			goals = 0;
			roundsWon = 0;
			roundDraw = 0;
			roundLost = 0;
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
			onScoreGoal.Invoke();
		}
		#endregion


		//INITS
		void Awake()
		{
			this.game = GameController.current;
			arRaycastManager = this.game.arRaycastManager;
		}

		public void Initialize(TeamSettings settings)
		{
			Awake();

			setParameters();

			initTeamObjects();

			InitUnitPool();

			ResetStats();

			//Set team parameters
			void setParameters()
			{
				this.name = settings.name;
				this.color = settings.color;
				this.userType = settings.userType;
				switch (settings.stance)
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
			}
			//Setup team objects and color
			void initTeamObjects()
			{
				foreach (var to in teamObjects)
					to.SetTeam(this);
			}
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
			var unit = GameObject.Instantiate<Unit>(game.genericUnitPrefab, field.transform);
			return unit;
		}
		void OnGetUnit(Unit unit)
		{
			energy -= strategy.spawnCost;
			unit.SetTeam(this);    //Sets team, color, strategy
			unit.Spawn();               //Set spawn time so it can do it's spawn sequence
			unit.Activate();            //start unit
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
				// Debug.Log($"Energy: {energy}, MaxEnergy: {game.parameters.maxEnergy}");
				if (energy < game.parameters.maxEnergy)
				{
					energy += Time.deltaTime * strategy.energyRegenRate;
					onEnergyChange.Invoke(energy);

					if (energy > game.parameters.maxEnergy)
						energy = game.parameters.maxEnergy;
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
						//Spawn a unit every second at a random location on our home field
						SpawnUnit(this.field.GetRandomLocationOnField());
					}
					break;

				case Stance.Defensive:
					{
						//If opponent has active units
						//Spawn at a random point on a ray going from our goal to a random opponent attacker
						if (opponent.activeUnits.HasValue && opponent.activeUnits > 0)
						{
							//Find random attacker

							//Find ray going from goal to random attacker

							//Choose a random location that's not too close to our goal

							//Spawn
						}
						//else spawn at a random location
						else
						{
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
			if (unitPool.countActive == 0)
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
		internal void TrySpawnUnitOnRandomPositionOnField()
		{
			//Used when
		}

		internal void SpawnUnitDistanceFromOpponentUnit(Unit opponent, float distance)
		{
		}

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
			if (game.parameters.isARMode)
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
		public bool SpawnUnit(Vector3 positionOnField)
		{
			//GUARDS
			//Must have enough energy //NOTE: This may be double checked
			if (energy < strategy.spawnCost)
				return false;
			//Point must be on field
			if (!field.isPosWithinField(positionOnField))
				return false;

			//Access granted!
			//Get and position unit
			var spawn = unitPool.Get();
			spawn.transform.SetPositionAndRotation(positionOnField, Quaternion.LookRotation(attackDirection, Vector3.up));

			return true;
		}

		//DESPAWN and put back into the object pool
		public void DespawnUnit(Unit u)
		{
			unitPool.Recycle(u);
			despawns++;
			//onDespawnUnit.Invoke(u);
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
		#endregion
	}
}