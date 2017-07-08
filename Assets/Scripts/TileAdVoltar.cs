using UnityEngine;
using UnityEngine.EventSystems;

public class TileAdVoltar : MonoBehaviour,
	IPointerClickHandler
{
	public void OnPointerClick(PointerEventData eventData)
	{
		if (!Jogo.instancia.ChecarTileAdAnimator())
			return;

		Jogo.instancia.ReproduzirAudioClique();

		Jogo.instancia.TileAdVoltar();
	}
}
