using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LeMinhHuy
{
	[RequireComponent(typeof(AudioSource))]
	public class AudioSourceToggler : MonoBehaviour
	{
		//Inspector
		// [SerializeField] Image audioToggleImage = null;
		// [SerializeField] Sprite soundOn;
		// [SerializeField] Sprite soundOff;

		public UnityEvent onSoundOn;
		public UnityEvent onSoundOff;

		//Members
		AudioSource audioSource;

		void Start()
		{
			audioSource = GetComponent<AudioSource>();
		}

		public void Toggle()
		{
			audioSource.enabled = !audioSource.enabled;

			if (audioSource.enabled)
				onSoundOn.Invoke();
			else
				onSoundOff.Invoke();
		}
	}
}