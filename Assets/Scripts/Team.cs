
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

		[Header("Team Objects")]
		public Field field;
		public Goal goal;
		public Wall[] fences;

		//Pool; eliminate garbage allocation
		const int startingUnitsToPool = 5;
		Pool<Unit> unitPool;
		List<Unit> units = new List<Unit>();

		List<ARRaycastHit> arHitResults;

		//Events
		public FloatEvent onEnergyChange;
		private UnityEvent onScoreGoal;

		//Members
		GameController game;
		ARRaycastManager arRaycastManager;

		//Stats
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



		//INITS
		void Awake()
		{
			this.game = GameController.current;
			arRaycastManager = this.game.arRaycastManager;
		}

		public void Initialise(TeamSettings ts)
		{
			Awake();

			setParameters();

			initTeamObjects();

			InitUnitPool();

			ResetStats();

			//Set team parameters
			void setParameters()
			{
				this.name = ts.name;
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
			}
			//Setup team objects and color
			void initTeamObjects()
			{
				goal.SetTeam(this);
				foreach (var f in fences)
					f.SetTeam(this);
			}
		}

		//POOLING CALLBACKS
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
			unit.SetTeamAndColor(this);    //Sets team, color, strategy
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

		//ENERGY
		public void HandleEnergy()
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

		//SPAWN
		internal void TrySpawnUnitOnRandomPositionOnField()
		{
			//Used when
		}

		internal void TrySpawnUnitDistanceFromOpponentUnit(Unit opponent, float distance)
		{
		}

		public void TrySpawnUnitAtScreenPoint(Vector2 screenPoint)
		{
			//GUARDS
			//Reject if there's not enough energy; Check early for slight optimization
			if (energy < strategy.spawnCost)
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
					TrySpawnUnit(arHitResults[0].pose.position);
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
						TrySpawnUnit(hit.point);
					}
					else
					{
						//Probably clicked on your opponent's field
						// Debug.LogWarning("Click on wrong field!");
					}
				}
			}
		}

		//SPAWN player at specific location, facing toward the opposite team
		public bool TrySpawnUnit(Vector3 positionOnField)
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
	}
}