using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControladorTile : MonoBehaviour
	, IPointerClickHandler
{
	public int id;
	public string nome;
	public int hp;
	public float chanceBase;
	public float chanceMax;
	public float nivelMinimo;
	public float nivelMaximo;
	public float modificadorNivel;
	public int dinossauro;
	public int diamante;

	internal float chance;
	internal bool instanciarDinossauro;
	internal bool instanciarDiamante;
	internal int moedas;

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
		hp = controladorJogo.oneHitTiles ? 0 : hp - 1;

		if (hp == 0)
		{
			controladorJogo.DestruirTile(id, moedas);
			gameObject.SetActive(false);
		}
	}

	public int PegarChance(int nivelAtual)
	{
		return nivelAtual < nivelMinimo ? 0 : (int)(chanceBase + (nivelAtual * modificadorNivel));
	}
}
