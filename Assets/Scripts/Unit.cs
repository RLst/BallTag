using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using DG.Tweening;

namespace LeMinhHuy
{
	/// <summary>
	/// A player unit
	/// </summary>
	[SelectionBase]
	[RequireComponent(typeof(CapsuleCollider), typeof(NavMeshAgent))]
	public class Unit : TeamObject
	{
		public enum State
		{
			Attacking,      //Heading towards opponent's goal
			Defending,      //Heading towards opponent with ball
			ChasingBall,    //Heading towards the ball
			Advancing,      //Advancing in the direction of the opponents
			Waiting,        //Waiting for attackers
			Inactive,       //Caught and waiting for reactivation
			Despawned,      //Hit the end of the fence
		}

		//Inspector
		[Tooltip("If positive means this unit has been caught and is deactivated temporarily")]
		public float inactive = -1;

		//AI
		[Header("AI")]
		[Tooltip("AI ticks per second")]
		[SerializeField] float ticksPerSecond = 15f;     //Reduce CPU usage, save battery etc
		float tickRate => 1f / ticksPerSecond;
		public State state = State.Inactive;
		[SerializeField] float radiusActive = 0.5f;
		[SerializeField] float radiusInactive = 0.1f;
		Unit chaseTarget;

		[Space]
		[Tooltip("Where the player will hold the ball")]
		[SerializeField] Transform hands = null;
		[SerializeField] GameObject indicatorDirection = null;
		[SerializeField] GameObject indicatorCarry = null;
		[SerializeField] DetectionZone detectionZone = null;

		[Header("Graphics")]
		[SerializeField] Color inactiveColor = Color.green;

		//Events
		[Header("Events")]
		public UnityEvent onTag;    //This unit tags an opponent out
		public UnityEvent onOut;      //This unit got tagged by an opponent

		//Members
		// public Team team;
		// Collider col;
		NavMeshAgent agent;
		Transform origin;   //Initial spawn location so it knows where to return to

		//UNITY
		protected override void Init()
		{
			// col = GetComponent<CapsuleCollider>();
			agent = GetComponent<NavMeshAgent>();
		}

		protected override void Start()
		{
			//Start AI tick engine
			//InvokeRepeating() is simpler because when this unt gets deactivated it will turn off as well
			InvokeRepeating("Tick", 0f, tickRate);

			//Initial settings, hide indicators, when first instantiated
			col.isTrigger = true;
			indicatorCarry.SetActive(false);
			indicatorDirection.SetActive(false);
			detectionZone.Hide();
		}

		void Update()
		{
			handleInactiveUnits();

			void handleInactiveUnits()
			{
				//Handle inactive
				if (inactive > 0f)
				{
					//Countdown downtime timer
					inactive -= Time.deltaTime;
					if (inactive <= 0)
						Activate();
				}
			}

			//Temp
			// transform.Translate(transform..forward * team.strategy.baseSpeed * team.strategy.normalSpeedMult * Time.deltaTime);

			// if (Input.GetKeyDown(KeyCode.A)) Activate();
			// if (Input.GetKeyDown(KeyCode.D)) Deactivate();
			// if (Input.GetKeyDown(KeyCode.X)) Despawn();
		}

		//AI tick cycle that runs as specified rate per second to reduce processing
		void Tick()
		{
			// Debug.Log("Tick()");

			//Don't tick if inactive
			if (inactive > 0f)
				return;
			//This tick will be run right at instantiate when a team has yet to be assigned
			if (team is null)
				return;

			switch (team.strategy.stance)
			{
				case Stance.Offensive:
					PlayOffence();
					break;

				case Stance.Defensive:
					PlayDefence();
					break;

				default:
					Debug.LogWarning("Invalid stance reached!");
					break;
			}
			// yield return new WaitForSeconds(tickRate);
		}


		//AI METHODS
		void PlayOffence() { }
		void PlayDefence() { }

		//ACTIONS
		void MoveTowardTarget() { }
		void Advance() { }
		void TagOut() { }
		public void ScoreGoal(int amount = 1) => team.ScoreGoal(amount);


		#region Spawn
		public void Spawn()
		{
			inactive = team.strategy.spawnTime;

			//Color from clear/black to team color then Activate unit
			renderer.material.color = Color.clear;
			renderer.material.
				DOColor(team.color, team.strategy.spawnTime).
				OnComplete(Activate);
		}

		public void Activate()
		{
			if (inactive > 0) return;

			//Set back to team color
			SetTeam(team);
			//Let units be able to collider with other units
			agent.radius = radiusActive;
			//make sure that it's active
			gameObject.SetActive(true);
		}

		public void Deactivate()
		{
			//Deactivate but also start reactivation process
			inactive = team.strategy.reactivationTime;
			//Fade from team color to inactive color
			SetColor(inactiveColor);
			// mainRenderer.material.DOColor(team.color, team.strategy.reactivationTime);
		}

		public void Despawn()
		{
			team.DespawnUnit(this);
		}

		public override void SetColor(Color col)
		{
			renderer.material.DOKill();
			renderer.material.color = col;
		}
		#endregion


		//COLLISIONS
		void OnDetectionZoneEnter(Unit unit)
		{
		}
		void OnTriggerEnter(Collider other)
		{
			var ball = other.GetComponent<Ball>();
			var unit = other.GetComponent<Unit>();
		}



		public bool isOpponent(Unit otherUnit) => !otherUnit.team.Equals(this.team);
		public void SetActive(bool v) => gameObject.SetActive(v);
	}
}