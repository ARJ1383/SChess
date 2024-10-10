using System;
using System.Collections;
using System.Collections.Generic;
using NetcodePlus;
using UnityEditor.VersionControl;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject chessBoard;
    
    public GameObject whiteRookPrefab;
    public GameObject whiteKnightPrefab;
    public GameObject whiteBishopPrefab;
    public GameObject whiteQueenPrefab;
    public GameObject whiteKingPrefab;
    public GameObject whitePawnPrefab;
    
    public GameObject blackRookPrefab;
    public GameObject blackKnightPrefab;
    public GameObject blackBishopPrefab;
    public GameObject blackQueenPrefab;
    public GameObject blackKingPrefab;
    public GameObject blackPawnPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        var chessLogic = ChessLogic.GetInstance();
    }

    void OnMouseDown()
    {
        
    }
}




