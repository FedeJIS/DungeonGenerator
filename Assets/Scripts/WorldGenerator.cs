using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private DungeonPrefabs prefabs;
    [SerializeField] private int amount;
    [SerializeField] private int blocks;
    private Room[] rooms;
    
    private Vector3 currentPosition = Vector3.zero;


    private void Start()
    {
        rooms = new Room[amount];
       
        for (int i = 0; i < rooms.Length; i++)
        {   
            var root = new GameObject
            {
                name = "Parent"
            };
            root.transform.position = currentPosition;
            
            float right = Random.Range(0f, 1f);
            float up = Random.Range(0f, 1f);
            
            rooms[i] = new Room(root, prefabs,blocks, right > up);
            
            SetNextPosition(right > up);
            
            if (i < rooms.Length - 1)
            {
                GameObject road = Instantiate(prefabs.floorPrefab, root.transform);
                road.transform.localPosition = rooms[i].lastPosition;
            }
            else
            {
                GameObject final = Instantiate(prefabs.finalRoomPrefab, root.transform);
                final.transform.localPosition = rooms[i].lastPosition;
            }

        }
    }
    
    private void SetNextPosition(bool isRight)
    {
        var rightVector = new Vector3(prefabs.floorPrefab.gameObject.GetComponent<SpriteRenderer>().size.x*(blocks+1), 0, 0);
        var upVector = new Vector3(0, prefabs.floorPrefab.gameObject.GetComponent<SpriteRenderer>().size.y*(blocks+1), 0);
        
        if (isRight)
        {
            currentPosition += rightVector;
            return;
        }
        
        currentPosition += upVector;
        
        

    }

    
    private class Room
    {
        
        private GameObject[,] mainRoom;
        private Vector3 xBlockSize;
        private Vector3 yBlockSize;
        private Vector3 currentPosition = Vector3.zero;
        public Vector3 lastPosition;
        private bool determined;

        public Room(GameObject root, DungeonPrefabs prefabs, int blocks, bool isRight)
        {
            GenerateRoom(root, prefabs,blocks, isRight);
        }
        public void GenerateRoom(GameObject root, DungeonPrefabs prefabs, int blocks, bool isRight)
        {
            xBlockSize = new Vector3(prefabs.floorPrefab.gameObject.GetComponent<SpriteRenderer>().size.x, 0, 0);
            yBlockSize = new Vector3(0, -prefabs.floorPrefab.gameObject.GetComponent<SpriteRenderer>().size.y, 0);
            mainRoom = new GameObject[blocks,blocks];
            
            for (int i = 0; i < blocks; i++)
            {
                for (int j = 0; j < blocks; j++)
                {
                    
                    mainRoom[i,j] = Instantiate(prefabs.floorPrefab,root.transform);
                    mainRoom[i, j].transform.localPosition = currentPosition;
                    
                    bool populated = Random.Range(0f, 1f) >= 0.75f;

                    if (populated)
                    {
                        bool isItem = Random.Range(0f, 1f) >= 0.40f;
                        GameObject toInstantiate = isItem ? prefabs.itemPrefab : prefabs.enemyPrefab;
                        GameObject item = Instantiate(toInstantiate,root.transform);
                        item.transform.localPosition = currentPosition;
                    }
                    
                    currentPosition += xBlockSize;
                    if (isRight && j == blocks - 1 && !determined)
                    {
                        determined = Random.Range(0f, 1f) >= 1 - 1 / (float)blocks;
                        lastPosition = currentPosition;
                    }
                    
                    if (!isRight && i == 0 && !determined)
                    {
                        determined = Random.Range(0f, 1f) >= 1 - 1 / (float)blocks;
                        lastPosition = currentPosition - yBlockSize - xBlockSize;
                    }
                }

                currentPosition.x = 0;
                currentPosition += yBlockSize;
                
            }
        }
    }

    [Serializable]
    public struct DungeonPrefabs
    {
        public GameObject floorPrefab;
        public GameObject itemPrefab;
        public GameObject enemyPrefab;
        public GameObject finalRoomPrefab;
    }
    
}
