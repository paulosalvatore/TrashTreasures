using UnityEngine;
using UnityEngine.EventSystems;

public class GaleriaTesourosAbrir : MonoBehaviour,
	IPointerClickHandler
{
	public void OnPointerClick(PointerEventData eventData)
	{
		if (!Jogo.instancia.bloqueadorCliqueJohn)
		{
			Jogo.instancia.ReproduzirAudioClique();

			Jogo.instancia.ExibirGaleriaTesouros();
		}
	}
}
