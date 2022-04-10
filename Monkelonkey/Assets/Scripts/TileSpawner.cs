using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    public bool isGround;
    public bool isGrass;
    public bool isPlatform;
    public bool isDanger;
    
    public GameObject[] groundTiles;
    public GameObject[] grassTiles;
    public GameObject[] platforms;
    public GameObject[] dangerTiles;

    private GameObject[] chosenTiles;
    // Start is called before the first frame update
    void Start()
    {
        if (isPlatform)
        {
            chosenTiles = platforms;
        }
        else if (isDanger)
        {
            chosenTiles = dangerTiles;
        }
        else if (isGrass)
        {
            chosenTiles = grassTiles;
        }
        else
        {
            chosenTiles = groundTiles;
        } 
        int rand = Random.Range(0, chosenTiles.Length);
        Instantiate (chosenTiles[rand], transform.position, Quaternion.identity);
        Destroy (gameObject);
    }
}
