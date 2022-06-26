
using System.Collections.Generic;
using UnityEngine;

namespace LeMinhHuy
{
	/// <summary>
	/// Basically the game manager; Registers details and starts the match
	/// Is the only one that is a monobehaviour and is in the scene so it can pass in objects etc
	/// </summary>
	public class Umpire : Singleton<Umpire>
	{
		public GameSettings gameSettings;

		//Inspector
		//Make these an array if you ever need more than two teams
		[SerializeField] Team teamOne;
		[SerializeField] Team teamTwo;

		//Members
		Match masterMatch = new Match();    //There's one and only one match that get's recycled

		void OnValidate()
		{
			Start();
		}

		void Awake()
		{
			Debug.Assert(gameSettings != null, "MatchController does not have game settings");
			Debug.Assert(masterMatch != null, "No main match created!");
		}

		void Start()
		{
			//Calculate attack direction
			if (teamOne.field is object && teamTwo.field is object)
			{
				// Debug.Log(teamOne.field.transform.position);
				// Debug.Log(teamTwo.field.transform.position);
				teamOne.attackDirection = (teamTwo.field.transform.position - teamOne.field.transform.position).normalized;
				teamTwo.attackDirection = (teamOne.field.transform.position - teamTwo.field.transform.position).normalized;
			}
		}

		void StartMatch()
		{
			masterMatch.Reset();

			masterMatch.Start(gameSettings.playerOne, gameSettings.playerTwo);

			// //Set player type for each team
			// teams = new Team[2];

			// //Team one
			// teams[0].userType = gameSettings.playerOne.type;
			// teams[0].StartRound(Stance.Offensive);

			// //Team two
			// teams[1].userType = gameSettings.playerTwo.type;
			// teams[2].StartRound(Stance.Defensive);
		}

		public void NextRound()
		{

		}
	}
}