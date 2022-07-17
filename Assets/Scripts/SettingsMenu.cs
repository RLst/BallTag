using UnityEngine;
using TMPro;

namespace LeMinhHuy
{
	public class SettingsMenu : MonoBehaviour
	{
		[SerializeField] GameSettings settings;
		[SerializeField] TMP_InputField teamOneInput;
		[SerializeField] TMP_InputField teamTwoInput;

		void Awake()
		{
			Debug.Assert(teamOneInput is object);
			Debug.Assert(teamTwoInput is object);
		}

		void Start()
		{
			teamOneInput.text = settings.teamOneSettings.name;
			teamTwoInput.text = settings.teamTwoSettings.name;
		}
	}
}