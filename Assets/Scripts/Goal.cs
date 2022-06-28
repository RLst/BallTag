using System;
using UnityEngine;

namespace LeMinhHuy
{
	public class Goal : TeamObject
	{
		//The center of the goal for units to aim for
		public Transform target;

		protected override void Init()
		{
			Debug.Assert(target is object, "Must assign a target to goal");
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