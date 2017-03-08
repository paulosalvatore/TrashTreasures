using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tiles
{
	// Variáveis definidas na construção da classe
	public int id;
	public string nome;
	public int hp;
	public float chanceBase;
	public float chanceMax;
	public float nivelMinimo;
	public float nivelMaximo;
	public float modificadorNivel;
	public float chance;

	public Tiles(
		int aId,
		string aNome,
		int aHp,
		float aChanceBase,
		float aChanceMax,
		int aNivelMinimo,
		int aNivelMaximo,
		float aModificadorNivel
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

	// Nível e Pontuação
	private int nivel = 1;
	private float moedas;

	// Componentes de Texto do Canvas
	private Text nivelImage;
	private Text moedasText;

	// Definições da Área de Jogo
	private int[,] jogo;
	private int larguraJogo = 5;
	private int alturaJogo = 5;

	void Start()
	{
		nivelImage = GameObject.Find("Nível").GetComponent<Text>();
		moedasText = GameObject.Find("Moedas").GetComponent<Text>();

		tiles.Add(new Tiles(1, "Grama", 2, 0, 0, 0, 1, 0));
		tiles.Add(new Tiles(2, "Terra", 2, 80, 0, 0, 50, -1.5f));
		tiles.Add(new Tiles(3, "Areia", 1, 60, 0, 0, 50, -1.4f));
		tiles.Add(new Tiles(4, "Pedra", 10, 5, 40, 3, 70, 2f));
		tiles.Add(new Tiles(5, "Pedra2", 20, 5, 0, 20, 100, 1.2f));
		tiles.Add(new Tiles(6, "PedraLava", 60, 5, 0, 40, 100, 0.25f));
		tiles.Add(new Tiles(7, "Pedra3", 100, 5, 0, 40, 100, 0.25f));

		ConstruirMapa();

		AtualizarNivel();
		AtualizarMoedas();
	}
	
	void ConstruirMapa()
	{
		jogo = new int[larguraJogo, alturaJogo];

		quantidadeTiles = larguraJogo * alturaJogo;

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

				tileGameObject
					.GetComponent<Image>()
					.sprite = spriteTiles[tile.id - 1];

				tileGameObject.SetActive(true);

				jogo[x,y] = tile.id;
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

		return tileSelecionado;
	}

	void AvancarNivel()
	{
		nivel++;

		AtualizarNivel();
	}

	void AtualizarNivel()
	{
		nivelImage.text = "Level " + nivel;
	}

	void EncerrarNivel()
	{
		AvancarNivel();

		Invoke("ConstruirMapa", 1);
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

	public void DestruirTile(int id)
	{
		Tiles tileDestruido = tiles[id - 1];

		ProcessarAdicaoMoedas();

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
