using UnityEngine;

namespace LeMinhHuy
{
	[RequireComponent(typeof(Collider))]
	public abstract class TeamObject : MonoBehaviour
	{
		//Inspector
		[SerializeField] new protected Renderer renderer;

		//Properties
		public virtual Collider col { get; protected set; }
		public Team team { get; protected set; }
		protected GameController game
		{
			get
			{
				if (_game is null)
					_game = FindObjectOfType<GameController>();
				return _game;
			}
		}
		private GameController _game;

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
			SetColor();
		}
		public void SetColor() => SetColor(team.color);
		public virtual void SetColor(Color col) => GetComponent<Renderer>().material.color = col;
	}
}