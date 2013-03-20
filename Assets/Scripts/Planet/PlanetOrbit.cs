using UnityEngine;
using System.Collections;

public class PlanetOrbit : MonoBehaviour 
{
	public Transform orbitPlanet;
	
	public float orbitSpeed;
	
	private Transform trans;
	
	private float orbitDistance;

	// Use this for initialization
	void Start () 
	{
		trans = transform;
		
		orbitDistance = (trans.position - orbitPlanet.position).magnitude;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 fwd = Vector3.Cross((orbitPlanet.position - trans.position).normalized, Vector3.up);
		
		trans.position += fwd * orbitSpeed * Time.deltaTime;
		
		trans.position = orbitPlanet.position + (trans.position - orbitPlanet.position).normalized * orbitDistance;
	}
	
	void OnDrawGizmos()
	{
		OnDrawGizmosSelected();
	}

	void OnDrawGizmosSelected()
	{
		if (orbitPlanet)
		{
			float radius = (transform.position - orbitPlanet.position).magnitude;
			
			Gizmos.matrix = Matrix4x4.TRS(orbitPlanet.position, Quaternion.identity, Vector3.one);
			
			for (float i = 0; i < 360; i++)
			{
				float fromR = i * Mathf.Deg2Rad;
				float toR = (i + 1.0f) * Mathf.Deg2Rad;
				
				Vector3 from =  new Vector3(Mathf.Sin(fromR) * radius, 0.0f, Mathf.Cos(fromR) * radius);
				Vector3 to =  new Vector3(Mathf.Sin(toR) * radius, 0.0f, Mathf.Cos(toR) * radius);
				
				Gizmos.DrawLine(from, to);
			}
		}
	}
}
