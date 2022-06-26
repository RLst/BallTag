using UnityEngine;

namespace LeMinhHuy
{
	/// <summary>
	/// A player unit
	/// </summary>
	public class Player : MonoBehaviour
	{
		//Inspector
		public float offensiveSpeed = 6f;
		public float chaseSpeed;


		//Properties
		public bool hasBall { get; set; }
		public Stance stance { get; set; }
		public Team team { get; set; }

		// public AI offensive;
		// public AI defensive;


		//Core
		void Update()
		{
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
	}
}