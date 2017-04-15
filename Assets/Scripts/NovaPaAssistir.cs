using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NovaPaAssistir : MonoBehaviour,
	IPointerClickHandler
{
	private Jogo jogo;

	private void Start()
	{
		jogo = Jogo.Pegar();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		jogo.PaAssistir();
	}
}