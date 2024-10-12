using UnityEngine;

public class ChessPiece : MonoBehaviour
{
    private Renderer pieceRenderer;

    public static GameManager gameManager2;

    private int i, j;

    void Start()
    {
        // گرفتن Renderer مهره
        pieceRenderer = GetComponentInChildren<Renderer>();

        // ایجاد Material Property Block برای کنترل رنگ و حاشیه
        // پیدا کردن Game Manager
    }

    void OnMouseDown()
    {
        UpdateIJ(); 
        gameManager2.SelectPiece(this, i, j);
    }

    public void HighlightPiece()
    {
        // تنظیم رنگ و عرض حاشیه مستقیماً در متریال
        pieceRenderer.material.SetColor("_OutlineColor", Color.red);
        pieceRenderer.material.SetFloat("_OutlineWidth", 0.05f);
    }

    public void RemoveOutline()
    {
        // حذف حاشیه با تغییر رنگ و عرض به مقدار اولیه
        pieceRenderer.material.SetColor("_OutlineColor", Color.clear);
        pieceRenderer.material.SetFloat("_OutlineWidth", 0.0f);
    }

    private void UpdateIJ()
    {
        float x = transform.position.x;
        float z = transform.position.z;
        i = GameManager.ReturnCoordinate(x, z).i;
        j = GameManager.ReturnCoordinate(x, z).j;
    }
    
    public int getI() { return i; }
    
    public int getJ() { return j; }
}