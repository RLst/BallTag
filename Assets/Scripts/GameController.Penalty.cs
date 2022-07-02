using System;
using UnityEngine;

namespace LeMinhHuy
{
	public partial class GameController : Singleton<GameController>
	{
		/// <summary>
		/// Invoke when the match is a draw
		/// Generate maze, clear area around goals, rebake field navmesh, place ball at random, place a unit at team goal, let run attacker AI logic
		/// </summary>
		public void BeginPenaltyRound()
		{
			//This may be called before any matches begin ie. Penalty game mode

			teamOne.DeactivateAllUnits();
			teamTwo.DeactivateAllUnits();

			Stadium.current.GenerateMaze();

			CreateBall();

			//Launch ball at some random location on the entire field
			LaunchBallAtRandomLocationOnField(null, 20f);
			ball.Show();

			//Place player in front of their goal
			PlaceUnitInFrontOfGoal(teamOne);

			teamOne.strategy.stance = Stance.Offensive;

			//Start penalty match
			isPlaying = true;
			Time.timeScale = 1f;
		}

		void PlaceUnitInFrontOfGoal(Team team)
		{
			team.energy = 100;
			team.SpawnUnit(Stadium.current.frontOfGoal.position);
		}
	}
}