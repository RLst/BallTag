using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LeMinhHuy
{
	[RequireComponent(typeof(Canvas))]
	public class GameUIController : MonoBehaviour
	{
		[SerializeField] TextMeshProUGUI timeLeft = null;
		[Header("Team One")]
		[SerializeField] TextMeshProUGUI teamOneDetails = null;
		[SerializeField] Slider teamOneEnergyBarInt = null;
		[SerializeField] Slider teamOneEnergyBarFloat = null;

		[Header("Team Two")]
		[SerializeField] TextMeshProUGUI teamTwoDetails = null;
		[SerializeField] Slider teamTwoEnergyBarInt = null;
		[SerializeField] Slider teamTwoEnergyBarFloat = null;
		[SerializeField] float floatGaugeAlpha = 0.5f;

		[SerializeField] GameObject endRoundScreen;

		//Members
		GameController game;
		Canvas c;

		void Awake()
		{
			game = GameController.current;
			c = GetComponent<Canvas>();
		}

		void Start()
		{
			//Hide main game UI
			c.enabled = false;
		}

		//Event rego
		void OnEnable()
		{
			game.onBeginMatch.AddListener(BeginMatchUI);
			game.onEndRound.AddListener(EndRoundUI);
			game.onEndMatch.AddListener(EndMatchUI);
		}
		void OnDisable()
		{
			game.onBeginMatch.RemoveListener(BeginMatchUI);
			game.onEndRound.RemoveListener(EndRoundUI);
			game.onEndMatch.RemoveListener(EndMatchUI);
		}

		//Set up the UI, titles, names, colors etc
		public void BeginMatchUI()
		{
			c.enabled = true;

			//Title
			teamOneDetails.text = $"{game.teamOne.name}: {game.teamOne.strategy.stance.ToString()}";
			teamTwoDetails.text = $"{game.teamTwo.name}: {game.teamTwo.strategy.stance.ToString()}";

			//Energy bars
			//Team 1
			ColorBlock temp = teamOneEnergyBarFloat.colors;
			temp.disabledColor = new Color(game.teamOne.color.r, game.teamOne.color.g, game.teamOne.color.b, floatGaugeAlpha);
			teamOneEnergyBarFloat.colors = temp;

			temp = teamOneEnergyBarInt.colors;
			temp.disabledColor = game.teamOne.color;
			teamOneEnergyBarInt.colors = temp;

			//Team 2
			temp = teamTwoEnergyBarFloat.colors;
			temp.disabledColor = new Color(game.teamTwo.color.r, game.teamTwo.color.g, game.teamTwo.color.b, floatGaugeAlpha);
			teamTwoEnergyBarFloat.colors = temp;

			temp = teamTwoEnergyBarInt.colors;
			temp.disabledColor = game.teamTwo.color;
			teamTwoEnergyBarInt.colors = temp;
		}

		public void BeginRoundUI()
		{
			endRoundScreen.SetActive(false);
		}

		public void EndRoundUI()
		{
			endRoundScreen.SetActive(true);
		}

		public void EndMatchUI()
		{
			c.enabled = false;
		}

		void Update()
		{
			if (c.enabled)  //If canvas is enabled means the game is running
			{
				//Control timer
				if (game.currentRoundRemainingTime > 15f)
				{
					timeLeft.color = Color.white;
					timeLeft.text = string.Format("{0:000}", game.currentRoundRemainingTime);
				}
				else if (game.currentRoundRemainingTime > 0f && game.currentRoundRemainingTime <= 15f)
				{
					timeLeft.color = Color.red;
					int fraction = (int)game.currentRoundRemainingTime * 10;
					fraction %= 10;
					timeLeft.text = string.Format("{0:000}", game.currentRoundRemainingTime);
					timeLeft.text = game.currentRoundRemainingTime.ToString();
				}
				else if (game.currentRound <= 0f)
				{
					timeLeft.color = Color.red;
					timeLeft.text = "00:00";
				}
			}
		}
	}
}