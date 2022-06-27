using UnityEngine;

namespace LeMinhHuy
{
	public class LogToConsole : MonoBehaviour
	{
		public string message = "Message to log to the console";

		public void Log()
		{
			Debug.Log(message);
		}

		public void LogCustomMessasge(string customMessage = "Custom message!")
		{
			Debug.Log(customMessage);
		}
	}
}
