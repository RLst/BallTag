using UnityEngine;
using UnityEngine.UI;

namespace LeMinhHuy
{
	public class TeamIntegerGauge : MonoBehaviour
	{
		Slider gauge;
		void Awake() => gauge = GetComponent<Slider>();
		public void SetValueFloored(float value)
		{
			gauge.value = Mathf.FloorToInt(value);
		}
		public void SetValueCeil(float value)
		{
			gauge.value = Mathf.CeilToInt(value);
		}
	}
}