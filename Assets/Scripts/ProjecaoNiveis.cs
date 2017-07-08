using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class ProjecaoNiveis : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Q))
			CriarProjecaoNiveis();
	}

	private void CriarProjecaoNiveis()
	{
		foreach (Transform child in transform)
			Destroy(child.gameObject);

		ObscuredInt niveis = 100;

		for (ObscuredInt nivel = 1; nivel <= niveis; nivel++)
		{
			Transform mapaDestino = new GameObject(string.Format("Mapa {0}", nivel)).transform;

			mapaDestino.parent = transform;

			mapaDestino.localPosition = new Vector2(
				nivel * 6,
				0
			);

			Jogo.instancia.ConstruirMapa(mapaDestino, nivel);

			Transform texto = new GameObject("Texto").transform;

			texto.parent = mapaDestino;

			texto.localPosition = new Vector3(
				-0.5f,
				2f
			);

			TextMesh textMesh = texto.gameObject.AddComponent<TextMesh>();
			textMesh.text = string.Format("Nível {0}", nivel);
		}
	}
}
