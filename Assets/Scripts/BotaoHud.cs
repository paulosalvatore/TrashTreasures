using UnityEngine;
using UnityEngine.EventSystems;

public class BotaoHud : MonoBehaviour,
	IPointerClickHandler
{
	public AudioClip audioClique;

	private Jogo jogo;
	private AudioSource audioSource;

	private void Start()
	{
		jogo = Jogo.Pegar();

		audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.clip = audioClique;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!jogo.bloqueadorCliqueJohn)
			audioSource.Play();
	}
}
