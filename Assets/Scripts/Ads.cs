using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.EventSystems;

public class Ads : MonoBehaviour,
	IPointerClickHandler
{
	private Jogo jogo;

	void Start()
	{
		jogo = Jogo.Pegar();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		ShowRewardedAd();
	}

	/*
	public void ShowAd()
	{
		if (Advertisement.IsReady())
		{
			Advertisement.Show();
		}
	}
	*/

	void ShowRewardedAd()
	{
		if (Advertisement.IsReady("rewardedVideo"))
		{
			var options = new ShowOptions {
				resultCallback = HandleShowResult
			};

			Advertisement.Show("rewardedVideo", options);
		}
	}

	void HandleShowResult(ShowResult result)
	{
		switch (result)
		{
			case ShowResult.Finished:
				Invoke("ProcessarAdConcluido", 1f);
				break;
			case ShowResult.Skipped:
				Debug.Log("The ad was skipped before reaching the end.");
				break;
			case ShowResult.Failed:
				Debug.LogError("The ad failed to be shown.");
				break;
		}
	}

	void ProcessarAdConcluido()
	{
		jogo.AdicionarMoedas(transform, 1);
	}
}
