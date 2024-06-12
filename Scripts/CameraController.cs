using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���C���J��������N���X(Main Camera�ɃA�^�b�`)
/// </summary>
public class CameraController : MonoBehaviour
{
    // �I�u�W�F�N�g�E�R���|�[�l���g
    public Transform backGroundTransform; // �w�i�X�v���C�gTransform

    // �e��ϐ�
    private Vector2 basePos; // ��_���W
    private Rect limitQuad; // �L�����̃J�����ړ������͈�

    // �萔��`
    public const float BG_Scroll_Speed = 0.5f; // �w�i�X�N���[�����x(1.0�ŃJ�����̈ړ��ʂƓ���)

    /// <summary>
    /// �J�����̈ʒu�𓮂���
    /// </summary>
    /// <param name="targetPos">���W</param>
    public void SetPosition(Vector2 targetPos)
    {
        basePos = targetPos;
    }

    // FixedUpdate
    private void FixedUpdate()
    {
        // �J�����ړ�
        Vector3 pos = transform.localPosition;
        // �A�N�^�[�̌��݈ʒu��菭���E����f���悤��X�EY���W��␳
        pos.x = basePos.x + 2.5f; // X���W
        pos.y = basePos.y + 1.5f; // Y���W
                                  // Z���W�͌��ݒl(transform.localPosition)�����̂܂܎g�p

        // �J�������͈͂𔽉f
        pos.x = Mathf.Clamp(pos.x, limitQuad.xMin, limitQuad.xMax); // x�����̉��͈�
        pos.y = Mathf.Clamp(pos.y, limitQuad.yMax, limitQuad.yMin);    // y�����̉��͈�

        // �v�Z��̃J�������W�𔽉f
        transform.localPosition = Vector3.Lerp(transform.localPosition, pos, 0.08f);

        // �w�i�X�v���C�g�ړ�
        // (�J�������ړ����锼���̑��x�Ŕw�i���ړ�����)
        Vector3 bgPos = transform.localPosition * BG_Scroll_Speed;
        bgPos.z = backGroundTransform.position.z;
        backGroundTransform.position = bgPos;
    }

    /// <summary>
    /// CameraMovingLimitter(�J�����ړ��͈�)���w��̂��̂ɐ؂�ւ���
    /// </summary>
    /// <param name="targetMovingLimitter">�ؑ֐�CameraMovingLimitter</param>
    public void ChangeMovingLimitter(CameraMovingLimitter targetMovingLimitter)
    {
        // �J�����������͈͂��Z�b�g
        limitQuad = targetMovingLimitter.GetSpriteRect();    // ���͈͂�Rect�^�Ŏ擾
    }
}