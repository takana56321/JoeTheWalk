using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �S�G���ʁF�G�l�~�[�ƃA�N�^�[���ڐG�������A�N�^�[�Ƀ_���[�W�𔭐������鏈��
/// </summary>
public class EnemyBodyAttack : MonoBehaviour
{
    // �I�u�W�F�N�g�E�R���|�[�l���g
    private EnemyBase enemyBase;

    // Start
    void Start()
    {
        // �Q�Ǝ擾
        enemyBase = GetComponentInParent<EnemyBase>();

        // �����蔻��̑傫����K�p
        var Coll_TouchArea = GetComponent<BoxCollider2D>();
        var Coll_Body = enemyBase.gameObject.GetComponent<BoxCollider2D>();
        Coll_TouchArea.offset = Coll_Body.offset;
        Coll_TouchArea.size = Coll_Body.size;
        Coll_TouchArea.size *= 0.8f;
    }

    // �g���K�[�؍ݎ��Ɍďo
    private void OnTriggerStay2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;

        if (tag == "Actor")
        {// �A�N�^�[�ƐڐG
            enemyBase.BodyAttack(collision.gameObject); // �A�N�^�[�ɐڐG�_���[�W��^����
        }
    }
}