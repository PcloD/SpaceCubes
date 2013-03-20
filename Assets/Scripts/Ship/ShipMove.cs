using UnityEngine;
using System.Collections;

public class ShipMove : MonoBehaviour 
{
	public float rotationForce = 10.0f;
	public float rotationDrag = 4.0f;
	
	public float engineDrag = 4.0f;
	public float engineForce = 80.0f;
	
	private Transform trans;
	private Rigidbody rb;
	
	private float engine;
	
	
	// Use this for initialization
	void Start () 
	{
		trans = transform;
		rb = rigidbody;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		rb.angularDrag = rotationDrag;
		rb.drag = engineDrag;
		
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = -Input.GetAxis("Vertical");
		float acceleration = Input.GetAxis("Acceleration");
		
		rb.AddTorque(horizontal * trans.up * rotationForce);
		rb.AddTorque(vertical * trans.right * rotationForce);
		
		if (acceleration > 0.0f)
			engine += engineForce * Time.deltaTime;
		
		if (acceleration < 0.0f)
			engine -= engineForce * Time.deltaTime;
		
		engine = Mathf.Clamp(engine, 0.0f, engineForce);
		
		if (Mathf.Abs(rb.angularVelocity.magnitude) < 0.001f)
			rb.angularVelocity = Vector3.zero;
		
		if (Mathf.Abs(rb.velocity.magnitude) < 0.001f)
			rb.velocity = Vector3.zero;
	
		rb.AddForce(trans.forward * engine);
	}
	
	public void OnCollisionEnter(Collision collision) 
	{
		if (collision.relativeVelocity.magnitude > 1.0f)
		{
			float forwardVelocity = Vector3.Dot(collision.relativeVelocity, trans.forward);
			
			engine -= engine * Mathf.Abs(forwardVelocity / collision.relativeVelocity.magnitude);
			
			engine = Mathf.Clamp(engine, 0.0f, engineForce);
		}
	}
	
	
	void OnGUI()
	{
		GUI.Label(new Rect(0, 0, 200, 100), 
			string.Format("Engine Force: {0:0.00}\nSpeed: {1:0.00}", engine, rb.velocity.magnitude));
	}
}
