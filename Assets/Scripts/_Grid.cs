using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _Grid : MonoBehaviour
{

    private Transform[,] grid;
    private Tile[,] tile;

    public float cellWidth;
    public int gridSize;


    // Start is called before the first frame update
    void Start()
    {
        grid = new Transform[gridSize, gridSize];
        tile = new Tile[gridSize, gridSize];

        for(int i = 0; i < gridSize; i++)
        {
            for(int j = 0; j < gridSize; j++)
            {
                grid[i, j].localPosition = new Vector3(i * cellWidth, 0.0f, j * cellWidth);



                tile[i, j].transform.position = grid[i, j].localPosition;
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
