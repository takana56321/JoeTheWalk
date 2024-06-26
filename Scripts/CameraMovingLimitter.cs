using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// メインカメラの可動範囲をこのオブジェクトが持つスプライトの大きさで指定できるようにする処理を行う
/// (スプライトは透明でも可)
/// </summary>
public class CameraMovingLimitter : MonoBehaviour
{
    // オブジェクト・コンポーネント
    private SpriteRenderer spriteRenderer;

    // Start
    void Start()
    {
        // 参照取得
        spriteRenderer = GetComponent<SpriteRenderer>();
        // スプライトを透明にする
        spriteRenderer.color = Color.clear;
    }

    /// <summary>
    /// スプライトの上下左右の端座標をRect型にして返す
    /// </summary>
    /// <returns>上下左右端座標のRect</returns>
    public Rect GetSpriteRect()
    {
        Rect result = new Rect(); // 矩形情報
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite sprite = spriteRenderer.sprite; // オブジェクトのスプライト情報

        // 矩形範囲を計算
        // Spriteサイズの半分を取得
        float halfSizeX = sprite.bounds.extents.x;
        float halfSizeY = sprite.bounds.extents.y;
        // 左上頂点座標を取得
        Vector3 topLeft = new Vector3(-halfSizeX, halfSizeY, 0f);
        topLeft = spriteRenderer.transform.TransformPoint(topLeft);
        // 右下頂点座標を取得
        Vector3 bottomRight = new Vector3(halfSizeX, -halfSizeY, 0f);
        bottomRight = spriteRenderer.transform.TransformPoint(bottomRight);

        //- Rectで矩形範囲を呼出元に渡す
        result.xMin = topLeft.x;
        result.yMin = topLeft.y;
        result.xMax = bottomRight.x;
        result.yMax = bottomRight.y;

        return result;
    }
}