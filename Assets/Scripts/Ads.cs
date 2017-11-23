using UnityEngine;
using UnityEngine.Advertisements;

public class Ads : MonoBehaviour
{
	internal bool checarAd;

	private void Awake()
	{
		InvokeRepeating("ChecarAd", 0.5f, 0.5f);
	}

	private void ChecarAd()
	{
		ChecarAd(false);
	}

	private void ChecarAd(bool teste)
	{
		if (teste)
			Debug.Log("ChecarAd");

		if (teste)
			Debug.Log("ChecarAd - Before: " + checarAd);

		checarAd = Advertisement.IsReady("rewardedVideo");

		if (teste)
			Debug.Log("ChecarAd - After: " + checarAd);

#if UNITY_EDITOR
		checarAd = true;
#endif
	}

	public void ExibirAd()
	{
		Debug.Log("ExibirAd");

		ChecarAd(true);

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

		Debug.Log(result);

		switch (result)
		{
			case ShowResult.Finished:
				Debug.Log("Finished");
				Invoke("ProcessarAdConcluido", 0.1f);
				break;

			case ShowResult.Skipped:
				Debug.Log("Skipped");
				Debug.Log("A propaganda encerrou antes de chegar ao final.");
				break;

			case ShowResult.Failed:
				Debug.Log("Failed");
				Debug.LogError("A propaganda falhou ao tentar ser exibida.");
				break;
		}
	}

	private void ProcessarAdConcluido()
	{
		Debug.Log("ProcessarAdConcluido, recompensa: " + Jogo.instancia.recompensa);

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
