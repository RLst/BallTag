using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LeMinhHuy
{
	/// <summary>
	/// Handle main menus
	/// </summary>
	[RequireComponent(typeof(Canvas))]
	public class MainMenuController : Singleton<MainMenuController>
	{
		//Since this is a floating dont destroy on load class it shouldn't directly reference anything else other than itself
		[SerializeField] string mainSceneName = "Main";
		[SerializeField] string ARSceneName = "AR";

		[Space]
		[SerializeField] TextMeshProUGUI arToggleButtonText;
		[SerializeField] GameObject titles;     //The gameobject that holds all of the titls and prompts etc
		[SerializeField] Button startSensor;

		[Space]
		[Tooltip("On switching between normal and AR mode")]
		public UnityEvent onSwitchMode;

		//Members
		Canvas canvas;
		GameController game
		{
			//Will try to always find the current GameController depending on the scene
			get
			{
				// if (_game is null)
				_game = FindObjectOfType<GameController>();
				return _game;
			}
		}
		private GameController _game;

		void Awake()
		{
			DontDestroyOnLoad(this);
			canvas = GetComponent<Canvas>();
			Show();
		}

		void Start()
		{
			startSensor.onClick.AddListener(OnStart);
		}

		public void SetActiveStartSensor(bool active) => startSensor.enabled = (active);
		public void SetActiveTitles(bool active) => titles.SetActive(active);

		public void Show() => canvas.enabled = true;
		public void Hide() => canvas.enabled = false;

		public void OnStart()
		{
			game.BeginMatch();
		}

		public void ToggleARMode()
		{
			game.isARMode = !game.isARMode;

			var currentScene = SceneManager.GetActiveScene();

			if (game.isARMode)
			{
				if (currentScene.name == ARSceneName)
					throw new System.Exception("Invalid scene name");

				SceneManager.LoadScene(ARSceneName);

				arToggleButtonText.text = "AR\nOFF";
			}
			else
			{
				if (currentScene.name == mainSceneName)
					throw new System.Exception("Invalid scene name");

				SceneManager.LoadScene(mainSceneName);

				arToggleButtonText.text = "AR";
			}

			onSwitchMode.Invoke();
		}

		public void ReloadGameScene()
		{
			if (game.isARMode)
			{
				SceneManager.LoadScene(ARSceneName);
			}
			else
			{
				SceneManager.LoadScene(mainSceneName);
			}
		}

		public void Quit()
		{
			Application.Quit();
		}
	}
}