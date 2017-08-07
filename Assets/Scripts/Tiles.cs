using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections;
using UnityEngine;

/*
[System.Serializable]
public class TilesInfo
{
	public string tileName;

	[Header("Informações Básicas")]
	public int hp;

	[Header("Chances")]
	public float chanceBase;
	public float chanceMin;
	public float chanceMax;
	public bool limiteUmPorNivel;
	public int aparecerObrigatoriamenteNivel;

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

	[Header("Áudio")]
	public AudioClip hit;
	public AudioClip destruir;

	[Header("Partículas")]
	public GameObject particula;
	public bool particulaHit;
	public bool particulaDestroy;

	[Header("Escalonamento no Hit")]
	public float delayEscalonamento;
	public float porcentagemEscalonamento;
	public iTween.EaseType animacaoEscalonamento;

	[Header("Tiles Especiais")]
	public bool bauTesouro;
	public bool ads;

	[Header("Moedas")]
	public bool fornecerMoedas;
}
*/

public class Tiles : MonoBehaviour
{
	[Header("Informações Básicas")]
	public ObscuredInt hp;
	internal ObscuredInt hpAdicional;
	public bool tileQuebradoInvertido;

	[Header("Chances")]
	public ObscuredFloat chanceBase;
	public ObscuredFloat chanceMin;
	public ObscuredFloat chanceMax;
	public ObscuredBool limiteUmPorNivel;
	public ObscuredInt aparecerObrigatoriamenteNivel;
	internal ObscuredFloat chance;

	[Header("chanceBase + (level * modificadorNivel)")]
	public ObscuredFloat modificadorNivel;

	[Header("Níveis que o Tile irá aparecer")]
	public ObscuredFloat nivelMinimo;
	public ObscuredFloat nivelMaximo;

	[Header("Dinossauro/Diamante")]
	public Sprite dinossauro;
	public GameObject particulaDinossauro;
	public Sprite diamante;
	public GameObject particulaDiamante;
	internal ObscuredBool instanciarDinossauro;
	internal ObscuredBool instanciarDiamante;

	[Header("Áudio")]
	public AudioClip hit;
	public AudioClip destruir;

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
	public ObscuredBool bauTesouro;
	public ObscuredBool ads;

	[Header("Moedas")]
	public ObscuredBool fornecerMoedas;
	internal ObscuredInt moedas = -1;
	internal Range moedasRange = new Range(0, 0);

	internal SpriteRenderer spriteRenderer;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();

		escalonamentoInicial = transform.localScale;
	}

#if (UNITY_EDITOR)

	private void OnMouseOver()
	{
		if (!Jogo.instancia.bloqueadorClique &&
			(Input.GetMouseButtonDown(0) ||
			Input.GetMouseButton(0) && Jogo.instancia.modoShovelGun)
		)
		{
			HitTile();
		}
	}

#endif

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
			hp -= Jogo.instancia.paSelecionada.ataque;
		else
			hpAdicional--;

		int hpTotal = Mathf.Max(0, hp) + hpAdicional;

		if (particula && ((particulaHit && hpTotal > 0) || (particulaDestroy && hpTotal <= 0)))
		{
			GameObject instanciarParticula = particula;

			if (spriteRenderer.sprite == dinossauro)
				instanciarParticula = particulaDinossauro;
			else if (spriteRenderer.sprite == diamante)
				instanciarParticula = particulaDiamante;

			Instantiate(instanciarParticula, transform.position, transform.rotation);
		}

		if (Jogo.instancia.exibirTileQuebrado && transform.childCount == 0)
			ExibirTileQuebrado();

		if (Jogo.instancia.oneHitTiles || hpTotal <= 0)
			DestruirTile();
		else
		{
			Jogo.ReproduzirAudio(hit);

			StartCoroutine(EscalonarTile());
		}
	}

	private IEnumerator EscalonarTile()
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

	private void ExibirTileQuebrado()
	{
		GameObject tileQuebrado =
			Instantiate(
				tileQuebradoInvertido
					?
				Jogo.instancia.tileQuebradoInvertido
					:
				Jogo.instancia.tileQuebrado
			);

		tileQuebrado.transform.parent = transform;

		tileQuebrado.transform.localPosition = Vector3.zero;
	}

	private void DestruirTile()
	{
		if (bauTesouro)
			Jogo.instancia.AdicionarTesouro();
		else if (ads)
			Jogo.instancia.ExibirAd();

		Jogo.ReproduzirAudio(destruir);

		Jogo.instancia.ProcessarTileDestruido(
			transform,
			fornecerMoedas,
			moedas,
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

	private int PegarQuantidadeMoedas(Range range)
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

			hpAdicional = instanciarDinossauro ? Jogo.instancia.hpBaseDinossauro : Jogo.instancia.hpBaseDiamante;

			moedas =
				PegarQuantidadeMoedas(
					instanciarDinossauro
						?
					Jogo.instancia.moedasDinossauro
						:
					Jogo.instancia.moedasDiamante
				);

			instanciarDinossauro = false;
			instanciarDiamante = false;
		}
	}
}
