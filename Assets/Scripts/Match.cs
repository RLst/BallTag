
using System;
using UnityEngine;

namespace LeMinhHuy
{
	public class Match
	{
		//Basic presets
		public float roundTime = 140f;
		public float timeLeft = -1;

		public int totalRounds = 5;
		public int currentRound = 0;


		//Members
		Team teamOne = new Team();
		Team teamTwo = new Team();


		//Start the match
		// public void Start(User playerOne, User playerTwo)
		// {
		// 	//Init team settings
		// 	teamOne.Initialise(playerOne);
		// 	teamTwo.Initialise(playerTwo);

		// 	//Set attack directions

		// }

		public void StartNextRound()
		{
			currentRound++;
			if (currentRound > totalRounds)
			{
				EndMatch();
				return;
			}

		}

		void SwapTeamStances()
		{
			//! This only works if both teams are always opposite
			var temp = teamOne.stance;
			teamOne.stance = teamTwo.stance;
			teamTwo.stance = temp;
		}

		public void EndMatch()
		{
			throw new NotImplementedException();
		}

		public void Reset()
		{
			currentRound = 0;
		}
	}
}