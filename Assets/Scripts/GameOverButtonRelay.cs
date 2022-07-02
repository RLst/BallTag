using UnityEngine;

namespace LeMinhHuy
{
	public class GameOverButtonRelay : MonoBehaviour
	{
		public void GameOver()
		{
			MainMenuController.current.ReloadGameScene();
		}
	}
}