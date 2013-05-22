using UnityEngine;
using System.Collections;

public class ShipFire : MonoBehaviour 
{
	public float timeBetweenShots = 0.2f;
	public float shotSpeed = 200.0f;
	
	public Transform fireFrom;
	public GameObject shotPrefab;
	
	private float nextFireTime;
	private Rigidbody rb;
	
	// Use this for initialization
	public void Start () 
	{
		rb = rigidbody;
	}
	
	// Update is called once per frame
	public void Update () 
	{
		nextFireTime -= Time.deltaTime;
	}

	public void Fire()
	{
		if (nextFireTime <= 0) 
		{
            GameObject goFire = (GameObject)GameObject.Instantiate(shotPrefab, fireFrom.position, fireFrom.rotation);

			goFire.rigidbody.velocity = rb.velocity + fireFrom.forward * shotSpeed;

			nextFireTime = timeBetweenShots;
		}
	}
}
