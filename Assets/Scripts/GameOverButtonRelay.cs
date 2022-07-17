using UnityEngine;

namespace LeMinhHuy
{
	public class GameOverButtonRelay : MonoBehaviour
	{
		MainMenuController mm;
		void Awake() => mm = MainMenuController.current;
		public void GameOver()
		{
			//Reload the current game scene and unhide the main menu
			mm.ReloadGameScene();
			mm.Show();
		}
	}
}