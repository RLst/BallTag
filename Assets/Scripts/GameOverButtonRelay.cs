using UnityEngine;

namespace LeMinhHuy
{
	public class GameOverButtonRelay : MonoBehaviour
	{
		MainMenuController mm;
		void Awake() => mm = MainMenuController.current;
		public void GameOver()
		{
			mm.ReloadGameScene();
			mm.Show();
		}
	}
}