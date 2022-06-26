using UnityEngine;

namespace LeMinhHuy
{
	[RequireComponent(typeof(UserInput))]
	public class UserInputHandler : MonoBehaviour
	{
		//Members
		UserInput ui;
		Umpire umpire;

		void Awake()
		{
			ui = GetComponent<UserInput>();
			umpire = Umpire.current;
		}

		void OnEnable()
		{
			ui.onScreenPosInput.AddListener(umpire.teamOne.TrySpawnUnitAtScreenPoint);
			ui.onScreenPosInput.AddListener(umpire.teamTwo.TrySpawnUnitAtScreenPoint);
		}
	}
}