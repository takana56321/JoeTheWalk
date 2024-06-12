using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �G�l�~�[�̒ʏ�e�����N���X
/// </summary>
public class EnemyShot : MonoBehaviour
{
    // �e��ϐ�
    private float speed;        // �e��
    private float angle;        // �p�x(0-360)0�ŉE�E90�ŏ�
    private int damage;         // �������̃_���[�W
    private float limitTime;    // ���ݎ���(�b)���̎��Ԃ��߂���Ə���
    private bool isDestroyHitToGround; // true:�n�ʂ�ǂɓ��������������

    /// <summary>
    /// �������֐�(����������ďo)
    /// </summary>
    /// <param name="_speed">�e��</param>
    /// <param name="_angle">�p�x</param>
    /// <param name="_damage">�������̃_���[�W</param>
    /// <param name="_isDestroyHitToGround">true:�n�ʂ�ǂɓ��������������</param>
    public void Init(float _speed, float _angle, int _damage, float _limitTime, bool _isDestroyHitToGround)
    {
        // �ϐ��擾
        speed = _speed;
        angle = _angle;
        damage = _damage;
        limitTime = _limitTime;
        isDestroyHitToGround = _isDestroyHitToGround;
    }

    // Update
    void Update()
    {
        // �ړ��x�N�g���v�Z
        Vector3 vec = new Vector3(speed * Time.deltaTime, 0.0f, 0.0f);
        vec = Quaternion.Euler(0, 0, angle) * vec; // �x�N�g����]

        // �ړ�
        transform.Translate(vec);

        // ���Ŕ���
        limitTime -= Time.deltaTime;
        if (limitTime < 0.0f)
        {// ���ݎ��Ԃ�0�ɂȂ��������
            Destroy(gameObject);
        }
    }

    // �g���K�[�i�����Ɍďo
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;

        // �����Ώۂ��Ƃ̏���
        if (tag == "Actor")
        {// �A�N�^�[�ɖ���
            ActorController actorController = collision.gameObject.GetComponent<ActorController>();
            if (actorController != null)
            {
                actorController.Damaged(damage); // �_���[�W����
            }
        }
        else if (tag == "Ground")
        {// �n�ʁE�ǂɖ���
            if (isDestroyHitToGround)
                Destroy(gameObject);
        }
    }
}