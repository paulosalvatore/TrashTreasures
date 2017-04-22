using UnityEngine;
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
		if ((jogo.bloqueadorClique &&
			jogo.ChecarNovoTesouroAnimator() &&
			jogo.tempoTesouroAberto <= Time.time) ||
			!jogo.bloqueadorClique)
		{
			jogo.ReproduzirAudioClique();

			jogo.OcultarTesouro();
		}
	}
}
