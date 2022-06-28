using UnityEngine;

namespace LeMinhHuy
{
	public class Wall : TeamObject
	{
		void OnTriggerEnter(Collider other)
		{
			var hit = other.GetComponent<Unit>();
			if (hit is object)
			{
				//Unit has hit the fence, despawn if it's not on our team
				if (!hit.team.Equals(this.team))
				{
					hit.Despawn();
				}
			}
		}
	}
}
