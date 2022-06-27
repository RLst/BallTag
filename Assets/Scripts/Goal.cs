using UnityEngine;

namespace LeMinhHuy
{
	public class Goal : MonoBehaviour
	{
		public void SetColor(Color col)
			=> GetComponent<Renderer>().material.color = col;

	}
}