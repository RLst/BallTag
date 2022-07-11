using UnityEngine;

namespace LeMinhHuy
{
	[RequireComponent(typeof(GameController), typeof(AudioSource))]
	public class EndRoundSoundHandler : MonoBehaviour
	{
		//Inspector
		[SerializeField] AudioClip win;
		[SerializeField] AudioClip lose;
		[SerializeField] AudioClip draw;

		GameController game;
		AudioSource audioSource;
		void Awake()
		{
			game = GetComponent<GameController>();
			audioSource = GetComponent<AudioSource>();
		}

		void Start()
		{
			game.onEndRound.AddListener(OnEndRound);
		}

		public void OnEndRound((Team team, Result result) teamResults)
		{
			switch (teamResults.result)
			{
				case Result.Wins:
					audioSource.PlayOneShot(win);
					break;
				case Result.Lose:
					audioSource.PlayOneShot(lose);
					break;
				case Result.Draw:
					audioSource.PlayOneShot(draw);
					break;
			}
		}
	}
}