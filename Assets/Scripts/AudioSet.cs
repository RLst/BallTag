using System.Collections.Generic;
using UnityEngine;

namespace LeMinhHuy
{
	/// <summary>
	/// Holds a collection of clips
	/// </summary>
	[CreateAssetMenu(menuName = "LeMinhHuy/AudioSet")]
	public class AudioSet : ScriptableObject
	{
		[Range(0f, 1f)] public float volume = 1f;
		[Range(0, 100)] public int chance = 50;
		public bool avoidRepetitions = true;

		//Members
		internal AudioClip lastPlayed = null;

		public List<AudioClip> clips;
	}
}