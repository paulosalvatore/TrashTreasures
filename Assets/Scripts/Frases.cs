using System.Collections.Generic;
using UnityEngine;

public class Frases : MonoBehaviour
{
	[Header("Nível")]
	public int nivel;
	public Range nivelRange;

	[Header("Frase Aleatória")]
	public bool aleatoria;

	[Header("Chance (0-100)")]
	public float chance;
	internal float chanceInicial;

	[Header("Tesouros Completos")]
	public bool tesouros;

	[Header("Frases")]
	public List<string> frases;
	internal List<string> frasesReal;

	private void Awake()
	{
		chanceInicial = chance;

		if (aleatoria)
			AtualizarListaFrases();
	}

	public void AtualizarListaFrases()
	{
		frasesReal = new List<string>(frases);
	}
}
