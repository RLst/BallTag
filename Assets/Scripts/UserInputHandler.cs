using UnityEngine;

namespace LeMinhHuy
{
	[RequireComponent(typeof(UserInput))]
	public class UserInputHandler : MonoBehaviour
	{
		//Members
		UserInput ui;
		GameController controller;

		void Awake()
		{
			ui = GetComponent<UserInput>();
			controller = GameController.current;
		}

		void OnEnable()
		{
			//Enable user input for these events
			// controller.onBeginMatch.AddListener(RegisterInputEvents);
			controller.onBeginRound.AddListener(RegisterInputEvents);
			controller.onUnPause.AddListener(RegisterInputEvents);

			//Disable user input for these events
			controller.onPause.AddListener(UnregisterInputEvents);
			controller.onEndRound.AddListener(UnregisterInputEvents);
			// controller.onEndMatch.AddListener(UnregisterInputEvents);
		}

		public void RegisterInputEvents()
		{
			Debug.Log("RegisterInputEvents", this);
			ui.onScreenPosInput.AddListener(controller.teamOne.TrySpawnUnitAtScreenPoint);
			ui.onScreenPosInput.AddListener(controller.teamTwo.TrySpawnUnitAtScreenPoint);
		}

		public void UnregisterInputEvents()
		{
			Debug.Log("UnregisterInputEvents", this);
			ui.onScreenPosInput.RemoveListener(controller.teamOne.TrySpawnUnitAtScreenPoint);
			ui.onScreenPosInput.RemoveListener(controller.teamTwo.TrySpawnUnitAtScreenPoint);
		}
	}
}