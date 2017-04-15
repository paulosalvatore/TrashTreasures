using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Jogo : MonoBehaviour
{
	[Header("Mapa")]
	public Vector2 posicaoMapa;
	public float duracaoMovimentoMapa;
	public float delayEncerrarNivel;
	public iTween.EaseType animacaoMapa;
	public bool desativarAnimacaoMapa;
	private Transform mapa;
	private Vector3 posicaoFinalMapa;

	[Header("Tela")]
	public Vector2 resolucaoTela;
	public List<Color> coresFundoJogo;
	private Color corAnteriorFundoJogo;
	private SpriteRenderer fundoJogo;
	private Animator telaAnimator;
	internal bool bloqueadorClique;

	[Header("Nível")]
	public int nivelMaximo;
	internal int nivel = 1;
	private Text nivelText;
	private AudioSource nivelAudio;
	private Animator limpoAnimator;
	private Text nivelLimpoText;

	[Header("Tiles")]
	public GameObject tileQuebrado;
	private List<Tiles> tilesDisponiveis;
	private int quantidadeTiles;
	public bool oneHitTiles;
	public bool exibirTileQuebrado;
	private Vector2 tileObrigatorioPosicao;

	[Header("Chances ao Destruir Tile (0 - 100)")]
	public float chanceMoeda;

	[Header("Dinossauro")]
	public float chanceBaseDinossauro;
	public int hpBaseDinossauro;
	public Range moedasDinossauro;

	[Header("Diamante")]
	public float chanceBaseDiamante;
	public int hpBaseDiamante;
	public Range moedasDiamante;

	[Header("Moedas")]
	public Moedas moeda;
	public float delayEntreMoedas;
	public AudioClip moedasAudio;
	private int moedas;
	private Text moedasText;
	internal Image moedasImage;

	[Header("Pá")]
	public int paInicialId;
	public float duracaoAnimacaoPas;
	private int paId;
	internal Pas paSelecionada;
	private Pas proximaPa;
	private List<Pas> pasDisponiveis = new List<Pas>();
	private Animator paDisponivelAnimator;
	private Animator novaPaAnimator;
	private Image novaPaImage;
	private Text novaPaText;
	private Animator novaPaAssistirAnimator;
	private Animator novaPaComprarAnimator;
	private Text novaPaComprarText;
	private Transform pasAdquiridas;
	private Image paAdquiridaBase;

	[Header("Tesouros")]
	public float delayExibicaoTesouros;
	public float duracaoAnimacaoTesouros;
	internal float tempoTesouroAberto;
	private Animator novoTesouroAnimator;
	private Image novoTesouroFundo;
	private Image novoTesouroImage;
	private Text novoTesouroText;
	private Text tesouroText;
	private List<Tesouros> tesourosDisponiveis = new List<Tesouros>();
	private List<Tesouros> tesourosAdquiridos = new List<Tesouros>();

	// Definições da Área de Jogo
	private int[,] jogo;
	private int larguraJogo = 5;
	private int alturaJogo = 5;

	[Header("Ads")]
	public float duracaoAnimacaoTileAd;
	internal string recompensa;
	private Ads ads;
	private GameObject tileAd;
	private Animator tileAdAnimator;
	private Animator tileAdAssistirAnimator;

	// Audio Source

	private AudioSource audioSource;

	// Player Prefs

	private int nivelPref;

	private int moedasPref;
	private int paPref;

	[Header("John")]
	public float delayProximaPalavra;
	public float delayProximaFrase;
	public float duracaoMovimentoJohn;
	public bool separarFrasesJohn;
	public List<AudioClip> sonsJohn;
	internal bool bloqueadorCliqueJohn;
	private float timeUltimoCliqueJohn;
	private GameObject john;
	private Animator johnAnimator;
	private Transform johnBalao;
	private Text johnBalaoText;
	private Frases[] frases;
	private List<string> processarFrases;
	private int processarFrasesIndex = 0;
	private bool processandoFalasJohn;

	// Métodos Básicos

	private void Start()
	{
		// Definir Resolução Base da Tela
		DefinicoesScreen();

		// Inicialização de Componentes Externos
		PegarComponentes();

		// Definição de Variáveis Iniciais
		DefinirVariaveisIniciais();

		// Player Prefs
		PegarPlayerPrefs();

		// Mapa
		IniciarMapa();

		// Pás
		AtualizarPas();

		// Nível
		AtualizarNivel();

		// Moedas
		AtualizarMoedas();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Z))
			EncerrarNivel();
		else if (Input.GetKeyDown(KeyCode.W))
			ResetarPlayerPrefs();
		else if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (ChecarNovaPaAnimator())
				PaVoltar();
			else
				ResetarPlayerPrefs();
		}

		PegarTilesHit();

		PegarCliqueJohn();
	}

	// Métodos de Inicialização de Componentes e Variáveis

	private void DefinicoesScreen()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		Screen.SetResolution(
			(int)resolucaoTela.x,
			(int)resolucaoTela.y, true
		);
	}

	private void PegarComponentes()
	{
		// Mapa
		mapa = GameObject.Find("Mapa").transform;

		// Tela
		fundoJogo = GameObject.Find("FundoJogo").GetComponent<SpriteRenderer>();
		telaAnimator = GameObject.Find("Tela").GetComponent<Animator>();

		// Nível
		nivelText = GameObject.Find("Nível").GetComponent<Text>();
		nivelAudio = nivelText.GetComponent<AudioSource>();
		limpoAnimator = GameObject.Find("Limpo").GetComponent<Animator>();
		nivelLimpoText = limpoAnimator.transform.FindChild("NívelLimpo").GetComponent<Text>();

		// Moedas
		moedasText = GameObject.Find("Moedas").GetComponent<Text>();
		moedasImage = moedasText.transform.FindChild("Imagem").GetComponent<Image>();

		// Pás
		paDisponivelAnimator = GameObject.Find("PáDisponível").GetComponent<Animator>();
		novaPaAnimator = GameObject.Find("NovaPá").GetComponent<Animator>();
		novaPaImage = novaPaAnimator.transform.FindChild("Imagem").GetComponent<Image>();
		novaPaText = novaPaAnimator.transform.FindChild("Nome").GetComponent<Text>();
		novaPaAssistirAnimator = novaPaAnimator.transform.FindChild("Assistir").GetComponent<Animator>();
		novaPaComprarAnimator = novaPaAnimator.transform.FindChild("Comprar").GetComponent<Animator>();
		novaPaComprarText = novaPaComprarAnimator.transform.FindChild("Texto").GetComponent<Text>();
		pasAdquiridas = GameObject.Find("PásAdquiridas").transform;
		paAdquiridaBase = pasAdquiridas.FindChild("Pá (Base)").GetComponent<Image>();

		// Tesouros
		novoTesouroAnimator = GameObject.Find("NovoTesouro").GetComponent<Animator>();
		novoTesouroFundo = novoTesouroAnimator.GetComponent<Image>();
		novoTesouroImage = novoTesouroAnimator.transform.FindChild("Imagem").GetComponent<Image>();
		novoTesouroText = novoTesouroAnimator.transform.FindChild("Nome").GetComponent<Text>();
		tesouroText = GameObject.Find("TextoTesouro").GetComponent<Text>();

		// Ads
		ads = GetComponent<Ads>();
		tileAd = GameObject.Find("TileAd");
		tileAdAnimator = tileAd.GetComponent<Animator>();
		tileAdAssistirAnimator = tileAd.transform.FindChild("Assistir").GetComponent<Animator>();

		// AudioSource
		audioSource = GetComponent<AudioSource>();

		// John
		john = GameObject.Find("John");
		johnAnimator = john.GetComponent<Animator>();
		johnBalao = john.transform.FindChild("Balão");
		johnBalaoText = johnBalao.FindChild("Texto").GetComponent<Text>();
		frases = john.GetComponents<Frases>();
	}

	private void DefinirVariaveisIniciais()
	{
		// Mapa
		posicaoFinalMapa = mapa.position;

		if (desativarAnimacaoMapa)
			duracaoMovimentoMapa = 0;

		// Tela
		corAnteriorFundoJogo = fundoJogo.color;

		// Pá
		paId = paInicialId;

		// Pás
		PegarPasDisponiveis();

		// Tesouros
		PegarTesourosDisponiveis();
	}

	// Mapa/Tela

	private void IniciarMapa()
	{
		// Definimos a quantidade de tiles
		quantidadeTiles = larguraJogo * alturaJogo;

		// Zeramos o Mapa
		ZerarMapa();

		// Construímos o Mapa
		ConstruirMapa();

		// Movimentamos o Mapa
		MovimentarMapa();

		// Chamamos o método que irá tremer a tela assim que o movimento do mapa terminar
		Invoke("TremerTela", duracaoMovimentoMapa);

		// Chamamos o método que irá alterar a cor de fundo do jogo assim que o movimento do mapa terminar
		Invoke("AlterarCorFundoJogo", duracaoMovimentoMapa);

		// Chamamos o método que irá tocar o áudio do nível assim que o movimento do mapa terminar
		Invoke("TocarAudioNivel", duracaoMovimentoMapa);

		// Bloquear o Clique enquanto o mapa se movimenta
		BloquearClique();

		// Desbloquear o Clique após a movimentação do Mapa
		Invoke("DesbloquearClique", duracaoMovimentoMapa);

		// Checar se o John irá aparecer no nível atual
		ChecarJohn();
	}

	private void ZerarMapa()
	{
		foreach (Transform child in mapa)
			Destroy(child.gameObject);
	}

	private void ReiniciarPosicaoMapa()
	{
		mapa.position = posicaoMapa;
	}

	public void ConstruirMapa(Transform mapaDestino = null, int nivelMapa = 0)
	{
		PegarTilesDisponiveis();

		tileObrigatorioPosicao =
			new Vector2(
				Random.Range(0, larguraJogo),
				Random.Range(0, alturaJogo)
			);

		for (int y = 0; y < alturaJogo; y++)
			for (int x = 0; x < larguraJogo; x++)
				InstanciarTile(x, y, mapaDestino, nivelMapa);
	}

	private void MovimentarMapa()
	{
		// Reiniciamos a posição do mapa
		ReiniciarPosicaoMapa();

		// Movimentamos o mapa utilizando a biblioteca iTween que anima o movimento
		iTween.MoveTo(
			mapa.gameObject,
			iTween.Hash(
				"position", posicaoFinalMapa,
				"easeType", animacaoMapa,
				"time", duracaoMovimentoMapa
			)
		);
	}

	private void TremerTela()
	{
		// Aplica o Trigger "Animar" do Animator atrelado ao Background
		telaAnimator.SetTrigger("Animar");
	}

	private void AlterarCorFundoJogo()
	{
		while (corAnteriorFundoJogo == fundoJogo.color)
			fundoJogo.color = PegarCorFundoAleatoria();

		corAnteriorFundoJogo = fundoJogo.color;
	}

	private Color PegarCorFundoAleatoria()
	{
		return coresFundoJogo[Random.Range(0, coresFundoJogo.Count)];
	}

	private void BloquearClique()
	{
		bloqueadorClique = true;
	}

	private void DesbloquearClique()
	{
		if (!bloqueadorCliqueJohn)
			bloqueadorClique = false;
	}

	// Nível

	private void AvancarNivel()
	{
		nivel++;

		DefinirPlayerPrefs();

		Invoke("AtualizarNivel", duracaoMovimentoMapa);
	}

	private void AtualizarNivel()
	{
		nivelText.text = string.Format("Level {0}", nivel);

		ExibirPaDisponivel();

		Invoke("VibrarNivel", (nivel == 1 || nivel == nivelPref ? duracaoMovimentoMapa : 0) + 0.2f);
	}

	private void VibrarNivel()
	{
		Handheld.Vibrate();
	}

	private void EncerrarNivel()
	{
		ExibirTextoNivelLimpo();

		Invoke("AvancarNivel", delayEncerrarNivel);

		Invoke("IniciarMapa", delayEncerrarNivel);
	}

	private void ExibirTextoNivelLimpo()
	{
		nivelLimpoText.text = nivelText.text;

		limpoAnimator.SetTrigger("Animar");
	}

	private void TocarAudioNivel()
	{
		nivelAudio.Play();
	}

	// Tiles

	private void InstanciarTile(int x, int y, Transform mapaDestino, int nivelMapa)
	{
		nivelMapa = nivelMapa == 0 ? nivel : nivelMapa;
		/*
		 * Pegar o Tile que será instanciado
		 * Se estivermos no nível 1 e o y for 0, instanciamos a Grama
		 * Caso contrário, pegamos um tile aleatório.
		 */
		Tiles tile =
			nivelMapa == 1 && y == 0
				?
			tilesDisponiveis[0]
				:
			PegarTileAleatorio(x, y, nivelMapa);

		// Instanciamos o tile na cena baseado na posição calculada e na rotação base do prefab
		Tiles tileInstanciado = Instantiate(tile);

		// Definimos um nome para o tile "x,y" para que fique mais organizado
		tileInstanciado.name = string.Format("{0},{1}", x, y);

		// Colocamos o tile instanciado dentro do GameObject destinado ao mapa, para melhor organização
		tileInstanciado.transform.parent = mapaDestino ? mapaDestino : mapa;

		// Definimos a posição do tile baseado no X e no Y do for
		Vector2 posicaoTile = new Vector2(
			x,
			y * -1
		);

		// Aplicar a posição definida para o tile no localPosition
		tileInstanciado.transform.localPosition = posicaoTile;

		/*
		 * Se o tile instanciado tiver disponibilidade de ser um DINOSSAURO iremos
		 * chamar um número aleatório para checar se alteramos ele ou não
		 */
		if (tile.dinossauro != null)
		{
			float chance = Random.Range(0, 100);

			if (chance < chanceBaseDinossauro)
				tileInstanciado.instanciarDinossauro = true;
		}

		/*
		 * Se o tile instanciado tiver disponibilidade de ser um DIAMANTE iremos
		 * chamar um número aleatório para checar se alteramos ele ou não
		 */
		if (!tileInstanciado.instanciarDinossauro && tile.diamante != null)
		{
			float chance = Random.Range(0, 100);

			if (chance < chanceBaseDiamante)
				tileInstanciado.instanciarDiamante = true;
		}

		tileInstanciado.Instanciar();
	}

	private void PegarTilesDisponiveis()
	{
		tilesDisponiveis = new List<Tiles>();

		Transform tiles = GameObject.Find("TilesDisponíveis").transform;

		foreach (Transform tile in tiles)
			tilesDisponiveis.Add(tile.GetComponent<Tiles>());
	}

	private Tiles PegarTileAleatorio(int x, int y, int nivelMapa)
	{
		List<Tiles> tilesChances = new List<Tiles>();

		int chanceIndex = 0;

		Tiles tileSelecionado = null;

		foreach (Tiles tile in tilesDisponiveis)
		{
			if (nivelMapa == tile.aparecerObrigatoriamenteNivel &&
				tileObrigatorioPosicao.x == x &&
				tileObrigatorioPosicao.y == y)
			{
				tileSelecionado = tile;

				tilesDisponiveis.Remove(tile);

				break;
			}

			int chance = tile.PegarChance(nivelMapa);

			if (tile.bauTesouro && tesourosAdquiridos.Count == tesourosDisponiveis.Count)
				chance = 0;

			if (chance > 0)
			{
				chanceIndex += chance;
				tile.chance = chanceIndex;

				tilesChances.Add(tile);
			}
		}

		float numeroAleatorio = Random.Range(0, chanceIndex + 1);

		if (tileSelecionado == null)
		{
			tileSelecionado = tilesChances[0];

			foreach (Tiles tile in tilesChances)
			{
				if (numeroAleatorio < tile.chance)
				{
					tileSelecionado = tile;

					if (tile.limiteUmPorNivel)
						tilesDisponiveis.Remove(tile);

					break;
				}
			}
		}

		return tileSelecionado;
	}

	public void ProcessarTileDestruido(Transform tileDestruido, int moedas, bool tileEspecial)
	{
		if (moedas == -1)
			moedas = ProcessarAdicaoMoedas();

		if (moedas > 0)
			AdicionarMoedas(tileDestruido, moedas);

		quantidadeTiles--;

		if (quantidadeTiles == 0 && !tileEspecial)
			EncerrarNivel();
	}

	private void PegarTilesHit()
	{
		if (bloqueadorClique)
			return;

		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);
			if (touch.phase == TouchPhase.Began)
			{
				Vector3 posicao = Camera.main.ScreenToWorldPoint(touch.position);
				RaycastHit2D hit = Physics2D.Raycast(posicao, Vector2.zero);
				if (hit != null && hit.collider != null)
				{
					Tiles tileHit = hit.collider.GetComponent<Tiles>();

					if (tileHit)
						tileHit.HitTile();
				}
			}
		}
	}

	// Moedas

	private int ProcessarAdicaoMoedas()
	{
		int adicionarMoedas = 0;

		float chance = Random.Range(0, 100);
		if (chance < chanceMoeda)
			adicionarMoedas = 1;

		return adicionarMoedas;
	}

	public void AdicionarMoedas(Transform origem, int adicionarMoedas = 0)
	{
		StartCoroutine(AdicionarMoedas(origem, adicionarMoedas, true));
	}

	private IEnumerator AdicionarMoedas(Transform origem, int adicionarMoedas, bool coroutine)
	{
		Vector3 posicao = origem.position;
		Quaternion rotacao = origem.rotation;

		for (int i = 0; i < adicionarMoedas; i++)
		{
			Jogo.ReproduzirAudio(moedasAudio);

			StartCoroutine(AtualizarMoedas(1));

			Instantiate(moeda, posicao, rotacao);

			yield return new WaitForSeconds(delayEntreMoedas);
		}
	}

	private void AtualizarMoedas()
	{
		moedasText.text = moedas.ToString();
	}

	private IEnumerator AtualizarMoedas(int adicionarMoedas)
	{
		yield return new WaitForSeconds(moeda.duracao);

		moedas += adicionarMoedas;

		AtualizarMoedas();
	}

	// Pás

	private void PegarPasDisponiveis()
	{
		Transform pas = GameObject.Find("PásDisponíveis").transform;

		foreach (Transform child in pas)
			pasDisponiveis.Add(child.GetComponent<Pas>());
	}

	private void SelecionarPa()
	{
		paSelecionada = pasDisponiveis[paId - 1];

		if (paId < pasDisponiveis.Count)
			proximaPa = pasDisponiveis[paId];

		AlterarPaAnimator(false);

		AtualizarPaAdquirida();
	}

	public void EvoluirPa(bool gratuito = false, bool fechar = true)
	{
		int custo = gratuito ? 0 : proximaPa.moedas;

		if (paId >= pasDisponiveis.Count || nivel < proximaPa.nivel || moedas < custo)
		{
			audioSource.Play();

			return;
		}

		if (custo > 0)
		{
			moedas -= custo;

			AtualizarMoedas();
		}

		if (fechar)
			PaVoltar();

		paId++;

		SelecionarPa();
	}

	private void ExibirPaDisponivel()
	{
		if (paId >= pasDisponiveis.Count)
			return;

		if (nivel >= proximaPa.nivel)
			AlterarPaAnimator(true);
	}

	private void AlterarPaAnimator(bool estado)
	{
		paDisponivelAnimator.SetBool("Exibir", estado);
	}

	public void ExibirNovaPaDisponivel()
	{
		novaPaImage.sprite = proximaPa.sprite;
		novaPaText.text = proximaPa.nome;
		novaPaComprarText.text = proximaPa.moedas.ToString();

		AlterarNovaPaAnimator(true);

		AlterarNovaPaAssistirAnimator(!ads.ChecarAd());

		BloquearClique();
	}

	private void AlterarNovaPaAnimator(bool estado)
	{
		novaPaAnimator.SetBool("Exibir", estado);
	}

	private void AlterarNovaPaAssistirAnimator(bool estado)
	{
		novaPaAssistirAnimator.SetBool("Inativo", estado);
	}

	private bool ChecarNovaPaAnimator()
	{
		return novaPaAnimator.GetBool("Exibir");
	}

	public void PaAssistir()
	{
		recompensa = "pa";

		ads.ExibirAd();
	}

	public void PaComprar()
	{
		EvoluirPa();
	}

	public void PaVoltar()
	{
		Invoke("DesbloquearClique", duracaoAnimacaoPas);

		AlterarNovaPaAnimator(false);
	}

	private void AtualizarPas()
	{
		SelecionarPa();

		for (int i = paInicialId; i < paPref; i++)
			EvoluirPa(true, false);
	}

	private void AtualizarPaAdquirida()
	{
		Image paAdquirida = Instantiate(paAdquiridaBase);

		paAdquirida.enabled = true;
		paAdquirida.sprite = pasDisponiveis[paId - 1].sprite;
		paAdquirida.name = string.Format("Pá ({0})", paId);
		paAdquirida.transform.SetParent(pasAdquiridas);
		paAdquirida.transform.localPosition = paAdquiridaBase.transform.localPosition;
		paAdquirida.transform.localScale = paAdquiridaBase.transform.localScale;

		paAdquirida.rectTransform.anchoredPosition =
			new Vector2(
				(
					paAdquiridaBase.rectTransform.anchoredPosition.x +
					(paAdquiridaBase.rectTransform.sizeDelta.x / 2 * (paId - 1))
				),
				paAdquirida.rectTransform.anchoredPosition.y
			);
	}

	// Tesouros

	private void PegarTesourosDisponiveis()
	{
		Transform tesouros = GameObject.Find("TesourosDisponíveis").transform;

		foreach (Transform child in tesouros)
			tesourosDisponiveis.Add(child.GetComponent<Tesouros>());
	}

	private Tesouros PegarTesouro()
	{
		Tesouros tesouroSelecionado = null;

		foreach (Tesouros tesouro in tesourosDisponiveis)
		{
			if (!tesourosAdquiridos.Contains(tesouro))
			{
				tesouroSelecionado = tesouro;
				break;
			}
		}

		return tesouroSelecionado;
	}

	public void AdicionarTesouro(Tesouros tesouro = null)
	{
		tempoTesouroAberto = Time.time + delayExibicaoTesouros + duracaoAnimacaoTesouros;

		bool exibirAnimator = true;

		if (tesouro != null)
			exibirAnimator = false;
		else
			tesouro = PegarTesouro();

		if (tesouro != null)
		{
			tesourosAdquiridos.Add(tesouro);

			AtualizarPorcentagemTesouros();

			novoTesouroImage.sprite = tesouro.sprite;
			novoTesouroText.text = tesouro.nome;

			if (exibirAnimator)
			{
				AlterarCorFundoNovoTesouro();

				AlterarNovoTesouroAnimator(true);

				BloquearClique();
			}
		}
		else if (quantidadeTiles == 0)
			EncerrarNivel();
	}

	public void OcultarTesouro()
	{
		Invoke("DesbloquearClique", duracaoAnimacaoTesouros);

		AlterarNovoTesouroAnimator(false);

		if (tesourosAdquiridos.Count == tesourosDisponiveis.Count)
			ChecarJohn(true);
		else if (quantidadeTiles == 0)
			Invoke("EncerrarNivel", duracaoAnimacaoTesouros);
	}

	private void AlterarNovoTesouroAnimator(bool estado)
	{
		novoTesouroAnimator.SetBool("Exibir", estado);
	}

	private void AtualizarPorcentagemTesouros()
	{
		float porcentagem = tesourosAdquiridos.Count * 100 / tesourosDisponiveis.Count;

		tesouroText.text = string.Format("{0}%", porcentagem);
	}

	private void AlterarCorFundoNovoTesouro()
	{
		novoTesouroFundo.color = PegarCorFundoAleatoria();
	}

	// Ads

	public void ExibirAd(string novaRecompensa = "")
	{
		recompensa = novaRecompensa;

		ads.ExibirAd();
	}

	public void AssistirTileAd()
	{
		ExibirAd("tesouro");

		OcultarTileAd();
	}

	public void TileAdVoltar()
	{
		OcultarTileAd();

		Invoke("DesbloquearClique", duracaoAnimacaoTileAd);

		if (quantidadeTiles == 0)
			Invoke("EncerrarNivel", duracaoAnimacaoTileAd);
	}

	public void ExibirTileAd()
	{
		AlterarTileAdAnimator(true);

		AlterarTileAdAssistirAnimator(!ads.ChecarAd());

		BloquearClique();
	}

	private void OcultarTileAd()
	{
		AlterarTileAdAnimator(false);
	}

	private void AlterarTileAdAnimator(bool estado)
	{
		tileAdAnimator.SetBool("Exibir", estado);
	}

	private void AlterarTileAdAssistirAnimator(bool estado)
	{
		tileAdAssistirAnimator.SetBool("Inativo", estado);
	}

	// Player Prefs

	private void PegarPlayerPrefs()
	{
		nivelPref = PlayerPrefs.GetInt("Nível");

		if (nivelPref > 0)
			nivel = nivelPref;

		moedasPref = PlayerPrefs.GetInt("Moedas");

		if (moedasPref > 0)
			moedas = moedasPref;

		paPref = PlayerPrefs.GetInt("Pá");

		foreach (Tesouros tesouroDisponivel in tesourosDisponiveis)
		{
			bool tesouroAdquirido = PlayerPrefs.GetInt(tesouroDisponivel.nome) == 1;

			if (tesouroAdquirido)
				AdicionarTesouro(tesouroDisponivel);
		}
	}

	private void DefinirPlayerPrefs()
	{
		PlayerPrefs.SetInt("Nível", nivel);
		PlayerPrefs.SetInt("Moedas", moedas);
		PlayerPrefs.SetInt("Pá", paId);

		foreach (Tesouros tesouroDisponivel in tesourosDisponiveis)
		{
			int tesouroAdquirido = tesourosAdquiridos.Contains(tesouroDisponivel) ? 1 : 0;

			PlayerPrefs.SetInt(tesouroDisponivel.nome, tesouroAdquirido);
		}
	}

	private void ResetarPlayerPrefs()
	{
		PlayerPrefs.SetInt("Nível", 1);
		PlayerPrefs.SetInt("Moedas", 0);
		PlayerPrefs.SetInt("Pá", 1);

		foreach (Tesouros tesouroDisponivel in tesourosDisponiveis)
			PlayerPrefs.SetInt(tesouroDisponivel.nome, 0);

		ReiniciarCena();
	}

	// Cena

	private void ReiniciarCena()
	{
		int cena = SceneManager.GetActiveScene().buildIndex;
		SceneManager.LoadScene(cena, LoadSceneMode.Single);
	}

	// John

	private void ChecarJohn(bool tesouro = false)
	{
		foreach (Frases frase in frases)
		{
			bool exibirFraseTesouro = false;

			if (tesouro)
			{
				if (frase.tesouros)
					exibirFraseTesouro = true;
				else
					continue;
			}

			if (frase.nivel > 0 && nivel == frase.nivel ||
				frase.nivelRange.min != 0 && frase.nivelRange.min >= frase.nivel ||
				frase.nivelRange.max != 0 && frase.nivelRange.max <= frase.nivel ||
				exibirFraseTesouro)
			{
				if (frase.chance > 0)
				{
					float chance = Random.Range(1, 101);

					if (chance > frase.chance)
						continue;
				}

				if (frase.aleatoria)
					processarFrases = new List<string>(
						new string[] {
							frase.frases[
								Random.Range(0, frase.frases.Count)
							]
						}
					);
				else
					processarFrases = frase.frases;

				processarFrasesIndex = 0;
				processandoFalasJohn = true;
				LimparFalaJohn();

				Invoke("ExibirJohn", duracaoMovimentoMapa);
				BloquearCliqueJohn();

				float delay = duracaoMovimentoMapa + duracaoMovimentoJohn;
				Invoke("ProcessarFrasesJohn", delay);
				timeUltimoCliqueJohn = Time.time + delay;

				break;
			}
		}
	}

	private void BloquearCliqueJohn()
	{
		bloqueadorCliqueJohn = true;

		BloquearClique();
	}

	private void DesbloquearCliqueJohn()
	{
		bloqueadorCliqueJohn = false;

		DesbloquearClique();
	}

	private void ProcessarFrasesJohn()
	{
		if (processarFrasesIndex >= processarFrases.Count)
		{
			processandoFalasJohn = false;

			OcultarJohn();
		}
		else
		{
			LimparFalaJohn();

			StartCoroutine("ConstruirFrase");

			ReproduzirAudioJohn();

			timeUltimoCliqueJohn = Time.time;
		}
	}

	private IEnumerator ConstruirFrase()
	{
		string frase = processarFrases[processarFrasesIndex];

		if (separarFrasesJohn)
		{
			string[] palavras = frase.Split(' ');

			foreach (string palavra in palavras)
			{
				johnBalaoText.text = string.Format("{0} {1}", johnBalaoText.text, palavra);

				yield return new WaitForSeconds(delayProximaPalavra);
			}
		}
		else
			johnBalaoText.text = frase;
	}

	private void LimparFalaJohn()
	{
		johnBalaoText.text = "";
	}

	private void ProximaFraseJohn()
	{
		processarFrasesIndex++;

		ProcessarFrasesJohn();
	}

	private void ExibirJohn()
	{
		AlterarAnimator("Entrar");

		Invoke("ReproduzirAudioJohn", duracaoMovimentoJohn / 2f);

		if (ChecarNovaPaAnimator())
			AlterarNovaPaAnimator(false);
	}

	private void OcultarJohn()
	{
		AlterarAnimator("Sair");

		Invoke("DesbloquearCliqueJohn", duracaoMovimentoJohn);

		if (quantidadeTiles == 0 &&
			tesourosAdquiridos.Count == tesourosDisponiveis.Count)
		{
			Invoke("EncerrarNivel", duracaoMovimentoJohn);
		}
	}

	private void AlterarAnimator(string trigger)
	{
		johnAnimator.SetTrigger(trigger);
	}

	private void PegarCliqueJohn()
	{
		if (!processandoFalasJohn)
			return;

		if (Input.GetMouseButtonDown(0) &&
			timeUltimoCliqueJohn + delayProximaFrase <= Time.time)
		{
			ProximaFraseJohn();
		}
	}

	private void ReproduzirAudioJohn()
	{
		ReproduzirAudio(sonsJohn[Random.Range(0, sonsJohn.Count)]);
	}

	// Métodos Estáticos

	static public Jogo Pegar()
	{
		return GameObject.Find("Jogo").GetComponent<Jogo>();
	}

	static public void ReproduzirAudio(AudioClip clip = null)
	{
		if (clip == null)
			return;

		GameObject objeto = new GameObject();

		objeto.name = clip.name;

		AudioSource audioSource = objeto.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.Play();

		Destroy(objeto, clip.length);
	}
}
