using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

	[Header("Nível")]
	public int nivelMaximo;
	internal int nivel = 1;
	private Text nivelText;
	private AudioSource nivelAudio;
	private Animator limpoAnimator;
	private Text nivelLimpoText;

	[Header("Tiles")]
	public GameObject tileQuebrado;
	private List<Tiles> tilesDisponiveis = new List<Tiles>();
	private int quantidadeTiles;
	public bool oneHitTiles;
	public bool exibirTileQuebrado;

	[Header("Chances ao Destruir Tile (0 - 100)")]
	public float chanceTesouro;
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
	private float moedas;
	private Text moedasText;
	internal Image moedasImage;

	[Header("Pá")]
	public int paInicialId;
	private int paId;
	internal Pas paSelecionada;
	private Pas proximaPa;
	private List<Pas> pasDisponiveis = new List<Pas>();
	private Animator paDisponivelAnimator;

	// Definições da Área de Jogo
	private int[,] jogo;
	private int larguraJogo = 5;
	private int alturaJogo = 5;

	void Start()
	{
		// Definir Resolução Base da Tela
		DefinirResolucao();

		// Inicialização de Componentes Externos
		PegarComponentesExternos();

		// Definição de Variáveis Iniciais
		DefinirVariaveisIniciais();

		// Lista de Tiles Disponíveis
		PegarTilesDisponiveis();

		// Mapa
		IniciarMapa();

		// Nível
		AtualizarNivel();

		// Moedas
		AtualizarMoedas();

		// Pás
		PegarPasDisponiveis();
		SelecionarPa();
	}
	
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Z))
			EncerrarNivel();
	}

	/*
	 * Métodos de Inicialização de Componentes e Variáveis
	 */

	void DefinirResolucao()
	{
		Screen.SetResolution((int)resolucaoTela.x, (int)resolucaoTela.y, true);
	}

	void PegarComponentesExternos()
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
	}

	/*
	 * Mapa/Tela
	 */
	
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

	void ConstruirMapa()
	{
		for (int y = 0; y < alturaJogo; y++)
			for (int x = 0; x < larguraJogo; x++)
				InstanciarTile(x, y);
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

	/*
	 * Nível
	 */
	
	void AvancarNivel()
	{
		nivel++;

		Invoke("AtualizarNivel", duracaoMovimentoMapa);
	}

	void AtualizarNivel()
	{
		nivelText.text = string.Format("Level {0}", nivel);

		ExibirPaDisponivel();
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

	/*
	 * Tiles
	 */

	void InstanciarTile(int x, int y)
	{
		/*
		 * Pegar o Tile que será instanciado
		 * Se estivermos no nível 1 e o y for 0, instanciamos a Grama
		 * Caso contrário, pegamos um tile aleatório.
		 */
		Tiles tile =
			nivel == 1 && y == 0
				?
			tilesDisponiveis[0]
				:
			PegarTileAleatorio();

		// Instanciamos o tile na cena baseado na posição calculada e na rotação base do prefab
		Tiles tileInstanciado = Instantiate(tile);

		// Definimos um nome para o tile "x,y" para que fique mais organizado
		tileInstanciado.name = string.Format("{0},{1}", x, y);

		// Colocamos o tile instanciado dentro do GameObject destinado ao mapa, para melhor organização
		tileInstanciado.transform.parent = mapa;

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
		Transform tiles = GameObject.Find("TilesDisponíveis").transform;

		foreach (Transform tile in tiles)
			tilesDisponiveis.Add(tile.GetComponent<Tiles>());
	}

	Tiles PegarTileAleatorio()
	{
		List<Tiles> tilesChances = new List<Tiles>();

		int chanceIndex = 0;

		foreach (Tiles tile in tilesDisponiveis)
		{
			int chance = tile.PegarChance();
			
			if (chance > 0)
			{
				chanceIndex += chance;
				tile.chance = chanceIndex;

				tilesChances.Add(tile);
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
	
	/*
	 * Moedas
	 */

	int ProcessarAdicaoMoedas()
	{
		int adicionarMoedas = 0;

		float chance = Random.Range(0, 100);

		if (chance < chanceTesouro)
			AdicionarTesouro();
		else if (chance < chanceTesouro + chanceMoeda)
			adicionarMoedas = 1;

		return adicionarMoedas;
	}

	public void AdicionarMoedas(Transform tileDestruido, int adicionarMoedas = 0)
	{
		StartCoroutine(AdicionarMoedas(tileDestruido, adicionarMoedas, true));
	}

	IEnumerator AdicionarMoedas(Transform tileDestruido, int adicionarMoedas, bool coroutine)
	{
		Vector3 posicao = tileDestruido.position;
		Quaternion rotacao = tileDestruido.rotation;

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

	/*
	 * Tesouros
	 */

	void AdicionarTesouro()
	{
		Debug.Log("Adicionar Tesouro.");
	}

	/*
	 * Pás
	 */
	
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

	public void EvoluirPa()
	{
		if (paId >= pasDisponiveis.Count || moedas < proximaPa.moedas || nivel < proximaPa.nivel)
			return;

		moedas -= proximaPa.moedas;

		paId++;

		SelecionarPa();
	}

	void ExibirPaDisponivel()
	{
		if (paId >= pasDisponiveis.Count)
			return;
		
		if (nivel == proximaPa.nivel)
			AtualizarPaAnimator(true);
	}

	void AtualizarPaAnimator(bool estado)
	{
		paDisponivelAnimator.SetBool("Piscar", estado);
	}

	/*
	 * Métodos Estáticos
	 */

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
