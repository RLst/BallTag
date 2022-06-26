using System;
using UnityEngine;

namespace LeMinhHuy
{
	/// <summary>
	/// A player unit
	/// </summary>
	public class Unit : MonoBehaviour
	{
		//Inspector



		//Properties
		public bool hasBall { get; set; } = false;
		public Stance stance => team.stance;
		public Color color => team.color;

		//Members
		public Team team;

		//Inits
		public void Init(Team team)
		{
			//Set team, stance, color
			this.team = team;
			GetComponent<Renderer>().material.color = this.color;

		}


		//Core
		void Update()
		{
			//Temp
			transform.Translate(transform.forward * Time.deltaTime);

			switch (stance)
			{
				case Stance.Offensive:

					break;

				case Stance.Defensive:
					break;

				default:
					Debug.LogWarning("Invalid stance reached!");
					break;
			}
		}

		void PlayOffence()
		{

		}

		void PlayDefence()
		{

		}

		public void SetColor(Color col)
		{
			GetComponent<Renderer>().material.color = col;
		}

		public void Hide()
		{
			gameObject.SetActive(false);
		}

		public void Show()
		{
			gameObject.SetActive(true);
		}
	}
}