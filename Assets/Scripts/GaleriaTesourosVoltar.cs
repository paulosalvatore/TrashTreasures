using UnityEngine;
using UnityEngine.EventSystems;

public class GaleriaTesourosVoltar : MonoBehaviour,
	IPointerClickHandler
{
	public void OnPointerClick(PointerEventData eventData)
	{
		if (!Jogo.instancia.ChecarGaleriaTesourosAnimator())
			return;

		Jogo.instancia.ReproduzirAudioClique();

		Jogo.instancia.GaleriaTesourosVoltar();
	}
}
