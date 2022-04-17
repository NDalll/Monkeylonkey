using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StageBuilder : MonoBehaviour//Dette er det script der s�rge for at der bliver generet en bane som er muglig gennemf�re og at det er tilf�dige lootrooms
{
    private char[] directions = new char[] { 'L', 'L', 'L', 'L', 'L','R', 'R', 'R', 'R', 'R', 'T', 'T', };//en liste at muglige retninger, l�g m�rke til at det ikke er muglig at lave en room ned af, og at der er flere R/L end T da det �ger chancen for at der g�es til siden end op n�r der bliver valgt en retning

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

    private char? formerRoom = null; //denne er null og bliver sat til null, da vi gerne vil kunne samlinge med til at starte med, uden at vi har defineret en specifik v�rdig p� den

    void Start()
    {
        startX = Random.Range(1, sizeX + 1);//Starter med at s�tte en tilf�dig start placering p� x asken i vores bane grid
        startY = 0;//s�tte y til 0 da man altid starter i bunden
        curX = startX;//gemmer vores startv�rdiger som cur koordinater
        curY = startY;
        mainPath = GenerateMainPath();//kalder mainparth funktionen som retunere mainpathen som en string om gemmer den som mainPath 
        Debug.Log("startPos: " + startX + ", " + startY + ": " + mainPath);//printer mainpathen med start positionen
        roomTemplate = CreateTemplate();//Generet en template for helerummet ud fra den fundende path
        Debug.Log(roomTemplate);//printer roomtemplaten
        BuildRooms();//byg rummende
        SetLootRooms();//generer lootrooms
        FillVoid();//udfylder de resetende rum
    }

    private string GenerateMainPath()//funktion for generatePath der skal returnere en string
    {
        string path = "F";//starte med en path som bare hedder "F" da det s� for at du er i det starter rummet
        bool end = false;//boolean for hold styr p� om man har n�et en ende p� banen
        while (!end)//while loop det k�re indtil at man end er blevet true
        {
            char nexDir = directions[Random.Range(0, directions.Length)];//udv�gler en tilf�ldig reting 
            switch (nexDir)//tjekker om det er mugligt at g� i den rening
            {
                case 'L':
                    if (curX != 1 && formerRoom != 'L')//tjekker at man ikke er n�et gr�nsen for at g� til venstre, og at det tidligere rum ikke er til venster
                    {
                        //hvis kan kan g� til venstre s� tr�kke vi en fra curX postioen og s�tter at det tidligere rum er til h�jre, og derefter tilf�jer L til pathen
                        curX--;
                        formerRoom = 'R';
                        path += "L";
                    }
                    break;
                case 'R':
                    if (curX != sizeX && formerRoom != 'R')//samme som ved L bare omvendt
                    {
                        curX++;
                        formerRoom = 'L';
                        path += "R";
                    }
                    break;
                case 'T':
                    if(curY != sizeY-1)//tjekker at man ikke er n�et toppen(vi beh�ves ikke at tjekke om der er et rum der da den kun er muligt at g� op ikke ned i forhold til rum sti)
                    {
                        curY++;//g�r en op p� curY
                        formerRoom = 'B';//sider at det tidligere rum er B
                        path += "T";//tilf�jer T til pathen
                    }
                    else
                    {
                        //hvis man er n�et toppen s� er end true, aka while loopet stopper
                        end = true;
                    }
                    break;
            }
            
        }
        return path;//retunere den generet path
    }
    private string CreateTemplate()
    {
        string template = "";//opretter en tom string til templaten
        int layer = 0;
        int xpos = startX;
        for(int i = 0; i < sizeY*sizeX; i++)//starter med at udfylde hele listen "0", hvor m�ngden af dem er antallet af der er iforhold til vores stor vi har sat griddet til at v�re
        {
            template += "0";
        }
        //kalder p� stringfunktionen der starter erstatter et char i en specifiveret sting med et bestem char, ved p� en speciferet plads(index) i stringen  
        template = replaceCharInString(template, '1', xpos);//her erstartter vi s� plads xpos i stringen med '1', som betyder der er et rumm der
       
        
        foreach (char x in mainPath)//k�re igennem alle char i mainpath(obs dette er pathen, IKKE templaten vi er igang med at lave) stringen
        {
            switch (x)
            {
                case 'L'://hvis den ser en L s� rykker den xpos en ned(til h�jre) og erstratter '0' med '1' iforhold til indexet der passer med hvilket xposition og lag man er ved
                    xpos--;
                    template = replaceCharInString(template, '1', (xpos + sizeX * layer));//l�g m�rke til at her bliver indexet i stringen udregnet med xpos og laget vi er ved 
                    break;
                case 'R'://hvis dem ser en R s� rykke man til h�jre
                    xpos++;
                    template = replaceCharInString(template, '1', (xpos + sizeX * layer));
                    break;
                case 'T'://Hvis den ser en T, s� rykke man et lag op
                    layer++;
                    template = replaceCharInString(template, '1', (xpos + sizeX * layer));
                    break;
            }
        }
        return template;//returene templaten
    }

    private void BuildRooms()
    {
        //s�tter koordinater for at build rummende ud fra, jeg tr�kker en fra x for at tage h�jde for at tage h�jde for index start ved rykke griddet 1 til ventre, s� den passe med det rum vi har sat af i scene for levelgeration
        int currX = startX-1;
        int currY = startY;
        for (int i = 0; i < mainPath.Length; i++)//for loop der k�re igennem mainpathen(den med bogstaver ikke tal)
        {
            List<char> roomCon = new List<char>();//vi opretter en liste char, som kommer til at indhold condistions iforhold til hvilke �bninger vores rum skal have
            if (mainPath[i] == 'F')//vi tjekker for om vi er ved �bningsrummet 
            {
                //hvis vi er det s� skal rumme have en �bning i bunden og en �bning i forhorhold til hvor den n�ste retning er
                roomCon.Add('B');
                roomCon.Add(mainPath[i + 1]);
            }
            else
            {
                //vi starter med at tilf�je condition for man skal kunne komme ind i rummet
                roomCon.Add(GetCondition(mainPath[i]));

                //conditionen for at man kan komme ud af rumme
                if(i + 1 != mainPath.Length)//tjekker for at man ikke er ved det sidste rum
                {
                    //hvis ikke er ved det sidste rum s�, s� den tilf�je den reting som det n�ste rum er i som condition
                    roomCon.Add(mainPath[i + 1]);
                }
                else
                {
                    //hvis man er ved sidste rum s� skal den have en �bning i toppen
                    roomCon.Add('T');
                }
                
                //opdatere positionen i griddet rummet er ved
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

            List<GameObject> goodRooms = GetGoodRooms(roomCon);//henter prefabs for de rum som passe p� placeringen
            GameObject room = Instantiate(goodRooms[Random.Range(0, goodRooms.Count - 1)]);//instantiere et tilf�ldigt rum af de gode rum;
            room.transform.position = new Vector3((currX * roomSizeX) + (roomSizeX/2), (currY * roomSizeY) + (roomSizeY/2)+yOffset, 0);//her udregner den positionen for rummet i worldspace i scenen
            //Dette bliver gjort ved at vi gange x/y posotionen i griddet med rumst�rrelsen i x/y aksen, og derfter l�gger halvdenlen af rumst�rrelsen til da origo punket p� rummet er i midten af det   
        }
    }
    private List<GameObject> GetGoodRooms(List<char> con)//funktion der henter det "goderum"
    {
        List<GameObject> gRooms = new List<GameObject>();//opretter list af gameobjekts til at retunrere
        foreach(GameObject x in rooms)//den k�re igennem af prefabs af rum vi har
        {
            if (x.name.Contains(con[0].ToString()) && x.name.Contains(con[1].ToString()))//tjekker om rummet man er ved indholder begge conditions.
            {
                gRooms.Add(x);
            }
        }
        return gRooms;
    }
    private char GetCondition(char conRoom)//denne g�r s�dan at rummet har den rigtige ingang, da hvis man f.eks. kom ind i rummet ved at g� til venstere, skal rummet have en �bning til h�jre
    {
        switch (conRoom)
        {
            case 'R':
                return 'L';
            case 'L':
                return 'R';
            case 'T':
                return 'B';
            default://s�ger bare for at der altid vil blive retuneret noget, s� funktion er gyldig, hvis alt fungere retunere man aldrig ' '
                return ' ';
        }
    }
    private string replaceCharInString(string str, char ch, int pos)//funktion der tager en string(str) ind og udskrifter et char ved index position(pos), med et nyt char(ch) 
    {
        char[] charTemp = str.ToCharArray();//vi starter med at lave stringen til et array
        charTemp[pos-1] = ch;//derefter s�tter den char med indexet positon - 1, for at tageh�jde for at index i liste starter fra 0
        return new string(charTemp);//retunere chararrayet som string
    }

    private void SetLootRooms()//s�tter lootrooms i templaten
    {
        int LootCount = 0;//holder styr p� hvor mange lootroom der er, da der maks m� v�re 2
        for(int i = 0; i < roomTemplate.Length; i++)//k�re igennem roomtemplaten
        {
            if(roomTemplate[i] == '0')//hvis at den finde et '0'(det ikke er en del af pathen)
            {
                int rand = Random.Range(1, 101);//generet til tilf�digt tal mellem 1 og 100
                if(rand <= lootChance && LootCount < MaxLoot)//hvis rand er under lootchancen og at max rooms endnu ikke er n�et
                {
                    roomTemplate =  replaceCharInString(roomTemplate, '2', i+1);//inds�t rummet i roomtemplaten
                    LootCount++;
                }

            }
        }
    }

    private void FillVoid()//udfylder alt det der ikke er en del af pathen
    {
        for(int i = 0; i < roomTemplate.Length; i++)//k�re igennm roomteplate igen
        {
            if(roomTemplate[i] == '0')//hvis det ser et nul
            {
                GameObject room = Instantiate(rooms[Random.Range(0,rooms.Count)]);//instantiere et tilf�ldigt rum, uden noge specifikke krav
                room.transform.position = new Vector3((i % sizeX) * roomSizeX + (roomSizeX / 2), (Mathf.Floor(i / sizeX)) * roomSizeY + (roomSizeY / 2) + yOffset, 0);//udregner positionen for rummet i worldspace ud fra hvilket position den har i roomplaten
                //x findes ved at man f�rst at finde grid placeirngen ved at sige i%sizeX som er divisions restproduktet. Derfter finder man s� worldspace placeringen som tidligere
                //y findes ved at man f�rst at finde grid placeirngen ved at sige i/sizeY nedrundet. Derfter finder man s� worldspace placeringen som tidligere 
            }
            if (roomTemplate[i] == '2')//samme som hvis det er et nul, ved 2 instatiere den bare et lootroom istedet
            {
                GameObject room = Instantiate(LootRooms[Random.Range(0, LootRooms.Count)]);
                room.transform.position = new Vector3((i % sizeX) * roomSizeX + (roomSizeX / 2), (Mathf.Floor(i / sizeX)) * roomSizeY + (roomSizeY / 2) + yOffset, 0);
            }
        }
    }
}
