using System.Collections;
using System.Collections.Generic; // remover dps
using UnityEngine;
using UnityEngine.EventSystems;

public class Tiles : MonoBehaviour
{
	[Header("Informações Básicas")]
	public int hp;
	internal int hpAdicional;

	[Header("Chances")]
	public float chanceBase;
	public float chanceMin;
	public float chanceMax;
	public bool limiteUmPorNivel;
	public int aparecerObrigatoriamenteNivel;
	internal float chance;

	[Header("chanceBase + (level * modificadorNivel)")]
	public float modificadorNivel;

	[Header("Níveis que o Tile irá aparecer")]
	public float nivelMinimo;
	public float nivelMaximo;

	[Header("Dinossauro/Diamante")]
	public Sprite dinossauro;
	public GameObject particulaDinossauro;
	public Sprite diamante;
	public GameObject particulaDiamante;
	internal bool instanciarDinossauro;
	internal bool instanciarDiamante;

	[Header("Áudio")]
	public AudioClip hit;
	public AudioClip destruir;
	private AudioSource audioSourceHit;
	private AudioSource audioSourceDestruir;

	[Header("Partículas")]
	public GameObject particula;
	public bool particulaHit;
	public bool particulaDestroy;

	[Header("Escalonamento no Hit")]
	public float delayEscalonamento;
	public float porcentagemEscalonamento;
	public iTween.EaseType animacaoEscalonamento;
	private Vector3 escalonamentoInicial;

	[Header("Tiles Especiais")]
	public bool bauTesouro;
	public bool ads;

	[Header("Moedas")]
	public bool fornecerMoedas;
	internal int moedas = -1;
	internal Range moedasRange = new Range(0, 0);

	private Jogo jogo;
	internal SpriteRenderer spriteRenderer;

	void Awake()
	{
		jogo = Jogo.Pegar();

		spriteRenderer = GetComponent<SpriteRenderer>();

		escalonamentoInicial = transform.localScale;
	}

	void OnMouseOver()
	{
		if (Application.platform == RuntimePlatform.WindowsEditor &&
			Input.GetMouseButtonDown(0) &&
			!jogo.bloqueadorClique)
		{
			HitTile();
		}
	}

	public void HitTile()
	{
		if (!spriteRenderer.enabled)
			return;

		if (bauTesouro && hp > 0)
		{
			hpAdicional = hp;
			hp = 0;
		}

		if (hp > 0)
			hp -= jogo.paSelecionada.ataque;
		else
			hpAdicional--;

		int hpTotal = hp + hpAdicional;

		if (particula && ((particulaHit && hpTotal > 0) || (particulaDestroy && hpTotal <= 0)))
		{
			GameObject instanciarParticula = particula;

			if (spriteRenderer.sprite == dinossauro)
				instanciarParticula = particulaDinossauro;
			else if (spriteRenderer.sprite == diamante)
				instanciarParticula = particulaDiamante;

			Instantiate(instanciarParticula, transform.position, transform.rotation);
		}

		if (jogo.exibirTileQuebrado && transform.childCount == 0)
			ExibirTileQuebrado();

		if (jogo.oneHitTiles || hpTotal <= 0)
			DestruirTile();
		else
		{
			Jogo.ReproduzirAudio(hit);

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
		if (bauTesouro)
			jogo.AdicionarTesouro();
		else if (ads)
			jogo.ExibirAd("tesouro");

		Jogo.ReproduzirAudio(destruir);

		jogo.ProcessarTileDestruido(
			transform,
			fornecerMoedas
				?
					moedas
				:
					0,
			bauTesouro || ads
				?
					true
				:
					false
		);

		Destroy(gameObject);
	}

	public int PegarChance(int nivelMapa)
	{
		int chanceCalculada = (int)(chanceBase + (nivelMapa * modificadorNivel));

		if (nivelMinimo > 0 && nivelMapa < nivelMinimo)
			return 0;
		else if (nivelMaximo > 0 && nivelMapa > nivelMaximo)
			return 0;
		else if (chanceMin < chanceMax && chanceMax > 0)
			return
				(int)Mathf.Clamp(
					chanceCalculada,
					chanceMin,
					chanceMax
				);
		else if (chanceMin > 0 && chanceMax == 0)
			return (int)Mathf.Max(chanceCalculada, chanceMin);
		else if (chanceMax > 0)
			return (int)Mathf.Min(chanceCalculada, chanceMin);
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
		if (instanciarDinossauro || instanciarDiamante)
		{
			spriteRenderer.sprite = instanciarDinossauro ? dinossauro : diamante;

			hpAdicional = instanciarDinossauro ? jogo.hpBaseDinossauro : jogo.hpBaseDiamante;

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
