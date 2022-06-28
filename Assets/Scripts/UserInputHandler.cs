using UnityEngine;

namespace LeMinhHuy
{
	[RequireComponent(typeof(UserInput))]
	public class UserInputHandler : MonoBehaviour
	{
		//Members
		UserInput uIn;
		GameController gCon;

		void Awake()
		{
			uIn = GetComponent<UserInput>();
			gCon = GameController.current;
		}

		void OnEnable()
		{
			//Enable user input for these events
			gCon.onBeginRound.AddListener(RegisterInputEvents);
			gCon.onUnpause.AddListener(RegisterInputEvents);

			//Disable user input for these events
			gCon.onPause.AddListener(UnregisterInputEvents);
			gCon.onEndRound.AddListener(UnregisterInputEvents);
		}

		void OnDisable()
		{
			//Enable user input for these events
			gCon.onBeginRound.RemoveListener(RegisterInputEvents);
			gCon.onUnpause.RemoveListener(RegisterInputEvents);

			//Disable user input for these events
			gCon.onPause.RemoveListener(UnregisterInputEvents);
			gCon.onEndRound.RemoveListener(UnregisterInputEvents);
		}

		public void RegisterInputEvents()
		{
			//Only register user input events if the team is Player controlled
			if (gCon.teamOne.userType == UserType.Player)
			{
				Debug.Log("TeamOne Input Registered", this);
				uIn.onScreenPosInput.AddListener(gCon.teamOne.Void_SpawnUnitAtScreenPoint);
			}
			if (gCon.teamTwo.userType == UserType.Player)
			{
				Debug.Log("TeamTwo Input Registered", this);
				uIn.onScreenPosInput.AddListener(gCon.teamTwo.Void_SpawnUnitAtScreenPoint);
			}
		}

		public void UnregisterInputEvents()
		{
			// Debug.Log("UnregisterInputEvents", this);
			if (gCon.teamOne.userType == UserType.Player)
			{
				Debug.Log("TeamOne Input Unregistered", this);
				uIn.onScreenPosInput.RemoveListener(gCon.teamOne.Void_SpawnUnitAtScreenPoint);
			}
			if (gCon.teamTwo.userType == UserType.Player)
			{
				Debug.Log("TeamTwo Input Unregistered", this);
				uIn.onScreenPosInput.RemoveListener(gCon.teamTwo.Void_SpawnUnitAtScreenPoint);
			}
		}
	}
}