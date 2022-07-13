using System;
using UnityEngine;

namespace LeMinhHuy
{
	public partial class GameController : MonoBehaviour
	{
		/// <summary>
		/// NOTE: The logic in here should be as self-contained, bespoke and complete isolated from the rest of GameController
		/// Invoke when the match is a draw
		/// Generate maze, clear area around goals, rebake field navmesh, place ball at random, place a unit at team goal, let run attacker AI logic
		/// </summary>
		public void BeginPenaltyRound()
		{
			var stadium = Stadium.current;

			//This may be called before any matches begin ie. Penalty game mode
			teamOne.DeactivateAllUnits();
			teamTwo.DeactivateAllUnits();

			stadium.GenerateMaze();

			//Prevent overwriting game settings and messing things up
			teamOne.strategy = new Strategy();  //Defaults to offensive strategy

			//Start penalty match
			isPlaying = true;
			isPenaltyRound = true;
			Time.timeScale = 1f;
			currentRoundRemainingTime = settings.penaltyRoundRemainingTime;

			//Spawn unit in front of goal
			teamOne.energy = int.MaxValue;
			teamOne.SpawnUnit(stadium.frontOfGoal.position);

			//Launch ball at some random location on the entire field
			CreateBall();
			ball.ResetParentToStadium();
			LaunchBallAtRandomLocationOnField(null, 20f);
			ball.Show();

			onBeginPenaltyRound.Invoke();
		}

	}
}