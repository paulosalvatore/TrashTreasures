using UnityEngine;
using UnityEngine.EventSystems;

public class TileAdVoltar : MonoBehaviour,
	IPointerClickHandler
{
	private Jogo jogo;

	private void Start()
	{
		jogo = Jogo.Pegar();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		jogo.TileAdVoltar();
	}
}
