﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NovaPaComprar : MonoBehaviour,
	IPointerClickHandler
{
	private Jogo jogo;

	void Start()
	{
		jogo = Jogo.Pegar();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		jogo.PaComprar();
	}
}
