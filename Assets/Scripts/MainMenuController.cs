using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LeMinhHuy
{
	/// <summary>
	/// Handle menus etc
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
		[SerializeField] Button tapToStartSensor;

		//Members
		Canvas canvas;
		GameController game
		{
			//Will try to always find the current GameController depending on the scene
			get
			{
				if (_game is null)
					_game = FindObjectOfType<GameController>();
				return _game;
			}
		}
		private GameController _game;

		// protected override void Init()
		void Awake()
		{
			DontDestroyOnLoad(this);
			canvas = GetComponent<Canvas>();
			Show();
		}

		void Start()
		{
			tapToStartSensor.onClick.AddListener(() => game.BeginMatch());
		}

		public void SetActiveStartSensor(bool active) => tapToStartSensor.enabled = (active);
		public void SetActiveTitles(bool active) => titles.SetActive(active);

		public void Show() => canvas.enabled = true;
		public void Hide() => canvas.enabled = false;

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