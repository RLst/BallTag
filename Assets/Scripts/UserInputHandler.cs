using UnityEngine;

namespace LeMinhHuy
{
	[RequireComponent(typeof(UserInput))]
	public class UserInputHandler : MonoBehaviour
	{
		//Members
		UserInput uIn;
		GameController game;

		void Awake()
		{
			uIn = GetComponent<UserInput>();
			game = GameController.current;
		}

		void Start()
		{
			//Enable user input for these events
			game.onBeginRound.AddListener(RegisterInputEvents);
			game.onUnpause.AddListener(RegisterInputEvents);

			//Disable user input for these events
			game.onPause.AddListener(UnregisterInputEvents);
			game.onEndRound.AddListener(_ => UnregisterInputEvents());
		}

		public void RegisterInputEvents()
		{
			//Only register user input events if the team is Player controlled
			if (game.teamOne.userType == UserType.Player)
			{
				Debug.Log("TeamOne Input Registered", this);
				uIn.onScreenPosInput.AddListener(game.teamOne.Void_SpawnUnitAtScreenPoint);
			}
			if (game.teamTwo.userType == UserType.Player)
			{
				Debug.Log("TeamTwo Input Registered", this);
				uIn.onScreenPosInput.AddListener(game.teamTwo.Void_SpawnUnitAtScreenPoint);
			}
		}

		public void UnregisterInputEvents()
		{
			// Debug.Log("UnregisterInputEvents", this);
			if (game.teamOne.userType == UserType.Player)
			{
				Debug.Log("TeamOne Input Unregistered", this);
				uIn.onScreenPosInput.RemoveListener(game.teamOne.Void_SpawnUnitAtScreenPoint);
			}
			if (game.teamTwo.userType == UserType.Player)
			{
				Debug.Log("TeamTwo Input Unregistered", this);
				uIn.onScreenPosInput.RemoveListener(game.teamTwo.Void_SpawnUnitAtScreenPoint);
			}
		}
	}
}