using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SingleMaterialCubeEntity : CubeEntity 
{
	public int sizeX;
	public int sizeY;
	public int sizeZ;
	
	public CubeMaterialType materialType;
	
	private BitArray data;
	
	protected override void OnStart ()
	{
		Create();
	}
	
	private void Create()
	{
		data = new BitArray(sizeX * sizeY * sizeZ, true);
		
		//The outermost layer is always empty.. we do this to avoid bounds validation in the UpdateMesh() code
		for (int x = 0; x < sizeX; x++)
		{
			for (int y = 0; y < sizeY; y++)
			{
				SetTile(x, y, 0, false);
				SetTile(x, y, sizeZ - 1, false);
			}
		}
				
		for (int x = 0; x < sizeX; x++)
		{
			for (int z = 0; z < sizeZ; z++)
			{
				SetTile(x, 0, z, false);
				SetTile(x, sizeY - 1, z, false);
			}
		}
		
		for (int y = 0; y < sizeY; y++)
		{
			for (int z = 0; z < sizeZ; z++)
			{
				SetTile(0, y, z, false);
				SetTile(sizeX - 1, y, z, false);
			}
		}
		
		UpdateMesh();
	}
	
    static private List<int> triangles = new List<int>();
    static private List<Vector3> vertices = new List<Vector3>();
    static private List<Vector3> normals = new List<Vector3>();
    static private List<Vector2> uvs = new List<Vector2>();
	
	public bool GetTile(int x, int y, int z)
	{
		return data[z * (sizeX * sizeY) + y * (sizeX) + x];
	}
	
	public void SetTile(int x, int y, int z, bool val)
	{
		data[z * (sizeX * sizeY) + y * (sizeX) + x] = val;
	}
	
	public int GetTileOffset(int x, int y, int z)
	{
		return z * (sizeX * sizeY) + y * (sizeX) + x;
	}
	
	/*public void  Update()
	{
	    if (Input.GetMouseButtonDown(0))
		{
	        RaycastHit hit;
	        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	        
			if (meshCollider.Raycast(ray, out hit, 1000.0f))
			{
				data[triangleCubeMap[hit.triangleIndex]] = false;
				UpdateMesh();
	        }
	    }
    }*/
	
	protected override void UpdateMesh()
	{
		MeshUtils.InitStaticValues();
		
        Vector3[] faceVectors = MeshUtils.faceVectorsNormal;
    	TilePosition[] faceNormalsTile = MeshUtils.faceNormalsTile;
    	Vector3[] faceNormals = MeshUtils.faceNormals;
		
		mesh.Clear();
		
		vertices.Clear();
		normals.Clear();
		triangleCubeMap.Clear();
		triangleCubeFaceNumber.Clear();
		triangles.Clear();
		uvs.Clear();
		
        int index = 0;
		
		float fromTX = (1.0f / 8.0f) * (((int) materialType) % 8);
		float toTX = fromTX + (1.0f / 8.0f);
		
		float fromTY = 1.0f - (1.0f / 8.0f) * (((int) materialType) / 8);
		float toTY = fromTY - (1.0f / 8.0f);
		
		Vector3 cubeCenter = MeshUtils.GetCubeCenter(sizeX, sizeY, sizeZ);
		
        for (int z = 1; z < sizeZ - 1; z++)
        {
            for (int y = 1; y < sizeY - 1; y++)
            {
				int dataOffset = GetTileOffset(1, y, z);
			
                for (int x = 1; x < sizeX - 1; x++)
				{
					if (!data[dataOffset++])
						continue;
				
                    TilePosition pos = new TilePosition(x, y, z);
                    Vector3 offset = new Vector3(x, y, z) * MeshUtils.TILE_SIZE - cubeCenter;
					
					for (int face = 0; face < 6; face++)
					{
                        TilePosition normalInt = faceNormalsTile[face];
                        TilePosition near = pos + normalInt;
						
						//We draw this face only if there isn't a visible tile in the direction of the face
						if (!GetTile(near.x, near.y, near.z))
						{
                            Vector3 faceNormal = faceNormals[face];
							
							//for (int i = 0; i < 4; i++)
							//{
                            //    normals.Add(faceNormal);
                            //    vertices.Add(faceVectors[(face << 2) + i] + offset);
							//}
							
							//START MANUAL LOOP UNROLLING OF LOOP ABOVE
                            normals.Add(faceNormal);
                            normals.Add(faceNormal);
                            normals.Add(faceNormal);
                            normals.Add(faceNormal);
							
                            vertices.Add(faceVectors[(face << 2) + 0] + offset);
                            vertices.Add(faceVectors[(face << 2) + 1] + offset);
                            vertices.Add(faceVectors[(face << 2) + 2] + offset);
                            vertices.Add(faceVectors[(face << 2) + 3] + offset);
							//END MANUAL LOOP UNROLLING
							
                            uvs.Add(new Vector2(fromTX, fromTY));
                            uvs.Add(new Vector2(fromTX, toTY));
                            uvs.Add(new Vector2(toTX, toTY));
                            uvs.Add(new Vector2(toTX, fromTY));
							
                            triangles.Add(index + 0);
                            triangles.Add(index + 1);
                            triangles.Add(index + 2);

                            triangles.Add(index + 2);
                            triangles.Add(index + 3);
                            triangles.Add(index + 0);
									
							triangleCubeMap.Add(dataOffset - 1);
							triangleCubeMap.Add(dataOffset - 1);
							
							triangleCubeFaceNumber.Add((byte) face);
							triangleCubeFaceNumber.Add((byte) face);
							
                            index += 4;
						}
					}
				}
			}
		}
		
        mesh.vertices = vertices.ToArray();
		mesh.uv = uvs.ToArray();
        mesh.normals = normals.ToArray();
		mesh.triangles = triangles.ToArray();
		
		if (meshFilter)
			meshFilter.sharedMesh = mesh;
		
		if (meshCollider)
		{
			meshCollider.sharedMesh = null;
			meshCollider.sharedMesh = mesh;
		}
	}
	
	protected override bool OnCubeHit(int cubeId, ContactPoint cp)
	{
		if (cp.otherCollider.GetComponent<Shot>())
		{
			data[cubeId] = false;
			return true;
		}
		
		return false;
	}
	
	public void OnDrawGizmosSelected()
	{
		OnDrawGizmos();
	}
	
	public void OnDrawGizmos()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		
		Gizmos.DrawCube(Vector3.zero, new Vector3(sizeX, sizeY, sizeZ));
	}
}
