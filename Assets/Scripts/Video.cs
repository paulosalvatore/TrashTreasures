using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Video : MonoBehaviour
{
	private void Start()
	{
		Handheld.PlayFullScreenMovie(
			"Video.mp4",
			Color.black,
			PlayerPrefs.GetInt("Nível") > 0
				? FullScreenMovieControlMode.CancelOnInput
				: FullScreenMovieControlMode.Hidden,
			FullScreenMovieScalingMode.AspectFill
		);

		Invoke("IniciarJogo", 0.5f);
	}

	private void IniciarJogo()
	{
		SceneManager.LoadScene("Jogo");
	}
}
