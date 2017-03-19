using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NovoTesouro : MonoBehaviour,
	IPointerClickHandler
{
	private Jogo jogo;
	private float inicio;

	void Start()
	{
		jogo = Jogo.Pegar();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (jogo.tempoTesouroAberto <= Time.time)
			jogo.OcultarTesouro();
	}
}
