using UnityEngine;
using UnityEngine.Advertisements;

public class Ads : MonoBehaviour
{
	internal bool checarAd;

	private void Start()
	{
		ChecarAd();

		InvokeRepeating("ChecarAd", 1f, 1f);
	}

	private void ChecarAd()
	{
		checarAd = Advertisement.IsReady("rewardedVideo");
	}

	public void ExibirAd()
	{
		ChecarAd();

		if (checarAd)
		{
			Pausar.PausarJogo();

			var opcoes = new ShowOptions
			{
				resultCallback = HandleShowResult
			};

			Advertisement.Show("rewardedVideo", opcoes);
		}
	}

	private void HandleShowResult(ShowResult result)
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

	private void ProcessarAdConcluido()
	{
		switch (Jogo.instancia.recompensa)
		{
			case "tesouro":
				Jogo.instancia.AdicionarTesouro();
				break;

			case "pa":
				Jogo.instancia.EvoluirPa(true);
				break;

			case "shovel_gun":
				Jogo.instancia.IniciarModoShovelGun();
				break;

			case "moedas":
				Jogo.instancia.AdicionarMoedasInstantaneo(Jogo.instancia.quantidadeMoedasAd);
				break;
		}

		Jogo.instancia.recompensa = "";
	}
}
