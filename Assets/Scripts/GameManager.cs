using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NetcodePlus;
using NetcodePlus.Demo;
using Unity.Netcode;
using Unity.VisualScripting.FullSerializer;
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
    public static GameModeData gameModeData;
    
    public static int i_attacker, j_attacker, i_defender, j_defender;
    public static bool hostWins;
    public static bool hostAttack;
    
    public PieceSelected? pieceSelected;
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void OnSpawn()
    {
        base.OnSpawn();
        Cursor.lockState = CursorLockMode.None;
        actions = new SNetworkActions(this);
        actions.RegisterSerializable("sync", ReceiveSync,NetworkDelivery.Reliable);
        actions.RegisterSerializable("attack", getAttack,NetworkDelivery.Reliable);
        actions.RegisterSerializable("afterAttack", afterAttack,NetworkDelivery.Reliable);
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
        print(TheNetwork.Get());
        actions?.Trigger("sync", new PlayerMoveState(((PieceSelected)pieceSelected).i,((PieceSelected)pieceSelected).j,i, j,logic.IsWhite()));
        ((PieceSelected)pieceSelected).selectedPiece.transform.position = new Vector3(returnPos(i, j).x, Tile.tileHeight, returnPos(i, j).z);
    }

    public async void attack(int i, int j)
    {
        i_attacker = ((PieceSelected)pieceSelected).selectedPiece.getI();
        j_attacker = ((PieceSelected) pieceSelected).selectedPiece.getJ();
        i_defender = i;
        j_defender = j;
        
        actions?.Trigger("afterAttack",
            new PlayerMoveState(i_attacker, j_attacker, i, j,
                TheNetwork.Get().IsHost));
        hostAttack = TheNetwork.Get().IsHost;
        if (TheNetwork.Get().IsHost)
        {
            setTankName(((PieceSelected)pieceSelected).selectedPiece, true);
            setTankName(GetChessPieceByIAndJ(i, j), false);

            await Task.Delay(1000);

            TheNetwork.tank = true;
            GameMode mode = GameMode.Tank;
            GameModeData gmdata = GameModeData.Get(mode);
            DemoConnectData cdata = new DemoConnectData(mode);
            TheNetwork.Get().SetConnectionExtraData(cdata);
            TheNetwork.Get().LoadScene(gmdata.scene);
        }
        else
        {
            actions?.Trigger("attack",
                new PlayerMoveState(((PieceSelected)pieceSelected).i, ((PieceSelected)pieceSelected).j, i, j,
                    logic.IsWhite()));
        }
    }

    public void afterAttack(SerializedData data)
    {
        PlayerMoveState sync_state = data.Get<PlayerMoveState>();
        if ((sync_state.white && TheNetwork.Get().IsHost) || (!sync_state.white && !TheNetwork.Get().IsHost))
            return;
        i_attacker = sync_state.primary_i;
        j_attacker = sync_state.primary_j;
        i_defender = sync_state.secondary_i;
        j_defender = sync_state.secondary_j;
        hostAttack = !TheNetwork.Get().IsHost;
    }

    private void setTankName(ChessPiece chessPiece, bool isHost) {
        if (isHost)
        {
            for (int k = 0; k < 32; k++)
            {
                if (chessPiece == chessPieces[k].GetComponent<ChessPiece>())
                {
                    if (k < 8)
                        TankGame.name = "WhitePawn";
                    else if (k < 16)
                        TankGame.name = "BlackPawn";
                    else if (k < 18)
                        TankGame.name = "WhiteRook";
                    else if (k < 20)
                        TankGame.name = "BlackRook";
                    else if (k < 22)
                        TankGame.name = "WhiteKnight";
                    else if (k < 24)
                        TankGame.name = "BlackKnight";
                    else if (k < 26)
                        TankGame.name = "WhiteBishop";
                    else if (k < 28)
                        TankGame.name = "BlackBishop";
                    else if (k < 29)
                        TankGame.name = "WhiteQueen";
                    else if (k < 30)
                        TankGame.name = "BlackQueen";
                    else if (k < 31)
                        TankGame.name = "WhiteKing";
                    else if (k < 32)
                        TankGame.name = "BlackKing";
                }
            }
        }
        else
        {
            for (int k = 0; k < 32; k++)
            {
                if (chessPiece == chessPieces[k].GetComponent<ChessPiece>())
                {
                    if (k < 8)
                        TankGame.name2 = "WhitePawn";
                    else if (k < 16)
                        TankGame.name2 = "BlackPawn";
                    else if (k < 18)
                        TankGame.name2 = "WhiteRook";
                    else if (k < 20)
                        TankGame.name2 = "BlackRook";
                    else if (k < 22)
                        TankGame.name2 = "WhiteKnight";
                    else if (k < 24)
                        TankGame.name2 = "BlackKnight";
                    else if (k < 26)
                        TankGame.name2 = "WhiteBishop";
                    else if (k < 28)
                        TankGame.name2 = "BlackBishop";
                    else if (k < 29)
                        TankGame.name2 = "WhiteQueen";
                    else if (k < 30)
                        TankGame.name2 = "BlackQueen";
                    else if (k < 31)
                        TankGame.name2 = "WhiteKing";
                    else if (k < 32)
                        TankGame.name2 = "BlackKing";
                }
            }
        }
       
    }
    

    public async void getAttack(SerializedData sdata)
    {
        PlayerMoveState sync_state = sdata.Get<PlayerMoveState>();
        setTankName(GetChessPieceByIAndJ(sync_state.secondary_i, sync_state.secondary_j), true);
        setTankName(GetChessPieceByIAndJ(sync_state.primary_i, sync_state.primary_j), false);

        await Task.Delay(1000);

        TheNetwork.tank = true;
        GameMode mode = GameMode.Tank;
        GameModeData gmdata = GameModeData.Get(mode);
        DemoConnectData cdata = new DemoConnectData(mode);
        TheNetwork.Get().SetConnectionExtraData(cdata);
        TheNetwork.Get().LoadScene(gmdata.scene);
    }
    public void ReceiveSync(SerializedData sdata) {
        print(GameManager2.chessPieces[0]);
        if(tiles == null) tiles = GameManager2.tiles;
        if(chessPieces[0] == null) chessPieces = GameManager2.chessPieces;
        logic = GameManager2.logic;
        PlayerMoveState sync_state = sdata.Get<PlayerMoveState>();
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

    public struct PlayerAttackState : INetworkSerializable
    {
        public int i, j;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref i);
            serializer.SerializeValue(ref j); 
        }

        public PlayerAttackState(int i,int j) {
            this.i = i;
            this.j = j;
        }
    }

}