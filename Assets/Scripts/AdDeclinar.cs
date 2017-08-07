using UnityEngine;
using UnityEngine.EventSystems;

public class AdDeclinar : MonoBehaviour,
	IPointerClickHandler
{
	public void OnPointerClick(PointerEventData eventData)
	{
		Jogo.instancia.DeclinarAd();
	}
}
