using TMPro;
using UnityEngine;

namespace LeMinhHuy
{
	public class GoalCounter : MonoBehaviour
	{
		TextMeshProUGUI textMeshProUGUI;
		void Awake() => textMeshProUGUI = GetComponent<TextMeshProUGUI>();
		public void SetValue(int value)
		{
			textMeshProUGUI.text = value.ToString();
		}
	}
}