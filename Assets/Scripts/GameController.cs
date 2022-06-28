using System;
using System.Collections;
using System.Collections.Generic;
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
		[SerializeField] bool isPlaying = false;
		[SerializeField] bool isPaused = false;
		//MATCH PARAMS
		public int currentRound { get; private set; } = 0;
		public float currentRoundRemainingTime { get; private set; } = -1;

		[Space]
		public GameParameters parameters;
		public Unit genericUnitPrefab;

		[Header("AR")]
		public ARSessionOrigin arSessionOrigin;
		public ARRaycastManager arRaycastManager;

		//Make these an array if you ever need more than two teams
		[Header("Teams")]
		public Team teamOne;
		public Team teamTwo;

		//Events
		[Header("Events")]
		public UnityEvent onBeginMatch;
		public UnityEvent onBeginRound;
		public UnityEvent onPause;
		public UnityEvent onUnpause;
		public UnityEvent onEndRound;
		public UnityEvent onEndMatch;


		//INITS
		void OnValidate() => CalculateAttackDirectionForEachTeam();
		void Awake()
		{
			Debug.Assert(parameters != null, "No game parameters found!");

			Debug.Assert(teamOne != null, "Team one not found!");
			Debug.Assert(teamTwo != null, "Team two not found!");

			if (isARMode)
			{
				Debug.Assert(arRaycastManager != null, "No ARRaycastManager found!");
			}
		}
		void Start()
		{
			CalculateAttackDirectionForEachTeam();
			onBeginMatch.AddListener(() =>
			{
				StartCoroutine(TickTeams());
			});
		}
		public void CalculateAttackDirectionForEachTeam()
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

		#region Gameplay
		public void BeginMatch()
		{
			//Teams need to know their opponents
			teamOne.opponent = teamTwo;
			teamTwo.opponent = teamOne;

			teamOne.Initialize(parameters.teamOneSettings);
			teamTwo.Initialize(parameters.teamTwoSettings);
			CalculateAttackDirectionForEachTeam();

			currentRound = 0;
			BeginRound();

			isPlaying = true;

			//Hook up user input events etc
			Debug.Log("Begin Match");
			onBeginMatch.Invoke();
		}

		public void BeginRound()
		{
			//Guard
			if (currentRound >= parameters.roundsPerMatch)
			{
				EndRound();
				return;
			}

			currentRound++;
			currentRoundRemainingTime = parameters.startingRoundRemainingTime;

			//Don't swap strategies for the first round
			if (currentRound > 1)
			{
				switch (teamOne.strategy.stance)
				{
					case Stance.Offensive:
						teamOne.strategy = parameters.defensiveStrategy;
						teamTwo.strategy = parameters.offensiveStrategy;
						break;

					case Stance.Defensive:
						teamOne.strategy = parameters.offensiveStrategy;
						teamTwo.strategy = parameters.defensiveStrategy;
						break;
				}
			}

			Debug.Log("Begin Round");
			onBeginRound.Invoke();
		}

		public void EndRound()
		{
			Debug.Log("End Round");
			onEndRound.Invoke();
		}

		void BeginPenaltyRound() { }

		//End the match immediately
		public void EndMatch()
		{
			//Resolve match
			isPlaying = false;

			//ie. stop input
			onEndMatch.Invoke();
		}
		#endregion

		//CORE
		void Update()
		{
			if (isPlaying == false)
				return;

			//Handle round timer IF a round is currently going on
			if (currentRoundRemainingTime > 0)
			{
				currentRoundRemainingTime -= Time.deltaTime;

				if (currentRoundRemainingTime <= 0)
				{
					EndRound();
				}
			}

			//Update teams
			teamOne.Update();
			teamTwo.Update();
		}

		//AI
		WaitForSeconds waitOneSecond = new WaitForSeconds(1f);  //will always be one second
		IEnumerator TickTeams()
		{
			while (true)
			{
				// Debug.Log("TickTeams()");
				teamOne.Tick();
				teamTwo.Tick();
				yield return new WaitForSeconds(1f);
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