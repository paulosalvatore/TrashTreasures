using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifetime : MonoBehaviour
{
	public float lifetime;

	private void Awake()
	{
		Destroy(gameObject, lifetime);
	}
}
