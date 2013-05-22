using UnityEngine;

public class ShipInputMobile : MonoBehaviour
{
	public ShipMove shipMove;
	public ShipFire shipFire;

	public void Start()
	{
		#if !UNITY_ANDROID && !UNITY_IPHONE
		enabled = false;
		#endif
	}

	public void Update()
	{
		shipMove.inputHorizontal = Mathf.Clamp(Input.acceleration.x / 0.3f, -1.0f, 1.0f);
		shipMove.inputVertical = Mathf.Clamp((Input.acceleration.y + 0.5f) / 0.3f, -1.0f, 1.0f);

        float targetInputAcceleration = 0.0f;

        for (int i = 0; i < Input.touches.Length; i++)
        {
            Touch touch = Input.touches[i];

            if (touch.position.x > Screen.width / 2)
            {
                shipFire.Fire();
            }
            else
            {
                if (touch.position.y > Screen.height / 2)
                    targetInputAcceleration = 1.0f;
                else
                    targetInputAcceleration = -1.0f;
            }
        }

        shipMove.inputAcceleration = Mathf.MoveTowards(shipMove.inputAcceleration, targetInputAcceleration, Time.deltaTime);
	}
}


