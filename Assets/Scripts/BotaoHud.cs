using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BotaoHud : MonoBehaviour,
	IPointerClickHandler
{
	public AudioClip audioClique;

	private AudioSource audioSource;

	void Start()
	{
		audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.clip = audioClique;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		audioSource.Play();
	}
}
