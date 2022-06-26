
using System.Collections.Generic;
using UnityEngine;

namespace LeMinhHuy
{
	/// <summary>
	/// Handle menus etc
	/// </summary>
	[RequireComponent(typeof(MatchController))]
	public class GameController : MonoBehaviour
	{
		MatchController mc;

		void Awake()
		{
			mc = GetComponent<MatchController>();
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

	/// <summary>
	/// Controls the main game
	/// </summary>
	public class MatchController : MonoBehaviour
	{
		public List<Team> teams = new List<Team>();

		void Update()
		{

		}

		public void AddNewPlayer()
		{
			
		}
	}
}