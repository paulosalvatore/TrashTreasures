using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Tiles
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

	public float chance;
	public bool instanciarDinossauro = false;
	public bool instanciarDiamante = false;

	public Tiles(
		int aId,
		string aNome,
		int aHp,
		float aChanceBase,
		float aChanceMax,
		int aNivelMinimo,
		int aNivelMaximo,
		float aModificadorNivel,
		int aDinossauro = 0,
		int aDiamante = 0
	)
	{
		id = aId;
		nome = aNome;
		hp = aHp;
		chanceBase = aChanceBase;
		chanceMax = aChanceMax;
		nivelMinimo = aNivelMinimo;
		nivelMaximo = aNivelMaximo;
		modificadorNivel = aModificadorNivel;
		dinossauro = aDinossauro;
		diamante = aDiamante;
	}

	public int PegarChance(int nivelAtual)
	{
		return nivelAtual < nivelMinimo ? 0 : (int)(chanceBase + (nivelAtual * modificadorNivel));
	}
}

public class ControladorJogo : MonoBehaviour
{
	[Header("Cores do Fundo")]
	public List<Color> coresFundoJogo;

	[Header("Tiles")]
	public List<Sprite> spriteTiles;
	private List<Tiles> tiles = new List<Tiles>();
	private int quantidadeTiles;

	[Header("Chances ao Destruir Tile")]
	public float chanceTesouro;
	public float chanceMoeda;

	[Header("Dinossauro")]
	public List<Sprite> spriteDinossauros;
	public float chanceBaseDinossauro;
	public int hpBaseDinossauro;
	public int moedasDinossauro;

	[Header("Diamante")]
	public List<Sprite> spriteDiamantes;
	public float chanceBaseDiamante;
	public int hpBaseDiamante;
	public int moedasDiamanteMin;
	public int moedasDiamanteMax;

	[Header("Mapa")]
	public float duracaoMovimentoMapa;
	public float delayEncerrarNivel;
	private float duracaoMovimentoMapaInicial;

	[Header("Tipo de Animação do Mapa")]
	public string animacaoMapa;

	[Header("Variáveis para Testes")]
	public bool oneHitTiles;
	public bool desativarAnimacaoMapa;

	// Nível e Pontuação
	private int nivel = 1;
	private float moedas;

	// Componentes do Canvas
	private Text nivelText;
	private Text moedasText;
	private Image fundoJogo;
	private Color corAnteriorFundoJogo;
	private Animator limpoAnimator;
	private Animator telaAnimator;

	// Definições da Área de Jogo
	private int[,] jogo;
	private int larguraJogo = 5;
	private int alturaJogo = 5;
	private RectTransform mapa;

	void Start()
	{
		duracaoMovimentoMapaInicial = duracaoMovimentoMapa;

		nivelText = GameObject.Find("Nível").GetComponent<Text>();
		moedasText = GameObject.Find("Moedas").GetComponent<Text>();
		fundoJogo = GameObject.Find("FundoJogo").GetComponent<Image>();
		limpoAnimator = GameObject.Find("Limpo").GetComponent<Animator>();

		telaAnimator = GameObject.Find("Tela").GetComponent<Animator>();

		mapa = GameObject.Find("Jogo").GetComponent<RectTransform>();

		corAnteriorFundoJogo = fundoJogo.color;

		tiles.Add(new Tiles(1, "Grama", 2, 0, 0, 0, 1, 0));
		tiles.Add(new Tiles(2, "Terra", 2, 80, 0, 0, 50, -1.5f));
		tiles.Add(new Tiles(3, "Areia", 1, 60, 0, 0, 50, -1.4f));
		tiles.Add(new Tiles(4, "Pedra", 10, 5, 40, 3, 70, 2f, 1, 1));
		tiles.Add(new Tiles(5, "Pedra2", 20, 5, 0, 20, 100, 1.2f, 2, 2));
		tiles.Add(new Tiles(6, "PedraLava", 60, 5, 0, 40, 100, 0.25f, 3, 3));
		tiles.Add(new Tiles(7, "Pedra3", 100, 5, 0, 40, 100, 0.25f, 4, 4));
		tiles.Add(new Tiles(8, "Exclamação", 1, 1, 1, 1, 100, 0f));

		ConstruirMapa();

		AtualizarNivel();
		AtualizarMoedas();
	}
	
	void Update()
	{
		if (desativarAnimacaoMapa)
			duracaoMovimentoMapa = 0;
		else if (duracaoMovimentoMapa != duracaoMovimentoMapaInicial)
			duracaoMovimentoMapa = duracaoMovimentoMapaInicial;

		if (Input.GetKeyDown(KeyCode.Z))
			EncerrarNivel();
	}

