using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace LeMinhHuy
{
	/// <summary>
	/// Basically the game manager; Registers details and starts the match
	/// Is the only one that is a monobehaviour and is in the scene so it can pass in objects etc
	/// </summary>
	public class Umpire : Singleton<Umpire>
	{
		//Inspector
		public GameParameters _gameParameters;

		[Header("AR")]
		public ARSessionOrigin arSessionOrigin;
		public ARRaycastManager arRaycastManager;

		//Make these an array if you ever need more than two teams
		[Header("Teams")]
		public Team teamOne;
		public Team teamTwo;

		//Members
		Match masterMatch = new Match();    //There's one and only one match that get's recycled

		void OnValidate()
		{
			Start();
		}

		void Awake()
		{
			Debug.Assert(_gameParameters != null, "No game parameters found!");
			Debug.Assert(masterMatch != null, "No main match created!");
			// Debug.Assert(arRaycastManager != null, "No ARRaycastManager found!");
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

			teamOne.Initialise();
			teamTwo.Initialise();

			// masterMatch.Start(gameSettings.playerOne, gameSettings.playerTwo);

			// //Set player type for each team
			// teams = new Team[2];

			// //Team one
			// teams[0].userType = gameSettings.playerOne.type;
			// teams[0].StartRound(Stance.Offensive);

			// //Team two
			// teams[1].userType = gameSettings.playerTwo.type;
			// teams[2].StartRound(Stance.Defensive);
		}
		public void StartNextRound()
		{
			//Reset all stats
			//Set user type
			//Set stance
			//Clear all players off the field
		}

		//Core
		void Update()
		{
			//temp
			if (Input.GetKeyDown(KeyCode.S))
				StartMatch();

			HandleDowntime();
		}

		void HandleDowntime()
		{
			if (teamOne.recoveryTime > 0)
				teamOne.recoveryTime -= Time.deltaTime;
			if (teamTwo.recoveryTime > 0)
				teamTwo.recoveryTime -= Time.deltaTime;
		}
	}
}