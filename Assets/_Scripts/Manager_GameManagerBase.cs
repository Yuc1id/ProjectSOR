using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectSOR
{
	public abstract class GameManagerBase : MonoBehaviour
	{
		static AppState s_appState;
		protected AppState AppState => s_appState;

		static SceneControl s_sceneControl;
		protected SceneControl SceneControl => s_sceneControl;

		protected virtual void Awake()
		{
			s_appState ??= new AppState();
			s_sceneControl ??= new SceneControl();
		}
	}

	public class AppState
	{
		public bool IsGamePaused { get; set; }
		public bool[] IsArmsGet { get; set; } = new bool[4];
	}

	public class SceneControl
	{
		public void MoveScene(string nextSceneName)
		{
			SceneManager.LoadScene(nextSceneName);
		}
	}
}
