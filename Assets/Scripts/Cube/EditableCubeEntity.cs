using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class EditableCubeEntity : CubeEntity 
{
	public string dataString;
	
	private int sizeX = 1;
	private int sizeY = 1;
	private int sizeZ = 1;
	
	private byte[] data;
	
	protected override void OnStart ()
	{
		Deserialize();
		UpdateMesh ();
	}
	
	private void Serialize()
	{
		System.IO.MemoryStream ms = new System.IO.MemoryStream();
		
		System.IO.BinaryWriter bw = new System.IO.BinaryWriter(ms);
		
		bw.Write(sizeX);
		bw.Write(sizeY);
		bw.Write(sizeZ);
		bw.Write(data);
		
		bw.Flush();
		
		dataString = EncodeTo64(ms.ToArray());
	}
	
	private void Deserialize()
	{
		if (!string.IsNullOrEmpty(dataString))
		{
			byte[] bytes = DecodeFrom64(dataString);
			System.IO.BinaryReader br = new System.IO.BinaryReader(new System.IO.MemoryStream(bytes));
			
			sizeX = br.ReadInt32();
			sizeY = br.ReadInt32();
			sizeZ = br.ReadInt32();
			
			data = br.ReadBytes(sizeX * sizeY * sizeZ);
		}
		else
		{
			sizeX = 1;
			sizeY = 1;
			sizeZ = 1;
			data = new byte[1];
			data[0] = (byte) CubeMaterialType.Dirt;
		}
	}
	
	static public byte[] DecodeFrom64(string encodedData)
    {
		return System.Convert.FromBase64String(encodedData);
    }	
	
	static public string EncodeTo64(byte[] toEncode)
    {
		return System.Convert.ToBase64String(toEncode);
    }	
	
    static private List<int> triangles = new List<int>();
    static private List<Vector3> vertices = new List<Vector3>();
    static private List<Vector3> normals = new List<Vector3>();
    static private List<Vector2> uvs = new List<Vector2>();
	
	public CubeMaterialType GetTile(int x, int y, int z)
	{
		return (CubeMaterialType) data[z * (sizeX * sizeY) + y * (sizeX) + x];
	}
	
	public void SetTile(int x, int y, int z, CubeMaterialType val)
	{
		data[z * (sizeX * sizeY) + y * (sizeX) + x] = (byte) val;
	}
	
	public int GetTileOffset(int x, int y, int z)
	{
		return z * (sizeX * sizeY) + y * (sizeX) + x;
	}
	
	public void GetTileCoordinates(int index, out int x, out int y, out int z)
	{
		z = index / (sizeX * sizeY);
		y = (index % (sizeX * sizeY)) / sizeX;
		x = (index % (sizeX * sizeY)) % sizeX;
	}
	
	public bool IsValidTilePosition(int x, int y, int z)
	{
		return x >= 0 && y >= 0 && z >= 0 && x < sizeX && y <sizeY && z < sizeZ;
	}
	
#if UNITY_EDITOR
	
	[HideInInspector]
	public CubeMaterialType materialTypeToAdd = CubeMaterialType.Dirt;
	
	public void Update()
	{
		if (!Application.isPlaying)
		{
			Deserialize();
			UpdateMesh();
		}
	}
	
	public void EditorMouseClick(Event current)
	{
		if (current.button == 1)
		{
			if (!mesh)
			{
				mesh = null;
				UpdateMesh();
			}
			
			if (mesh)
			{
				meshCollider = GetComponent<MeshCollider>();
				
				if (!meshCollider)
				{
					meshCollider = gameObject.AddComponent<MeshCollider>();
					meshCollider.sharedMesh = mesh;
				}
			}
			
	        RaycastHit hit;
			
		 	Ray ray = HandleUtility.GUIPointToWorldRay(current.mousePosition);
	        
			if (meshCollider.Raycast(ray, out hit, 1000.0f))
			{
				int x, y, z;
				
				GetTileCoordinates(triangleCubeMap[hit.triangleIndex], out x, out y, out z);
				
				MeshUtils.InitStaticValues();
				
				if (!current.alt && !current.shift && !current.control)
				{
					TilePosition faceNormal = MeshUtils.faceNormalsTile[triangleCubeFaceNumber[hit.triangleIndex]];
					
					Undo.RegisterUndo(EditorUtility.CollectDeepHierarchy(new Object[] { gameObject }), "Added Cube");
					
					SetOrAddTile(x + faceNormal.x, y + faceNormal.y, z + faceNormal.z, materialTypeToAdd);
					
					UpdateMesh();
					
					Serialize();
					EditorUtility.SetDirty (this);
				}
				else if (current.shift && !current.alt && !current.control)
				{
					Undo.RegisterUndo(EditorUtility.CollectDeepHierarchy(new Object[] { gameObject }), "Removed Cube");
					
					SetTile(x, y, z, CubeMaterialType.Empty);
					
					CheckMinSize();
					
					UpdateMesh();
					
					Serialize();
				}
				else if (!current.shift && !current.alt && current.control)
				{
					Undo.RegisterUndo(EditorUtility.CollectDeepHierarchy(new Object[] { gameObject }), "Changed Cube");
					
					SetTile(x, y, z, materialTypeToAdd);
					
					UpdateMesh();
					
					Serialize();
				}
				
				//data[triangleCubeMap[hit.triangleIndex]] = (byte) CubeMaterialType.Empty;
				//UpdateMesh();
	        }
			else
			{
				//Debug.Log("mesh not hit");
			}
		}
	}
#endif
	
	private void CheckMinSize()
	{
		int minX = 0, minY = 0, minZ = 0;
		
		for (int dz = 0; dz < sizeZ; dz++)
		{
			for (int dy = 0; dy < sizeY; dy++)
			{
				for (int dx = 0; dx < sizeX; dx++)
				{
					if (GetTile(dx, dy, dz) != CubeMaterialType.Empty)
					{
						minX = Mathf.Max(minX, dx);
						minY = Mathf.Max(minY, dy);
						minZ = Mathf.Max(minZ, dz);
					}
				}
			}
		}
		
		//Offset are ZERO based, increase them in ONE to get size
		minX++;
		minY++;
		minZ++;
		
		if (minX != sizeX || minY != sizeY || minZ != sizeZ)
		{
			byte[] newData = new byte[minX * minY * minZ];
			
			for (int dz = 0; dz < minZ; dz++)
				for (int dy = 0; dy < minY; dy++)
					for (int dx = 0; dx < minX; dx++)
						newData[dz * (minX * minY) + dy * (minX) + dx] = data[dz * (sizeX * sizeY) + dy * (sizeX) + dx];
			
			Vector3 oldCubeCenter = MeshUtils.GetCubeCenter(sizeX, sizeY, sizeZ);
			Vector3 newCubeCenter = MeshUtils.GetCubeCenter(minX, minY, minZ);
			this.transform.localPosition += Vector3.Scale(newCubeCenter - oldCubeCenter, transform.localScale);
			
			this.sizeX = minX;
			this.sizeY = minY;
			this.sizeZ = minZ;
			this.data = newData;
		}
	}
	
	private void SetOrAddTile(int x, int y, int z, CubeMaterialType materialType)
	{
		if (IsValidTilePosition(x, y, z))
		{
			SetTile(x, y, z, materialType);
		}
		else
		{
			int rootX = 0, rootY = 0, rootZ = 0;
			
			int newSizeX = Mathf.Max(sizeX, x + 1);
			int newSizeY = Mathf.Max(sizeY, y + 1);
			int newSizeZ = Mathf.Max(sizeZ, z + 1);
			
			if (x < 0)
			{
				rootX = -x;
				x = 0;
				newSizeX += rootX;
			}
			
			if (y < 0)
			{
				rootY = -y;
				y = 0;
				newSizeY += rootY;
			}
			
			if (z < 0)
			{
				rootZ = -z;
				z = 0;
				newSizeZ += rootZ;
			}
			
			byte[] newData = new byte[newSizeX * newSizeY * newSizeZ];
			
			for (int dz = 0; dz < sizeZ; dz++)
				for (int dy = 0; dy < sizeY; dy++)
					for (int dx = 0; dx < sizeX; dx++)
						newData[(dz + rootZ) * (newSizeX * newSizeY) + (dy + rootY) * (newSizeX) + (dx + rootX)] = data[dz * (sizeX * sizeY) + dy * (sizeX) + dx];
			
			newData[z * (newSizeX * newSizeY) + y * (newSizeX) + x] = (byte) materialType;
			
			Vector3 oldCubeCenter = MeshUtils.GetCubeCenter(sizeX, sizeY, sizeZ) + new Vector3(rootX, rootY, rootZ) * MeshUtils.TILE_SIZE;//  new Vector3((sizeX + rootX * 2.0f) * 0.5f - 0.5f, (sizeY + rootY * 2.0f) * 0.5f - 0.5f, (sizeZ + rootZ * 2.0f) * 0.5f - 0.5f);
			Vector3 newCubeCenter = MeshUtils.GetCubeCenter(newSizeX, newSizeY, newSizeZ);//  new Vector3(newSizeX * 0.5f - 0.5f, newSizeY * 0.5f - 0.5f, newSizeZ * 0.5f - 0.5f);
			this.transform.localPosition += Vector3.Scale(newCubeCenter - oldCubeCenter, transform.localScale);
			
			this.sizeX = newSizeX;
			this.sizeY = newSizeY;
			this.sizeZ = newSizeZ;
			this.data = newData;
		}
	}
	
	protected override void UpdateMesh()
	{
		MeshUtils.InitStaticValues();
		
        Vector3[] faceVectors = MeshUtils.faceVectorsNormal;
    	TilePosition[] faceNormalsTile = MeshUtils.faceNormalsTile;
    	Vector3[] faceNormals = MeshUtils.faceNormals;
		
		mesh.Clear();
		
		vertices.Clear();
		normals.Clear();
		triangles.Clear();
		uvs.Clear();
		
		triangleCubeMap.Clear();
		triangleCubeFaceNumber.Clear();
		
        int index = 0;
		int dataOffset = 0;
		
		Vector3 cubeCenter = MeshUtils.GetCubeCenter(sizeX, sizeY, sizeZ);
		
        for (int z = 0; z < sizeZ; z++)
        {
            for (int y = 0; y < sizeY; y++)
            {
	            for (int x = 0; x < sizeX; x++)
				{
					CubeMaterialType materialType = (CubeMaterialType) data[dataOffset++];
					
					if (materialType == CubeMaterialType.Empty)
						continue;
				
                    TilePosition pos = new TilePosition(x, y, z);
                    Vector3 offset = new Vector3(x, y, z) * MeshUtils.TILE_SIZE - cubeCenter;
					
					float fromTX = (1.0f / 8.0f) * (((int) materialType) % 8);
					float toTX = fromTX + (1.0f / 8.0f);
					
					float fromTY = 1.0f - (1.0f / 8.0f) * (((int) materialType) / 8);
					float toTY = fromTY - (1.0f / 8.0f);
					
					for (int face = 0; face < 6; face++)
					{
                        TilePosition normalInt = faceNormalsTile[face];
                        TilePosition near = pos + normalInt;
						
						//We draw this face only if there isn't a visible tile in the direction of the face
						if (!IsValidTilePosition(near.x, near.y, near.z) || GetTile(near.x, near.y, near.z) == CubeMaterialType.Empty)
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
			data[cubeId] = (byte) CubeMaterialType.Empty;
			return true;
		}
		
		return false;
	}
}
