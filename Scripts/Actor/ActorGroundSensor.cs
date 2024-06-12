using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorGroundSensor : MonoBehaviour
{
    // �R���|�[�l���g�Q��
    private ActorController actorCtrl;

    // �ڒn����p�ϐ�
    // �ڒn����true������
    [HideInInspector] public bool isGround = false;

    // Start�i�I�u�W�F�N�g�L��������1�x���s�j
    void Start()
    {
        // �R���|�[�l���g�Q�Ǝ擾
        actorCtrl = GetComponentInParent<ActorController>();
    }

    // �e�g���K�[�Ăяo������
    // �g���K�[�؍ݎ��Ɍďo
    private void OnTriggerStay2D(Collider2D collision)
    {
        // �ڒn����I��
        if (collision.tag == "Ground")
            isGround = true;
    }
    // �g���K�[���痣�ꂽ���Ɍďo
    private void OnTriggerExit2D(Collider2D collision)
    {
        // �ڒn����I�t
        if (collision.tag == "Ground")
        {
            isGround = false;
        }
    }
}
