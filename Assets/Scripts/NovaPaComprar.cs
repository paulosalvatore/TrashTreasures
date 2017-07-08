using UnityEngine;
using UnityEngine.EventSystems;

public class NovaPaComprar : MonoBehaviour,
	IPointerClickHandler
{
	public void OnPointerClick(PointerEventData eventData)
	{
		if (!Jogo.instancia.ChecarNovaPaAnimator())
			return;

		Jogo.instancia.PaComprar();
	}
}
