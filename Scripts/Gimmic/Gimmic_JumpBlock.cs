using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �X�e�[�W�M�~�b�N�F�W�����v��
/// </summary>
public class Gimmic_JumpBlock : MonoBehaviour
{
    // �ݒ荀��
    [Header("�W�����v��")]
    public float JumpPower;

    // �e�g���K�[�Ăяo������
    // �g���K�[�؍ݎ��Ɍďo
    private void OnTriggerStay2D(Collider2D collision)
    {
        // �ڂ��Ă���̂��A�N�^�[�̐ڒn����I�u�W�F�N�g�łȂ��ꍇ�͏I��
        ActorGroundSensor actorGroundSensor = collision.gameObject.GetComponent<ActorGroundSensor>();
        if (actorGroundSensor == null)
            return;

        // �A�N�^�[���ړ�������
        var rigidbody2D = collision.gameObject.GetComponentInParent<Rigidbody2D>();
        rigidbody2D.velocity += new Vector2(0.0f, JumpPower);
    }
}