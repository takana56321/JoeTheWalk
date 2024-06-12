using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �A�N�^�[�̃��[�U�[�I�u�W�F�N�g�N���X
/// </summary>
public class ActorLaser : MonoBehaviour
{
    // �I�u�W�F�N�g�E�R���|�[�l���g
    private SpriteRenderer spriteRenderer;

    // �e��ϐ�
    private int damage;     // �������̃_���[�W
    private float limitTime;    // ���ݎ���(�b)���̎��Ԃ��߂���Ə���
    private float maxLimitTime;    // �ő呶�ݎ���

    /// <summary>
    /// �������֐�(����������ďo)
    /// </summary>
    /// <param name="_damage">�������̃_���[�W</param>
    /// <param name="_limitTime">���ݎ���</param>
    public void Init(int _damage, float _limitTime)
    {
        // �Q�Ǝ擾
        spriteRenderer = GetComponent<SpriteRenderer>();

        // �ϐ��擾
        damage = _damage;
        limitTime = maxLimitTime = _limitTime;
    }

    // Update
    void Update()
    {
        // ���Ŕ���
        limitTime -= Time.deltaTime;

        // �X�v���C�g������
        Color color = spriteRenderer.color;
        color.a = limitTime / maxLimitTime;
        spriteRenderer.color = color;

        // ���ݎ��Ԃ�0�ɂȂ��������
        if (limitTime < 0.0f)
        {
            Destroy(gameObject);
        }
    }

    // �e�g���K�[�Ăяo������
    // �g���K�[�i�����Ɍďo
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;

        // �����Ώۂ��Ƃ̏���
        if (tag == "Enemy")
        {// �G�l�~�[�ɖ���
            EnemyBase enemyBase = collision.gameObject.GetComponent<EnemyBase>();
            if (enemyBase != null && !enemyBase.isInvis)
            {
                enemyBase.Damaged(damage); // �_���[�W����
            }
        }
    }
}