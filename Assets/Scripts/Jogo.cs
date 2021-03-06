﻿using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Jogo : MonoBehaviour
{
	public static Jogo instancia;

	[Header("Mapa")]
	public Vector2 posicaoMapa;
	public float duracaoMovimentoMapa;
	public float delayEncerrarNivel;
	public iTween.EaseType animacaoMapa;
	public bool desativarAnimacaoMapa;
	public Transform mapa;
	private Vector3 posicaoFinalMapa;

	[Header("Tela")]
	public Vector2 resolucaoTela;
	public List<Color> coresFundoJogo;
	public Transform canvas;
	private Color corAnteriorFundoJogo;
	public SpriteRenderer fundoJogo;
	public Animator telaAnimator;
	internal bool bloqueadorClique;

	[Header("Nível")]
	internal ObscuredInt nivel = 1;
	public Text nivelText;
	public AudioClip nivelAudio;
	public Animator limpoAnimator;
	public Text nivelLimpoText;
	public AudioClip nivelLimpoAudioClip;

	[Header("Tiles")]
	public GameObject tileQuebrado;
	public GameObject tileQuebradoInvertido;
	private List<Tiles> tilesDisponiveis;
	private ObscuredInt quantidadeTiles;
	public ObscuredBool oneHitTiles;
	public bool exibirTileQuebrado;
	private Vector2 tileObrigatorioPosicao;

	[Header("Chances ao Destruir Tile (0 - 100)")]
	public ObscuredFloat chanceMoeda;

	[Header("Dinossauro")]
	public ObscuredFloat chanceBaseDinossauro;
	public ObscuredInt hpBaseDinossauro;
	public Range moedasDinossauro;

	[Header("Diamante")]
	public ObscuredFloat chanceBaseDiamante;
	public ObscuredInt hpBaseDiamante;
	public Range moedasDiamante;

	[Header("Moedas")]
	public Moedas moeda;
	public float delayEntreMoedas;
	public AudioClip moedasAudio;
	private ObscuredInt moedas;
	public Text moedasText;
	public Image moedasImage;

	[Header("Pá")]
	public ObscuredInt paInicialId;
	public float duracaoAnimacaoPas;
	private ObscuredInt paId;
	internal Pas paSelecionada;
	private Pas proximaPa;
	private List<Pas> pasDisponiveis = new List<Pas>();
	public Animator paDisponivelAnimator;
	public Animator novaPaAnimator;
	public Image novaPaImage;
	public Text novaPaText;
	public Animator novaPaAssistirAnimator;
	public Animator novaPaComprarAnimator;
	public Text novaPaComprarText;
	public Transform pasAdquiridas;
	public Image paAdquiridaBase;
	private bool cliqueBloqueadoPa;

	[Header("Pá - Modo Shovel Gun")]
	public GameObject modoShovelGunObject;
	public ObscuredFloat duracaoModoShovelGun;
	private ObscuredFloat tempoInicialShovelGun;
	internal ObscuredBool modoShovelGun;
	private IEnumerator coroutineShovelGun;

	[Header("Tesouros")]
	public float delayExibicaoTesouros;
	public float duracaoAnimacaoTesouros;
	public float duracaoAnimacaoTesouroBau;
	internal float tempoTesouroAberto;
	public Animator novoTesouroAnimator;
	public Image novoTesouroImage;
	public Text novoTesouroText;
	public Text tesouroText;
	private List<Tesouros> tesourosDisponiveis = new List<Tesouros>();
	private List<Tesouros> tesourosAdquiridos = new List<Tesouros>();
	private bool cliqueBloqueadoTesouro;
	public AudioClip tesouroColetadoAudioClip;

	[Header("Galeria de Tesouros")]
	public float duracaoAnimacaoGaleriaTesouros;
	public GaleriaTesourosBotao botaoGaleriaTesouros;
	public Transform galeriaTesourosContent;
	public Animator galeriaTesourosAnimator;
	public Text quantidadeTesourosText;
	private bool cliqueBloqueadoGaleria;

	[Header("Exibir Tesouros na Galeria")]
	public Animator tesouroDestaqueAnimator;
	public Image tesouroDestaqueImage;
	public Text tesouroDestaqueText;

	// Definições da Área de Jogo
	private int[,] jogo;
	private int larguraJogo = 5;
	private int alturaJogo = 5;

	[Header("Ads")]
	public int nivelExibicaoAd;
	public int nivelExibicaoAdObrigatoria;
	public float chanceExibicaoAd;
	private float chanceInicialExibicaoAd;
	public float chanceCorrecaoExibicaoAd;
	public List<string> recompensas;
	internal ObscuredString recompensa;
	public int quantidadeMoedasAd;
	public Ads ads;
	public Animator adAssistirAnimator;

	[Header("Áudios")]
	public AudioClip audioBotaoDesativado;
	public AudioClip audioClique;
	public AudioClip audioComemoracao;

	// Player Prefs
	private ObscuredInt nivelPref;
	private ObscuredInt moedasPref;
	private ObscuredInt paPref;

	[Header("John")]
	public float delayProximaPalavra;
	public float delayProximaFrase;
	public float duracaoMovimentoJohn;
	public bool separarFrasesJohn;
	public List<AudioClip> sonsJohn;
	internal bool bloqueadorCliqueJohn;
	private float timeUltimoCliqueJohn;
	public GameObject john;
	public Animator johnAnimator;
	public Transform johnBalao;
	public Text johnBalaoText;
	private Frases[] frases;
	private List<string> processarFrases;
	private int processarFrasesIndex = 0;
	private bool processandoFalasJohn;
	private bool processandoAdJohn;
	public float chanceEquilibrioFraseJohn;
	private bool forcarExibicaoTesouroJohn;
	private bool atualizacaoJohnLiberada;
	public Image paJohn;
	public GameObject shovelGunJohn;
	public GameObject moedasJohn;
	public GameObject tesouroJohn;
	public Transform tesourosJohn;
	public Image chapeuJohnImage;
	public List<Sprite> chapeuJohnSprites;
	public Animator olhosJohnAnimator;
	public Image olhosJohnImage;
	public List<Sprite> olhosJohnSprites;
	public Animator bocaJohnAnimator;
	public Image bocaJohnImage;
	public List<Sprite> bocaJohnSprites;
	public Image bracoJohnImage;
	public List<Sprite> bracoJohnSprites;
	private bool forcarFraseAdJohn;
	public GameObject botaoAdAssistir;
	public GameObject botaoAdDeclinar;
	private ObscuredBool checarJohnTesouro = false;

	[Header("Partículas")]
	public GameObject confete;
	public int quantidadeConfetes;
	public float intervaloConfetes;

	// Métodos Básicos

	private void Start()
	{
		// Definir Resolução Base da Tela
		DefinicoesScreen();

		// Definição de Variáveis Iniciais
		DefinirVariaveisIniciais();

		// Galeria de Tesouros
		CriarGaleriaTesouros();

		// Player Prefs
		PegarPlayerPrefs();

		// Pás
		AtualizarPas();

		// Checar Exibição inicial de AD
		ChecarExibicaoAd(true);

		// Mapa
		IniciarMapa();

		// Nível
		AtualizarNivel();

		// Moedas
		AtualizarMoedas();

		// Atualizar Quantidade de Tesouros da Galeria
		AtualizarQuantidadeGaleriaTesouros();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			FecharTelasCanvas();
		else if (Input.GetKeyDown(KeyCode.Z))
			EncerrarNivel();
		else if (Input.GetKeyDown(KeyCode.W))
			ResetarPlayerPrefs();
		else if (Input.GetKeyDown(KeyCode.E))
			AdicionarMoedasInstantaneo(1000);
		else if (Input.GetKeyDown(KeyCode.R))
			IniciarModoShovelGun();
		else if (Input.GetKeyDown(KeyCode.T))
			AdicionarTesouro();
		else if (Input.GetKeyDown(KeyCode.P))
			ReiniciarCena();

		PegarTilesHit();

		PegarCliqueJohn();
	}

	// Métodos de Inicialização de Variáveis

	private void Awake()
	{
		instancia = this;

		Application.targetFrameRate = 60;
	}

	private void DefinicoesScreen()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		Screen.SetResolution(
			(int)resolucaoTela.x,
			(int)resolucaoTela.y, true
		);
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

		// John
		frases = john.GetComponents<Frases>();

		chanceInicialExibicaoAd = chanceExibicaoAd;
	}

	// Mapa/Tela

	private void IniciarMapa()
	{
		// Definimos a quantidade de tiles inicial
		quantidadeTiles = 0;

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

	private bool FecharTelasCanvas()
	{
		if (ChecarNovaPaAnimator())
			PaVoltar();
		else if (ChecarNovoTesouroAnimator())
			OcultarTesouro();
		else if (ChecarGaleriaTesourosAnimator())
			GaleriaTesourosVoltar();
		else if (ChecarJohnAnimator())
			OcultarJohn();
		else
			return false;

		return true;
	}

	// Nível

	private void AvancarNivel()
	{
		nivel++;

		DefinirPlayerPrefs();

		Invoke("AtualizarNivel", duracaoMovimentoMapa);

		EquilibrarChances();
	}

	private void AtualizarNivel()
	{
		nivelText.text = string.Format("Level {0}", nivel);

		Invoke("ChecarExibirPaDisponivel", duracaoMovimentoMapa);

		Invoke("VibrarNivel", (nivel == 1 || nivel == nivelPref ? duracaoMovimentoMapa : 0) + 0.2f);
	}

	private void VibrarNivel()
	{
		Handheld.Vibrate();
	}

	private void EncerrarNivel()
	{
		ExibirTextoNivelLimpo();

		ReproduzirAudio(nivelLimpoAudioClip);

		Invoke("AvancarNivel", delayEncerrarNivel);

		Invoke("IniciarMapa", delayEncerrarNivel);

		ChecarExibicaoAd();
	}

	private void ExibirTextoNivelLimpo()
	{
		nivelLimpoText.text = nivelText.text;

		limpoAnimator.SetTrigger("Animar");
	}

	private void TocarAudioNivel()
	{
		ReproduzirAudio(nivelAudio);
	}

	// Tiles

	private void InstanciarTile(int x, int y, Transform mapaDestino, ObscuredInt nivelMapa, bool incluirTesouro = true)
	{
		nivelMapa =
			nivelMapa == 0
				? nivel
				: nivelMapa;

		/*
		 * Pegar o Tile que será instanciado
		 * Se estivermos no nível 1 e o y for 0, instanciamos a Grama
		 * Caso contrário, pegamos um tile aleatório.
		 */
		Tiles tile =
			nivelMapa == 1 && y == 0
				? tilesDisponiveis[0]
				: PegarTileAleatorio(x, y, nivelMapa, incluirTesouro);

		// Instanciamos o tile na cena baseado na posição calculada e na rotação base do prefab
		Tiles tileInstanciado = Instantiate(tile);

		// Definimos um nome para o tile "x,y" para que fique mais organizado
		tileInstanciado.name = string.Format("{0},{1}", x, y);

		// Colocamos o tile instanciado dentro do GameObject destinado ao mapa, para melhor organização
		tileInstanciado.transform.SetParent(mapaDestino ? mapaDestino : mapa);

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

		if (tile.bauTesouro &&
			nivelMapa > tile.aparecerObrigatoriamenteNivel[0])
		{
			InstanciarTile(x, y, mapaDestino, nivelMapa, false);
		}

		quantidadeTiles++;
	}

	private void PegarTilesDisponiveis()
	{
		tilesDisponiveis = new List<Tiles>();

		Transform tiles = GameObject.Find("TilesDisponíveis").transform;

		foreach (Transform tile in tiles)
			tilesDisponiveis.Add(tile.GetComponent<Tiles>());
	}

	private Tiles PegarTileAleatorio(int x, int y, int nivelMapa, bool incluirTesouro)
	{
		List<Tiles> tilesChances = new List<Tiles>();

		int chanceIndex = 0;

		Tiles tileSelecionado = null;

		foreach (Tiles tile in tilesDisponiveis)
		{
			if (tile.aparecerObrigatoriamenteNivel.Contains(nivelMapa) &&
				tileObrigatorioPosicao.x == x &&
				tileObrigatorioPosicao.y == y)
			{
				tileSelecionado = tile;

				tilesDisponiveis.Remove(tile);

				break;
			}

			int chance = tile.PegarChance(nivelMapa);

			if ((tile.bauTesouro && (!incluirTesouro || !ChecarTesouroDisponivel())) ||
				(tile.ads && !ads.checarAd)
			)
			{
				chance = 0;
			}

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

	public void ProcessarTileDestruido(Transform tileDestruido, ObscuredBool fornecerMoedas, ObscuredInt moedas, bool tileEspecial)
	{
		if (!fornecerMoedas)
			moedas = 0;

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
			if (modoShovelGun || touch.phase == TouchPhase.Began)
			{
				Vector3 posicao = Camera.main.ScreenToWorldPoint(touch.position);
				RaycastHit2D hit = Physics2D.Raycast(posicao, Vector2.zero);
				if (hit && hit.collider != null)
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

	public void AdicionarMoedas(Transform origem, int quantidade = 0)
	{
		StartCoroutine(AdicionarMoedas(origem, quantidade, true));
	}

	private IEnumerator AdicionarMoedas(Transform origem, int quantidade, bool coroutine)
	{
		Vector2 posicao =
			RectTransformUtility.WorldToScreenPoint(
				Camera.main,
				origem.position
			);

		// Vector2 posicao = origem.position;
		Quaternion rotacao = origem.rotation;

		for (int i = 0; i < quantidade; i++)
		{
			ReproduzirAudio(moedasAudio);

			StartCoroutine(AtualizarMoedas(1));

			Moedas moedaInstanciada = Instantiate(moeda, posicao, rotacao);
			moedaInstanciada.transform.SetParent(canvas);
			moedaInstanciada.transform.SetAsFirstSibling();
			moedaInstanciada.transform.localScale = Vector3.one;

			yield return new WaitForSeconds(delayEntreMoedas);
		}
	}

	public void AdicionarMoedasInstantaneo(int quantidade, int sons = 5)
	{
		for (int i = 0; i < sons; i++)
			ReproduzirAudio(moedasAudio);

		moedas += quantidade;

		AtualizarMoedas();
	}

	private void RemoverMoedas(int quantidade)
	{
		moedas -= quantidade;

		AtualizarMoedas();

		ReproduzirAudio(moedasAudio);
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

		AlterarPaDisponivelAnimator(false);

		AtualizarPaAdquirida();

		if (!forcarExibicaoTesouroJohn)
			AtualizarPaJohn();
	}

	public void EvoluirPa(bool gratuito = false, bool fechar = true)
	{
		ObscuredInt custo = proximaPa.moedas;

		if (gratuito)
			custo = 0;

		if (paId >= pasDisponiveis.Count || nivel < proximaPa.nivel || moedas < custo)
		{
			if (!gratuito)
				ReproduzirAudioAcaoProibida();

			return;
		}

		if (custo > 0)
		{
			ReproduzirAudioClique();

			RemoverMoedas(custo);

			AlterarNovaPaAdquiridaAnimator(true);

			ReproduzirAudioComemoracao();
		}

		if (fechar)
			PaVoltar();

		paId++;

		SelecionarPa();

		ForcarExibicaoJohn();
	}

	private void ExibirPaDisponivel()
	{
		if (paId >= pasDisponiveis.Count)
			return;

		if (nivel >= proximaPa.nivel)
			AlterarPaDisponivelAnimator(true);
	}

	private void AlterarPaDisponivelAnimator(bool estado)
	{
		paDisponivelAnimator.SetBool("Exibir", estado);
	}

	public void ExibirNovaPaDisponivel()
	{
		novaPaImage.sprite = proximaPa.sprite;
		novaPaText.text = proximaPa.nome;
		novaPaComprarText.text = proximaPa.moedas.ToString();

		AlterarNovaPaAnimator(true);

		CriarConfetes(quantidadeConfetes);

		cliqueBloqueadoPa = true;

		StartCoroutine(ManterCliqueBloqueadoPa());
	}

	private IEnumerator ManterCliqueBloqueadoPa()
	{
		while (cliqueBloqueadoPa)
		{
			BloquearClique();

			yield return null;
		}
	}

	private void AlterarNovaPaAnimator(bool estado)
	{
		novaPaAnimator.SetBool("Exibir", estado);

		Invoke("OcultarNovaPaAdquiridaAnimator", duracaoAnimacaoPas);
	}

	private void OcultarNovaPaAdquiridaAnimator()
	{
		AlterarNovaPaAdquiridaAnimator(false);
	}

	private void AlterarNovaPaAdquiridaAnimator(bool estado)
	{
		novaPaAnimator.SetBool("PáAdquirida", estado);
	}

	public bool ChecarNovaPaAnimator()
	{
		return novaPaAnimator.GetBool("Exibir");
	}

	public void PaAssistir()
	{
		if (ads.checarAd)
		{
			ReproduzirAudioClique();

			AlterarNovaPaAdquiridaAnimator(true);

			recompensa = "pa";

			ads.ExibirAd();

			ReproduzirAudioComemoracao();
		}
		else
			ReproduzirAudioAcaoProibida();
	}

	public void PaComprar()
	{
		EvoluirPa();
	}

	public void PaVoltar()
	{
		cliqueBloqueadoPa = false;

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

	public void IniciarModoShovelGun()
	{
		modoShovelGun = true;

		tempoInicialShovelGun = Time.time;

		AtualizarModoShovelGun();

		if (coroutineShovelGun == null)
		{
			coroutineShovelGun = EncerrarModoShovelGun();
			StartCoroutine(coroutineShovelGun);
		}
	}

	private IEnumerator EncerrarModoShovelGun()
	{
		while (modoShovelGun)
		{
			if (tempoInicialShovelGun + duracaoModoShovelGun <= Time.time)
			{
				modoShovelGun = false;

				AtualizarModoShovelGun();

				coroutineShovelGun = null;
			}

			yield return new WaitForSeconds(1f);
		}
	}

	private void AtualizarModoShovelGun()
	{
		modoShovelGunObject.SetActive(modoShovelGun);
	}

	public void ChecarExibirPaDisponivel()
	{
		if (!ChecarJohnAnimator())
			ExibirPaDisponivel();
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
		if (tesourosDisponiveis.Count == tesourosAdquiridos.Count)
			return null;

		Tesouros tesouroSelecionado = null;

		List<Tesouros> tesourosDisponiveisReal = new List<Tesouros>(tesourosDisponiveis);

		foreach (Tesouros tesouro in tesourosAdquiridos)
		{
			tesourosDisponiveisReal.Remove(tesouro);
		}

		while (tesouroSelecionado == null)
		{
			int chaveAleatoria = Random.Range(0, tesourosDisponiveisReal.Count);

			tesouroSelecionado = tesourosDisponiveisReal[chaveAleatoria];
		}

		return tesouroSelecionado;
	}

	public bool AdicionarTesouro(Tesouros tesouro = null, bool gratuito = true)
	{
		bool exibirAnimator = true;

		if (tesouro != null)
		{
			if (!gratuito)
			{
				if (moedas < tesouro.preco)
				{
					ReproduzirAudioAcaoProibida();

					return false;
				}
				else
				{
					ReproduzirAudioClique();

					RemoverMoedas(tesouro.preco);
				}
			}

			exibirAnimator = false;
		}
		else
			tesouro = PegarTesouro();

		if (tesouro != null)
		{
			tempoTesouroAberto = Time.time + delayExibicaoTesouros + duracaoAnimacaoTesouros;

			tesourosAdquiridos.Add(tesouro);

			novoTesouroImage.sprite = tesouro.sprite;
			novoTesouroText.text = tesouro.nome;

			AtualizarBotaoTesouro(tesouro);

			AtualizarQuantidadeGaleriaTesouros();

			if (exibirAnimator)
			{
				AlterarNovoTesouroAnimator(true);

				if (atualizacaoJohnLiberada)
					ExibirTesouroJohn(tesouro);
				else
					StartCoroutine(ProgramarExibicaoTesourosJohn(tesouro, duracaoMovimentoJohn));

				CriarConfetes(quantidadeConfetes);

				cliqueBloqueadoTesouro = true;

				StartCoroutine(ManterCliqueBloqueadoTesouro());
			}
			else
				AtualizarPorcentagemTesouros();
		}
		else if (quantidadeTiles == 0)
			EncerrarNivel();
		else
			DesbloquearClique();

		return true;
	}

	public void OcultarTesouro()
	{
		cliqueBloqueadoTesouro = false;

		Invoke("DesbloquearClique", duracaoAnimacaoTesouros);

		AlterarNovoTesouroAnimator(false);

		Invoke("AtualizarPorcentagemTesouros", duracaoAnimacaoTesouros + duracaoAnimacaoTesouroBau);

		if (!ChecarTesouroDisponivel())
		{
			ChecarJohn(true);
		}
		else if (quantidadeTiles == 0)
			Invoke("EncerrarNivel", duracaoAnimacaoTesouros + duracaoAnimacaoTesouroBau);
	}

	private IEnumerator ManterCliqueBloqueadoTesouro()
	{
		while (cliqueBloqueadoTesouro)
		{
			BloquearClique();

			yield return null;
		}
	}

	public bool ChecarNovoTesouroAnimator()
	{
		return novoTesouroAnimator.GetBool("Exibir");
	}

	private void AlterarNovoTesouroAnimator(bool estado)
	{
		novoTesouroAnimator.SetBool("Exibir", estado);

		if (estado)
			novoTesouroAnimator.SetBool("Exibe", estado);

		Audios.instancia.exibindoTesouro = estado;
	}

	private void AtualizarPorcentagemTesouros()
	{
		float porcentagem = tesourosAdquiridos.Count * 100 / tesourosDisponiveis.Count;

		tesouroText.text = string.Format("{0}%", porcentagem);
	}

	private bool ChecarTesouroDisponivel()
	{
		return tesourosAdquiridos.Count < tesourosDisponiveis.Count;
	}

	public void ReproduzirAudioTesouroColetado()
	{
		ReproduzirAudio(tesouroColetadoAudioClip);
	}

	// Galeria de Tesouros

	private void CriarGaleriaTesouros()
	{
		foreach (Tesouros tesouro in tesourosDisponiveis)
		{
			GaleriaTesourosBotao botaoInstanciado = Instantiate(botaoGaleriaTesouros);

			botaoInstanciado.transform.SetParent(galeriaTesourosContent);

			botaoInstanciado.transform.localPosition = new Vector3(
				botaoInstanciado.transform.localPosition.x,
				botaoInstanciado.transform.localPosition.y,
				0
			);

			botaoInstanciado.transform.localScale = Vector3.one;

			tesouro.botaoGaleria = botaoInstanciado;

			tesouro.botaoGaleria.Inicializar(tesouro);

			AtualizarBotaoTesouro(tesouro);
		}
	}

	private void AtualizarBotaoTesouro(Tesouros tesouro)
	{
		bool desbloqueado = tesourosAdquiridos.Contains(tesouro);
		tesouro.botaoGaleria.Atualizar(desbloqueado);
	}

	public void ExibirGaleriaTesouros()
	{
		AlterarExibicaoGaleriaTesouros(true);

		cliqueBloqueadoGaleria = true;

		StartCoroutine(ManterCliqueBloqueadoGaleria());
	}

	private void OcultarGaleriaTesouros()
	{
		AlterarExibicaoGaleriaTesouros(false);

		cliqueBloqueadoGaleria = false;

		Invoke("DesbloquearClique", duracaoAnimacaoGaleriaTesouros);
	}

	private IEnumerator ManterCliqueBloqueadoGaleria()
	{
		while (cliqueBloqueadoGaleria)
		{
			BloquearClique();

			yield return null;
		}
	}

	private void AlterarExibicaoGaleriaTesouros(bool estado)
	{
		AtualizarGaleriaTesourosAnimator(estado);

		foreach (Tesouros tesouro in tesourosDisponiveis)
		{
			tesouro.botaoGaleria.AlterarExibicao(estado);
		}
	}

	public void GaleriaTesourosVoltar()
	{
		OcultarGaleriaTesouros();
	}

	public bool ChecarGaleriaTesourosAnimator()
	{
		return galeriaTesourosAnimator.GetBool("Exibir");
	}

	private void AtualizarGaleriaTesourosAnimator(bool estado)
	{
		galeriaTesourosAnimator.SetBool("Exibir", estado);
	}

	private void AtualizarQuantidadeGaleriaTesouros()
	{
		quantidadeTesourosText.text = string.Format("{0}/{1}", tesourosAdquiridos.Count, tesourosDisponiveis.Count);
	}

	// Exibição de Tesouros na Galeria de Tesouros

	public void ComprarTesouroGaleria(Tesouros tesouro)
	{
		bool adicionado = AdicionarTesouro(tesouro, false);

		if (adicionado)
		{
			ReproduzirAudioComemoracao();

			CriarConfetes(quantidadeConfetes);

			ExibirTesouroDestaque(tesouro);

			ExibirTesouroJohn(tesouro);
		}
	}

	public void ExibirTesouroDestaque(Tesouros tesouro)
	{
		ConstruirTesouroDestaque(tesouro);

		AlterarExibirTesouroAnimator(true);
	}

	private void ConstruirTesouroDestaque(Tesouros tesouro)
	{
		tesouroDestaqueImage.sprite = tesouro.sprite;
		tesouroDestaqueText.text = tesouro.nome;
	}

	public void OcultarTesouroDestaque()
	{
		AlterarExibirTesouroAnimator(false);

		OcultarTesouro();
	}

	private void AlterarExibirTesouroAnimator(bool estado)
	{
		tesouroDestaqueAnimator.SetBool("Exibir", estado);

		Audios.instancia.exibindoTesouro = estado;
	}

	public bool ChecarTesouroDestaqueAnimator()
	{
		return tesouroDestaqueAnimator.GetBool("Exibir");
	}

	// Ads

	private void ChecarExibicaoAd(bool inicio = false)
	{
		chanceExibicaoAd += chanceCorrecaoExibicaoAd;

		if ((!inicio && nivel + 1 != nivelExibicaoAdObrigatoria ||
			inicio && nivel != nivelExibicaoAdObrigatoria) &&
			(!ads.checarAd || nivel < nivelExibicaoAd || forcarExibicaoTesouroJohn))
			return;

		float chance = Random.Range(0, 100);

		string _recompensa = "";

		if ((!inicio && nivel + 1 == nivelExibicaoAdObrigatoria) ||
			(inicio && nivel == nivelExibicaoAdObrigatoria))
		{
			_recompensa = "shovel_gun";
			chance = 0;
		}

		if (chance < chanceExibicaoAd)
		{
			ExibirAd(_recompensa);

			chanceExibicaoAd = chanceInicialExibicaoAd;
		}
	}

	private string GerarRecompensaAd()
	{
		string _recompensa = "";

		/*
		// As linhas abaixo permitem gerar uma recompensa aleatória novamente caso
		// o jogador já tenha todos os tesouros, como atualmente temos apenas 3 possibilidades,
		// tesouro, moeda ou shovelgun, e não dá pra fazer nada com moeda a não ser comprar
		// tesouros, então desativei esse trecho.
		// Esse trecho pode ser reabilitado caso haja novas possibilidades de compras com
		// moedas.

		bool gerarNovaRecompensa = true;

		while (gerarNovaRecompensa)
		{
			_recompensa = recompensas[Random.Range(0, recompensas.Count)];

			if (!(_recompensa == "tesouro" &&
				tesourosDisponiveis.Count == tesourosAdquiridos.Count))
				gerarNovaRecompensa = false;
		}
		*/

		_recompensa = recompensas[Random.Range(0, recompensas.Count)];

		// Caso o Jogador tenha todos os tesouros, o AD sempre será do ShovelGunMode
		if (tesourosDisponiveis.Count == tesourosAdquiridos.Count)
			_recompensa = "shovel_gun";

		return _recompensa;
	}

	public void ExibirAd(string novaRecompensa = "")
	{
		if (novaRecompensa == "")
			novaRecompensa = GerarRecompensaAd();

		recompensa = novaRecompensa;

		ForcarExibicaoJohn();

		forcarFraseAdJohn = true;

		ChecarExibicaoPersonalizadaAdJohn();

		ExibirBotoesAdJohn();

		AlterarAdAssistirAnimator(!ads.checarAd);
	}

	public void AssistirAd()
	{
		if (!processandoAdJohn)
			return;

		ReproduzirAudioClique();

		ads.ExibirAd();

		OcultarJohn();
	}

	public void DeclinarAd()
	{
		if (!processandoAdJohn)
			return;

		ReproduzirAudioClique();

		OcultarJohn();
	}

	private void AlterarAdAssistirAnimator(bool estado)
	{
		adAssistirAnimator.SetBool("Inativo", estado);
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
		if (modoShovelGun)
		{
			if (tesouro)
				checarJohnTesouro = true;

			return;
		}

		if (checarJohnTesouro)
		{
			checarJohnTesouro = false;

			tesouro = true;
		}

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

					frase.chance = frase.chanceInicial;
				}

				if (forcarFraseAdJohn)
				{
					processarFrases = new List<string>(
						new string[] {
							MontarFraseAdJohn()
						}
					);

					forcarFraseAdJohn = false;

					Invoke("ProcessarAdJohn", 2.5f);
				}
				else if (frase.aleatoria)
				{
					string fraseSelecionada = frase.frasesReal[Random.Range(0, frase.frasesReal.Count)];

					processarFrases = new List<string>(
						new string[] {
							fraseSelecionada
						}
					);

					frase.frasesReal.Remove(fraseSelecionada);

					if (frase.frasesReal.Count == 0)
						frase.AtualizarListaFrases();
				}
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

	private void ProcessarAdJohn()
	{
		processandoAdJohn = true;
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
			OcultarJohn();
		}
		else
		{
			ReproduzirAudioJohn();

			AtualizarBocaJohnAnimator();

			LimparFalaJohn();

			StartCoroutine("ConstruirFrase");

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
		atualizacaoJohnLiberada = false;

		FecharTelasCanvas();

		AlterarJohnAnimator(true);

		AlterarPaDisponivelAnimator(false);
	}

	private void ForcarExibicaoJohn()
	{
		foreach (Frases frase in frases)
			if (frase.aleatoria)
				frase.chance = 100;
	}

	private void OcultarJohn()
	{
		processandoAdJohn = false;

		processandoFalasJohn = false;

		AlterarJohnAnimator(false);

		Invoke("ExibirPaDisponivel", duracaoMovimentoJohn);

		Invoke("DesbloquearCliqueJohn", duracaoMovimentoJohn);

		if (quantidadeTiles == 0 &&
			!ChecarTesouroDisponivel())
		{
			Invoke("EncerrarNivel", duracaoMovimentoJohn);
		}

		Invoke("AtualizarPaJohn", duracaoMovimentoJohn);

		Invoke("ReiniciarComportamentoJohn", duracaoMovimentoJohn);
	}

	private bool ChecarJohnAnimator()
	{
		return johnAnimator.GetBool("Exibir");
	}

	private void AlterarJohnAnimator(bool estado)
	{
		johnAnimator.SetBool("Exibir", estado);

		Audios.instancia.exibindoJohn = estado;
	}

	private void PegarCliqueJohn()
	{
		if (!processandoFalasJohn || processandoAdJohn)
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

	private void AtualizarBocaJohnAnimator()
	{
		bocaJohnAnimator.SetTrigger("Falar");
	}

	private void AtualizarPaJohn()
	{
		paJohn.sprite = paSelecionada.sprite;

		ExibirPaJohn();

		AlterarBracoJohn("fechado");
	}

	private void ExibirPaJohn()
	{
		OcultarObjetosJohn();

		paJohn.gameObject.SetActive(true);
	}

	private void ExibirShovelGunBracoJohn()
	{
		OcultarObjetosJohn();

		shovelGunJohn.SetActive(true);
	}

	private void ExibirMoedasBracoJohn()
	{
		OcultarObjetosJohn();

		moedasJohn.SetActive(true);
	}

	private void ExibirTesouroBracoJohn()
	{
		OcultarObjetosJohn();

		tesouroJohn.SetActive(true);
	}

	private IEnumerator ProgramarExibicaoTesourosJohn(Tesouros tesouro, float delay)
	{
		yield return new WaitForSeconds(delay);

		ExibirTesouroJohn(tesouro);
	}

	private void ExibirTesouroJohn(Tesouros tesouro)
	{
		OcultarObjetosJohn();

		foreach (Transform child in tesourosJohn)
		{
			if (tesouro.name == child.name)
			{
				child.gameObject.SetActive(true);

				ForcarExibicaoJohn();

				forcarExibicaoTesouroJohn = true;

				ChecarExibicaoPersonalizadaTesouroJohn(tesouro);

				break;
			}
		}
	}

	private void OcultarObjetosJohn()
	{
		paJohn.gameObject.SetActive(false);

		shovelGunJohn.SetActive(false);

		moedasJohn.SetActive(false);

		tesouroJohn.SetActive(false);

		foreach (Transform child in tesourosJohn)
			child.gameObject.SetActive(false);
	}

	private void ChecarExibicaoPersonalizadaTesouroJohn(Tesouros tesouro)
	{
		int tesouroId = int.Parse(Regex.Replace(tesouro.name, "[^0-9]", ""));

		switch (tesouroId)
		{
			case 1:
				AlterarChapeuJohn(2);

				AlterarOlhosJohn("fechados");

				AlterarBocaJohn("bico");

				break;

			default:
				break;
		}
	}

	private void ChecarExibicaoPersonalizadaAdJohn()
	{
		switch (recompensa)
		{
			case "tesouro":
				AlterarBracoJohn("aberto");

				ExibirTesouroBracoJohn();

				break;

			case "shovel_gun":
				ExibirShovelGunBracoJohn();

				break;

			case "moedas":
				ExibirMoedasBracoJohn();

				break;

			default:
				break;
		}
	}

	private void ExibirBotoesAdJohn()
	{
		botaoAdAssistir.SetActive(true);
		botaoAdDeclinar.SetActive(true);
	}

	private void OcultarBotoesAdJohn()
	{
		botaoAdAssistir.SetActive(false);
		botaoAdDeclinar.SetActive(false);
	}

	// Modificações de Comportamento do John (Alteração da Face, entre outros)

	private void AlterarChapeuJohn(int chapeuId)
	{
		chapeuJohnImage.sprite = chapeuJohnSprites[chapeuId - 1];
	}

	private void ExibirChapeuJohn()
	{
		AlterarExibicaoChapeuJohn(true);
	}

	private void OcultarChapeuJohn()
	{
		AlterarExibicaoChapeuJohn(false);
	}

	private void AlterarExibicaoChapeuJohn(bool estado)
	{
		chapeuJohnImage.gameObject.SetActive(estado);
	}

	private void AlterarOlhosJohn(string estado = "")
	{
		AnimarOlhosJohn(false);

		int olhosId;

		switch (estado)
		{
			case "abertos":
				olhosId = 0;
				break;

			case "fechados":
				olhosId = 1;
				break;

			default:
				olhosId = 0;
				break;
		}

		olhosJohnImage.sprite = olhosJohnSprites[olhosId];
	}

	private void AnimarOlhosJohn(bool estado)
	{
		olhosJohnAnimator.enabled = estado;
	}

	private void AnimarBocaJohn(bool estado)
	{
		bocaJohnAnimator.enabled = estado;
	}

	private void AlterarBocaJohn(string estado = "")
	{
		AnimarBocaJohn(false);

		int bocaId;

		switch (estado)
		{
			case "aberta":
				bocaId = 1;
				break;

			case "fechada":
				bocaId = 2;
				break;

			case "bico":
				bocaId = 3;
				break;

			default:
				bocaId = 0;
				break;
		}

		bocaJohnImage.sprite = bocaJohnSprites[bocaId];
	}

	private void AlterarBracoJohn(string estado = "")
	{
		int bracoId;

		switch (estado)
		{
			case "fechado":
				bracoId = 0;
				break;

			case "aberto":
				bracoId = 1;
				break;

			default:
				bracoId = 0;
				break;
		}

		bracoJohnImage.sprite = bracoJohnSprites[bracoId];
	}

	private void ReiniciarComportamentoJohn()
	{
		OcultarBotoesAdJohn();

		forcarExibicaoTesouroJohn = false;

		atualizacaoJohnLiberada = true;

		AnimarOlhosJohn(true);

		AnimarBocaJohn(true);

		AlterarExibicaoChapeuJohn(true);

		AlterarChapeuJohn(1);
	}

	private string MontarFraseAdJohn()
	{
		string frase = "";

		switch (recompensa)
		{
			case "tesouro":
				frase = "Do you wanna a trash treasure NOW?";
				break;

			case "shovel_gun":
				frase = "Do you wanna start SHOVEL GUN mode NOW?";
				break;

			case "moedas":
				frase = string.Format("Do you wanna win {0} coins NOW?", quantidadeMoedasAd);
				break;

			default:
				break;
		}

		return frase;
	}

	// Confetes

	private void CriarConfetes(int quantidade)
	{
		for (int i = 0; i < quantidade; i++)
		{
			Invoke("CriarConfete", i * intervaloConfetes);
		}
	}

	private void CriarConfete()
	{
		Instantiate(confete);
	}

	// Equilibrar Chances

	private void EquilibrarChances()
	{
		EquilibrarChancesJohn();
	}

	private void EquilibrarChancesJohn()
	{
		foreach (Frases frase in frases)
		{
			if (frase.aleatoria)
			{
				frase.chance += chanceEquilibrioFraseJohn;
			}
		}
	}

	// Áudios

	public void ReproduzirAudioClique()
	{
		ReproduzirAudio(audioClique);
	}

	public void ReproduzirAudioAcaoProibida()
	{
		ReproduzirAudio(audioBotaoDesativado);
	}

	public void ReproduzirAudioComemoracao()
	{
		ReproduzirAudio(audioComemoracao);
	}

	// Métodos Estáticos

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
