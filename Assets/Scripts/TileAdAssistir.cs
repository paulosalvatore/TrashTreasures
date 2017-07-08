using UnityEngine;
using UnityEngine.EventSystems;

public class TileAdAssistir : MonoBehaviour,
	IPointerClickHandler
{
	public void OnPointerClick(PointerEventData eventData)
	{
		if (!Jogo.instancia.ChecarTileAdAnimator())
			return;

		Jogo.instancia.AssistirTileAd();
	}
}
