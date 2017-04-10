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

	[Header("Tesouros")]
	public float delayExibicaoTesouros;
	public float duracaoAnimacaoTesouros;
	private Animator novoTesouroAnimator;
	private Image novoTesouroImage;
	private Text novoTesouroText;
	private Text tesouroText;
	private Animator novoTesouroGrabAnimator;
	private List<Tesouros> tesourosDisponiveis = new List<Tesouros>();
	private List<Tesouros> tesourosAdquiridos = new List<Tesouros>();
	internal float tempoTesouroAberto;

	// Definições da Área de Jogo
	private int[,] jogo;
	private int larguraJogo = 5;
	private int alturaJogo = 5;

	// Ads
	internal string recompensa;
	private Ads ads;

	// Audio Source
	private AudioSource audioSource;

	// Player Prefs
	private int nivelPref;
	private int moedasPref;
	private int paPref;
	private List<int> tesourosPref = new List<int>();

	void Start()
	{
		// Definir Resolução Base da Tela
		DefinirResolucao();

		// Inicialização de Componentes Externos
		PegarComponentes();

		// Definição de Variáveis Iniciais
		DefinirVariaveisIniciais();

		// Player Prefs
		PegarPlayerPrefs();

		// Mapa
		IniciarMapa();

		// Pás
		SelecionarPa();

		// Nível
		AtualizarNivel();

		// Moedas
		AtualizarMoedas();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Z))
			EncerrarNivel();
		else if (Input.GetKeyDown(KeyCode.W))
			ResetarPlayerPrefs();

		PegarTilesHit();
	}

	// Métodos de Inicialização de Componentes e Variáveis

	void DefinirResolucao()
	{
		Screen.SetResolution((int)resolucaoTela.x, (int)resolucaoTela.y, true);
	}

	void PegarComponentes()
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

		// Tesouros
		novoTesouroAnimator = GameObject.Find("NovoTesouro").GetComponent<Animator>();
		novoTesouroImage = novoTesouroAnimator.transform.FindChild("Imagem").GetComponent<Image>();
		novoTesouroText = novoTesouroAnimator.transform.FindChild("Nome").GetComponent<Text>();
		novoTesouroGrabAnimator = novoTesouroAnimator.transform.FindChild("Pegar").GetComponent<Animator>();
		tesouroText = GameObject.Find("TextoTesouro").GetComponent<Text>();

		// Ads
		ads = GetComponent<Ads>();

		// AudioSource
		audioSource = GetComponent<AudioSource>();
	}

	void DefinirVariaveisIniciais()
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

	void IniciarMapa()
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
	}

	void ZerarMapa()
	{
		foreach (Transform child in mapa)
			Destroy(child.gameObject);
	}

	void ReiniciarPosicaoMapa()
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

	void MovimentarMapa()
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

	void TremerTela()
	{
		// Aplica o Trigger "Animar" do Animator atrelado ao Background
		telaAnimator.SetTrigger("Animar");
	}

	void AlterarCorFundoJogo()
	{
		while (corAnteriorFundoJogo == fundoJogo.color)
			fundoJogo.color = coresFundoJogo[Random.Range(0, coresFundoJogo.Count)];

		corAnteriorFundoJogo = fundoJogo.color;
	}

	void AlterarBloqueadorClique(bool bloqueio)
	{
		bloqueadorClique = bloqueio;
	}

	void BloquearClique()
	{
		AlterarBloqueadorClique(true);
	}

	void DesbloquearClique()
	{
		AlterarBloqueadorClique(false);
	}

	// Nível

	void AvancarNivel()
	{
		nivel++;

		DefinirPlayerPrefs();

		Invoke("AtualizarNivel", duracaoMovimentoMapa);
	}

	void AtualizarNivel()
	{
		nivelText.text = string.Format("Level {0}", nivel);

		ExibirPaDisponivel();
		
		Invoke("VibrarNivel", (nivel == 1 || nivel == nivelPref ? duracaoMovimentoMapa : 0) + 0.2f);
	}

	void VibrarNivel()
	{
		Handheld.Vibrate();
	}

	void EncerrarNivel()
	{
		ExibirTextoNivelLimpo();

		Invoke("AvancarNivel", delayEncerrarNivel);

		Invoke("IniciarMapa", delayEncerrarNivel);
	}

	void ExibirTextoNivelLimpo()
	{
		nivelLimpoText.text = nivelText.text;

		limpoAnimator.SetTrigger("Animar");
	}

	void TocarAudioNivel()
	{
		nivelAudio.Play();
	}

	// Tiles

	void InstanciarTile(int x, int y, Transform mapaDestino, int nivelMapa)
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

	void PegarTilesDisponiveis()
	{
		tilesDisponiveis = new List<Tiles>();
		
		Transform tiles = GameObject.Find("TilesDisponíveis").transform;

		foreach (Transform tile in tiles)
			tilesDisponiveis.Add(tile.GetComponent<Tiles>());
	}

	Tiles PegarTileAleatorio(int x, int y, int nivelMapa)
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

	public void ProcessarTileDestruido(Transform tileDestruido, int moedas)
	{
		if (moedas == -1)
			moedas = ProcessarAdicaoMoedas();

		if (moedas > 0)
			AdicionarMoedas(tileDestruido, moedas);

		quantidadeTiles--;

		if (quantidadeTiles == 0)
			EncerrarNivel();
	}

	void PegarTilesHit()
	{
		if (bloqueadorClique)
			return;

		for (int i = 0; i < Input.touchCount; ++i)
		{
			Touch touch = Input.GetTouch(i);
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

	int ProcessarAdicaoMoedas()
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

	IEnumerator AdicionarMoedas(Transform origem, int adicionarMoedas, bool coroutine)
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

	void AtualizarMoedas()
	{
		moedasText.text = moedas.ToString();
	}

	IEnumerator AtualizarMoedas(int adicionarMoedas)
	{
		yield return new WaitForSeconds(moeda.duracao);

		moedas += adicionarMoedas;

		AtualizarMoedas();
	}

	// Pás

	void PegarPasDisponiveis()
	{
		Transform pas = GameObject.Find("PásDisponíveis").transform;

		foreach (Transform child in pas)
			pasDisponiveis.Add(child.GetComponent<Pas>());
	}

	void SelecionarPa()
	{
		paSelecionada = pasDisponiveis[paId - 1];

		if (paId < pasDisponiveis.Count)
			proximaPa = pasDisponiveis[paId];

		AtualizarPaAnimator(false);
	}

	public void EvoluirPa(bool gratuito = false)
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

		Invoke("DesbloquearClique", duracaoAnimacaoPas);

		AtualizarNovaPaAnimator(false);

		paId++;

		SelecionarPa();
	}

	void ExibirPaDisponivel()
	{
		if (paId >= pasDisponiveis.Count)
			return;

		if (nivel >= proximaPa.nivel)
			AtualizarPaAnimator(true);
	}

	void AtualizarPaAnimator(bool estado)
	{
		paDisponivelAnimator.SetBool("Exibir", estado);
	}

	public void ExibirNovaPaDisponivel()
	{
		novaPaImage.sprite = proximaPa.sprite;
		novaPaText.text = string.Format("{0} Shovel", proximaPa.nome);
		novaPaComprarText.text = proximaPa.moedas.ToString();

		AtualizarNovaPaAnimator(true);

		BloquearClique();
	}

	void AtualizarNovaPaAnimator(bool estado)
	{
		novaPaAnimator.SetBool("Exibir", estado);
		novaPaAssistirAnimator.SetBool("Animar", estado);
		novaPaComprarAnimator.SetBool("Animar", estado);
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

	// Tesouros

	void PegarTesourosDisponiveis()
	{
		Transform tesouros = GameObject.Find("TesourosDisponíveis").transform;

		foreach (Transform child in tesouros)
			tesourosDisponiveis.Add(child.GetComponent<Tesouros>());
	}

	Tesouros PegarTesouro()
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
				AlterarNovoTesouroAnimator(true);

				BloquearClique();
			}
		}
	}

	public void OcultarTesouro()
	{
		Invoke("DesbloquearClique", duracaoAnimacaoTesouros);

		AlterarNovoTesouroAnimator(false);
	}

	void AlterarNovoTesouroAnimator(bool estado)
	{
		novoTesouroAnimator.SetBool("Exibir", estado);
		novoTesouroGrabAnimator.SetBool("Animar", estado);
	}

	void AtualizarPorcentagemTesouros()
	{
		float porcentagem = tesourosAdquiridos.Count * 100 / tesourosDisponiveis.Count;

		tesouroText.text = string.Format("{0}%", porcentagem);
	}

	// Ads

	public void ExibirAd(string novaRecompensa = "")
	{
		recompensa = novaRecompensa;

		ads.ExibirAd();
	}

	// Player Prefs

	void PegarPlayerPrefs()
	{
		nivelPref = PlayerPrefs.GetInt("Nível");

		if (nivelPref > 0)
			nivel = nivelPref;

		moedasPref = PlayerPrefs.GetInt("Moedas");

		if (moedasPref > 0)
			moedas = moedasPref;

		paPref = PlayerPrefs.GetInt("Pá");

		if (paPref > 0)
		{
			paId = paPref;
			paInicialId = paId;
		}

		foreach (Tesouros tesouroDisponivel in tesourosDisponiveis)
		{
			bool tesouroAdquirido = PlayerPrefs.GetInt(tesouroDisponivel.nome) == 1;

			if (tesouroAdquirido)
				AdicionarTesouro(tesouroDisponivel);
		}
	}

	void DefinirPlayerPrefs()
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

	void ResetarPlayerPrefs()
	{
		PlayerPrefs.SetInt("Nível", 1);
		PlayerPrefs.SetInt("Moedas", 0);
		PlayerPrefs.SetInt("Pá", 1);

		foreach (Tesouros tesouroDisponivel in tesourosDisponiveis)
			PlayerPrefs.SetInt(tesouroDisponivel.nome, 0);

		ReiniciarCena();
	}

	// Cena

	void ReiniciarCena()
	{
		int cena = SceneManager.GetActiveScene().buildIndex;
		SceneManager.LoadScene(cena, LoadSceneMode.Single);
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

		AudioSource audioSource = objeto.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.Play();

		Destroy(objeto, clip.length);
	}
}
