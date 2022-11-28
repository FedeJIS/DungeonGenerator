using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonCreator : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private GameObject[] tiles;
    [SerializeField] private GameObject selectedTile;
    
    public void SelecTile(int tileId)
    {
        selectedTile = tiles[tileId];
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            CreateTile();
        }
    }

    public void CreateTile()
    {
        if (selectedTile)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 2.0f; // we want 2m away from the camera position
            Vector3 objectPos = camera.ScreenToWorldPoint(mousePos);
            Instantiate(selectedTile, objectPos, Quaternion.identity);
        }
    }
}
