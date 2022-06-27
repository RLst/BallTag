
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

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
		public Stance stance;
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

		//Stats
		public int roundsWon { get; set; }
		public int roundDraw { get; set; }
		public int roundLost { get; set; }
		public int opponentsCaught { get; set; }
		public int membersCaught { get; set; }    //Team members that got caught
		public int ballPasses { get; set; }

		//Properties
		GameParameters gameParameters   //Lazy
		{
			get
			{
				if (_gameParameters is null)
					_gameParameters = Umpire.current._gameParameters;
				return _gameParameters;
			}
		}

		//Members
		List<Unit> units = new List<Unit>();
		GameParameters _gameParameters;
		ARRaycastManager arRaycastManager;
		private List<ARRaycastHit> arHitResults;

		//Initialise the team and do awake/start stuff
		public void Initialise()
		{
			//Cache from monobehaviour umpire
			arRaycastManager = Umpire.current.arRaycastManager;

			//Register events
			UserInput.current.onScreenPosInput.AddListener(TrySpawnUnitAtScreenPoint);

			PrepareObjectPool();
			SetTeamColors();
		}
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
		void SetTeamColors()
		{
			goal.SetColor(this.color);
			foreach (var f in fences)
				f.SetColor(this.color);
		}

		//Pooling callbacks
		Unit SpawnUnit()
		{
			var unit = GameObject.Instantiate<Unit>(gameParameters.genericUnitPrefab, field.parent.transform);
			unit.team = this;
			unit.SetColor(this.color);
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

		//Functions required:
		//- Spawn a unit
		//- Spawn from a user point input
		//- despawn unit
		// - deactivate unit

		public void TrySpawnUnitAtScreenPoint(Vector2 screenPoint)
		{
			//Reject user input if the team is computer controlled
			if (userType == UserType.Computer)
				return;

			//Raycast to point
			if (gameParameters.isARMode)
			{
				//AR Raycast
				if (arRaycastManager.Raycast(screenPoint, arHitResults, UnityEngine.XR.ARSubsystems.TrackableType.Planes))
				{
					SpawnUnitOnField(arHitResults[0].pose.position);
				}
			}
			else
			{
				//Normal raycast
				var ray = Camera.main.ScreenPointToRay(screenPoint);
				if (Physics.Raycast(ray, out RaycastHit hit, RAYCAST_MAXDISTANCE))
				{
					SpawnUnitOnField(hit.point);
				}
			}

			//Spawn unit at point
		}

		//Spawn player at specific location, facing toward the opposite team
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
		public bool TrySpawnUnitOnField(Vector3 positionOnField)
		{
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