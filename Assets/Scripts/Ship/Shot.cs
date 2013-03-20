using UnityEngine;
using System.Collections;

public class Shot : MonoBehaviour
{
	public float lifetime = 5.0f;
	
	public void Update()
	{
		lifetime -= Time.deltaTime;
		
		if (lifetime <= 0)
			GameObject.Destroy(gameObject);
	}
	
	void OnCollisionEnter (Collision collision)
	{
		GameObject.Destroy(gameObject);
	}
}

