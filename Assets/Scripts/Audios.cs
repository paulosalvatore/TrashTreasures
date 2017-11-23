using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audios : MonoBehaviour
{
	[Header("Gameplay")]
	public List<AudioClip> audiosGameplay;
	private List<AudioClip> audiosGameplayReproducao = new List<AudioClip>();

	[Header("John")]
	public List<AudioClip> audiosJohn;
	private List<AudioClip> audiosJohnReproducao = new List<AudioClip>();
	public bool exibindoJohn = false;
	private bool tocandoJohn = false;

	[Header("Tesouro")]
	public AudioClip audioTesouro;
	public bool exibindoTesouro = false;
	private bool tocandoTesouro = false;

	// Variáveis controladoras de estado
	private AudioClip proximoAudioClip = null;
	private float tempoAnterior;
	private bool realizarTroca;

	// Componentes e Instância
	private AudioSource audioSource;
	public static Audios instancia;

	private void Awake()
	{
		instancia = this;

		audioSource = GetComponent<AudioSource>();
	}

	private void Update()
	{
		ProcessarTrocaAudio();
	}

	/// <summary>
	/// Processa a troca do Áudio
	/// </summary>
	private void ProcessarTrocaAudio()
	{
		// Detecta alteração do John e força a troca de áudio

		if (exibindoJohn != tocandoJohn)
		{
			tocandoJohn = exibindoJohn;

			realizarTroca = true;
		}
		else if (exibindoTesouro != tocandoTesouro)
		{
			tocandoTesouro = exibindoTesouro;

			realizarTroca = true;
		}

		// Checar Listas de Áudio Vazias

		if (audiosGameplayReproducao.Count == 0)
			audiosGameplayReproducao = new List<AudioClip>(audiosGameplay);

		if (audiosJohnReproducao.Count == 0)
			audiosJohnReproducao = new List<AudioClip>(audiosJohn);

		// Checar Sortear/Trocar Áudio

		if (ChecarAudioFinalizado() || realizarTroca)
		{
			realizarTroca = false;

			if (exibindoJohn)
				proximoAudioClip = SortearAudio(ref audiosJohnReproducao);
			else if (exibindoTesouro)
				proximoAudioClip = audioTesouro;
			else
				proximoAudioClip = SortearAudio(ref audiosGameplayReproducao);
		}

		// Trocar Áudio, caso necessário

		if (proximoAudioClip != null &&
			proximoAudioClip != audioSource.clip)
		{
			float tempo = Mathf.Min(proximoAudioClip.length, audioSource.time);

			audioSource.clip = proximoAudioClip;

			audioSource.Play();

			audioSource.time = tempo;

			proximoAudioClip = null;
		}

		tempoAnterior = audioSource.time;
	}

	/// <summary>
	/// Sorteia um AudioClip de uma lista, remove e retorna.
	/// </summary>
	/// <param name="audios">Referência da Lista de Áudios</param>
	/// <returns>Retorna o AudioClip sorteado da lista de AudioClips</returns>
	private AudioClip SortearAudio(ref List<AudioClip> audios)
	{
		AudioClip audio = audios[Random.Range(0, audios.Count)];

		audios.Remove(audio);

		return audio;
	}

	/// <summary>
	/// Checa se o Áudio que está tocando no AudioSource foi finalizado.
	/// </summary>
	/// <returns>Retorna true caso tenha finalizado ou false caso ainda esteja tocando.</returns>
	private bool ChecarAudioFinalizado()
	{
		return tempoAnterior > audioSource.clip.length * 0.9f &&
			audioSource.time < audioSource.clip.length * 0.1f;
	}
}
