using UnityEngine;

namespace LeMinhHuy
{
	public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		static T _current;
		public static bool isInstanced => Singleton<T>._current != null;

		public static T current
		{
			get
			{
				if (_current == null)
				{
					_current = FindExistingSingleton() ?? CreateNewSingleton();
				}
				return _current;
			}
		}
		/// <summary>
		/// Finds an existing instance of this singleton in the scene.
		/// </summary>
		static T FindExistingSingleton()
		{
			T[] existingInstances = FindObjectsOfType<T>();

			if (existingInstances == null || existingInstances.Length == 0)
				return null;

			return existingInstances[0];
		}

		/// <summary>
		/// If no instance of the T MonoBehaviour exists,
		/// creates a new GameObject in the scene and adds T to it.
		/// </summary>
		static T CreateNewSingleton()
		{
			GameObject container = new GameObject(typeof(T).Name + " (Singleton)");
			return container.AddComponent<T>();
		}

		/// <summary>
		/// This MUST run and will ALWAYS run regardless of whether
		/// it's intentionally hidden or overridden (because of reflection)
		/// </summary>
		void Awake()
		{
			T thisSingleton = this.GetComponent<T>();

			if (_current == null)
			{
				_current = thisSingleton;
				// DontDestroyOnLoad(_singleton);
			}
			else if (thisSingleton != _current)
			{
				Debug.LogWarningFormat("Duplicate singleton found with type {0} in GameObject {1}",
					this.GetType(), this.gameObject.name);

				Component.Destroy(this.GetComponent<T>());
				return;
			}
		}
	}
}