using System;
using UnityEngine;
using UnityEngine.Events;

namespace LeMinhHuy
{
	/// <summary>
	/// A player unit
	/// </summary>
	public class Unit : MonoBehaviour
	{
		//Inspector
		[SerializeField] GameObject indicatorDirection;
		[SerializeField] GameObject indicatorCarry;
		[SerializeField] GameObject indicatorDetectionZone;
		[SerializeField] Renderer mainRenderer;

		//Properties
		[field: SerializeField] public bool hasBall { get; set; } = false;
		//"If positive means this unit has been caught and is deactivated temporarily")]
		[field: SerializeField] public float inactive { get; set; } = -1;

		//Events
		public UnityEvent onTag;    //This unit tags an opponent out
		public UnityEvent onOut;      //This unit got tagged by an opponent

		//Members
		public Team team;
		Unit target;


		//INITS
		public void Init(Team team)
		{
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
			transform.Translate(transform.forward * Time.deltaTime);

			//Handle inactive
			if (inactive > 0)
			{

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

		void PlayOffence()
		{
			//
		}
		void PlayDefence()
		{

		}

		//Opponent tagged you out
		public void OnTagged()
		{
			//Deactivate
			//Downtime
		}

		public bool IsOpponent(Unit otherUnit) => !otherUnit.team.Equals(this.team);

		public void SetColor(Color col) => mainRenderer.material.color = col;

		public void Hide()
		{
			// Debug.Log("Hiding " + name);
			gameObject.SetActive(false);
		}

		public void Show() => gameObject.SetActive(true);
	}
}