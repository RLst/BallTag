using UnityEngine;

namespace LeMinhHuy
{
	[RequireComponent(typeof(UserInput))]
	public class UserInputHandler : MonoBehaviour
	{
		//Members
		UserInput ui;
		GameController umpire;

		void Awake()
		{
			ui = GetComponent<UserInput>();
			umpire = GameController.current;
		}

		void OnEnable()
		{
			ui.onScreenPosInput.AddListener(umpire.teamOne.TrySpawnUnitAtScreenPoint);
			ui.onScreenPosInput.AddListener(umpire.teamTwo.TrySpawnUnitAtScreenPoint);
		}
	}
}