using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator
{
    public string seed = "Eee";
    public bool useRandomSeed = true;

    [Range(0,100)]
    public int randFillPercent;
    int width;
    int height;
    public int[,]  map;


    public MapGenerator(int width, int height){
        this.width = width;
        this.height = height;
        map = new int[width, height];
        RandomFillMap();
    }

    void RandomFillMap(){
        if(useRandomSeed){
            seed = Time.time.ToString();
        }
        //Pseudo random
        System.Random prng = new System.Random(seed.GetHashCode());
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x,y] = prng.Next(0,100) < randFillPercent ? 1 : 0;
            }
        }
    }
}
