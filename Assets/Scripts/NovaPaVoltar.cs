using UnityEngine;
using UnityEngine.EventSystems;

public class NovaPaVoltar : MonoBehaviour,
	IPointerClickHandler
{
	public void OnPointerClick(PointerEventData eventData)
	{
		if (!Jogo.instancia.ChecarNovaPaAnimator())
			return;

		Jogo.instancia.ReproduzirAudioClique();

		Jogo.instancia.PaVoltar();
	}
}
