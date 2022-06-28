using UnityEngine;

namespace LeMinhHuy
{
	[RequireComponent(typeof(Collider))]
	public abstract class TeamObject : MonoBehaviour
	{
		//Inspector
		[SerializeField] new protected Renderer renderer;

		//Properties
		public Team team { get; protected set; }
		public virtual Collider col { get; protected set; }


		protected virtual void Awake()
		{
			col = GetComponent<Collider>();
			if (renderer is null) GetComponentInChildren<Renderer>(includeInactive: true);
			Init();
		}
		protected virtual void Start() => col.isTrigger = true;
		protected virtual void Init() { }

		public virtual void SetTeam(Team team)
		{
			this.team = team;
			SetColor(team.color);
		}
		public virtual void SetColor(Color col) => GetComponent<Renderer>().material.color = col;
	}
}