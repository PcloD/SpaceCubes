using UnityEngine;

public class ShipInputPC : MonoBehaviour
{
	public ShipMove shipMove;
	public ShipFire shipFire;

	public void Start()
	{
#if UNITY_ANDROID || UNITY_IPHONE
		enabled = false;
#endif
	}

	public void Update()
	{
		shipMove.inputHorizontal = Input.GetAxis("Horizontal");
		shipMove.inputVertical = -Input.GetAxis("Vertical");
		shipMove.inputAcceleration = Input.GetAxis("Acceleration");

        if (Input.GetButton("Fire1") || Input.GetAxis("Fire1") > 0)
            shipFire.Fire();
	}
}


