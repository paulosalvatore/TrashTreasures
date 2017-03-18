using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tiles : MonoBehaviour
{
	[Header("Informações Básicas")]
	public int hp;

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

	[Header("Dinossauro/Diamante")]
	public Sprite dinossauro;
	public Sprite diamante;
	internal bool instanciarDinossauro;
	internal bool instanciarDiamante;

	[Header("Áudio")]
	public AudioClip hit;
	public AudioClip destruir;
	private AudioSource audioSourceHit;
	private AudioSource audioSourceDestruir;

	[Header("Partículas")]
	public GameObject particulaHit;

	[Header("Escalonamento no Hit")]
	public float delayEscalonamento;
	public float porcentagemEscalonamento;
	public iTween.EaseType animacaoEscalonamento;
	private Vector3 escalonamentoInicial;

	//private Image crack;

	// Moedas
	internal int moedas = -1;
	internal Range moedasRange = new Range(0, 0);

	private Jogo jogo;
	internal SpriteRenderer spriteRenderer;

	void Awake ()
	{
		jogo = Jogo.Pegar();

		spriteRenderer = GetComponent<SpriteRenderer>();

		escalonamentoInicial = transform.localScale;
	}

	public void OnMouseOver()
	{
		if (Input.GetMouseButtonDown(0))
			HitTile();
	}

	public void HitTile(bool oneHit = false)
	{
		if (!spriteRenderer.enabled)
			return;

		hp = oneHit || jogo.oneHitTiles ? 0 : hp - 1;

		if (particulaHit)
			Instantiate(particulaHit, transform.position, transform.rotation);

		if (jogo.exibirTileQuebrado && transform.childCount == 0)
			ExibirTileQuebrado();

		if (hp == 0)
			DestruirTile();
		else
		{
			if (audioSourceHit)
				audioSourceHit.Play();

			StartCoroutine(EscalonarTile());
		}
	}

	IEnumerator EscalonarTile()
	{
		iTween.ScaleTo(
				gameObject,
				iTween.Hash(
					"scale", escalonamentoInicial * (1 + porcentagemEscalonamento / 100),
					"time", delayEscalonamento,
					"easeType", animacaoEscalonamento
				)
			);

		yield return new WaitForSeconds(delayEscalonamento);

		iTween.ScaleTo(
				gameObject,
				iTween.Hash(
					"scale", escalonamentoInicial,
					"time", delayEscalonamento,
					"easeType", animacaoEscalonamento
				)
			);
	}

	void ExibirTileQuebrado()
	{
		GameObject tileQuebrado = Instantiate(jogo.tileQuebrado);

		tileQuebrado.transform.parent = transform;

		tileQuebrado.transform.localPosition = Vector3.zero;
	}
	
	void DestruirTile()
	{
		spriteRenderer.enabled = false;

		if (transform.childCount == 1)
			Destroy(transform.GetChild(0).gameObject);

		float delayDestruirTile = 0;

		if (audioSourceDestruir)
		{
			audioSourceDestruir.Play();
			delayDestruirTile = destruir.length;
		}

		Destroy(gameObject, delayDestruirTile);

		jogo.ProcessarTileDestruido(transform, moedas);
	}

	public int PegarChance()
	{
		int chanceCalculada = (int)(chanceBase + (jogo.nivel * modificadorNivel));

		if (nivelMinimo > 0 && jogo.nivel < nivelMinimo)
			return 0;
		else if (nivelMaximo > 0 && jogo.nivel > nivelMaximo)
			return 0;
		else if (chanceMin < chanceMax && chanceMax > 0)
			return
				(int)Mathf.Clamp(
					chanceCalculada,
					chanceMin,
					chanceMax
				);
		else if (chanceMin > 0 && chanceMax == 0)
			return (int)Mathf.Min(chanceCalculada, chanceMin);
		else if (chanceMax > 0)
			return (int)Mathf.Max(chanceCalculada, chanceMin);
		else
			return chanceCalculada;
	}
	
	int PegarQuantidadeMoedas(Range range)
	{
		return
			Random.Range(
				range.min,
				range.max + 1
			);
	}

	public void Instanciar()
	{
		if (hit != null)
			audioSourceHit = Jogo.AdicionarAudioSource(gameObject, hit);

		if (destruir != null)
			audioSourceDestruir = Jogo.AdicionarAudioSource(gameObject, destruir);

		if (instanciarDinossauro || instanciarDiamante)
		{
			spriteRenderer.sprite = instanciarDinossauro ? dinossauro : diamante;

			hp += instanciarDinossauro ? jogo.hpBaseDinossauro : jogo.hpBaseDiamante;

			moedas =
				PegarQuantidadeMoedas(
					instanciarDinossauro
						?
					jogo.moedasDinossauro
						:
					jogo.moedasDiamante
				);

			instanciarDinossauro = false;
			instanciarDiamante = false;
		}
	}
}
