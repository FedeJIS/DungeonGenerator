using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class SimpleWorldGenerator : MonoBehaviour
{
    [SerializeField] private DungeonPrefabs prefabs;
    [SerializeField] private int amount;
    [SerializeField] private int blocks;
    private Cardinals nextDirection;
    private Cardinals prevDirection;
    private Room[] rooms;
    private GameObject dungeonRoot;
    private Vector3 currentPosition = Vector3.zero;


    private void Start()
    {
        GenerateDungeon();
    }

    private void GenerateDungeon()
    {
        rooms = new Room[amount];
        prevDirection = GetNextDirection();
        dungeonRoot = new GameObject()
        {
            name = "Dungeon"
        };
        
        for (int i = 0; i < rooms.Length; i++)
        {   
            var root = new GameObject
            {
                name = "Room_"+i
            };
            root.transform.position = currentPosition;
            root.transform.parent = dungeonRoot.transform;
            
            nextDirection = GetNextDirection();
            while (nextDirection.ToGoDirection == prevDirection.CurrentDirection)
            {
                nextDirection = GetNextDirection();
            }

            rooms[i] = new Room(root, prefabs,blocks, nextDirection.ToGoDirection);
            
            SetNextPosition(nextDirection);
            
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Restart();
        }
    }

    private void Restart()
    {
        currentPosition = Vector3.zero;
        Destroy(dungeonRoot.gameObject);
        GenerateDungeon();
    }

    private Cardinals GetNextDirection()
    {
        float right = Random.Range(0f, 1f);
        float up = Random.Range(0f, 1f);
        float down = Random.Range(0f, 1f);
        float left = Random.Range(0f, 1f);

        if (right > left && right > up && right > down) return new Cardinals(Directions.Right, Directions.Left);
        if (left > right && left > up && left > down) return new Cardinals(Directions.Left, Directions.Right);
        if (up > right && up > left && up > down) return new Cardinals(Directions.Up, Directions.Down); 
        return new Cardinals(Directions.Down, Directions.Up);
    }
    private void SetNextPosition(Cardinals lastDirection)
    {
        var rightVector = new Vector3(prefabs.floorPrefab.gameObject.GetComponent<SpriteRenderer>().size.x*(blocks+1), 0, 0);
        var upVector = new Vector3(0, prefabs.floorPrefab.gameObject.GetComponent<SpriteRenderer>().size.y*(blocks+1), 0);
        var leftVector = new Vector3(-prefabs.floorPrefab.gameObject.GetComponent<SpriteRenderer>().size.x*(blocks+1), 0, 0);
        var downVector = new Vector3(0, -prefabs.floorPrefab.gameObject.GetComponent<SpriteRenderer>().size.y*(blocks+1), 0);

        switch (lastDirection.ToGoDirection)
        {
            case Directions.Up:
                currentPosition += upVector;
                break;
            case Directions.Down:
                currentPosition += downVector;
                break;
            case Directions.Right:
                currentPosition += rightVector;
                break;
            case Directions.Left:
                currentPosition += leftVector;
                break;
        }

    }

    
    private class Room
    {
        
        private GameObject[,] mainRoom;
        private Vector3 xBlockSize;
        private Vector3 yBlockSize;
        private Vector3 currentPosition = Vector3.zero;
        public Vector3 lastPosition;
        private bool determined;

        public Room(GameObject root, DungeonPrefabs prefabs, int blocks, Directions toGoDirection)
        {
            GenerateRoom(root, prefabs,blocks, toGoDirection);
        }
        public void GenerateRoom(GameObject root, DungeonPrefabs prefabs, int blocks, Directions toGoDirection)
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
                    
                    if (toGoDirection == Directions.Left && j == 0 && !determined)
                    {
                        determined = Random.Range(0f, 1f) >= 1 - 1 / (float)blocks;
                        lastPosition = currentPosition - xBlockSize*2;
                    }
                    if (toGoDirection == Directions.Right && j == blocks - 1 && !determined)
                    {
                        determined = Random.Range(0f, 1f) >= 1 - 1 / (float)blocks;
                        lastPosition = currentPosition;
                    }
                    
                    if (toGoDirection == Directions.Up && i == 0 && !determined)
                    {
                        determined = Random.Range(0f, 1f) >= 1 - 1 / (float)blocks;
                        lastPosition = currentPosition - yBlockSize - xBlockSize;
                    }
                    
                    if (toGoDirection == Directions.Down && i == blocks - 1 && !determined)
                    {
                        determined = Random.Range(0f, 1f) >= 1 - 1 / (float)blocks;
                        lastPosition = currentPosition + yBlockSize - xBlockSize;
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

    public struct Cardinals
    {
        public Directions ToGoDirection;
        public Directions CurrentDirection;
        
        public Cardinals(Directions toGoDirection, Directions avoidDirection)
        {
            ToGoDirection = toGoDirection;
            CurrentDirection = avoidDirection;
        }
        
    }
    
    public enum Directions
    {
        Up,
        Down,
        Right,
        Left
    }
    
}
