using System;
using System.Collections;
using System.Collections.Generic;
using LeMinhHuy.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;

namespace LeMinhHuy
{
	/// <summary>
	/// Basically the game manager; Registers details and starts the match
	/// Is the only one that is a monobehaviour and is in the scene so it can pass in objects etc
	/// </summary>
	public class GameController : Singleton<GameController>     //Rename to GameManager or GameController?
	{
		//Inspector
		[SerializeField] bool playDemoOnStart = true;
		[Space]
		public bool isARMode = false;
		[SerializeField] public bool isPlaying = false;
		[SerializeField] bool isPaused = false;
		//MATCH PARAMS
		public int currentRound { get; private set; } = 0;
		public float currentRoundRemainingTime { get; private set; } = -1;

		[Space]
		public GameSettings settings;

		[Header("Prefabs")]
		public Unit unitPrefab = null;
		[SerializeField] Ball ballPrefab = null;
		[SerializeField] float ballReleaseHeight = 10f;

		[Header("AR")]
		public ARRaycastManager arRaycastManager;
		public ARGamePlacer arGamePlacer;

		//Make these an array if you ever need more than two teams
		[Header("Teams")]
		public Team teamOne;    //This is the player
		public Team teamTwo;

		//Events
		[Header("Events")]
		public UnityEvent onBeginMatch;
		public UnityEvent onBeginRound;
		public UnityEvent onPause;
		public UnityEvent onUnpause;
		public ResultEvent onEndRound;
		public ResultEvent onEndMatch;
		public UnityEvent onBeginPenaltyRound;

		//Members
		Ball ball;
		WaitForSeconds waitOneSecond;

		//INITS
		void OnValidate() => CalculateAttackDirectionForEachTeam();
		void Awake()
		{
			Debug.Assert(settings != null, "No game parameters found!");
			Debug.Assert(teamOne != null, "Team one not found!");
			Debug.Assert(teamTwo != null, "Team two not found!");

			if (isARMode)
			{
				Debug.Assert(arRaycastManager != null, "No ARRaycastManager found!");
				Debug.Assert(arGamePlacer != null, "No ARGamePlacer found!");
			}
		}
		void Start()
		{
			waitOneSecond = new WaitForSeconds(1f);

			CalculateAttackDirectionForEachTeam();

			//Register events
			onBeginRound.AddListener(() => StartCoroutine(TickTeams()));
			onEndRound.AddListener(_ => StopAllCoroutines());

			teamOne.onScoreGoal.AddListener(EndRound);
			teamOne.onNoActiveUnits.AddListener(EndRound);

			teamTwo.onScoreGoal.AddListener(EndRound);
			teamTwo.onNoActiveUnits.AddListener(EndRound);

			if (playDemoOnStart) BeginDemo();
		}
		internal void BeginDemo()
		{
			isPlaying = true;
			currentRound = 0;

			//Generate random colors
			teamOne.color = UnityEngine.Random.ColorHSV(
					hueMin: 0f, hueMax: 1f,
					saturationMin: 0.55f, saturationMax: 0.65f,
					valueMin: 0.9f, valueMax: 1f);
			teamOne.Awake();
			teamOne.InitTeamObjects();
			teamOne.InitUnitPool();
			teamOne.SetStance();

			teamTwo.color = UnityEngine.Random.ColorHSV(
					hueMin: 0f, hueMax: 1f,
					saturationMin: 0.55f, saturationMax: 0.65f,
					valueMin: 0.9f, valueMax: 1f);
			teamTwo.Awake();
			teamTwo.InitTeamObjects();
			teamTwo.InitUnitPool();
			teamTwo.SetStance();

			CalculateAttackDirectionForEachTeam();  //should already be done
			if (ball is null)
				ball = Instantiate<Ball>(ballPrefab);

			SetOpponents();
			BeginRound();

			// StartCoroutine(TickTeams());
			print("Playing demo");
		}

		#region Gameplay
		/// <summary>
		/// Prep teams
		/// Set team's opponent
		/// Create the ball
		/// Start the first round
		/// </summary>
		public void BeginMatch()
		{
			currentRound = 0;

			SetOpponents();
			InitTeams();
			CalculateAttackDirectionForEachTeam();

			//Create ball
			if (ball is null)
				ball = Instantiate<Ball>(ballPrefab);

			BeginRound();

			//Hook up user input events etc
			Debug.Log("Begin Match");
			onBeginMatch.Invoke();
		}
		void SetOpponents()
		{
			//Teams need to know their opponents
			teamOne.opponent = teamTwo;
			teamTwo.opponent = teamOne;
		}
		void InitTeams()
		{
			//Set colors and strategies
			teamOne.Initialize(settings.teamOneSettings);
			teamTwo.Initialize(settings.teamTwoSettings);
		}
		public void CalculateAttackDirectionForEachTeam()
		{
			//Calculate attack direction vector from one team to the other
			if (teamOne.field is object && teamTwo.field is object)
			{
				teamOne.attackDirection = (teamTwo.field.transform.position - teamOne.field.transform.position).normalized;
				teamTwo.attackDirection = (teamOne.field.transform.position - teamTwo.field.transform.position).normalized;
			}
		}

