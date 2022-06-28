using UnityEngine;
using UnityEngine.SceneManagement;

namespace LeMinhHuy
{
	/// <summary>
	/// Handle menus etc
	/// </summary>
	public class MainMenuController : MonoBehaviour
	{
		[SerializeField] string mainSceneName = "Main";
		[SerializeField] string ARSceneName = "AR";

		GameController gc;
		GameParameters gp;

		void Awake()
		{
			DontDestroyOnLoad(this);
			gc = FindObjectOfType<GameController>();
			gp = gc.parameters;
		}

		public void ToggleARMode()
		{
			gp.isARMode = !gp.isARMode;

			var currentScene = SceneManager.GetActiveScene();

			if (gp.isARMode)
			{
				if (currentScene.name == ARSceneName)
					return;

				SceneManager.LoadScene(ARSceneName);
			}
			else
			{
				if (currentScene.name == mainSceneName)
					return;

				SceneManager.LoadScene(mainSceneName);
			}
		}

		public void Quit()
		{
			Application.Quit();
		}
	}
}