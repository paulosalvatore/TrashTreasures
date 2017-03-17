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
	public bool instanciarAtributos;
	internal string nome;

	[Header("Chances")]
	public float chanceBase;
	public float chanceMin;
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

	[Header("Partículas")]
	public GameObject particulaHit;

	private Image crack;

	internal int moedas;

	internal bool instanciarDinossauro;
	internal bool instanciarDiamante;

	private ControladorJogo controladorJogo;
	private RectTransform rectTransform;
	private Image image;

	void Start()
	{
		controladorJogo = ControladorJogo.Pegar();

		rectTransform = GetComponent<RectTransform>();
		image = GetComponent<Image>();

		crack = transform.GetChild(0).GetComponent<Image>();
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

	public void HitTile(bool oneHit = false)
	{
		hp = oneHit || controladorJogo.oneHitTiles ? 0 : hp - 1;
		
		if (particulaHit)
		{
			Vector3 posicaoParticula = Input.mousePosition;
			posicaoParticula.z = 10.0f;
			posicaoParticula = Camera.main.ScreenToWorldPoint(posicaoParticula);
			Instantiate(particulaHit, posicaoParticula, transform.rotation);
		}
		
		crack.enabled = true;

		if (hp == 0)
			DestruirTile();
		else
			audioSourceHit.Play();
	}

	void DestruirTile()
	{
		controladorJogo.DestruirTile(id, moedas);

		if (audioSourceDestruir)
			audioSourceDestruir.Play();

		image.enabled = false;
		crack.enabled = false;
	}

	public int PegarChance(int nivelAtual)
	{
		return
			nivelAtual < nivelMinimo
				?
			0
				:
			(int) Mathf.Clamp(
				(
					chanceBase +
					(
						nivelAtual * modificadorNivel
					)
				),
				chanceMin,
				chanceMax
			);
	}

	void InstanciarAtributos()
	{
		instanciarAtributos = false;

		audioSourceHit = ControladorJogo.AdicionarAudioSource(gameObject, hit);
		audioSourceDestruir = ControladorJogo.AdicionarAudioSource(gameObject, destruir);
	}
}
