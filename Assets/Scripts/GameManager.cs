using System;
using System.Collections.Generic;
using UnityEngine;

public struct pos
{
    public float x, z;

    public pos(float x, float z)
    {
        this.x = x;
        this.z = z;
    }
}

public struct Coordinate
{
    public int i, j;

    public Coordinate(int i, int j)
    {
        this.i = i;
        this.j = j;
    }
}

public struct PieceSelected
{
    public ChessPiece selectedPiece;
    public int i, j;

    public PieceSelected(ChessPiece piece, int i, int j)
    {
        selectedPiece = piece;
        this.i = i;
        this.j = j;
    }
}

public class GameManager : MonoBehaviour
{
    private GameObject[,] tiles = new GameObject[8, 8];
    public GameObject[] chessPieces = new GameObject[32];
    public static float tileA1x = 3.24f, tileA1z = 3.24f;
    public static float pieceA1x = 3.77f, pieceA1z = 3.77f;

    public GameObject blackTilePrefab, whiteTilePrefab;

    public GameObject whitePawnPrefab, blackPawnPrefab;
    public GameObject whiteKingPrefab, blackKingPrefab;
    public GameObject whiteQueenPrefab, blackQueenPrefab;
    public GameObject whiteKnightPrefab, blackKnightPrefab;
    public GameObject whiteBishopPrefab, blackBishopPrefab;
    public GameObject whiteRookPrefab, blackRookPrefab;
    private ChessLogic logic;
    
    public PieceSelected? pieceSelected;
    
    public static pos returnPos(int i, int j)
    {
        return new pos(pieceA1x - i * Tile.tileWidth, pieceA1z - j * Tile.tileWidth);
    }

    public static Coordinate ReturnCoordinate(float x, float z)
    {
        return new Coordinate((int)Math.Round((pieceA1x - x) / Tile.tileWidth),
            (int)Math.Round((pieceA1z - z) / Tile.tileWidth));
    }


