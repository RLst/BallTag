using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using DG.Tweening;
using LeMinhHuy.Events;

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
			Starting,       //Set this as the default. This unit will figure out what to do
			Chasing,        //Heading towards the ball
			Attacking,      //Heading towards opponent's goal
			Advancing,      //Advancing in the direction of the opponents
			Receiving,      //Waiting and Receiving a ball
			Standby,        //Waiting for attackers
			Defending,      //Heading towards opponent with ball
			Inactive,       //Caught and waiting for reactivation / moving back to origin
			Despawning,      //Hit the end of the fence
		}

		//Inspector
		[Tooltip("If positive means this unit has been caught and is deactivated temporarily")]
		public float inactive = -1;

		//AI
		[Header("AI")]
		[Tooltip("AI ticks per second")]
		[SerializeField] float ticksPerSecond = 5f;     //Reduce CPU usage, save battery etc
		float tickRate => 1f / ticksPerSecond;
		public State state = State.Starting;
		[SerializeField] float radiusNormal = 0.65f;
		[SerializeField] float radiusPassthrough = 0.01f;

		[Space]
		[Tooltip("Where the player will hold the ball")]
		public Transform hands = null;
		[SerializeField] GameObject indicatorMoving = null;
		[SerializeField] GameObject indicatorAttacking = null;
		[SerializeField] GameObject longRangeDetector = null;

		[Header("Graphics")]
		[SerializeField] Color inactiveColor = Color.green;

		//Events
		[Header("Events")]
		public UnityEvent onTag;    //This unit tags an opponent out
		public UnityEvent onOut;      //This unit got tagged by an opponent
		public StateEvent onChangedState;      //This unit got tagged by an opponent

		//State
		public bool hasBall { get; private set; }

		//Members
		NavMeshAgent agent;
		Rigidbody rb;
		Vector3 origin;   //Initial spawn location so it knows where to return to
		Ball ball;     //Required to chase after the ball
		Unit targetAttacker;    //The attacker you're trying to defend against

		//UNITY
		protected override void Init()
		{
			agent = GetComponent<NavMeshAgent>();
			rb = GetComponent<Rigidbody>();
			ball = GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>();
			Debug.Assert(hands is object, "Unit has no hands");
		}

		protected override void Start()
		{
			//Start AI tick engine; InvokeRepeating() is simpler because when this unt gets deactivated it will turn off as well
			InvokeRepeating("Tick", 0f, tickRate);

			//Initial settings, hide indicators, when first instantiated
			col.isTrigger = true;       //The agent collider will provide collision like behaviour?
			rb.isKinematic = true;
			HideAuxillaries();
		}
		void HideAuxillaries()
		{
			indicatorAttacking.SetActive(false);
			indicatorMoving.SetActive(false);
			longRangeDetector.SetActive(false);
		}
		public void ScoreGoal(int amount = 1) => team.ScoreGoal(amount);
		public bool isOpponent(Unit otherUnit) => !otherUnit.team.Equals(this.team);
		public void SetActive(bool v) => gameObject.SetActive(v);
		internal void SetOrigin() => origin = transform.position;


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
		}


		#region  AI
		//AI tick cycle that runs as specified rate per second to reduce processing
		void Tick()
		{
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
					Debug.LogWarning("Invalid stance!");
					break;
			}
		}

		void SetState(State newState)
		{
			if (this.state == newState)
				return;
			this.state = newState;

			// print($"{name}'s new state: {newState}");
			onChangedState.Invoke(newState);
		}

		#region Offense
		void PlayOffence()
		{
			switch (state)
			{
				case State.Starting:
					hasBall = false;
					HideAuxillaries();
					SetState(State.Chasing);
					break;
				case State.Chasing:
					//If our team no in possession then chase after ball
					if (!team.hasBall)
					{
						Chase();
					}
					//Otherwise advance towards opponent
					else
					{
						SetState(State.Advancing);
					}
					break;
				case State.Attacking:
					//You have the ball so head towards the goal
					Attack();
					break;
				case State.Advancing:
					//Head towards the opponent in a straight line
					Advance();
					break;
				case State.Receiving:
					Receive();
					break;
				case State.Inactive:
					name = "Inactive (Offense)";
					agent.SetDestination(transform.position);
					break;
			}
		}
		void Chase()
		{
			name = "Chaser";
			agent.SetDestination(ball.transform.position);
			agent.speed = team.strategy.normalSpeed;
			agent.radius = radiusNormal;
			indicatorMoving.SetActive(true);
		}
		void OnBallTouch()
		{
			//Grab ball if chasing
			if (state == State.Chasing)
			{
				ReceiveBall();
			}
			else if (state == State.Receiving)
			{
				// print("Ball caught!");
				ball.transform.DOKill();    //"Catch" the ball
				ReceiveBall();
			}
		}
		public void SeizeBall()
		{
			//Grab ball and turn it off so it can't drift
			hasBall = true;
			ball.transform.SetParent(hands);
			ball.transform.localPosition = Vector3.zero;
			ball.SetActivatePhysics(active: false);
		}
		void Attack()
		{
			name = "Attacker";
			agent.SetDestination(team.opponent.goal.target.transform.position);
			agent.speed = team.strategy.dribbleSpeed;
			agent.radius = radiusNormal;
			HideAuxillaries();
			indicatorAttacking.SetActive(true);
		}
		void Advance()
		{
			name = "Advancer";
			agent.SetDestination(transform.position + team.attackDirection * 10f);
			agent.speed = team.strategy.normalSpeed;
			agent.radius = radiusPassthrough;
			HideAuxillaries();
			indicatorMoving.SetActive(true);
		}
		void PassBall()
		{
			// print("Passing ball");
			//Pass to the nearest team member
			if (team.FindNearestActiveUnit(this, out Unit nearest))
			{
				// print("Closest active unit found");
				nearest.SetState(State.Receiving);
				ball.transform.SetParent(game.transform);
				ball.Pass(nearest);
			}
			//No active team members left, you have lost the round
			else
			{
				print("No more active units found!");
				team.NotifyNoUnitsLeftToPassBallTo();
			}
		}
		public void ReceiveBall()
		{
			SeizeBall();
			SetState(State.Attacking);
		}
		void Receive()
		{
			//Stand still and wait for ball to come
			name = "Receiver";
			agent.SetDestination(ball.transform.position);  //look at the ball
			agent.speed = team.strategy.normalSpeed * 0.5f;
			agent.radius = radiusNormal;
			HideAuxillaries();
		}
		void Tagout()
		{
			print($"{name} tagged out!");

			//Unit has been tagged. Pass the ball to a nearby player then self deactivate
			if (state != State.Attacking)
				Debug.LogError("Non attacker tagged out! Error in logic");

			PassBall();
			Deactivate();

			onOut.Invoke();
		}
		#endregion

		#region Defence
		void PlayDefence()
		{
			switch (state)
			{
				case State.Starting:
					hasBall = false;
					HideAuxillaries();
					SetState(State.Standby);
					break;
				case State.Standby:
					Standby();
					break;
				case State.Defending:
					Defend();
					break;
				case State.Inactive:
					name = "Inactive (Defense)";
					//Move back towards origin after tagging someone out
					agent.SetDestination(origin);
					agent.speed = team.strategy.returnSpeed;
					break;
			}
		}
		void Standby()
		{
			//Enable and display unit detector and wait for an Attacker to come through
			name = "Standby";
			HideAuxillaries();
			longRangeDetector.SetActive(true);
		}
		void OnInsideDetectionZone(Unit unit)
		{
			//If standing by and an Attacking unit enters then start Defending
			if (unit.state == State.Attacking)
			{
				targetAttacker = unit;
				longRangeDetector.SetActive(false);
				SetState(State.Defending);
			}
		}
		void Defend()
		{
			//Disable detector and start chasing the attacker
			// longRangeDetector.SetActive(false);
			name = "Defender";
			agent.SetDestination(targetAttacker.transform.position);
			agent.speed = team.strategy.normalSpeed;
			HideAuxillaries();
			indicatorMoving.SetActive(true);
		}
		void OnUnitTouch(Unit other)
		{
			// print($"{name} touches {other.name}");
			//If the unit touched is Attacker and this unit is Defending then Tag out
			if (other.state == State.Attacking && this.state == State.Defending)
			{
				other.Tagout();
				Deactivate();

				onTag.Invoke();
			}
		}
		#endregion
		#endregion


		#region Spawn
		//Spawning units are dark, can be passed through, have a slight delay before they move and visible
		public void Spawn()
		{
			SetColor(inactiveColor);
			agent.radius = radiusPassthrough;
			inactive = team.strategy.spawnTime;
			SetActive(true);
		}

		//Active units are team colored, can't be walked through, visible
		public void Activate()
		{
			SetTeam(team);
			agent.radius = radiusNormal;
			SetState(State.Starting);
			SetActive(true);
		}

		//Inactive units are dark/translucent, can be passed through, visible
		//Delay before they can move again And/or move back to their origin if defending
		public void Deactivate(bool indefinite = false)
		{
			if (state == State.Despawning)
				return;

			SetColor(inactiveColor);
			inactive = indefinite ? -1f : team.strategy.reactivationTime;

			if (agent.isOnNavMesh)
			{
				agent.SetDestination(transform.position);       //Stop unit without turning off agent
				agent.radius = radiusPassthrough;
			}
			SetState(State.Inactive);
			SetActive(true);
			HideAuxillaries();
		}

		//Despawned units are invisible
		//Put back into the pool
		public void Despawn()
		{
			//NOTE: Don't bring the ball with you!
			if (hasBall)
			{
				ball.transform.SetParent(null);
				hasBall = false;
			}

			team.DespawnUnit(this);
			SetActive(false);
			SetState(State.Despawning);
		}

		public override void SetColor(Color col)
		{
			// renderer.material.DOKill();
			renderer.material.color = col;
		}
		#endregion
	}
}