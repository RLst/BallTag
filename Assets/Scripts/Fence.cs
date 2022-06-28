using UnityEngine;

namespace LeMinhHuy
{
	[RequireComponent(typeof(BoxCollider))]
	public class Fence : MonoBehaviour
	{
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
				//Unit has hit the fence, despawn if it's not on our team
				if (!hit.team.Equals(this.team))
				{
					hit.Despawn();
				}
			}
		}

		public void SetColor(Color col) => GetComponent<Renderer>().material.color = col;
	}
}