		/// <summary>
		/// Starts a new/next round
		/// Drops ball somewhere random over in attacker's field
		/// Set/Swap each team's strategy
		/// Set round number and time left
		/// </summary>
		public void BeginRound()
		{
			isPlaying = true;

			//Guard
			if (currentRound >= settings.roundsPerMatch)
			{
				EndMatch();
			}

			//Increment next round
			currentRound++;

			//Reset timer
			currentRoundRemainingTime = settings.startingRoundRemainingTime;

			//Reset and show the ball
			ResetBall();
			ball.Show();

			//Launch ball on the offensive side (if there is one)
			switch (teamOne.strategy.stance)
			{
				case Stance.Offensive:
					{
						//Launch ball
						ball.transform.SetPositionAndRotation(teamOne.field.GetRandomLocationOnField(ballReleaseHeight), Quaternion.identity);
						ball.Show();    //let the ball bounce
					}
					break;

				case Stance.Defensive:
					{
						//Launch ball
						ball.transform.SetPositionAndRotation(teamTwo.field.GetRandomLocationOnField(ballReleaseHeight), Quaternion.identity);
						ball.Show();
					}
					break;
			}

			//Clear all units from the playing field
			teamOne.DespawnAllUnits();
			teamTwo.DespawnAllUnits();

			Debug.Log("Begin Round");
			onBeginRound.Invoke();
		}
		//Hide the ball, release from parents, zero
		//NOTE: Ball should NEVER be deactivated
		internal void ResetBall()
		{
			print("Resetball");
			ball.Hide();
			ball.transform.SetParent(null);
			ball.transform.position = Vector3.zero;
		}

		/// <summary>
		/// Count down time left
		/// End round if out of time
		/// Let teams borrow MonoBehaviour.Update()
		/// </summary>
		void Update()
		{
			if (!isPlaying)
				return;

			//Handle round timer IF a round is currently going on
			if (currentRoundRemainingTime > 0)
			{
				currentRoundRemainingTime -= Time.deltaTime;

				if (currentRoundRemainingTime <= 0)
					EndRound((null, Result.Draws));
			}

			//Update teams
			teamOne.Update();
			teamTwo.Update();
		}

		/// <summary>
		/// Invoke when out of round time OR a goal is scored
		/// CIf no more rounds left then end match
		/// Rest a bit so player can continue
		/// </summary>
		public void EndRound((Team team, Result result) teamResult)
		{
			isPlaying = false;

			teamOne.DeactivateAllUnits(indefinite: true);
			teamTwo.DeactivateAllUnits(indefinite: true);

			//Set scores
			if (teamResult.result == Result.Wins)
			{
				teamResult.team.wins++;
			}
			else if (teamResult.result == Result.Draws)
			{
				teamResult.team.draws++;
			}

			//Switch team stances
			var temp = teamOne.strategy;
			teamOne.strategy = teamTwo.strategy;
			teamTwo.strategy = temp;

			Debug.Log("End Round");
			onEndRound.Invoke(teamResult);
		}


		//End the match immediately. If early then you lose
		//Show match end screen
		//Start playing ending animations and cutscenes
		public void EndMatch()
		{
			isPlaying = false;

			//Turn everything off
			teamOne.DeactivateAllUnits(indefinite: true);
			teamTwo.DeactivateAllUnits(indefinite: true);

			//Resolve match
			//onEndMatch being listened by GameUIController and PenaltyMatchSystem?
			if (teamOne.wins == teamTwo.wins)
			{
				//DRAW; play penalty match
				onEndMatch.Invoke((teamOne, Result.Draws));
			}
			else if (teamOne.wins > teamTwo.wins)
			{
				//Player wins
				onEndMatch.Invoke((teamOne, Result.Wins));
			}
			else if (teamOne.wins < teamTwo.wins)
			{
				//Player loses
				onEndMatch.Invoke((teamOne, Result.Loses));
			}

			//Also stop input
		}

		/// <summary>
		/// Invoke when the match is a draw
		/// Generate maze, clear area around goals, rebake field navmesh, place ball at random, place a unit at team goal, let run attacker AI logic
		/// </summary>
		void BeginPenaltyRound()
		{
			
		}
		#endregion


		//AI
		IEnumerator TickTeams()
		{
			while (isPlaying)
			{
				// Debug.Log("TickTeams()");
				teamOne.Tick();
				teamTwo.Tick();
				yield return waitOneSecond;
			}
		}

		//PAUSE
		public void Pause()
		{
			Time.timeScale = 0f;
			isPaused = true;
			onPause.Invoke();
		}
		public void Unpause()
		{
			Time.timeScale = 1f;
			isPaused = false;
			onUnpause.Invoke();
		}
	}
}