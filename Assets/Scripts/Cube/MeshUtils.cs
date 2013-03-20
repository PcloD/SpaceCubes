using UnityEngine;

public struct MeshUtils
{
	public const float HALF_TILE_SIZE = 2.0f;
	public const float TILE_SIZE = 4.0f;
	
    static public Vector3[] faceVectorsNormal;
    static public Vector3[] faceNormals;
    static public TilePosition[] faceNormalsTile;

    static public void InitStaticValues()
    {
		if (faceVectorsNormal != null)
			return;
		
        faceVectorsNormal = new Vector3[6 * 4];
        faceNormals = new Vector3[6];
        faceNormalsTile = new TilePosition[6];

        //back
        faceVectorsNormal[0] = new Vector3(-HALF_TILE_SIZE, -HALF_TILE_SIZE, -HALF_TILE_SIZE);
        faceVectorsNormal[1] = new Vector3(-HALF_TILE_SIZE, HALF_TILE_SIZE, -HALF_TILE_SIZE);
        faceVectorsNormal[2] = new Vector3(HALF_TILE_SIZE, HALF_TILE_SIZE, -HALF_TILE_SIZE);
        faceVectorsNormal[3] = new Vector3(HALF_TILE_SIZE, -HALF_TILE_SIZE, -HALF_TILE_SIZE);

        faceNormals[0] = Vector3.back;

        //front
        faceVectorsNormal[4] = new Vector3(HALF_TILE_SIZE, -HALF_TILE_SIZE, HALF_TILE_SIZE);
        faceVectorsNormal[5] = new Vector3(HALF_TILE_SIZE, HALF_TILE_SIZE, HALF_TILE_SIZE);
        faceVectorsNormal[6] = new Vector3(-HALF_TILE_SIZE, HALF_TILE_SIZE, HALF_TILE_SIZE);
        faceVectorsNormal[7] = new Vector3(-HALF_TILE_SIZE, -HALF_TILE_SIZE, HALF_TILE_SIZE);

        faceNormals[1] = Vector3.forward;

        //bottom
        faceVectorsNormal[8] = new Vector3(HALF_TILE_SIZE, -HALF_TILE_SIZE, -HALF_TILE_SIZE);
        faceVectorsNormal[9] = new Vector3(HALF_TILE_SIZE, -HALF_TILE_SIZE, HALF_TILE_SIZE);
        faceVectorsNormal[10] = new Vector3(-HALF_TILE_SIZE, -HALF_TILE_SIZE, HALF_TILE_SIZE);
        faceVectorsNormal[11] = new Vector3(-HALF_TILE_SIZE, -HALF_TILE_SIZE, -HALF_TILE_SIZE);

        faceNormals[2] = Vector3.down;

        //top
        faceVectorsNormal[12] = new Vector3(HALF_TILE_SIZE, HALF_TILE_SIZE, HALF_TILE_SIZE);
        faceVectorsNormal[13] = new Vector3(HALF_TILE_SIZE, HALF_TILE_SIZE, -HALF_TILE_SIZE);
        faceVectorsNormal[14] = new Vector3(-HALF_TILE_SIZE, HALF_TILE_SIZE, -HALF_TILE_SIZE);
        faceVectorsNormal[15] = new Vector3(-HALF_TILE_SIZE, HALF_TILE_SIZE, HALF_TILE_SIZE);

        faceNormals[3] = Vector3.up;

        //right
        faceVectorsNormal[16] = new Vector3(HALF_TILE_SIZE, -HALF_TILE_SIZE, -HALF_TILE_SIZE);
        faceVectorsNormal[17] = new Vector3(HALF_TILE_SIZE, HALF_TILE_SIZE, -HALF_TILE_SIZE);
        faceVectorsNormal[18] = new Vector3(HALF_TILE_SIZE, HALF_TILE_SIZE, HALF_TILE_SIZE);
        faceVectorsNormal[19] = new Vector3(HALF_TILE_SIZE, -HALF_TILE_SIZE, HALF_TILE_SIZE);

        faceNormals[4] = Vector3.right;

        //left
        faceVectorsNormal[20] = new Vector3(-HALF_TILE_SIZE, -HALF_TILE_SIZE, HALF_TILE_SIZE);
        faceVectorsNormal[21] = new Vector3(-HALF_TILE_SIZE, HALF_TILE_SIZE, HALF_TILE_SIZE);
        faceVectorsNormal[22] = new Vector3(-HALF_TILE_SIZE, HALF_TILE_SIZE, -HALF_TILE_SIZE);
        faceVectorsNormal[23] = new Vector3(-HALF_TILE_SIZE, -HALF_TILE_SIZE, -HALF_TILE_SIZE);

        faceNormals[5] = Vector3.left;

        for (int i = 0; i < 6; i++)
            faceNormalsTile[i] = new TilePosition((int)faceNormals[i].x, (int)faceNormals[i].y, (int)faceNormals[i].z);
    }
	
	static public Vector3 GetCubeCenter(int sizeX, int sizeY, int sizeZ)
	{
		return new Vector3(sizeX * TILE_SIZE * 0.5f - HALF_TILE_SIZE, sizeY * TILE_SIZE * 0.5f - HALF_TILE_SIZE, sizeZ * TILE_SIZE * 0.5f - HALF_TILE_SIZE);
	}
}

