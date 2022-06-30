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
		public State state = State.Starting;
		[SerializeField] float radiusNormal = 0.65f;
		[SerializeField] float radiusPassthrough = 0.01f;

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
		public StateEvent onChangedState;      //This unit got tagged by an opponent

		//State
		public bool hasBall { get; private set; }

		//Members
		NavMeshAgent agent;
		Rigidbody rb;
		Transform origin;   //Initial spawn location so it knows where to return to
		Ball ball;     //Required to chase after the ball

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
		}


		public void ScoreGoal(int amount = 1) => team.ScoreGoal(amount);
		public bool isOpponent(Unit otherUnit) => !otherUnit.team.Equals(this.team);
		public void SetActive(bool v) => gameObject.SetActive(v);


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
					Debug.LogWarning("Invalid stance reached!");
					break;
			}
			// yield return new WaitForSeconds(tickRate);
		}
		void SetState(State newState)
		{
			if (this.state == newState)
				return;

			this.state = newState;

			onChangedState.Invoke(newState);
		}

		//OFFENSE
		void PlayOffence()
		{
			switch (state)
			{
				case State.Starting:
					//First thing unit should do is try chasing
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
			}
		}
		void Chase()
		{
			agent.SetDestination(ball.transform.position);
			agent.speed = team.strategy.normalSpeed;
		}
		void SeizeBall()
		{
			//Grab ball and turn it off so it can't drift
			hasBall = true;
			ball.transform.SetParent(hands);
			ball.SetActivatePhysics(active: false);
		}
		void Attack()
		{
			agent.SetDestination(team.opponent.goal.target.transform.position);
			agent.speed = team.strategy.dribbleSpeed;
		}
		void Advance()
		{
			agent.SetDestination(transform.position + team.attackDirection * 10f);
			agent.speed = team.strategy.normalSpeed;
		}
		void PassBall(Unit u)
		{
			//Move towards player using ball's agent?

		}

		//DEFENSE
		void PlayDefence() { }
		void Standby() { }
		void Defend() { }
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
		public void Deactivate()
		{
			SetColor(inactiveColor);
			agent.radius = radiusPassthrough;
			inactive = team.strategy.reactivationTime;
			SetActive(true);
		}

		//Despawned units are invisible
		//Put back into the pool
		public void Despawn()
		{
			team.DespawnUnit(this);
			SetActive(false);
			SetState(State.Despawned);
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
		void OnBallTouch()
		{
			//Grab ball if chasing
			if (state == State.Chasing)
			{
				SeizeBall();
				SetState(State.Attacking);
			}
		}
	}
}

// if (Input.GetKeyDown(KeyCode.A)) Activate();
// if (Input.GetKeyDown(KeyCode.D)) Deactivate();
// if (Input.GetKeyDown(KeyCode.X)) Despawn();