using System;
using UnityEngine;

namespace LeMinhHuy
{
	public class Goal : MonoBehaviour
	{
		public Vector3 target => col.center;    //The center of the goal for units to aim for
		private Team team;
		private BoxCollider col;


		void Awake() => col = GetComponent<BoxCollider>();

		void Start()
		{
			col.isTrigger = true;
		}

		public void Init(Team team)
		{
			this.team = team;
			SetColor(team.color);
		}

		void OnTriggerEnter(Collider other)
		{
			var hit = other.GetComponent<Unit>();
			if (hit is object)
			{
				//If the unit is from the other team...
				if (!hit.team.Equals(this.team))
				{
					//And has a ball then score goal against us
					if (hit.hasBall)
						hit.ScoreGoal();

					//Otherwise/And despawn as usual
					hit.Despawn();
				}
				//Do nothing if it's our own team
			}
		}

		public void SetColor(Color col) => GetComponent<Renderer>().material.color = col;
	}
}