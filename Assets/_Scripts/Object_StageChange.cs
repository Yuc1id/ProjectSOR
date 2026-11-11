using UnityEngine;
//using UnityEngine.SceneManagement;

using ProjectSOR;

public class Object_StageChange : GameManagerBase
{
	[SerializeField] string playerTag = "Player";
	[SerializeField] string nextSceneName;

	void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag(playerTag))
		{
			SceneControl.MoveScene(nextSceneName);
		}
	}
}