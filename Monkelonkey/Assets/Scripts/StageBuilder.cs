using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBuilder : MonoBehaviour
{
    private char[] directions = new char[] { 'L', 'L', 'R', 'R', 'T' };

    public List<GameObject> rooms;
    public int sizeX;
    public int sizeY;

    private string mainPath;

    private int startX;
    private int startY;

    private int curX;
    private int curY;

    private int roomSizeX = 10;
    private int roomSizeY = 30;

    private char? formerDir = null;
    // Start is called before the first frame update
    void Start()
    {
        startX = Random.Range(0, sizeX + 1);
        startY = 0;
        curX = startX;
        curY = startY;
        mainPath = GenerateMainPath();
        Debug.Log(mainPath);
    }

    private string GenerateMainPath()
    {
        string path = "";
        bool end = false;
        while (!end)
        {
            char nexDir = directions[Random.Range(0, directions.Length)];
            switch (nexDir)
            {
                case 'L':
                    if (curX != sizeX && formerDir != 'L')
                    {
                        curX--;
                        formerDir = 'L';
                        path += "L";
                    }
                    break;
                case 'R':
                    if (curX != sizeX && formerDir != 'R')
                    {
                        curX++;
                        formerDir = 'R';
                        path += "R";
                    }
                    break;
                case 'T':
                    if(curY != sizeY)
                    {
                        curY++;
                        formerDir = 'T';
                        path += "T";
                    }
                    else
                    {
                        end = true;
                    }
                    break;
            }
        }
        Debug.Log(path);
        return path;

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
