using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Canvas��̉��z�{�^���̓��͏���
/// </summary>
public class VirtualButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // ���͏�ԕϐ�
    public bool input; // ���͒�
    public bool down; // ���͊J�n��
    public bool up; // ���͏I����

    // LateUpdate(Update����Ɏ��s)
    void LateUpdate()
    {
        down = false;
        up = false;
    }

    /// <summary>
    /// �^�b�v�J�n���Ɏ��s
    /// (IPointerDownHandler���K�v)
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!input)
            down = true;
        input = true;
    }
    /// <summary>
    /// �^�b�v�I�����Ɏ��s
    /// (IPointerUpHandler���K�v)
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (input)
            up = true;
        input = false;
    }
}