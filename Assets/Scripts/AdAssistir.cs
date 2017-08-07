using UnityEngine;
using UnityEngine.EventSystems;

public class AdAssistir : MonoBehaviour,
	IPointerClickHandler
{
	public void OnPointerClick(PointerEventData eventData)
	{
		Jogo.instancia.AssistirAd();
	}
}
