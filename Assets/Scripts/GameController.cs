using System;
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
		//MATCH PARAMS
		[field: SerializeField] public int currentRound { get; private set; } = 0;
		[field: SerializeField] public float currentRoundRemainingTime { get; private set; } = -1;

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
		public UnityEvent onUnPause;
		public UnityEvent onEndRound;
		public UnityEvent onEndMatch;

		//Official game running flag
		public bool isPlaying = false;
		public bool isPaused = false;


		//INITS
		void OnValidate() => CalculateAttackDirection();
		void Awake()
		{
			Debug.Assert(parameters != null, "No game parameters found!");

			Debug.Assert(teamOne != null, "Team one not found!");
			Debug.Assert(teamTwo != null, "Team two not found!");

			if (parameters.isARMode)
			{
				Debug.Assert(arRaycastManager != null, "No ARRaycastManager found!");
			}
		}
		void Start() => CalculateAttackDirection();
		public void CalculateAttackDirection()
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

		public void BeginMatch()
		{
			teamOne.Initialise(parameters.teamOneSettings);
			teamTwo.Initialise(parameters.teamTwoSettings);
			CalculateAttackDirection();

			currentRound = 0;
			BeginRound();

			isPlaying = true;

			//Hook up user input events etc
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

		void EndRound()
		{
			Debug.Log("End Round");
			onEndRound.Invoke();
		}

		//End the match immediately
		public void EndMatch()
		{
			//Resolve match
			isPlaying = false;

			//ie. stop input
			onEndMatch.Invoke();
		}

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

			//Energy
			teamOne.HandleEnergy();
			teamTwo.HandleEnergy();
			// foreach (var t in teams)
			// 	t.HandleEnergy();
		}


		//Pause/Unpause game
		public void Pause()
		{
			Time.timeScale = 0f;
			isPaused = true;
			onPause.Invoke();
		}
		public void UnPause()
		{
			Time.timeScale = 1f;
			isPaused = false;
			onUnPause.Invoke();
		}
	}
}