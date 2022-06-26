using UnityEngine;

namespace LeMinhHuy
{
	/// <summary>
	/// Handle menus etc
	/// </summary>
	[RequireComponent(typeof(Umpire))]
	public class MenuController : MonoBehaviour
	{
		Umpire umpire;

		void Awake()
		{
			umpire = GetComponent<Umpire>();
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