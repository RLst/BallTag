using System;
using UnityEngine;
using UnityEngine.Events;

namespace LeMinhHuy
{
	/// <summary>
	/// A player unit
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public class Unit : MonoBehaviour
	{
		enum State
		{
			Attacking,      //Heading towards opponent's goal
			Defending,      //Heading towards opponent with ball
			ChasingBall,    //Heading towards the ball
			Advancing,      //Advancing in the direction of the opponents
			Waiting,        //Waiting for attackers
			Inactive,       //Caught and waiting for reactivation
			Despawned,      //Hit the end of the fence
		}

		//Properties
		public bool hasBall = false;
		//"If positive means this unit has been caught and is deactivated temporarily")]
		public float inactive = -1;

		//Inspector
		[Space]
		[Tooltip("Where the player will hold the ball")]
		[SerializeField] Transform hands;
		[SerializeField] GameObject indicatorDirection;
		[SerializeField] GameObject indicatorCarry;
		[SerializeField] GameObject indicatorDetectionZone;

		[Header("Graphics")]
		[SerializeField] Renderer mainRenderer;

		[SerializeField] Color inactiveColor = Color.green;

		//Events
		[Header("Events")]
		public UnityEvent onTag;    //This unit tags an opponent out
		public UnityEvent onOut;      //This unit got tagged by an opponent

		//Members
		public Team team;
		Unit target;
		Collider col;



		//INITS
		void Awake() => col = GetComponent<Collider>();
		void Start() => col.isTrigger = true;

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
			transform.Translate(transform.forward * team.strategy.baseSpeed * team.strategy.normalSpeedMult * Time.deltaTime);

			//Handle inactive
			if (inactive > 0)
			{
				//Countdown downtime timer
				inactive -= Time.deltaTime;

				//Move unit back to origin?
			}
			//Else play
			else
			{
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
			}
		}

		void PlayOffence() { }
		void PlayDefence() { }

		public void ScoreGoal(int amount = 1) => team.ScoreGoal(amount);
		public void MoveTowardOpponentField()
		{

		}

		//Opponent tagged you out
		public void OnTagged()
		{
			//Deactivate
			//Downtime
		}

		void OnTriggerEnter(Collider other)
		{
			var ball = other.GetComponent<Ball>();
			var unit = other.GetComponent<Unit>();

			if (isDefending)
			{
				{

				}
			}
		}

		public void Deactivate()
		{
			//Change color
		}


		public void Despawn() => team.DespawnUnit(this);
		public bool isOpponent(Unit otherUnit) => !otherUnit.team.Equals(this.team);
		public void Hide() => gameObject.SetActive(false);
		public void Show() => gameObject.SetActive(true);
		public void SetColor(Color col) => mainRenderer.material.color = col;
	}
}