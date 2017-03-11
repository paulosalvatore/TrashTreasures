using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nuvem : MonoBehaviour
{
	public float limiteX;

	public float velocidadeMin;
	public float velocidadeMax;
	private float velocidade;

	private RectTransform rectTransform;

	void Start ()
	{
		rectTransform = GetComponent<RectTransform>();

		AtualizarVelocidade();
	}
	
	void Update ()
	{
		transform.Translate(Vector3.right * velocidade);
		
		if (Mathf.Abs(rectTransform.anchoredPosition.x) > limiteX)
			Reiniciar();
	}

	void AtualizarVelocidade()
	{
		velocidade = Random.Range(velocidadeMin, velocidadeMax);
	}

	void Reiniciar()
	{
		rectTransform.anchoredPosition = new Vector2(
			limiteX * -1,
			rectTransform.anchoredPosition.y
		);

		AtualizarVelocidade();
	}
}
