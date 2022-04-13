using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//room.transform.position = new Vector3((i % sizeX) * roomSizeX + 10, (Mathf.Floor(i / sizeX)) * roomSizeY + 17, 0);
public class StageBuilder : MonoBehaviour
{
    private char[] directions = new char[] { 'L', 'L', 'L', 'L', 'L','R', 'R', 'R', 'R', 'R', 'T', 'T', };

    public List<GameObject> rooms;
    public List<GameObject> LootRooms;
    public int lootChance;
    public int MaxLoot;
    public int sizeX;
    public int sizeY;
    public int yOffset;

    private string mainPath;
    private string roomTemplate;

    private int startX;
    private int startY;

    private int curX;
    private int curY;

    public int roomSizeX;
    public int roomSizeY;

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
        SetLootRooms();
        FillVoid();
        
        ;
        
    }

    private string GenerateMainPath()
    {
        string path = "F";
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
        List<GameObject> goodRooms;
        int currX = startX-1;
        int currY = startY;
        for (int i = 0; i < mainPath.Length; i++)
        {
            List<char> roomCon = new List<char>();
            if (mainPath[i] == 'F')
            {
                roomCon.Add('B');
                roomCon.Add(mainPath[i + 1]);
            }
            else
            {
                roomCon.Add(GetCondition(mainPath[i]));
                if(i + 1 != mainPath.Length)
                {
                    roomCon.Add(mainPath[i + 1]);
                }
                else
                {
                    roomCon.Add('T');
                }   
                switch (mainPath[i])
                {
                    case 'L':
                        currX--;
                        break;
                    case 'R':
                        currX++;
                        break;
                    case 'T':
                        currY++;
                        break;
                }
            }
            goodRooms = GetGoodRooms(roomCon);
            int roomIndex = Random.Range(0, goodRooms.Count-1);
            GameObject room = Instantiate(goodRooms[roomIndex]);
            room.transform.position = new Vector3((currX * roomSizeX) + (roomSizeX/2), (currY * roomSizeY) + (roomSizeY/2)+yOffset, 0);
        }
    }
    private List<GameObject> GetGoodRooms(List<char> con)
    {
        List<GameObject> gRooms = new List<GameObject>();
        foreach(GameObject x in rooms)
        {
            if(con.Count == 2)
            {
                if (x.name.Contains(con[0].ToString()) && x.name.Contains(con[1].ToString()))
                {
                    gRooms.Add(x);
                }
            }
            else
            {
                if (x.name.Contains(con[0].ToString()))
                {
                    gRooms.Add(x);
                }
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

    private void SetLootRooms()
    {
        int LootCount = 0;
        for(int i = 0; i < roomTemplate.Length; i++)
        {
            if(roomTemplate[i] == '0')
            {
                int rand = Random.Range(1, 101);
                if(rand <= lootChance && LootCount < MaxLoot)
                {
                    roomTemplate =  replaceCharInString(roomTemplate, '2', i+1);
                    LootCount++;
                }

            }
        }
    }

    private void FillVoid()
    {
        for(int i = 0; i < roomTemplate.Length; i++)
        {
            if(roomTemplate[i] == '0')
            {
                GameObject room = Instantiate(rooms[Random.Range(0,rooms.Count)]);
                room.transform.position = new Vector3((i % sizeX) * roomSizeX + (roomSizeX / 2), (Mathf.Floor(i / sizeX)) * roomSizeY + (roomSizeY / 2) + yOffset, 0);
            }
            if(roomTemplate[i] == '2')
            {
                GameObject room = Instantiate(LootRooms[Random.Range(0, LootRooms.Count)]);
                room.transform.position = new Vector3((i % sizeX) * roomSizeX + (roomSizeX / 2), (Mathf.Floor(i / sizeX)) * roomSizeY + (roomSizeY / 2) + yOffset, 0);
            }
        }
    }
    void Update()
    {
        
    }
}
