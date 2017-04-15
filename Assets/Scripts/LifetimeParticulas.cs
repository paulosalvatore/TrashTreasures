using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifetimeParticulas : MonoBehaviour
{
	private new ParticleSystem particleSystem;

	private void Start()
	{
		particleSystem = GetComponent<ParticleSystem>();
	}

	private void Update()
	{
		if (particleSystem && !particleSystem.IsAlive())
			Destroy(gameObject);
	}
}