using UnityEngine;
using UnityEngine.EventSystems;

public class NovaPaVoltar : MonoBehaviour,
	IPointerClickHandler
{
	private Jogo jogo;

	private void Start()
	{
		jogo = Jogo.Pegar();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		jogo.ReproduzirAudioClique();

		jogo.PaVoltar();
	}
}
