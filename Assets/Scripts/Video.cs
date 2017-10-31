using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Video : MonoBehaviour
{
	private VideoPlayer videoPlayer;

	private void Start()
	{
		videoPlayer = GetComponent<VideoPlayer>();

		Invoke("IniciarJogo", (float)videoPlayer.clip.length);
	}

	private void Update()
	{
		if (PlayerPrefs.GetInt("Nível") > 0 &&
			videoPlayer.time > 0.5f &&
			Input.touchCount > 0)
		{
			IniciarJogo();
		}
	}

	private void IniciarJogo()
	{
		SceneManager.LoadScene("Jogo");
	}
}
