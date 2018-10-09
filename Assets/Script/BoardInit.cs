using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardInit : MonoBehaviour {

    public GameObject whitePiece;
    public GameObject blackPiece;
    public Vector3 boardOffset = new Vector3(-4, 0, -4);
    public Vector3 pieceOffset = new Vector3(0.5f, 0, 0.5f);
    private Piece[,] pieces = new Piece[8,8];
	// Use this for initialization
	void Start () {
        GenerateBoard();
	}

    private void GenerateBoard()
    {
        for(int y = 0; y < 3; y++)
        {
            for(int x = 0; x < 8; x+=2)
            {
                GeneratePiece(y % 2 == 0 ? x : x + 1, y);
            }
        }

        for (int y = 7; y > 4; y--)
        {
            for (int x = 0; x < 8; x += 2)
            {
                GeneratePiece(y % 2 == 0 ? x : x + 1, y);
            }
        }
    }
    
    private void GeneratePiece(int x, int y)
    {
        GameObject go = Instantiate(y > 3 ? blackPiece : whitePiece) as GameObject;
        go.transform.SetParent(transform);
        Piece piece = go.GetComponent<Piece>();
        pieces[x, y] = piece;
        SetPiecePos(piece, x, y);
    }

    private void SetPiecePos(Piece piece, int x, int y)
    {
        piece.transform.position = Vector3.right * x + Vector3.forward * y + boardOffset + pieceOffset;
        Debug.Log(x + " " + y);
    }
}
