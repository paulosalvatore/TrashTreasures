using UnityEngine;

public class FpsDisplay : MonoBehaviour
{
	float deltaTime = 0.0f;

	void Update()
	{
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
	}

	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;

		GUIStyle style = new GUIStyle();

		Rect rect = new Rect(0, -15, w, h);
		style.alignment = TextAnchor.LowerCenter;
		style.fontSize = h * 3 / 100;
		style.normal.textColor = Color.green;
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms\n({1:0.0} fps)\nVersão: {2}", msec, fps, Application.version);
		GUI.Label(rect, text, style);
	}
}