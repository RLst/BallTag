using UnityEngine;

namespace LeMinhHuy
{
	public class Wall : TeamObject
	{
		void OnTriggerEnter(Collider other)
		{
			// print("Wall.ontriggerenter");

			var hit = other.GetComponent<Unit>();
			if (hit is object)
			{
				//If the unit is chasing or attacking it needs to be able get to the ball so don't despawn these guys (edge cases)
				if (hit.state == Unit.State.Chasing || hit.state == Unit.State.Attacking)
					return;

				//Unit has hit the fence, despawn if it's not on our team
				if (hit.team != this.team)
				{
					hit.Despawn();
				}
			}
		}
	}
}
