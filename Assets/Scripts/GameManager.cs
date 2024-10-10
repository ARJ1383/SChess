using UnityEngine;

public class GameManager : MonoBehaviour
{
    private ChessPiece selectedPiece;

    void Start()
    {
        
    }

    public void SelectPiece(ChessPiece newPiece)
    {
        // اگر مهره‌ای قبلاً انتخاب شده، حاشیه آن را حذف کن
        if (selectedPiece != null)
        {
            selectedPiece.RemoveOutline();
        }

        // تنظیم مهره جدید به عنوان مهره‌ی انتخاب شده
        selectedPiece = newPiece;
        selectedPiece.HighlightPiece();
        ColorTile();
        print(selectedPiece);
        
    }

    public void ColorTile()
    {
        
    }
}