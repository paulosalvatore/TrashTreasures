using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtualizarAdAnimator : MonoBehaviour
{
	private Animator animator;

	private void Awake()
	{
		animator = GetComponent<Animator>();

		InvokeRepeating("AtualizarAnimator", 1f, 1f);
	}

	private void AtualizarAnimator()
	{
		animator.SetBool("Inativo", !Jogo.instancia.ads.checarAd);
	}
}
