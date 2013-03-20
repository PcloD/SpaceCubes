using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubeEntity : MonoBehaviour
{
	protected Mesh mesh;
	protected MeshFilter meshFilter;
	protected MeshRenderer meshRenderer;
	protected MeshCollider meshCollider;
	
	protected List<int> triangleCubeMap = new List<int>(); //triangleCubeMap[triangleIndex] = z * (sizeX * sizeY) + y * (sizeX) + x (index into data array!)
	protected List<byte> triangleCubeFaceNumber = new List<byte>(); //triangleCubeMap[triangleIndex] = 0..5 (index into MeshUtils.faceNormalsTile]
	
	// Use this for initialization
	void Start () 
	{
		meshFilter = GetComponent<MeshFilter>();
		meshRenderer = GetComponent<MeshRenderer>();
		meshCollider = GetComponent<MeshCollider>();
		
		mesh = new Mesh();
		
		OnStart();
	}
	

	public void OnCollisionEnter(Collision collision) 
	{
		bool dirty = false;
		for (int i = 0; i < collision.contacts.Length; i++)
		{
			ContactPoint cp = collision.contacts[i];
			
			RaycastHit hitInfo;
			
			Ray ray = new Ray(cp.point - cp.normal,	cp.normal);
			
			if (meshCollider.Raycast(ray, out hitInfo, 2.0f))
			{
				if (OnCubeHit(triangleCubeMap[hitInfo.triangleIndex], cp))
					dirty = true;
			}
		}
		
		if (dirty)
			UpdateMesh();
	}
	
	protected virtual void OnStart()
	{
	}
	
	protected virtual bool OnCubeHit(int cubeId, ContactPoint cp)
	{
		return false;
	}
	
	protected virtual void UpdateMesh()
	{
		
	}
}

