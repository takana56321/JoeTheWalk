using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Canvas上の仮想ボタンの入力処理
/// </summary>
public class VirtualButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // 入力状態変数
    public bool input; // 入力中
    public bool down; // 入力開始時
    public bool up; // 入力終了時

    // LateUpdate(Updateより後に実行)
    void LateUpdate()
    {
        down = false;
        up = false;
    }

    /// <summary>
    /// タップ開始時に実行
    /// (IPointerDownHandlerが必要)
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!input)
            down = true;
        input = true;
    }
    /// <summary>
    /// タップ終了時に実行
    /// (IPointerUpHandlerが必要)
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (input)
            up = true;
        input = false;
    }
}