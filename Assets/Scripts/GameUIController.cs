using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LeMinhHuy
{
	[RequireComponent(typeof(Canvas))]
	public class GameUIController : MonoBehaviour
	{
		const float FLOAT_SLIDER_ALPHA = 0.5f;

		[SerializeField] TextMeshProUGUI timeLeft;

		[Header("Team One")]
		[SerializeField] TextMeshProUGUI teamOneDetails;
		[SerializeField] Slider teamOneEnergyBarInt;
		[SerializeField] Slider teamOneEnergyBarFloat;

		[Header("Team Two")]
		[SerializeField] TextMeshProUGUI teamTwoDetails;
		[SerializeField] Slider teamTwoEnergyBarInt;
		[SerializeField] Slider teamTwoEnergyBarFloat;

		//Members
		GameController gc;
		Canvas c;

		void Awake()
		{
			gc = GameController.current;
			c = GetComponent<Canvas>();
		}

		void Start()
		{
			c.enabled = false;
		}

		//Event rego
		void OnEnable()
		{
			gc.onBeginMatch.AddListener(BeginMatch);
			gc.onEndMatch.AddListener(EndMatch);
		}
		void OnDisable()
		{
			gc.onBeginMatch.RemoveListener(BeginMatch);
			gc.onEndMatch.RemoveListener(EndMatch);
		}

		public void BeginMatch()
		{
			c.enabled = true;

			//Title
			teamOneDetails.text = $"{gc.teamOne.name}: {gc.teamOne.strategy.stance.ToString()}";
			teamTwoDetails.text = $"{gc.teamTwo.name}: {gc.teamTwo.strategy.stance.ToString()}";

			//Energy bars
			ColorBlock temp = teamOneEnergyBarFloat.colors;
			temp.disabledColor = Color.Lerp(gc.teamOne.color, Color.white, 0.6f);
			// temp.disabledColor = new Color(gc.teamOne.color.r, gc.teamOne.color.g, gc.teamOne.color.b, FLOAT_SLIDER_ALPHA);
			teamOneEnergyBarFloat.colors = temp;

			temp = teamOneEnergyBarInt.colors;
			temp.disabledColor = gc.teamOne.color;
			teamOneEnergyBarInt.colors = temp;

			temp = teamTwoEnergyBarFloat.colors;
			// temp.disabledColor = Color.Lerp(gc.teamTwo.color, Color.white, 0.6f);
			temp.disabledColor = new Color(gc.teamOne.color.r, gc.teamOne.color.g, gc.teamOne.color.b, FLOAT_SLIDER_ALPHA);
			teamTwoEnergyBarFloat.colors = temp;

			temp = teamTwoEnergyBarInt.colors;
			temp.disabledColor = gc.teamTwo.color;
			teamTwoEnergyBarInt.colors = temp;
		}

		public void EndMatch()
		{
			c.enabled = false;
		}

		void Update()
		{
			if (c.enabled)  //If canvas is enabled means the game is running
			{
				//Control timer
				if (gc.currentRoundRemainingTime > 15f)
				{
					timeLeft.color = Color.white;
					timeLeft.text = string.Format("{0:000}", gc.currentRoundRemainingTime);
				}
				else if (gc.currentRoundRemainingTime > 0f && gc.currentRoundRemainingTime <= 15f)
				{
					timeLeft.color = Color.red;
					int fraction = (int)gc.currentRoundRemainingTime * 10;
					fraction %= 10;
					timeLeft.text = string.Format("{0:000}", gc.currentRoundRemainingTime);
					timeLeft.text = gc.currentRoundRemainingTime.ToString();
				}
				else if (gc.currentRound <= 0f)
				{
					timeLeft.color = Color.red;
					timeLeft.text = "00:00";
				}
			}
		}
	}
}