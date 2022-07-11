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

		public UnityEvent onBeginMatch;     //ie. Change game music
		public UnityEvent onEndMatch;       //ie. Revert game music

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
			print("gamecontrollerrelay.registerevents");
			game.onBeginMatch.AddListener(OnBeginMatch);
			game.onEndMatch.AddListener(_ => OnEndMatch());
		}

		void OnBeginMatch()
		{
			onBeginMatch.Invoke();
		}

		void OnEndMatch()
		{
			onEndMatch.Invoke();
		}
	}
}