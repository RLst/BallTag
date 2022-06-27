
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
		Team teamOne;
		Team teamTwo;

		public void Start(Team team1, Team team2)
		{
			teamOne = team1;
			teamTwo = team2;
		}

		public void StartNextRound()
		{
			currentRound++;
			if (currentRound > totalRounds)
			{
				End();
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

		public void End()
		{
			throw new NotImplementedException();
		}

		public void Reset()
		{
			currentRound = 0;
		}
	}
}