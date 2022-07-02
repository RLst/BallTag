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
	public partial class GameController : Singleton<GameController>     //Rename to GameManager or GameController?
	{
		//Inspector
		[SerializeField] bool playDemoOnStart = true;
		[Space]
		[Tooltip("Tick this if this scene is for AR")]
		public bool isARMode = false;
		[SerializeField] public bool isPlaying = false;     //Controls team and unit logic and AI
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

		//Make these an array if you ever need more than two teams
		[Header("Teams")]
		public Team teamOne;    //This is the player
		public Team teamTwo;

		//Events
		// [Header("Events")]
		[HideInInspector] public UnityEvent onBeginMatch;
		[HideInInspector] public UnityEvent onBeginRound;
		[HideInInspector] public UnityEvent onPause;
		[HideInInspector] public UnityEvent onUnpause;
		[HideInInspector] public ResultEvent onEndRound;
		[HideInInspector] public ResultEvent onEndMatch;
		[HideInInspector] public UnityEvent onBeginPenaltyRound;

		//Members
		bool isPlayingDemo = false;
		Ball ball;
		WaitForSeconds waitOneSecond;
		//AR
		ARRaycastManager arRaycastManager;
		ARGamePlacer arGamePlacer;

		//INITS
		void OnValidate() => CalculateAttackDirectionForEachTeam();
		void Awake()
		{
			Debug.Assert(settings != null, "No game parameters found!");
			Debug.Assert(teamOne != null, "Team one not found!");
			Debug.Assert(teamTwo != null, "Team two not found!");

			if (isARMode)
			{
				arRaycastManager = FindObjectOfType<ARRaycastManager>();
				arGamePlacer = FindObjectOfType<ARGamePlacer>();
				Debug.Assert(arGamePlacer != null, "No ARGamePlacer found!");
				Debug.Assert(arRaycastManager != null, "No ARRaycastManager found!");
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
			print("Playing Demo");

			//Bespoke settings only for the demo to not screw around with the main game
			isPlaying = true;
			isPlayingDemo = true;
			currentRound = 0;

			//Generate random colors for each team
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

			CreateBall();

			SetOpponents();

			BeginRound();
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
			Debug.Log("Begin Match");

			currentRound = 0;
			isPlayingDemo = false;

			SetOpponents();
			InitTeams();
			CalculateAttackDirectionForEachTeam();

			CreateBall();

			BeginRound();

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
		void CreateBall()
		{
			if (ball is object) return;

			ball = Instantiate<Ball>(ballPrefab);
			ball.ResetParentToStadium();
		}

		/// <summary>
		/// Starts a new/next round
		/// Drops ball somewhere random over in attacker's field
		/// Set/Swap each team's strategy
		/// Set round number and time left
		/// </summary>
		public void BeginRound()
		{
			//Guard
			if (currentRound >= settings.roundsPerMatch)
			{
				EndMatch();
				return;
			}

			Debug.Log("Begin Round");

			isPlaying = true;
			Time.timeScale = 1f;

			//Increment next round
			currentRound++;

			//Resets
			currentRoundRemainingTime = settings.startingRoundRemainingTime;
			teamOne.energy = 0;
			teamTwo.energy = 0;
			ball.ResetParentToStadium();
			ball.Show();

			//Launch ball on the offensive side (if there is one)
			switch (teamOne.strategy.stance)
			{
				case Stance.Offensive:
					{
						LaunchBallAtRandomLocationOnField(teamOne.field);
						ball.Show();    //let the ball bounce
					}
					break;

				case Stance.Defensive:
					{
						LaunchBallAtRandomLocationOnField(teamTwo.field);
						ball.Show();
					}
					break;
			}

			//Clear all units from the playing field
			teamOne.DespawnAllUnits();
			teamTwo.DespawnAllUnits();

			onBeginRound.Invoke();
		}
		void LaunchBallAtRandomLocationOnField(Field field = null, float releaseHeight = 15f)
		{
			if (field is null)
			{
				//Select a random field
				if (UnityEngine.Random.value > 0.5f)
				{
					field = teamOne.field;
				}
				else
				{
					field = teamTwo.field;
				}
			}

			ball.transform.SetPositionAndRotation(field.GetRandomLocationOnField(releaseHeight), Quaternion.identity);
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
					EndRound((null, Result.Draw));
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
			Debug.Log("End Round");

			isPlaying = false;
			Time.timeScale = settings.endOfRoundTimescale;

			//Stop units moving and scoring etc but still keep them on screen indefinitely
			teamOne.DeactivateAllUnits(indefinite: true);
			teamTwo.DeactivateAllUnits(indefinite: true);

			ball.ResetParentToStadium();

			//Don't let demo mess things up like the score and team stances
			if (isPlayingDemo)
			{
				//Keep restarting the demo
				Invoke("BeginDemo", 1f);
				return;
			}

			//Set scores
			if (teamResult.result == Result.Wins)
			{
				teamResult.team.wins++;
			}
			else if (teamResult.result == Result.Draw)
			{
				teamOne.draws++;
				teamTwo.draws++;
			}

			//Switch team stances
			var temp = teamOne.strategy;
			teamOne.strategy = teamTwo.strategy;
			teamTwo.strategy = temp;

			onEndRound.Invoke(teamResult);
		}

		//End the match immediately. If early then you lose
		//Show match end screen
		//Start playing ending animations and cutscenes
		public void EndMatch()
		{
			print("End of Main Match");

			isPlaying = false;

			//Turn everything off
			teamOne.DeactivateAllUnits(indefinite: true);
			teamTwo.DeactivateAllUnits(indefinite: true);

			//Resolve match
			//ie. Display final screen with stats, prompt to start a new match, turn main menu back on
			//onEndMatch being listened by GameUIController and PenaltyMatchSystem?
			if (teamOne.wins == teamTwo.wins)
			{
				//DRAW; play penalty match
				//Also delay displaying the final game over screen with stats etc
				onEndMatch.Invoke((teamOne, Result.Draw));
			}
			else if (teamOne.wins > teamTwo.wins)
			{
				//Player wins
				onEndMatch.Invoke((teamOne, Result.Wins));
			}
			else if (teamOne.wins < teamTwo.wins)
			{
				//Player loses
				onEndMatch.Invoke((teamOne, Result.Lose));
			}
		}

		/// <summary>
		/// Completely end all rounds, matches and game
		/// </summary>
		public void GameOver()
		{
			//Reload the main scene (slight slack)
			MainMenuController.current.ReloadGameScene();
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

		//TESTS
		public void TestDraw()
		{
			EndRound((teamOne, Result.Draw));
		}
		public void TestWin()
		{
			EndRound((teamOne, Result.Wins));
		}
		public void TestLose()
		{
			EndRound((teamOne, Result.Lose));
		}
	}
}