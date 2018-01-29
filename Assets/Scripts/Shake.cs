using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour {

	[SerializeField]
	float shakeFactor,
			decreaseFactor;

	public static float Value;
	
	// Update is called once per frame
	void Update ()
	{
		if (Value > 0)
		{
			transform.localPosition = Random.insideUnitSphere * Value * shakeFactor;
			Value -= Time.deltaTime * decreaseFactor;
		}
	}
}