using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pausar : MonoBehaviour
{
	public static void PausarJogo()
	{
		Time.timeScale = 0;
	}

	public static void DespausarJogo()
	{
		Time.timeScale = 1;
	}
}