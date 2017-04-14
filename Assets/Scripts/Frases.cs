using System.Collections;
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

	[Header("Tesouros Completos")]
	public bool tesouros;

	[Header("Frases")]
	public List<string> frases;
}
