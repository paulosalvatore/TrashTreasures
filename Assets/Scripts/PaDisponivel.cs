using UnityEngine;
using UnityEngine.EventSystems;

public class PaDisponivel : MonoBehaviour,
	IPointerClickHandler
{
	private Jogo jogo;

	private void Start()
	{
		jogo = Jogo.Pegar();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!jogo.bloqueadorCliqueJohn)
			jogo.ExibirNovaPaDisponivel();
	}
}
