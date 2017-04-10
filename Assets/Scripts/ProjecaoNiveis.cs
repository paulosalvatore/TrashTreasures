﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjecaoNiveis : MonoBehaviour
{
	private Jogo jogo;

	void Start()
	{
		jogo = Jogo.Pegar();
	}
	
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Q))
			CriarProjecaoNiveis();
	}

	void CriarProjecaoNiveis()
	{
		foreach (Transform child in transform)
			Destroy(child.gameObject);

		int niveis = 100;

		for (int nivel = 1; nivel <= niveis; nivel++)
		{
			Transform mapaDestino = new GameObject(string.Format("Mapa {0}", nivel)).transform;

			mapaDestino.parent = transform;

			mapaDestino.localPosition = new Vector2(
				nivel * 6,
				0
			);

			jogo.ConstruirMapa(mapaDestino, nivel);

			Transform texto = new GameObject("Texto").transform;

			texto.parent = mapaDestino;

			texto.localPosition = new Vector3(
				-0.5f,
				2f
			);

			TextMesh textMesh = texto.gameObject.AddComponent<TextMesh>();
			textMesh.text = string.Format("Nível {0}", nivel);
		}
	}
}