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
			teamTwo.DestroyAllUnits();

			//Create maze
			GenerateMaze();

			CreateBall();

			//Place the ball at some random location on the field
			LaunchBallAtRandomLocationOnField(null, 20f);
			ball.Show();

			//Place player in front of their goal
			PlaceUnitInFrontOfGoal(teamOne);
		}

		void PlaceUnitInFrontOfGoal(Team team)
		{
			throw new NotImplementedException();
		}

		void GenerateMaze()
		{

		}
	}
}