using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tiles : MonoBehaviour
	, IPointerClickHandler
{
	[Header("Informações Básicas")]
	public int id;
	public int hp;
	internal string nome;
	public bool instanciarAtributos;

	[Header("Chances")]
	public float chanceBase;
	public float chanceMax;
	internal float chance;

	[Header("chanceBase + (level * modificadorNivel)")]
	public float modificadorNivel;

	[Header("Níveis que o Tile irá aparecer")]
	public float nivelMinimo;
	public float nivelMaximo;

	[Header("ID do Dinossauro/Diamante")]
	public int dinossauro;
	public int diamante;

	[Header("Áudio")]
	public AudioClip hit;
	public AudioClip destruir;
	private AudioSource audioSourceHit;
	private AudioSource audioSourceDestruir;

	internal int moedas;

	internal bool instanciarDinossauro;
	internal bool instanciarDiamante;

	private ControladorJogo controladorJogo;
	private Image image;

	void Start()
	{
		controladorJogo = ControladorJogo.Pegar();

		image = GetComponent<Image>();
	}

	void Update()
	{
		if (instanciarAtributos && id != 0)
			InstanciarAtributos();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		HitTile();
	}

	void HitTile()
	{
		hp = controladorJogo.oneHitTiles ? 0 : hp - 1;

		if (hp == 0)
			DestruirTile();
		else
			audioSourceHit.Play();
	}

	void DestruirTile()
	{
		controladorJogo.DestruirTile(id, moedas);

		audioSourceDestruir.Play();

		image.enabled = false;
	}

	public int PegarChance(int nivelAtual)
	{
		return nivelAtual < nivelMinimo ? 0 : (int) (chanceBase + (nivelAtual * modificadorNivel));
	}

	void InstanciarAtributos()
	{
		instanciarAtributos = false;

		audioSourceHit = ControladorJogo.AdicionarAudioSource(gameObject, hit);
		audioSourceDestruir = ControladorJogo.AdicionarAudioSource(gameObject, destruir);
	}
}
