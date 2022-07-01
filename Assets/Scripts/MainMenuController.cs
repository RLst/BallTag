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
		[SerializeField] TextMeshProUGUI arToggleButtonText;
		[SerializeField] Button tapToStart;

		Canvas canvas;
		GameController game => GameController.current;

		void Awake()
		{
			DontDestroyOnLoad(this);
			canvas = GetComponent<Canvas>();
			Show();
		}

		void Start()
		{
			tapToStart.onClick.AddListener(() => game.BeginMatch());
		}

		public void SetActiveTapToStart(bool active) => tapToStart.enabled = (active);
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

				tapToStart.enabled = (false);
			}
			else
			{
				if (currentScene.name == mainSceneName)
					throw new System.Exception("Invalid scene name");

				SceneManager.LoadScene(mainSceneName);

				arToggleButtonText.text = "AR\nON";
			}
		}

		public void Quit()
		{
			Application.Quit();
		}
	}
}