using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace LeMinhHuy
{
	/// <summary>
	/// This basically relays the game controller
	/// </summary>
	public class GameControllerRelay : MonoBehaviour
	{
		//This ALWAYS gets the current GameController
		// GameController game => GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();  //Do not use this too often
		GameController game => FindObjectOfType<GameController>();

		[Tooltip("If the match ends in a draw, then don't invoke OnEndMatch event")]
		[SerializeField] bool ignoreDrawsOnEndMatch = true;

		public UnityEvent onBeginMatch;     //ie. Change game music
		public UnityEvent onEndMatch;       //ie. Revert game music
		public UnityEvent onPenaltyMatch;

		// void Start()
		// {
		// 	SceneManager.sceneLoaded += OnSceneLoaded;
		// }

		// private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
		// {
		// 	//Only register once game
		// 	print("scene loaded");

		// 	RegisterEvents();
		// }

		/// <summary>
		/// Call this every time you switch modes to re-register events
		/// </summary>
		public void RegisterEvents()
		{
			// print("gamecontrollerrelay.registerevents");
			game.onBeginMatch.AddListener(OnBeginMatch);
			game.onEndMatch.AddListener(OnEndMatch);
			game.onBeginPenaltyRound.AddListener(OnBeginPenaltyRound);
		}

		void OnBeginMatch()
		{
			onBeginMatch.Invoke();
		}

		void OnEndMatch((Team, Result results) TeamResults)
		{
			//TODO: BAD: This basically prevents menu music from resuming in case there's a penalty round
			if (ignoreDrawsOnEndMatch && TeamResults.results == Result.Draw)
				return;

			onEndMatch.Invoke();
		}

		void OnBeginPenaltyRound()
		{
			onPenaltyMatch.Invoke();
		}
	}
}