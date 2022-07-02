using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LeMinhHuy
{
	/// <summary>
	/// Controls the in game UI such as timer, energy gauges, team info, end round, end match screens etc
	/// NOTE: This is AR agnostic
	/// </summary>
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

		[Header("Penalty Round UI")]
		[SerializeField] GameObject penaltyRoundUI;

		[Header("End Game UI")]
		[SerializeField] GameObject endGameUI;
		[SerializeField] TextMeshProUGUI endGameResults;
		[SerializeField] TextMeshProUGUI endGameStats;


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

			//Event regos
			game.onBeginMatch.AddListener(BeginMatchUI);
			game.onBeginRound.AddListener(BeginRoundUI);
			game.onEndRound.AddListener(HandleEndRound);
			game.onEndMatch.AddListener(HandleEndMatch);
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

		public void HandleEndRound((Team team, Result result) teamResults)
		{
			//Display end round UI, show results
			endRoundUI.SetActive(true);
			teamOneResults.color = game.teamOne.color;
			teamTwoResults.color = game.teamTwo.color;

			//Round
			resultRoundNumber.text = "Round " + game.currentRound;

			//Draw scenario
			if (teamResults.result == Result.Draw)
			{
				teamOneResults.text = $"{game.teamOne.name}\n{teamResults.result.ToString()}";
				teamTwoResults.text = $"{game.teamTwo.name}\n{teamResults.result.ToString()}";
				return;
			}

			//Normal scenario
			if (teamResults.team == game.teamOne)
			{
				teamOneResults.text = $"{game.teamOne.name}\n{teamResults.result.ToString()}";
				teamTwoResults.text = $"{game.teamTwo.name}\n{(teamResults.result == Result.Wins ? "Lose" : "Wins")}";
			}
			else
			{
				teamTwoResults.text = $"{game.teamTwo.name}\n{teamResults.result.ToString()}";
				teamOneResults.text = $"{game.teamOne.name}\n{(teamResults.result == Result.Wins ? "Lose" : "Wins")}";
			}
		}

		public void HandleEndMatch((Team team, Result result) teamResults)
		{
			//Determine which screen to activate based on result
			//Draw > Penalty Round required
			if (teamResults.result == Result.Draw)
			{
				penaltyRoundUI.SetActive(true);
				//Don't need to adjust anything on this screen
				//The logic will come back and run this function again and the win/lose logic below
			}
			else
			{
				endGameUI.SetActive(true);

				//Populate end screen
				//Result
				if (teamResults.result == Result.Wins)
				{
					endGameResults.text = "YOU WIN!";
				}
				else
				{
					endGameResults.text = "YOU LOSE!";
				}

				//Stats
				Team t1 = game.teamOne, t2 = game.teamTwo;
				string t1c = ColorUtility.ToHtmlStringRGB(t1.color);
				string t2c = ColorUtility.ToHtmlStringRGB(t2.color);
				endGameStats.text = "Stats\n" +
					$"<color=#{t1c}>{t1.wins}</color> Goals <color=#{t2c}>{t2.wins}</color>\n" +
					$"<color=#{t1c}>{t2.wins}</color> Losses <color=#{t2c}>{t1.wins}</color>\n" +
					$"<color=#{t1c}>{t1.draws}</color> Draws <color=#{t2c}>{t2.draws}</color>\n" +
					$"<color=#{t1c}>{t1.tags}</color> Tags <color=#{t2c}>{t2.tags}</color>\n" +
					$"<color=#{t1c}>{t1.outs}</color> Outs <color=#{t2c}>{t2.outs}</color>\n" +
					$"<color=#{t1c}>{t1.passes}</color> Passes <color=#{t2c}>{t2.passes}</color>\n" +
					$"<color=#{t1c}>{t1.despawns}</color> Despawns <color=#{t2c}>{t2.despawns}</color>";
			}
		}

		void Update()
		{
			if (!c.enabled) return; //If canvas is enabled means the game is running
									// if (game.isPlaying) return;

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