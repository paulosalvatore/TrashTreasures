using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GaleriaTesourosAbrir : MonoBehaviour,
	IPointerClickHandler
{
	[Header("Escalonamento")]
	public float delayEscalonamento;
	public float porcentagemEscalonamento;
	public iTween.EaseType animacaoEscalonamento;
	private Vector3 escalonamentoInicial;

	[Header("Sprites")]
	public float delayAbrirTesouro;
	public Sprite bauAberto;
	private Sprite bauFechado;
	public Image image;

	private bool tesouroAberto = false;

	private void Awake()
	{
		bauFechado = image.sprite;

		escalonamentoInicial = transform.localScale;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!tesouroAberto &&
			!Jogo.instancia.bloqueadorCliqueJohn)
		{
			tesouroAberto = true;

			Jogo.instancia.ReproduzirAudioClique();

			StartCoroutine(Escalonar());

			image.sprite = bauAberto;

			Invoke("Abrir", delayAbrirTesouro);
		}
	}

	private void Abrir()
	{
		Jogo.instancia.ExibirGaleriaTesouros();

		Invoke("Fechar", delayAbrirTesouro);
	}

	private void Fechar()
	{
		image.sprite = bauFechado;

		tesouroAberto = false;
	}

	private IEnumerator Escalonar()
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
}
