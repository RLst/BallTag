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

		[Header("End Round UI")]
		[SerializeField] GameObject endRoundUI;
		[SerializeField] TextMeshProUGUI teamOneResults;
		[SerializeField] TextMeshProUGUI resultRoundNumber;
		[SerializeField] TextMeshProUGUI teamTwoResults;

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

			endRoundUI.SetActive(false);
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
			endRoundUI.SetActive(false);    //Turn off due to AI demo

			//Title
			teamOneDetails.text = $"{game.teamOne.name}: {game.teamOne.strategy.stance.ToString()}";
			teamTwoDetails.text = $"{game.teamTwo.name}: {game.teamTwo.strategy.stance.ToString()}";

			//Energy bars; set color, max values
			//Team 1
			ColorBlock temp = teamOneEnergyBarFloat.colors;
			temp.disabledColor = new Color(game.teamOne.color.r, game.teamOne.color.g, game.teamOne.color.b, floatGaugeAlpha);
			teamOneEnergyBarFloat.colors = temp;
			teamOneEnergyBarFloat.maxValue = game.settings.maxEnergy;

			temp = teamOneEnergyBarInt.colors;
			temp.disabledColor = game.teamOne.color;
			teamOneEnergyBarInt.colors = temp;
			teamOneEnergyBarInt.maxValue = game.settings.maxEnergy;

			//Team 2
			temp = teamTwoEnergyBarFloat.colors;
			temp.disabledColor = new Color(game.teamTwo.color.r, game.teamTwo.color.g, game.teamTwo.color.b, floatGaugeAlpha);
			teamTwoEnergyBarFloat.colors = temp;
			teamTwoEnergyBarFloat.maxValue = game.settings.maxEnergy;

			temp = teamTwoEnergyBarInt.colors;
			temp.disabledColor = game.teamTwo.color;
			teamTwoEnergyBarInt.colors = temp;
			teamTwoEnergyBarInt.maxValue = game.settings.maxEnergy;
		}

		public void BeginRoundUI()
		{
			endRoundUI.SetActive(false);
		}

		public void EndRoundUI((Team team, Result result) teamResults)
		{
			//Display end round UI, show results
			endRoundUI.SetActive(true);
			teamOneResults.color = game.teamOne.color;
			teamTwoResults.color = game.teamTwo.color;

			//Round
			resultRoundNumber.text = "Round " + game.currentRound;

			//Draw scenario
			if (teamResults.result == Result.Draws)
			{
				teamOneResults.text = $"{game.teamOne.name} {teamResults.result.ToString()}";
				teamTwoResults.text = $"{game.teamTwo.name} {teamResults.result.ToString()}";
				return;
			}

			//Normal scenario
			if (teamResults.team == game.teamOne)
			{
				teamOneResults.text = $"{game.teamOne.name} {teamResults.result.ToString()}";
				teamTwoResults.text = $"{game.teamTwo.name} {(teamResults.result == Result.Wins ? "Loses" : "Wins")}";
			}
			else
			{
				teamTwoResults.text = $"{game.teamOne.name} {teamResults.result.ToString()}";
				teamOneResults.text = $"{game.teamOne.name} {(teamResults.result == Result.Wins ? "Loses" : "Wins")}";
			}
		}

		public void EndMatchUI((Team team, Result result) teamResults)
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