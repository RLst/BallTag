using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace LeMinhHuy
{
	/// <summary>
	/// A player unit
	/// </summary>
	[SelectionBase]
	[RequireComponent(typeof(CapsuleCollider), typeof(NavMeshAgent))]
	public class Unit : MonoBehaviour
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
		public State state = State.Inactive;
		float tickRate => 1f / ticksPerSecond;
		Unit chaseTarget;

		[Space]
		[Tooltip("Where the player will hold the ball")]
		[SerializeField] Transform hands;
		[SerializeField] GameObject indicatorDirection;
		[SerializeField] GameObject indicatorCarry;
		[SerializeField] DetectionZone detectionZone;

		[Header("Graphics")]
		[SerializeField] Renderer mainRenderer;
		[SerializeField] Color inactiveColor = Color.green;

		//Events
		[Header("Events")]
		public UnityEvent onTag;    //This unit tags an opponent out
		public UnityEvent onOut;      //This unit got tagged by an opponent

		//Members
		public Team team;
		Collider col;
		NavMeshAgent agent;


		//INITS
		void Awake()
		{
			col = GetComponent<CapsuleCollider>();
			agent = GetComponent<NavMeshAgent>();
		}
		void Start()
		{
			//Initial settings, hide indicators
			col.isTrigger = true;
			indicatorCarry.SetActive(false);
			indicatorDirection.SetActive(false);
			detectionZone.Hide();

			//Start AI tick engine
			StartCoroutine(Tick());
			// InvokeRepeating("Tick", tickRate, tickRate);
		}

		public void Init(Team team)
		{
			//Make sure there's a renderer
			if (mainRenderer is null)
				mainRenderer = GetComponentInChildren<Renderer>();

			//Set team, stance, color
			this.team = team;
			SetColor(team.color);
		}

		//CORE
		void Update()
		{
			//Temp
			// transform.Translate(transform..forward * team.strategy.baseSpeed * team.strategy.normalSpeedMult * Time.deltaTime);

			//Handle inactive
			if (inactive > 0)
			{
				//Countdown downtime timer
				inactive -= Time.deltaTime;

				//Move unit back to origin?
			}
		}

		//AI tick cycle that runs as specified rate per second to reduce processing
		IEnumerator Tick()
		{
			if (inactive > 0)
				yield break;

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

			yield return new WaitForSeconds(tickRate);
		}


		//AI METHODS
		void PlayOffence() { }
		void PlayDefence() { }
		public void ScoreGoal(int amount = 1) => team.ScoreGoal(amount);
		public void Advance()
		{

		}

		//ACTIONS
		void Move()
		{
			// agent.
		}
		public void OnTagged()
		{
			//Deactivate
			//Downtime
		}
		public void Spawn(float spawnTime)
		{

		}
		public void Deactivate()
		{
			//Change color
		}


		void OnTriggerEnter(Collider other)
		{
			var ball = other.GetComponent<Ball>();
			var unit = other.GetComponent<Unit>();

		}

		void OnDetectionZoneEnter(Unit unit)
		{

		}

		public void Despawn() => team.DespawnUnit(this);
		public bool isOpponent(Unit otherUnit) => !otherUnit.team.Equals(this.team);
		public void Hide() => gameObject.SetActive(false);
		public void Show() => gameObject.SetActive(true);
		public void SetColor(Color col) => mainRenderer.material.color = col;
	}
}