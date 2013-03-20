using UnityEngine;
using System.Collections;

public class CameraFollowShip : MonoBehaviour 
{
	public float maxRotationSpeed = 60.0f;
	
	public Transform shipToFollow;
	
	public Vector3 followDistance;
	
	private Transform trans;

	void Start () 
	{
		trans = transform;
	
	}
	
	void FixedUpdate () 
	{
		if (shipToFollow)
		{
			trans.rotation = 
				Quaternion.RotateTowards(
					trans.rotation,
					shipToFollow.rotation,
					maxRotationSpeed * Time.deltaTime);
			
			trans.position =
				shipToFollow.position + trans.rotation * followDistance;
		}
	}
}
