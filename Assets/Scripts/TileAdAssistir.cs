using UnityEngine;
using UnityEngine.EventSystems;

public class TileAdAssistir : MonoBehaviour,
	IPointerClickHandler
{
	private Jogo jogo;

	private void Start()
	{
		jogo = Jogo.Pegar();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		jogo.AssistirTileAd();
	}
}
