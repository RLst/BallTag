using UnityEngine;

namespace LeMinhHuy
{
	public class MazeWall : MonoBehaviour
	{
		[SerializeField] float minDistanceFromGoal = 3f;
		[SerializeField] Goal[] goals = new Goal[2];
		[SerializeField] new Renderer renderer;
		void Awake()
		{
			//Get all goals
			goals = FindObjectsOfType<Goal>();
		}

		void Start()
		{
			//Delete self on spawn if too close to the specified goals
			bool markedForTermination = false;
			foreach (var g in goals)
			{
				if (Vector3.Distance(transform.position, g.transform.position) < minDistanceFromGoal)
					markedForTermination = true;
			}

			if (markedForTermination)
			{
				Destroy(gameObject);
				return;
			}

			//Color
			renderer.material.color = GameController.current.teamOne.color;
		}
	}
}