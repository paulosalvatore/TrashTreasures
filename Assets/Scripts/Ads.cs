using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class Ads : MonoBehaviour
{
	private Jogo jogo;

	void Start()
	{
		jogo = Jogo.Pegar();
	}

	public void ExibirAd()
	{
		Pausar.PausarJogo();

		if (Advertisement.IsReady("rewardedVideo"))
		{
			var opcoes = new ShowOptions {
				resultCallback = HandleShowResult
			};

			Advertisement.Show("rewardedVideo", opcoes);
		}
	}

	void HandleShowResult(ShowResult result)
	{
		Pausar.DespausarJogo();

		switch (result)
		{
			case ShowResult.Finished:
				Invoke("ProcessarAdConcluido", 0.1f);
				break;
			case ShowResult.Skipped:
				Debug.Log("A propaganda encerrou antes de chegar ao final.");
				break;
			case ShowResult.Failed:
				Debug.LogError("A propaganda falhou ao tentar ser exibida.");
				break;
		}
	}

	void ProcessarAdConcluido()
	{
		switch (jogo.recompensa)
		{
			case "tesouro":
				jogo.AdicionarTesouro();
				break;
			case "pa":
				jogo.EvoluirPa(true);
				break;
		}

		jogo.recompensa = "";
	}
}
