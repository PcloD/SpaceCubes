using UnityEngine;
using System.Collections;

public class PlanetRotation : MonoBehaviour 
{
	public float rotationSpeed = 1.0f;
	
	private Transform trans;
	
	void Start ()
	{
		trans = transform;
	}
	
	// Update is called once per frame
	void Update () 
	{
		trans.localRotation *= Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.up);
	}
}
