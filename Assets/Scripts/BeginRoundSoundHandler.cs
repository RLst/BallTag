using UnityEngine;

namespace LeMinhHuy
{
	[RequireComponent(typeof(GameController), typeof(AudioSource))]

	public class BeginRoundSoundHandler : MonoBehaviour
	{
		//Stop all field sounds from player ie. crowd cheer, boos,
		//Start playing general crowd noise

		[SerializeField] AudioClip crowdGeneral;

		GameController game;
		AudioSource audioSource;
		void Awake()
		{
			game = GetComponent<GameController>();
			audioSource = GetComponent<AudioSource>();
		}

		void Start()
		{
			game.onBeginRound.AddListener(OnBeginRound);
			game.onBeginPenaltyRound.AddListener(OnBeginRound);
		}

		public void OnBeginRound()
		{
			if (game.isPlayingDemo) return;  //Don't play if it's the demo

			audioSource.Stop();

			audioSource.clip = crowdGeneral;
			audioSource.loop = true;
			audioSource.Play();
		}
	}
}