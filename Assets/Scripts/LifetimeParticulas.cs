using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifetimeParticulas : MonoBehaviour
{
	private new ParticleSystem particleSystem;

	void Start()
	{
		particleSystem = GetComponent<ParticleSystem>();
	}
	
	void Update()
	{
		if (particleSystem && !particleSystem.IsAlive())
			Destroy(gameObject);
	}
}
