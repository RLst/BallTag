using UnityEngine;

namespace LeMinhHuy
{
	public class Fence : MonoBehaviour
	{
		public void SetColor(Color col)
		=> GetComponent<Renderer>().material.color = col;

	}
}
