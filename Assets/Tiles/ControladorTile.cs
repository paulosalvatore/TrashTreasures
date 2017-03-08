using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControladorTile : MonoBehaviour
	, IPointerClickHandler
{
	internal int id;
	internal int hp;

	private ControladorJogo controladorJogo;

	void Start()
	{
		controladorJogo = ControladorJogo.Pegar();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		DiminuirHpTile();
	}

	void DiminuirHpTile()
	{
		/*hp--;

		if (hp == 0)
		{*/
			controladorJogo.DestruirTile(id);
			gameObject.SetActive(false);
		//}
	}
}
