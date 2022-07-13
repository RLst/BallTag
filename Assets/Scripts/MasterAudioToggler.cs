using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LeMinhHuy
{
	public class MasterAudioToggler : MonoBehaviour
	{
		//Inspector
		// [SerializeField] AudioMixer masterMixer;
		[SerializeField] AudioMixerSnapshot soundEnabled;
		[SerializeField] AudioMixerSnapshot soundDisabled;
		[SerializeField] float transitionTime = 0.3f;

		[Space]
		public UnityEvent onSoundOn;
		public UnityEvent onSoundOff;

		//Members
		bool isSoundActive = true;

		public void Toggle()
		{
			isSoundActive = !isSoundActive;

			if (isSoundActive)
			{
				soundEnabled.TransitionTo(transitionTime);
				onSoundOn.Invoke();
			}
			else
			{
				soundDisabled.TransitionTo(transitionTime);
				onSoundOff.Invoke();
			}
		}
	}
}