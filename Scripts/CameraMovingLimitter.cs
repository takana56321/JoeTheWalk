using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���C���J�����̉��͈͂����̃I�u�W�F�N�g�����X�v���C�g�̑傫���Ŏw��ł���悤�ɂ��鏈�����s��
/// (�X�v���C�g�͓����ł���)
/// </summary>
public class CameraMovingLimitter : MonoBehaviour
{
    // �I�u�W�F�N�g�E�R���|�[�l���g
    private SpriteRenderer spriteRenderer;

    // Start
    void Start()
    {
        // �Q�Ǝ擾
        spriteRenderer = GetComponent<SpriteRenderer>();
        // �X�v���C�g�𓧖��ɂ���
        spriteRenderer.color = Color.clear;
    }

    /// <summary>
    /// �X�v���C�g�̏㉺���E�̒[���W��Rect�^�ɂ��ĕԂ�
    /// </summary>
    /// <returns>�㉺���E�[���W��Rect</returns>
    public Rect GetSpriteRect()
    {
        Rect result = new Rect(); // ��`���
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite sprite = spriteRenderer.sprite; // �I�u�W�F�N�g�̃X�v���C�g���

        // ��`�͈͂��v�Z
        // Sprite�T�C�Y�̔������擾
        float halfSizeX = sprite.bounds.extents.x;
        float halfSizeY = sprite.bounds.extents.y;
        // ���㒸�_���W���擾
        Vector3 topLeft = new Vector3(-halfSizeX, halfSizeY, 0f);
        topLeft = spriteRenderer.transform.TransformPoint(topLeft);
        // �E�����_���W���擾
        Vector3 bottomRight = new Vector3(halfSizeX, -halfSizeY, 0f);
        bottomRight = spriteRenderer.transform.TransformPoint(bottomRight);

        //- Rect�ŋ�`�͈͂��ďo���ɓn��
        result.xMin = topLeft.x;
        result.yMin = topLeft.y;
        result.xMax = bottomRight.x;
        result.yMax = bottomRight.y;

        return result;
    }
}