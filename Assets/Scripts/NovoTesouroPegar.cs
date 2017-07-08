using UnityEngine;
using UnityEngine.EventSystems;

public class NovoTesouroPegar : MonoBehaviour,
	IPointerClickHandler
{
	public void OnPointerClick(PointerEventData eventData)
	{
		if (!Jogo.instancia.ChecarNovoTesouroAnimator())
			return;

		if ((Jogo.instancia.bloqueadorClique &&
			Jogo.instancia.ChecarNovoTesouroAnimator() &&
			Jogo.instancia.tempoTesouroAberto <= Time.time) ||
			!Jogo.instancia.bloqueadorClique)
		{
			Jogo.instancia.ReproduzirAudioClique();

			Jogo.instancia.OcultarTesouro();
		}
	}
}
