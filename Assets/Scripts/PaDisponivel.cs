using UnityEngine;
using UnityEngine.EventSystems;

public class PaDisponivel : MonoBehaviour,
	IPointerClickHandler
{
	public void OnPointerClick(PointerEventData eventData)
	{
		if (!Jogo.instancia.bloqueadorCliqueJohn)
		{
			Jogo.instancia.ReproduzirAudioClique();

			Jogo.instancia.ExibirNovaPaDisponivel();
		}
	}
}