	void ConstruirMapa()
	{
		Invoke("TremerTela", duracaoMovimentoMapa);

		Invoke("AlterarCorFundoJogo", duracaoMovimentoMapa);

		jogo = new int[larguraJogo, alturaJogo];

		quantidadeTiles = larguraJogo * alturaJogo;

		AnimarPosicionamentoMapa();

		for (int x = 0; x < larguraJogo; x++)
		{
			for (int y = 0; y < alturaJogo; y++)
			{
				Tiles tile = nivel == 1 && x == 0 ? tiles[0] : PegarTileAleatorio();

				GameObject linha =
					GameObject
						.Find("Linha (" + (x + 1) + ")");

				GameObject tileGameObject =
					linha
						.transform
						.FindChild("Tile (" + (y + 1) + ")")
						.gameObject;

				ControladorTile controladorTile =
					tileGameObject
						.GetComponent<ControladorTile>();

				controladorTile.id = tile.id;
				controladorTile.hp = tile.hp;
				controladorTile.moedas = -1;
				Sprite sprite = spriteTiles[tile.id - 1];

				if (tile.instanciarDinossauro)
				{
					controladorTile.hp += hpBaseDinossauro;
					controladorTile.moedas = moedasDinossauro;
					sprite = spriteDinossauros[tile.dinossauro - 1];

					tile.instanciarDinossauro = false;
				}
				else if (tile.instanciarDiamante)
				{
					controladorTile.hp += hpBaseDiamante;
					controladorTile.moedas = Random.Range(moedasDiamanteMin, moedasDiamanteMax);
					sprite = spriteDiamantes[tile.diamante - 1];

					tile.instanciarDiamante = false;
				}

				tileGameObject
					.GetComponent<Image>()
					.sprite = sprite;

				tileGameObject.SetActive(true);

				jogo[x, y] = tile.id;
			}
		}
	}
	
	void TremerTela()
	{
		telaAnimator.SetTrigger("Animar");
	}

	void AlterarCorFundoJogo()
	{
		while (true)
		{
			fundoJogo.color = coresFundoJogo[Random.Range(0, coresFundoJogo.Count)];

			if (fundoJogo.color != corAnteriorFundoJogo)
			{
				corAnteriorFundoJogo = fundoJogo.color;
				break;
			}
		}
	}

	Tiles PegarTileAleatorio()
	{
		List<Tiles> tilesChances = new List<Tiles>();

		int chanceIndex = 0;

		foreach (Tiles tile in tiles)
		{
			if (tile.nivelMinimo <= nivel && tile.nivelMaximo >= nivel)
			{
				int chance = tile.PegarChance(nivel);

				if (chance > 0)
				{
					chanceIndex += chance;
					tile.chance = chanceIndex;

					tilesChances.Add(tile);
				}
			}
		}

		float numeroAleatorio = Random.Range(0, chanceIndex + 1);

		Tiles tileSelecionado = tilesChances[0];

		foreach (Tiles tile in tilesChances)
		{
			if (numeroAleatorio < tile.chance)
			{
				tileSelecionado = tile;
				break;
			}
		}

		if (tileSelecionado.dinossauro > 0)
		{
			float chance = Random.Range(0, 100);

			if (chance < chanceBaseDinossauro)
				tileSelecionado.instanciarDinossauro = true;
		}

		if (tileSelecionado.diamante > 0)
		{
			float chance = Random.Range(0, 100);

			if (chance < chanceBaseDiamante)
				tileSelecionado.instanciarDiamante = true;
		}

		return tileSelecionado;
	}

	void AvancarNivel()
	{
		nivel++;

		Invoke("AtualizarNivel", duracaoMovimentoMapa);
	}

	void AtualizarNivel()
	{
		nivelText.text = "Level " + nivel;
	}

	void AnimarPosicionamentoMapa()
	{
		mapa.localPosition = new Vector3(0, -1000);

		iTween.ValueTo(
			mapa.gameObject,
			iTween.Hash(
				"from", mapa.anchoredPosition,
				"to", new Vector2(0, 0),
				"time", duracaoMovimentoMapa,
				"onupdatetarget", this.gameObject,
				"onupdate", "MoveGuiElement",
				"easeType", animacaoMapa
			)
		);
	}

	void MoveGuiElement(Vector2 position)
	{
		mapa.anchoredPosition = position;
	}

	void ExibirTextoLimpo()
	{
		limpoAnimator.SetTrigger("Animar");
	}

	void EncerrarNivel()
	{
		ExibirTextoLimpo();

		Invoke("AvancarNivel", delayEncerrarNivel);

		Invoke("ConstruirMapa", delayEncerrarNivel);
	}

	void ProcessarAdicaoMoedas()
	{
		float chance = Random.Range(0, 100);

		if (chance < chanceTesouro)
		{
			Debug.Log("Tesouro");
		}
		else if (chance < chanceTesouro + chanceMoeda)
		{
			AdicionarMoedas(1);
		}
	}

	public void AdicionarMoedas(int aMoedas = 0)
	{
		moedas += aMoedas;

		AtualizarMoedas();
	}

	void AtualizarMoedas()
	{
		moedasText.text = moedas.ToString();
	}

	public void DestruirTile(int id, int moedas = -1)
	{
		Tiles tileDestruido = tiles[id - 1];

		if (moedas == -1)
			ProcessarAdicaoMoedas();
		else if (moedas > 0)
			AdicionarMoedas(moedas);

		quantidadeTiles--;

		if (quantidadeTiles == 0)
		{
			EncerrarNivel();
		}
	}

	static public ControladorJogo Pegar()
	{
		return GameObject.Find("ControladorJogo").GetComponent<ControladorJogo>();
	}
}
