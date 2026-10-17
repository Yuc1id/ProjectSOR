using UnityEngine;
using UnityEngine.SceneManagement;

public class Object_StageChange : MonoBehaviour
{
	[SerializeField] string playerTag = "Player";
	[SerializeField] string nextSceneName;

	void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag(playerTag))
		{
			SceneManager.LoadScene(nextSceneName);
		}
	}
}