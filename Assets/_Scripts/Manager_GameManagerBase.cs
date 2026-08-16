using UnityEngine;

namespace ProjectSOR
{
	public abstract class GameManagerBase : MonoBehaviour
	{
		static AppState s_appState;
		protected AppState AppState => s_appState;

		protected virtual void Awake()
		{
			s_appState ??= new AppState();
		}
	}

	public class AppState
	{
		public bool IsGamePaused { get; set; }
		public bool[] IsArmsGet { get; set; } = new bool[4];
	}
}
