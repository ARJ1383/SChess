using System;
using System.Collections.Generic;
using NetcodePlus;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

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

public class GameManager : SNetworkPlayer
{

    private SNetworkActions actions;
    private GameObject[,] tiles = null;
    public GameObject[] chessPieces = null;
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
    public PlayerMoveState sync_state = new PlayerMoveState();
    
    public PieceSelected? pieceSelected;
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void OnSpawn()
    {
        base.OnSpawn();
        actions = new SNetworkActions(this);
        actions.RegisterSerializable("sync", ReceiveSync,NetworkDelivery.Reliable);
        Tile.gameManager = this;
        ChessPiece.gameManager2 = this;
    }

    protected override void OnDespawn()
    {
        base.OnDespawn();
        actions.Clear();
    }
    public static pos returnPos(int i, int j)
    {
        return new pos(pieceA1x - i * Tile.tileWidth, pieceA1z - j * Tile.tileWidth);
    }

    public static Coordinate ReturnCoordinate(float x, float z)
    {
        return new Coordinate((int)Math.Round((pieceA1x - x) / Tile.tileWidth),
            (int)Math.Round((pieceA1z - z) / Tile.tileWidth));
    }


    public void SelectPiece(ChessPiece chessPiece, int i, int j)
    {
        if(tiles == null) tiles = GameManager2.tiles;
        if(chessPieces == null) chessPieces = GameManager2.chessPieces;
        logic = GameManager2.logic;
        if((TheNetwork.Get().IsHost && ChessLogic.GetInstance().IsWhite() ) || 
            (!TheNetwork.Get().IsHost && !ChessLogic.GetInstance().IsWhite() ))  {
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
        print(TheNetwork.Get().PlayerID);
        actions?.Trigger("sync", new PlayerMoveState(((PieceSelected)pieceSelected).i,((PieceSelected)pieceSelected).j,i, j,logic.IsWhite()));
        ((PieceSelected)pieceSelected).selectedPiece.transform.position = new Vector3(returnPos(i, j).x, Tile.tileHeight, returnPos(i, j).z);
    }
    public void ReceiveSync(SerializedData sdata) {
        print(GameManager2.chessPieces[0]);
        if(tiles == null) tiles = GameManager2.tiles;
        if(chessPieces[0] == null) chessPieces = GameManager2.chessPieces;
        logic = GameManager2.logic;
        PlayerMoveState sync_state = sdata.Get<PlayerMoveState>();
        print(IsOwner);
        print(TheNetwork.Get().IsHost);
        print(chessPieces[0]);
        if ((sync_state.white && TheNetwork.Get().IsHost) || (!sync_state.white && !TheNetwork.Get().IsHost)) {
            ChessPiece chessPiece = GetChessPieceByIAndJ(sync_state.primary_i,sync_state.primary_j);
            logic.MovePiece(new ChessPosition(sync_state.primary_j,sync_state.primary_i), new ChessPosition(sync_state.secondary_j,sync_state.secondary_i));
            logic.ChangeTurn();
            chessPiece.transform.position = new Vector3(returnPos(sync_state.secondary_i,sync_state.secondary_j).x,Tile.tileHeight,returnPos(sync_state.secondary_i,sync_state.secondary_j).z);
            print("here");
        }
        
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
    public struct PlayerMoveState : INetworkSerializable
    {
        public int primary_i, secondary_i, primary_j, secondary_j;
        public bool white;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref primary_i);
            serializer.SerializeValue(ref primary_j);
            serializer.SerializeValue(ref secondary_i);
            serializer.SerializeValue(ref secondary_j);
            serializer.SerializeValue(ref white);   
        }

        public PlayerMoveState(int primary_i,int primary_j, int secondary_i, int secondary_j,bool white) {
            this.primary_i = primary_i;
            this.secondary_i = secondary_i;
            this.primary_j = primary_j;
            this.secondary_j = secondary_j;
            this.white = white;
        }
    }

}