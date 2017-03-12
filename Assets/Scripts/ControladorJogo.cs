using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControladorJogo : MonoBehaviour
{
	[Header("Cores do Fundo")]
	public List<Color> coresFundoJogo;
	private Color corAnteriorFundoJogo;
	private Image fundoJogo;
	private RectTransform fundoJogoRect;

	[Header("Tiles")]
	public List<Sprite> spriteTiles;
	private List<Tiles> tiles = new List<Tiles>();
	private int quantidadeTiles;
	private float tamanhoTile;

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

	[Header("Tela")]
	public float margemExtraTop;
	public float margemBottom;
	private RectTransform ceu;
	private Animator telaAnimator;
	private bool atualizarTamanho = true;

	[Header("Mapa")]
	public float duracaoMovimentoMapa;
	public float delayEncerrarNivel;
	private float duracaoMovimentoMapaInicial;
	private RectTransform mapa;

	[Header("Tipo de Animação do Mapa")]
	public string animacaoMapa;

	[Header("Variáveis para Testes")]
	public bool oneHitTiles;
	public bool desativarAnimacaoMapa;

	[Header("Áudios")]
	public List<AudioClip> audios;

	// Definições da Área de Jogo
	private int[,] jogo;
	private int larguraJogo = 5;
	private int alturaJogo = 5;

	// Nível
	private int nivel = 1;
	private Text nivelText;
	private AudioSource nivelAudio;
	private Animator nivelLimpoAnimator;

	// Pontuação
	private float moedas;
	private Text moedasText;
	private AudioSource moedasAudio;

	void Start()
	{
		// Pegar Componentes Instanciados na Cena
		nivelText = GameObject.Find("Nível").GetComponent<Text>();
		nivelAudio = nivelText.GetComponent<AudioSource>();

		moedasText = GameObject.Find("Moedas").GetComponent<Text>();
		moedasAudio = moedasText.GetComponent<AudioSource>();

		fundoJogo = GameObject.Find("FundoJogo").GetComponent<Image>();
		fundoJogoRect = fundoJogo.GetComponent<RectTransform>();

		ceu = GameObject.Find("Céu").GetComponent<RectTransform>();

		nivelLimpoAnimator = GameObject.Find("NívelLimpo").GetComponent<Animator>();

		telaAnimator = GameObject.Find("Tela").GetComponent<Animator>();

		mapa = GameObject.Find("Jogo").GetComponent<RectTransform>();

		// Variáveis com Valores Iniciais
		duracaoMovimentoMapaInicial = duracaoMovimentoMapa;
		corAnteriorFundoJogo = fundoJogo.color;
		
		// Pegar os tiles associados ao ControladorJogo
		foreach (Transform child in transform)
			tiles.Add(child.GetComponent<Tiles>());

		tamanhoTile = Screen.width / 5;

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
	
	// Tela
	void ConstruirMapa()
	{
		Invoke("TremerTela", duracaoMovimentoMapa);

		Invoke("AlterarCorFundoJogo", duracaoMovimentoMapa);

		jogo = new int[larguraJogo, alturaJogo];

		quantidadeTiles = larguraJogo * alturaJogo;

		AnimarPosicionamentoMapa();

		if (atualizarTamanho)
		{
			fundoJogoRect
				.sizeDelta =
					new Vector2(
						Screen.width,
						Screen.width
					);

			fundoJogoRect
				.anchoredPosition =
					new Vector2(
						0,
						Screen.width / 2 + margemBottom
					);

			ceu.sizeDelta = new Vector2(
					Screen.width * 2,
					Screen.height - Screen.width - margemBottom + margemExtraTop
				);
		}

		for (int x = 0; x < larguraJogo; x++)
		{
			GameObject linha =
				GameObject
					.Find("Linha (" + (x + 1) + ")");

			if (atualizarTamanho)
			{
				RectTransform linhaRectTransform =
					linha
						.GetComponent<RectTransform>();

				linhaRectTransform
					.anchoredPosition =
						new Vector2(
							0,
							(larguraJogo - x - 1) * tamanhoTile + (tamanhoTile / 2) + margemBottom
						);

				linhaRectTransform
					.sizeDelta =
						new Vector2(
							Screen.width,
							tamanhoTile
						);
			}

			for (int y = 0; y < alturaJogo; y++)
			{
				Tiles tile = nivel == 1 && x == 0 ? tiles[0] : PegarTileAleatorio();

				GameObject tileGameObject =
					linha
						.transform
						.FindChild("Tile (" + (y + 1) + ")")
						.gameObject;

				Tiles tileInstanciado =
					tileGameObject
						.GetComponent<Tiles>();

				tileInstanciado.id = tile.id;
				tileInstanciado.nome = tile.name;
				tileInstanciado.hp = tile.hp;
				tileInstanciado.hit = tile.hit;
				tileInstanciado.destruir = tile.destruir;

				tileInstanciado.moedas = -1;

				Sprite sprite = spriteTiles[tile.id - 1];

				if (tile.instanciarDinossauro)
				{
					tileInstanciado.hp += hpBaseDinossauro;
					tileInstanciado.moedas = moedasDinossauro;
					sprite = spriteDinossauros[tile.dinossauro - 1];

					tile.instanciarDinossauro = false;
				}
				else if (tile.instanciarDiamante)
				{
					tileInstanciado.hp += hpBaseDiamante;
					tileInstanciado.moedas = Random.Range(moedasDiamanteMin, moedasDiamanteMax);
					sprite = spriteDiamantes[tile.diamante - 1];

					tile.instanciarDiamante = false;
				}

				Image tileImage =
					tileGameObject
						.GetComponent<Image>();

				tileImage.sprite = sprite;
				tileImage.enabled = true;

				jogo[x, y] = tile.id;

				if (atualizarTamanho)
				{
					tileGameObject
						.GetComponent<RectTransform>()
						.anchoredPosition =
							new Vector2(
								tamanhoTile * y,
								0
							);

					tileGameObject
						.GetComponent<RectTransform>()
						.sizeDelta =
							new Vector2(
								tamanhoTile - Screen.width,
								tamanhoTile
							);
				}
			}
		}

		if (atualizarTamanho)
			atualizarTamanho = false;
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

	// Tiles
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

	// Nível
	void AvancarNivel()
	{
		nivel++;

		Invoke("AtualizarNivel", duracaoMovimentoMapa);
	}

	void AtualizarNivel()
	{
		nivelText.text = "Level " + nivel;
	}

	void EncerrarNivel()
	{
		nivelAudio.Play();

		ExibirTextoNivelLimpo();

		Invoke("AvancarNivel", delayEncerrarNivel);

		Invoke("ConstruirMapa", delayEncerrarNivel);
	}
	
	void ExibirTextoNivelLimpo()
	{
		nivelLimpoAnimator.SetTrigger("Animar");
	}

	// Moedas
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

		moedasAudio.Play();

		AtualizarMoedas();
	}

	void AtualizarMoedas()
	{
		moedasText.text = moedas.ToString();
	}

	// Métodos Estáticos
	static public ControladorJogo Pegar()
	{
		return GameObject.Find("ControladorJogo").GetComponent<ControladorJogo>();
	}

	static public AudioSource AdicionarAudioSource(GameObject objeto, AudioClip clip)
	{
		AudioSource audioSource = objeto.AddComponent<AudioSource>();

		audioSource.playOnAwake = false;

		audioSource.clip = clip;

		return audioSource;
	}
}
