using System;
using System.Collections.Generic;
using NetcodePlus;
using Unity.Netcode;
using UnityEngine;


public class GameManager2 : SNetworkPlayer
{

    private SNetworkActions actions;
    public static GameObject[,] tiles = new GameObject[8, 8];
    public static GameObject[] chessPieces = new GameObject[32];
    public static float tileA1x = 3.24f, tileA1z = 3.24f;
    public static float pieceA1x = 3.77f, pieceA1z = 3.77f;

    public GameObject blackTilePrefab, whiteTilePrefab;

    public GameObject whitePawnPrefab, blackPawnPrefab;
    public GameObject whiteKingPrefab, blackKingPrefab;
    public GameObject whiteQueenPrefab, blackQueenPrefab;
    public GameObject whiteKnightPrefab, blackKnightPrefab;
    public GameObject whiteBishopPrefab, blackBishopPrefab;
    public GameObject whiteRookPrefab, blackRookPrefab;
    public static ChessLogic logic;
   
    public PieceSelected? pieceSelected;
    
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

        int whitePawn = 0, blackPawn = 8, whiteRook = 16, blackRook = 18, whiteKnight = 20, blackKnight = 22, whiteBishop = 24
            , blackBishop = 26, whiteQueen = 28, blackQueen = 29, whiteKing = 30, blackKing = 31;
        Piece[,] board = logic.GetBoard();
        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                switch (board[i, j].Type)
                {
                    case PieceType.Pawn:
                        if (board[i,j].Color == PieceColor.White)
                        {
                            chessPieces[whitePawn++] = Instantiate(whitePawnPrefab,
                                new Vector3(returnPos(j, i).x, Tile.tileHeight, returnPos(j, i).z), Quaternion.identity);
                        }
                        else
                        {
                            chessPieces[blackPawn++] = Instantiate(blackPawnPrefab,
                                new Vector3(returnPos(j, i).x, Tile.tileHeight, returnPos(j, i).z), Quaternion.identity);
                        }
                        break;
                    case PieceType.Rook:
                        if (board[i, j].Color == PieceColor.White)
                        {
                            chessPieces[whiteRook++] = Instantiate(whiteRookPrefab,
                                new Vector3(returnPos(j, i).x, Tile.tileHeight, returnPos(j, i).z), Quaternion.identity);
                        }
                        else
                        {
                            chessPieces[blackRook++] = Instantiate(blackRookPrefab,
                                new Vector3(returnPos(j, i).x, Tile.tileHeight, returnPos(j, i).z), Quaternion.identity);
                        }
                        break;
                    case PieceType.Knight:
                        if (board[i, j].Color == PieceColor.White)
                        {
                            chessPieces[whiteKnight++] = Instantiate(whiteKnightPrefab,
                                new Vector3(returnPos(j, i).x, Tile.tileHeight, returnPos(j, i).z), Quaternion.identity);
                            chessPieces[whiteKnight - 1].transform.localScale = new Vector3(chessPieces[whiteKnight - 1].transform.localScale.x,
                                chessPieces[whiteKnight - 1].transform.localScale.y, chessPieces[whiteKnight - 1].transform.localScale.z * -1);
                        }
                        else
                        {
                            chessPieces[blackKnight++] = Instantiate(blackKnightPrefab,
                               new Vector3(returnPos(j, i).x, Tile.tileHeight, returnPos(j, i).z), Quaternion.identity);
                        }
                        break;
                    case PieceType.Bishop:
                        if (board[i, j].Color == PieceColor.White)
                        {
                            chessPieces[whiteBishop++] = Instantiate(whiteBishopPrefab,
                               new Vector3(returnPos(j, i).x, Tile.tileHeight, returnPos(j, i).z), Quaternion.identity);
                        }
                        else
                        {
                            chessPieces[blackBishop++] = Instantiate(blackBishopPrefab,
                               new Vector3(returnPos(j, i).x, Tile.tileHeight, returnPos(j, i).z), Quaternion.identity);
                        }
                        break;
                    case PieceType.Queen:
                        if (board[i, j].Color == PieceColor.White)
                        {
                            chessPieces[whiteQueen++] = Instantiate(whiteQueenPrefab,
                               new Vector3(returnPos(j, i).x, Tile.tileHeight, returnPos(j, i).z), Quaternion.identity);
                        }
                        else
                        {
                            chessPieces[blackQueen++] = Instantiate(blackQueenPrefab,
                               new Vector3(returnPos(j, i).x, Tile.tileHeight, returnPos(j, i).z), Quaternion.identity);
                        }
                        break;
                    case PieceType.King:
                        if (board[i, j].Color == PieceColor.White)
                        {
                            chessPieces[whiteKing++] = Instantiate(whiteKingPrefab,
                               new Vector3(returnPos(j, i).x, Tile.tileHeight, returnPos(j, i).z), Quaternion.identity);
                        }
                        else
                        {
                            chessPieces[blackKing++] = Instantiate(blackKingPrefab,
                               new Vector3(returnPos(j, i).x, Tile.tileHeight, returnPos(j, i).z), Quaternion.identity);
                        }
                        break;

                }
            }
        }
    }

   public static Coordinate ReturnCoordinate(float x, float z)
    {
        return new Coordinate((int)Math.Round((pieceA1x - x) / Tile.tileWidth),
            (int)Math.Round((pieceA1z - z) / Tile.tileWidth));
    }


    
    public ChessPiece GetChessPieceByIAndJ(int i, int j) {
        foreach(GameObject ch in chessPieces) {
            if(ReturnCoordinate(ch.transform.position.x,ch.transform.position.z).i == i &&
                ReturnCoordinate(ch.transform.position.x,ch.transform.position.z).j == j  ) {
                return ch.GetComponent<ChessPiece>();
            }
        }
        return null;
    }
     public static pos returnPos(int i, int j)
    {
        return new pos(pieceA1x - i * Tile.tileWidth, pieceA1z - j * Tile.tileWidth);
    }

}
    