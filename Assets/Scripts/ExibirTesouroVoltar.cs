using UnityEngine;
using UnityEngine.EventSystems;

public class ExibirTesouroVoltar : MonoBehaviour,
	IPointerClickHandler
{
	public void OnPointerClick(PointerEventData eventData)
	{
		if (!Jogo.instancia.ChecarTesouroDestaqueAnimator())
			return;

		if ((Jogo.instancia.bloqueadorClique &&
			Jogo.instancia.ChecarTesouroDestaqueAnimator()) ||
			!Jogo.instancia.bloqueadorClique)
		{
			Jogo.instancia.OcultarTesouroDestaque();
		}
	}
}
