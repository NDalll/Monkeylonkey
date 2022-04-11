using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBuilder : MonoBehaviour
{
    private char[] directions = new char[] { 'L', 'L', 'L', 'L', 'L','R', 'R', 'R', 'R', 'R', 'T', 'T', };

    public List<GameObject> rooms;
    public int sizeX;
    public int sizeY;
    public int yOffset;

    private string mainPath;
    private string roomTemplate;

    private int startX;
    private int startY;

    private int curX;
    private int curY;

    private int roomSizeX = 20;
    private int roomSizeY = 30;

    private char? formerDir = null;

    void Start()
    {
        startX = Random.Range(1, sizeX + 1);
        startY = 0;
        curX = startX;
        curY = startY;
        mainPath = GenerateMainPath();
        Debug.Log("startPos: " + startX + ", " + startY + ": " + mainPath);
        roomTemplate = CreateTemplate();
        Debug.Log(roomTemplate);
        BuildRooms();
        
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
                    if (curX != 1 && formerDir != 'L')
                    {
                        curX--;
                        formerDir = 'R';
                        path += "L";
                    }
                    break;
                case 'R':
                    if (curX != sizeX && formerDir != 'R')
                    {
                        curX++;
                        formerDir = 'L';
                        path += "R";
                    }
                    break;
                case 'T':
                    if(curY != sizeY-1)
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
        return path;
    }
    private string CreateTemplate()
    {
        string template = "";
        int layer = 0;
        int xpos = startX;
        for(int i = 0; i < sizeY*sizeX; i++)
        {
            template += "0";
        }
        
        template = replaceCharInString(template, '1', xpos);
        foreach (char x in mainPath)
        {
            switch (x)
            {
                case 'L':
                    xpos--;
                    template = replaceCharInString(template, '1', (xpos + sizeX * layer));
                    break;
                case 'R':
                    xpos++;
                    template = replaceCharInString(template, '1', (xpos + sizeX * layer));
                    break;
                case 'T':
                    layer++;
                    template = replaceCharInString(template, '1', (xpos + sizeX * layer));
                    break;
            }
        }
        return template;
    }

    private void BuildRooms()
    {
        int currRoom = 0;
        char[] charRoomTemplate = roomTemplate.ToCharArray();
        
        
        for(int i = 0; i < roomTemplate.Length; i++)
        {
            if(charRoomTemplate[i] == '1')
            {
                List<char> conditions = new List<char>();
                bool first = false;
                if (currRoom == 0)
                {
                    first = true;
                }
                if (first)
                {
                    conditions.Add('B');
                    conditions.Add(mainPath[currRoom]);
                }
                else
                {
                    conditions.Add(GetCondition(mainPath[currRoom - 1]));
                    conditions.Add(mainPath[currRoom]);
                }
                List<GameObject> goodRooms = GetGoodRooms(conditions);
                int roomIndex = Random.Range(0, goodRooms.Count);
                GameObject room = Instantiate(goodRooms[roomIndex]);
                room.transform.position = new Vector3((i % sizeX)*roomSizeX + 10, (Mathf.Floor(i / sizeX))*roomSizeY + 17, 0);
                currRoom++;

            }
        }
    }
    private List<GameObject> GetGoodRooms(List<char> con)
    {
        List<GameObject> gRooms = new List<GameObject>();
        foreach(GameObject x in rooms)
        {
            if (x.name.Contains(con[0].ToString()) && x.name.Contains(con[1].ToString()))
            {
                gRooms.Add(x);
            }
            
        }
        return gRooms;
    }
    private char GetCondition(char conRoom)
    {
        switch (conRoom)
        {
            case 'R':
                return 'L';
            case 'L':
                return 'R';
            case 'T':
                return 'B';
            default:return ' ';
        }
    }
    private string replaceCharInString(string str, char ch, int pos)
    {
        char[] charTemp = str.ToCharArray();
        charTemp[pos-1] = ch;
        return new string(charTemp);
    }

    void Update()
    {
        
    }
}
