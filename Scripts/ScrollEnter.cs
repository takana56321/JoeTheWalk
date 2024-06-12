using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �g���K�[�͈͓��ɐi�������A�N�^�[���X�N���[��������
/// </summary>
public class ScrollEnter : MonoBehaviour
{
    private CameraController cameraController;

    [Header("�ؑ֐�G���A��AreaManager")]
    public AreaManager targetAreaManager;

    // Start
    void Start()
    {
        // �Q�Ǝ擾
        cameraController = Camera.main.GetComponent<CameraController>();

        // Sprite���\���ɂ���
        GetComponent<SpriteRenderer>().enabled = false;
    }

    // �e�g���K�[�Ăяo������
    // �g���K�[�i�����Ɍďo
    private void OnTriggerStay2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;

        // �A�N�^�[���i��
        if (tag == "Actor")
        {
            targetAreaManager.ActiveArea();
        }
    }
}