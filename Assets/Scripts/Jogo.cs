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
	public int nivelMaximo;
	internal int nivel = 1;
	public Text nivelText;
	public AudioClip nivelAudio;
	public Animator limpoAnimator;
	public Text nivelLimpoText;

	[Header("Tiles")]
	public GameObject tileQuebrado;
	private List<Tiles> tilesDisponiveis;
	private int quantidadeTiles;
	public bool oneHitTiles;
	public bool exibirTileQuebrado;
	private Vector2 tileObrigatorioPosicao;
	// public List<TilesInfo> tilesDisponiveis;

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
	public Text moedasText;
	public Image moedasImage;

	[Header("Pá")]
	public int paInicialId;
	public float duracaoAnimacaoPas;
	private int paId;
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

	[Header("Pá - Modo Shovel Gun")]
	public GameObject modoShovelGunObject;
	public float duracaoModoShovelGun;
	private float tempoInicialShovelGun;
	internal bool modoShovelGun;
	private IEnumerator coroutineShovelGun;

	[Header("Tesouros")]
	public float delayExibicaoTesouros;
	public float duracaoAnimacaoTesouros;
	public float duracaoAnimacaoTesouroBau;
	internal float tempoTesouroAberto;
	public Animator novoTesouroAnimator;
	public Image novoTesouroFundo;
	public Image novoTesouroImage;
	public Text novoTesouroText;
	public Text tesouroText;
	private List<Tesouros> tesourosDisponiveis = new List<Tesouros>();
	private List<Tesouros> tesourosAdquiridos = new List<Tesouros>();

	[Header("Galeria de Tesouros")]
	public float duracaoAnimacaoGaleriaTesouros;
	public GaleriaTesourosBotao botaoGaleriaTesouros;
	public Transform galeriaTesourosContent;
	public Animator galeriaTesourosAnimator;

	// Definições da Área de Jogo
	private int[,] jogo;
	private int larguraJogo = 5;
	private int alturaJogo = 5;

	[Header("Ads")]
	public float duracaoAnimacaoTileAd;
	internal string recompensa;
	public Ads ads;
	public GameObject tileAd;
	public Text tileAdTexto;
	public Animator tileAdAnimator;
	public Animator tileAdAssistirAnimator;

	[Header("Áudios")]
	public AudioClip audioBotaoDesativado;
	public AudioClip audioClique;

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
	public GameObject john;
	public Animator johnAnimator;
	public Transform johnBalao;
	public Text johnBalaoText;
	private Frases[] frases;
	private List<string> processarFrases;
	private int processarFrasesIndex = 0;
	private bool processandoFalasJohn;

	[Header("Resetar Player Prefs")]
	public float tempoEscNecessario;
	private float tempoEscPressionado;

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

		// Mapa
		IniciarMapa();

		// Pás
		AtualizarPas();

		// Nível
		AtualizarNivel();

		// Moedas
		AtualizarMoedas();

		// Tile Ad
		AtualizarMensagemTileAd();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Z))
			EncerrarNivel();
		else if (Input.GetKeyDown(KeyCode.W))
			ResetarPlayerPrefs();
		else if (Input.GetKeyDown(KeyCode.Escape))
		{
			tempoEscPressionado = Time.time;

			FecharTelasCanvas();
		}
		else if (Input.GetKeyDown(KeyCode.E))
			AdicionarMoedasInstantaneo(1000);
		else if (Input.GetKeyDown(KeyCode.R))
			IniciarModoShovelGun();
		else if (Input.GetKeyDown(KeyCode.P))
			ReiniciarCena();

		PegarTilesHit();

		PegarCliqueJohn();

		if (tempoEscPressionado > 0)
		{
			if (tempoEscPressionado + tempoEscNecessario <= Time.time)
			{
				ResetarPlayerPrefs();

				tempoEscPressionado = 0;
			}
			else if (!Input.GetKey(KeyCode.Escape))
				tempoEscPressionado = 0;
		}
	}

	// Métodos de Inicialização de Variáveis

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

	private void FecharTelasCanvas()
	{
		if (ChecarNovaPaAnimator())
			PaVoltar();
		else if (ChecarNovoTesouroAnimator())
			OcultarTesouro();
		else if (ChecarTileAdAnimator())
			TileAdVoltar();
		else if (ChecarGaleriaTesourosAnimator())
			GaleriaTesourosVoltar();
		else if (ChecarJohnAnimator())
			OcultarJohn();
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
		ReproduzirAudio(nivelAudio);
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

			if ((tile.bauTesouro && !ChecarTesouroDisponivel()) ||
				(tile.ads && !ads.checarAd)
			)
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

		Quaternion rotacao = origem.rotation;

		for (int i = 0; i < quantidade; i++)
		{
			Jogo.ReproduzirAudio(moedasAudio);

			StartCoroutine(AtualizarMoedas(1));

			Moedas moedaInstanciada = Instantiate(moeda, posicao, rotacao);
			moedaInstanciada.transform.SetParent(canvas);
			moedaInstanciada.transform.SetAsFirstSibling();

			yield return new WaitForSeconds(delayEntreMoedas);
		}
	}

	private void AdicionarMoedasInstantaneo(int quantidade)
	{
		moedas += quantidade;

		AtualizarMoedas();
	}

	private void RemoverMoedas(int quantidade)
	{
		moedas -= quantidade;

		AtualizarMoedas();
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
			if (!gratuito)
				ReproduzirAudioAcaoProibida();

			return;
		}

		if (custo > 0)
		{
			ReproduzirAudioClique();

			RemoverMoedas(custo);
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

		AlterarNovaPaAssistirAnimator(!ads.checarAd);

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
		if (ads.checarAd)
			ReproduzirAudioClique();
		else
			ReproduzirAudioAcaoProibida();

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

	public void AdicionarTesouro(Tesouros tesouro = null, bool gratuito = true)
	{
		bool exibirAnimator = true;

		if (tesouro != null)
		{
			if (!gratuito)
			{
				if (moedas < tesouro.preco)
				{
					ReproduzirAudioAcaoProibida();

					return;
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

			if (exibirAnimator)
			{
				AlterarCorFundoNovoTesouro();

				AlterarNovoTesouroAnimator(true);

				BloquearClique();
			}
			else
				AtualizarPorcentagemTesouros();
		}
		else if (quantidadeTiles == 0)
			EncerrarNivel();
		else
			DesbloquearClique();
	}

	public void OcultarTesouro()
	{
		Invoke("DesbloquearClique", duracaoAnimacaoTesouros);

		AlterarNovoTesouroAnimator(false);

		Invoke("AtualizarPorcentagemTesouros", duracaoAnimacaoTesouros + duracaoAnimacaoTesouroBau);

		if (!ChecarTesouroDisponivel())
		{
			ChecarJohn(true);

			AtualizarMensagemTileAd();
		}
		else if (quantidadeTiles == 0)
			Invoke("EncerrarNivel", duracaoAnimacaoTesouros + duracaoAnimacaoTesouroBau);
	}

	private bool ChecarNovoTesouroAnimator()
	{
		return novoTesouroAnimator.GetBool("Exibir");
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

	private bool ChecarTesouroDisponivel()
	{
		return tesourosAdquiridos.Count < tesourosDisponiveis.Count;
	}

	// Galeria de Tesouros

	private void CriarGaleriaTesouros()
	{
		foreach (Tesouros tesouro in tesourosDisponiveis)
		{
			GaleriaTesourosBotao botaoInstanciado = Instantiate(botaoGaleriaTesouros);

			botaoInstanciado.transform.SetParent(galeriaTesourosContent);

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

		BloquearClique();
	}

	private void OcultarGaleriaTesouros()
	{
		AlterarExibicaoGaleriaTesouros(false);

		Invoke("DesbloquearClique", duracaoAnimacaoGaleriaTesouros);
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

	private bool ChecarGaleriaTesourosAnimator()
	{
		return galeriaTesourosAnimator.GetBool("Exibir");
	}

	private void AtualizarGaleriaTesourosAnimator(bool estado)
	{
		galeriaTesourosAnimator.SetBool("Exibir", estado);
	}

	// Ads

	public void ExibirAd(string novaRecompensa = "")
	{
		recompensa = novaRecompensa;

		ads.ExibirAd();
	}

	public void AssistirTileAd()
	{
		if (ads.checarAd)
			ReproduzirAudioClique();
		else
			ReproduzirAudioAcaoProibida();

		if (ChecarTesouroDisponivel())
			ExibirAd("tesouro");
		else
			ExibirAd("shovel_gun");

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

		AlterarTileAdAssistirAnimator(!ads.checarAd);

		BloquearClique();
	}

	private void OcultarTileAd()
	{
		AlterarTileAdAnimator(false);
	}

	private bool ChecarTileAdAnimator()
	{
		return tileAdAnimator.GetBool("Exibir");
	}

	private void AlterarTileAdAnimator(bool estado)
	{
		tileAdAnimator.SetBool("Exibir", estado);
	}

	private void AlterarTileAdAssistirAnimator(bool estado)
	{
		tileAdAssistirAnimator.SetBool("Inativo", estado);
	}

	private void AtualizarMensagemTileAd()
	{
		if (!ChecarTesouroDisponivel())
			tileAdTexto.text = "Get a shovel gun for 30 seconds now for FREE!";
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
		FecharTelasCanvas();

		AlterarJohnAnimator(true);

		Invoke("ReproduzirAudioJohn", duracaoMovimentoJohn / 2f);
	}

	private void OcultarJohn()
	{
		AlterarJohnAnimator(false);

		Invoke("DesbloquearCliqueJohn", duracaoMovimentoJohn);

		if (quantidadeTiles == 0 &&
			!ChecarTesouroDisponivel())
		{
			Invoke("EncerrarNivel", duracaoMovimentoJohn);
		}
	}

	private bool ChecarJohnAnimator()
	{
		return johnAnimator.GetBool("Exibir");
	}

	private void AlterarJohnAnimator(bool estado)
	{
		johnAnimator.SetBool("Exibir", estado);
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

	// Áudios

	public void ReproduzirAudioClique()
	{
		ReproduzirAudio(audioClique);
	}

	public void ReproduzirAudioAcaoProibida()
	{
		ReproduzirAudio(audioBotaoDesativado);
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
