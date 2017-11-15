using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaleriaTesourosBotao : MonoBehaviour
{
	public Button botao;
	public GameObject comprar;
	public Text precoText;
	public Image tesouroImagem;
	public Animator animator;

	private Tesouros tesouro;

	private bool tesouroDesbloqueado = false;

	private Color corNormal;
	private Color corAdquirido;

	private void Awake()
	{
		corNormal = botao.colors.normalColor;
		corAdquirido = botao.colors.highlightedColor;

		botao.onClick.AddListener(EventoClick);
	}

	public void Inicializar(Tesouros tesouroAtual)
	{
		tesouro = tesouroAtual;

		precoText.text = tesouro.preco.ToString();
		tesouroImagem.sprite = tesouro.sprite;
	}

	public void Atualizar(bool estado)
	{
		tesouroImagem.gameObject.SetActive(estado);

		tesouroDesbloqueado = estado;

		comprar.SetActive(!estado);

		AtualizarCorBotao(estado);
	}

	private void AtualizarCorBotao(bool estado)
	{
		ColorBlock corBotao = botao.colors;

		corBotao.normalColor = estado ? corAdquirido : corNormal;
		corBotao.highlightedColor = estado ? corAdquirido : corNormal;

		botao.colors = corBotao;
	}

	public void AlterarExibicao(bool estado)
	{
		AtualizarAnimator(estado);
	}

	private void AtualizarAnimator(bool estado)
	{
		animator.SetBool("Exibir", estado);
	}

	public void EventoClick()
	{
		if (!tesouroDesbloqueado)
		{
			Jogo.instancia.ComprarTesouroGaleria(tesouro);
		}
		else
		{
			Jogo.instancia.ExibirTesouroDestaque(tesouro);
		}
	}
}
