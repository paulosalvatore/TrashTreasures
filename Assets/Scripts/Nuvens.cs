﻿using UnityEngine;

public class Nuvens : MonoBehaviour
{
	[Header("Limite na Tela")]
	public float limiteX;

	[Header("Velocidade de Movimento")]
	public RangeFloat velocidadeRange;
	private float velocidade;

	[Header("Variação de Tamanho")]
	public RangeFloat tamanhoRange;
	private float tamanho;

	private void Start()
	{
		AtualizarVelocidade();
		AtualizarTamanho();
	}

	private void Update()
	{
		transform.Translate(Vector3.right * velocidade * Time.deltaTime);

		if (Mathf.Abs(transform.localPosition.x) > limiteX)
			Reiniciar();
	}

	private void AtualizarVelocidade()
	{
		velocidade = Random.Range(velocidadeRange.min, velocidadeRange.max);
	}

	private void AtualizarTamanho()
	{
		tamanho = Random.Range(tamanhoRange.min, tamanhoRange.max);

		transform.localScale = new Vector2(tamanho, tamanho);
	}

	private void Reiniciar()
	{
		transform.localPosition = new Vector2(
			limiteX * -1,
			transform.localPosition.y
		);

		AtualizarVelocidade();

		AtualizarTamanho();
	}
}