    void Start()
    {
        logic = ChessLogic.GetInstance();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if ((i + j) % 2 == 0)
                    tiles[i, j] = Instantiate(blackTilePrefab,
                        new Vector3(tileA1x - i * Tile.tileWidth, 0, tileA1z - j * Tile.tileWidth),
                        Quaternion.identity);
                else
                    tiles[i, j] = Instantiate(whiteTilePrefab,
                        new Vector3(tileA1x - i * Tile.tileWidth, 0, tileA1z - j * Tile.tileWidth),
                        Quaternion.identity);
            }
        }

        for (int j = 0; j < 8; j++)
        {
            chessPieces[j] = Instantiate(whitePawnPrefab,
                new Vector3(returnPos(j, 1).x, Tile.tileHeight, returnPos(j, 1).z), Quaternion.identity);
            chessPieces[j + 8] = Instantiate(blackPawnPrefab,
                new Vector3(returnPos(j, 6).x, Tile.tileHeight, returnPos(j, 6).z), Quaternion.identity);
        }

        chessPieces[16] = Instantiate(whiteRookPrefab,
            new Vector3(returnPos(0, 0).x, Tile.tileHeight, returnPos(0, 0).z), Quaternion.identity);
        chessPieces[17] = Instantiate(whiteRookPrefab,
            new Vector3(returnPos(7, 0).x, Tile.tileHeight, returnPos(7, 0).z), Quaternion.identity);
        chessPieces[18] = Instantiate(blackRookPrefab,
            new Vector3(returnPos(0, 7).x, Tile.tileHeight, returnPos(0, 7).z), Quaternion.identity);
        chessPieces[19] = Instantiate(blackRookPrefab,
            new Vector3(returnPos(7, 7).x, Tile.tileHeight, returnPos(7, 7).z), Quaternion.identity);

        chessPieces[20] = Instantiate(whiteKnightPrefab,
            new Vector3(returnPos(1, 0).x, Tile.tileHeight, returnPos(1, 0).z), Quaternion.identity);
        chessPieces[21] = Instantiate(whiteKnightPrefab,
            new Vector3(returnPos(6, 0).x, Tile.tileHeight, returnPos(6, 0).z), Quaternion.identity);
        chessPieces[20].transform.localScale = new Vector3(chessPieces[20].transform.localScale.x,
            chessPieces[20].transform.localScale.y, chessPieces[20].transform.localScale.z * -1);
        chessPieces[21].transform.localScale = new Vector3(chessPieces[21].transform.localScale.x,
            chessPieces[21].transform.localScale.y, chessPieces[21].transform.localScale.z * -1);

        chessPieces[22] = Instantiate(blackKnightPrefab,
            new Vector3(returnPos(1, 7).x, Tile.tileHeight, returnPos(1, 7).z), Quaternion.identity);
        chessPieces[23] = Instantiate(blackKnightPrefab,
            new Vector3(returnPos(6, 7).x, Tile.tileHeight, returnPos(6, 7).z), Quaternion.identity);

        chessPieces[24] = Instantiate(whiteBishopPrefab,
            new Vector3(returnPos(2, 0).x, Tile.tileHeight, returnPos(2, 0).z), Quaternion.identity);
        chessPieces[25] = Instantiate(whiteBishopPrefab,
            new Vector3(returnPos(5, 0).x, Tile.tileHeight, returnPos(5, 0).z), Quaternion.identity);
        chessPieces[26] = Instantiate(blackBishopPrefab,
            new Vector3(returnPos(2, 7).x, Tile.tileHeight, returnPos(2, 7).z), Quaternion.identity);
        chessPieces[27] = Instantiate(blackBishopPrefab,
            new Vector3(returnPos(5, 7).x, Tile.tileHeight, returnPos(5, 7).z), Quaternion.identity);

        chessPieces[28] = Instantiate(whiteQueenPrefab,
            new Vector3(returnPos(3, 0).x, Tile.tileHeight, returnPos(3, 0).z), Quaternion.identity);
        chessPieces[29] = Instantiate(blackQueenPrefab,
            new Vector3(returnPos(3, 7).x, Tile.tileHeight, returnPos(3, 7).z), Quaternion.identity);

        chessPieces[30] = Instantiate(whiteKingPrefab,
            new Vector3(returnPos(4, 0).x, Tile.tileHeight, returnPos(4, 0).z), Quaternion.identity);
        chessPieces[31] = Instantiate(blackKingPrefab,
            new Vector3(returnPos(4, 7).x, Tile.tileHeight, returnPos(4, 7).z), Quaternion.identity);
    }

    public void SelectPiece(ChessPiece chessPiece, int i, int j)
    {
        unselectAll();
        pieceSelected = new PieceSelected(chessPiece, i, j);
        tiles[i, j].GetComponent<Tile>().ShowImageOnTile1();
        List<ChessPosition> valid = logic.GetPossibleMoves(new ChessPosition(j, i));
        
        foreach (var t in valid)
        {
            if(logic.GetPiece(new ChessPosition(t.Row, t.Column)).Type != PieceType.None)
                tiles[t.Column, t.Row].GetComponent<Tile>().ShowImageOnTile3();
            else
                tiles[t.Column, t.Row].GetComponent<Tile>().ShowImageOnTile2();
        }

    }

    public void unselectAll()
    {
        if (pieceSelected != null)
        {
            for (int m = 0; m < 8; m++)
            {
                for (int e = 0; e < 8; e++)
                {
                    tiles[m, e].GetComponent<Tile>().ResetTileMaterial();
                }
            }

            pieceSelected = null;
        }
    }
    
    public void MovePiece(int i, int j)
    {
        ((PieceSelected)pieceSelected).selectedPiece.transform.position = new Vector3(returnPos(i, j).x, Tile.tileHeight, returnPos(i, j).z);
    }
}