using UnityEngine;

namespace LeMinhHuy
{
	/// <summary>
	/// Works with Audio sets to play random sounds from it
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	public class AudioSystem : MonoBehaviour
	{
		//Inspector
		[TextArea(0, 2), SerializeField] string description = null;

		[Tooltip("Chance of clip playing. OVERRIDES the magazine's chance")]
		[Range(0, 100), SerializeField] int chance = 50;
		[SerializeField] AudioSet set = null;

		//Members
		AudioSource audioSource;
		void Awake()
		{
			if (!audioSource) audioSource = GetComponent<AudioSource>();
		}
		void OnValidate()
		{
			//No need to check for null because a audio source should always be attached to this gameobject
			audioSource = GetComponent<AudioSource>();
			audioSource.mute = false;
			audioSource.loop = false;
		}

		#region Play by Chance
		/// <summary>
		/// Plays the current loaded magazine according to the chance setting on this audio system
		/// </summary>
		public void PlayByChance()
		{
			if (!set) return;

			//Don't play the same sound over and over again
			while (true)
			{
				AudioClip clipToPlay = set.clips[UnityEngine.Random.Range(0, set.clips.Count)];
				if (clipToPlay == set.lastPlayed) continue;

				//Play by chance
				if (UnityEngine.Random.Range(0, 100) < set.chance)
				{
					audioSource.PlayOneShot(clipToPlay, set.volume);
					set.lastPlayed = clipToPlay;
				}
				break;
			}
		}
		/// <summary>
		/// Plays the selected clip according to the chance setting of this audio system
		/// </summary>
		/// <param name="clip">The audio clip to play based on chance</param>
		public void PlayByChance(AudioClip clip)
		{
			if (Random.Range(0, 100) < chance)
				audioSource.PlayOneShot(clip);
		}
		#endregion

		#region Animation Event Handlers
		public void PlayClipByChance(Object audioClip)
		{
			AudioClip ac = audioClip as AudioClip;
			if (Random.Range(0, 100) < chance)
				audioSource.PlayOneShot(ac);
		}

		/// <summary> Play one audio clip out of a magazine according to chance </summary>
		/// <param name="audioSet">AudioMagazine scriptable object</param>
		public void PlaySetByChance(Object audioSet)
		{
			//Cast
			AudioSet set = audioSet as AudioSet;
			if (!set)
			{
				print("Not an audio set!");
				return;
			}

			//Don't play the same sound over and over again
			//NOTE: With brief testing, the max iterations was 3. Usually 1 or 2. Performance is fine
			while (true)
			{
				AudioClip clipToPlay = set.clips[Random.Range(0, set.clips.Count)];
				if (set.avoidRepetitions &&
					set.clips.Count > 1 &&     //Need at least 2 clips to avoid repetitions
					clipToPlay == set.lastPlayed) continue;

				//Play by chance
				if (Random.Range(0, 100) < set.chance)
				{
					audioSource.outputAudioMixerGroup = set.mixerGroup;
					audioSource.PlayOneShot(clipToPlay, set.volume);
					set.lastPlayed = clipToPlay;
				}
				break;
			}
		}

		/// <summary>
		/// Play a random clip in a magazine by chance using passed in audiosource
		/// ie. in case the sounds need to be routed through a different mixer
		/// </summary>
		public void PlaySetByChance(AudioSet set, AudioSource source)
		{
			//Don't play the same sound over and over again
			//NOTE: With brief testing, the max iterations was 3. Usually 1 or 2. Performance is fine
			while (true)
			{
				AudioClip clipToPlay = set.clips[Random.Range(0, set.clips.Count)];
				if (set.avoidRepetitions &&
					set.clips.Count > 1 &&       //Need at least 2 clips to avoid repetitions, otherwise infinity loop
					clipToPlay == set.lastPlayed) continue;

				//Play by chance
				if (Random.Range(0, 100) < set.chance)
				{
					source.outputAudioMixerGroup = set.mixerGroup;
					source.PlayOneShot(clipToPlay, set.volume);
					set.lastPlayed = clipToPlay;
				}
				break;
			}
		}

		/// <summary> Play one audio clip once </summary>
		/// <param name="audioClip">Single AudioClip</param>
		public void PlayOnce(Object audioClip)
		{
			AudioClip ac = audioClip as AudioClip;
			audioSource.PlayOneShot(ac);
		}
		#endregion
	}
}