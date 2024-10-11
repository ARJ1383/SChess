using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Tile : MonoBehaviour
{
    public static float tileWidth = 1.08f, tileHeight = 0.3f;

    private bool canGo = false;

    private Material originalMaterial;

    private int i, j;
    // Start is called before the first frame update


    public Material imageMaterial; // Assign the material with your image texture
    public Material imageMaterial2; // Assign the material with your image texture
    public Material imageMaterial3; // Assign the material with your image texture

    private Renderer tileRenderer; // Reference to the tile's renderer
    public static GameManager gameManager;

    void Start()
    {
        // Get the Renderer component of the tile
        tileRenderer = GetComponent<Renderer>();
        originalMaterial = tileRenderer.material;
        i = (int)Math.Round((GameManager.tileA1x - this.transform.position.x) / tileWidth);
        j = (int)Math.Round((GameManager.tileA1z - this.transform.position.z) / tileWidth);
    }

    // Call this function to display the image on the tile
    public void ShowImageOnTile1()
    {
        if (tileRenderer != null && imageMaterial != null)
        {
            tileRenderer.material = imageMaterial; // Apply the new material with the image
        }
        
    }

    public void ShowImageOnTile2()
    {
        canGo = true;
        if (tileRenderer != null && imageMaterial2 != null)
        {
            tileRenderer.material = imageMaterial2; // Apply the new material with the image
        }
    }

    public void ShowImageOnTile3()
    {
        canGo = true;
        if (tileRenderer != null && imageMaterial3 != null)
        {
            tileRenderer.material = imageMaterial3; // Apply the new material with the image
        }
    }

    // Optional: Call this to reset the material back to the original
    public void ResetTileMaterial()
    {
        if (tileRenderer != null && originalMaterial != null)
        {
            tileRenderer.material = originalMaterial; // Reset to the original material
        }

        canGo = false;
    }

    void OnMouseDown()
    {
        if (canGo)
        {
            if (tileRenderer.material == imageMaterial3) {
                gameManager.attack(i,j);
                return;
            } 
            ChessLogic.GetInstance()
                .MovePiece(
                    new ChessPosition(((PieceSelected)(gameManager.pieceSelected)).j,
                        ((PieceSelected)(gameManager.pieceSelected)).i), new ChessPosition(j, i));
            ChessLogic.GetInstance().PrintBoard();
            ChessLogic.GetInstance().ChangeTurn();
            gameManager.MovePiece(i, j);
        }

        gameManager.unselectAll();
    }
}