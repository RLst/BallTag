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
			if (teamResults.result == Result.Draw)
			{
				//You draw
				audioSource.PlayOneShot(draw);
			}
			else if (teamResults.team == game.teamOne)
			{
				//You win
				if (teamResults.result == Result.Wins)
					audioSource.PlayOneShot(win);
				else
					audioSource.PlayOneShot(lose);
			}
			else if (teamResults.team == game.teamTwo)
			{
				//You lose
				if (teamResults.result == Result.Wins)
					audioSource.PlayOneShot(lose);
				else
					audioSource.PlayOneShot(win);
			}
		}
	}
}