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
		/// Finds an existing instance of this singleton in the scene.
		static T FindExistingSingleton()
		{
			T[] existingInstances = FindObjectsOfType<T>();

			if (existingInstances == null || existingInstances.Length == 0)
				return null;

			return existingInstances[0];
		}

		/// If no instance of the T MonoBehaviour exists, creates a new GameObject in the scene and adds T to it.
		static T CreateNewSingleton()
		{
			GameObject container = new GameObject(typeof(T).Name + " (Singleton)");
			return container.AddComponent<T>();
		}

		/// This MUST run and will ALWAYS run regardless of whether it's intentionally hidden or overridden (because of reflection)
		void Awake()
		{
			T thisSingleton = this.GetComponent<T>();

			if (_current == null)
			{
				//Assign this object as the main singleton if there is none
				_current = thisSingleton;
				gameObject.name = gameObject.name + "I"; //Show which instance it is
				if (this.transform.parent == null)
					DontDestroyOnLoad(this.gameObject);
			}
			//Otherwise if this singleton is not the current one then delete the entire object it's on
			//NOTE: This is very sporadic!
			else if (!thisSingleton.Equals(_current))
			{
				Debug.LogWarningFormat("Duplicate singleton found with type {0} in GameObject {1}", this.GetType(), this.gameObject.name);
				Destroy(this.gameObject);    //Doesn't work well
											 // Component.Destroy(this.GetComponent<T>());
				return;

			}
			Init();
		}

		protected virtual void Init() { }
	}
}