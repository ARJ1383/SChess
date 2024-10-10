using UnityEngine;

public class ChessPiece : MonoBehaviour
{
    private Renderer pieceRenderer;

    private GameManager gameManager;

    void Start()
    {
        // گرفتن Renderer مهره
        pieceRenderer = GetComponentInChildren<Renderer>();

        // ایجاد Material Property Block برای کنترل رنگ و حاشیه
        // پیدا کردن Game Manager
        gameManager = FindObjectOfType<GameManager>();
    }

    void OnMouseDown()
    {
        // وقتی روی مهره کلیک می‌شود، آن را به عنوان مهره انتخابی به Game Manager بده
        gameManager.SelectPiece(this);
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
}