using UnityEngine;

namespace LeMinhHuy
{
	/// <summary>
	/// Handle menus etc
	/// </summary>
	[RequireComponent(typeof(GameController))]
	public class MenuController : MonoBehaviour
	{
		GameController umpire;

		void Awake()
		{
			umpire = GetComponent<GameController>();
		}

		void Update()
		{

		}

		void Pause()
		{
			Time.timeScale = 0;
		}

		void UnPause()
		{
			Time.timeScale = 1;
		}
	}
}