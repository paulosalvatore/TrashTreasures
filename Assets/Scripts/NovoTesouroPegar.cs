﻿using UnityEngine;
using UnityEngine.EventSystems;

public class NovoTesouroPegar : MonoBehaviour,
	IPointerClickHandler
{
	private Jogo jogo;

	private void Start()
	{
		jogo = Jogo.Pegar();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (jogo.tempoTesouroAberto <= Time.time)
			jogo.OcultarTesouro();
	}
}