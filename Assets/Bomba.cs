using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bomba : MonoBehaviour
	, IPointerClickHandler
{
	void Start ()
	{
		
	}

	void Update ()
	{

	}

	public void OnPointerClick(PointerEventData eventData)
	{
		for (int x = 0; x < 5; x++)
		{
			GameObject linha =
				GameObject
					.Find("Linha (" + (x + 1) + ")");

			for (int y = 0; y < 5; y++)
			{
				Tiles tile =
					linha
						.transform
						.FindChild("Tile (" + (y + 1) + ")")
						.GetComponent<Tiles>();

				tile.HitTile(true);
			}
		}
	}
}
