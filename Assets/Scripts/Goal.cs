using System;
using UnityEngine;

namespace LeMinhHuy
{
	[RequireComponent(typeof(BoxCollider))]
	public class Goal : TeamObject
	{
		//The center of the goal for units to aim for
		public Vector3 target => boxCol.center;
		BoxCollider boxCol;
		protected override void Init()
		{
			boxCol = (BoxCollider)col;
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
					if (hit.state == Unit.State.Attacking)
						hit.ScoreGoal();

					//Otherwise/And despawn as usual
					hit.Despawn();
				}
				//Do nothing if it's our own team
			}
		}

	}
}