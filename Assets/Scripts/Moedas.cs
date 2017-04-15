using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moedas : MonoBehaviour
{
	[Header("Delay para Movimentar")]
	public float delay;

	[Header("Duração do Movimento")]
	public float duracao;

	[Header("Tipo da Animação")]
	public iTween.EaseType animacao;

	[Header("Movimento proporcional à distância")]
	public bool proporcionalDistancia;

	public float duracaoProporcional;

	private Jogo jogo;
	private Vector3 destino;

	private void Start()
	{
		jogo = Jogo.Pegar();

		destino = jogo.moedasImage.transform.position;

		if (proporcionalDistancia)
			duracao = Vector3.Distance(transform.position, destino) / duracaoProporcional;

		Invoke("Movimentar", delay);
	}

	private void Movimentar()
	{
		iTween.MoveTo(
			gameObject,
			iTween.Hash(
				"position", destino,
				"easeType", animacao,
				"time", duracao
			)
		);

		Destroy(gameObject, duracao);
	}
}