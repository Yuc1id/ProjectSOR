using UnityEngine;

using ProjectSOR;

public class Button_ChangeScene : GameManagerBase
{
	[SerializeField] string nextScene;

	bool _active = false;

	public void SceneChangeButton(string sceneName)
	{
		if (!_active)
		{
			_active = true;
			SceneControl.MoveScene(sceneName);
		}
	}
	public void PressStart() { SceneChangeButton(nextScene); }

	public void PressClose()
	{
		Application.Quit();

		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#endif
	}
}
