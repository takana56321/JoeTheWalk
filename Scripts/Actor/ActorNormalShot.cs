using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �A�N�^�[�̒ʏ�e�����N���X
/// </summary>
public class ActorNormalShot : MonoBehaviour
{
    // �e��ϐ�
    protected float speed;      // �e��
    protected float angle;      // �p�x(0-360)0�ŉE�E90�ŏ�
    protected int damage;       // �������̃_���[�W
    protected float limitTime;  // ���ݎ���(�b)���̎��Ԃ��߂���Ə���
    protected ActorController.ActorWeaponType useWeapon; // ���̒e�𔭎˂���̂Ɏg�p��������

/// <summary>
	/// �������֐�(����������ďo)
	/// </summary>
	/// <param name="_speed">�e��</param>
	/// <param name="_angle">�p�x</param>
	/// <param name="_damage">�������̃_���[�W</param>
	/// <param name="_limitTime">���ݎ���(�b)</param>
	/// <param name="_useWeapon">�g�p����</param>
	public void Init (float _speed, float _angle, int _damage, float _limitTime, ActorController.ActorWeaponType _useWeapon)
	{
		// �ϐ��擾
		speed = _speed;
		angle = _angle;
		damage = _damage;
		limitTime = _limitTime;
		useWeapon = _useWeapon;
	}

    // Update
    void Update()
    {
        // �ړ��x�N�g���v�Z(1�t���[�����̐i�s�����Ƌ������擾)
        Vector3 vec = new Vector3(speed * Time.deltaTime, 0.0f, 0.0f);
        vec = Quaternion.Euler(0, 0, angle) * vec; // �x�N�g����]

        // �ړ��x�N�g�������ƂɈړ�
        transform.Translate(vec);

        // ���Ŕ���
        limitTime -= Time.deltaTime;
        if (limitTime < 0.0f)
        {// ���ݎ��Ԃ�0�ɂȂ��������
            Destroy(gameObject);
        }
    }

    /// <summary>
	/// (�p�����Ďg�p)���̒e���G�Ƀ_���[�W��^�������̒ǉ�����
	/// </summary>
	protected virtual void OnDamagedEnemy(EnemyBase enemyBase)
    {
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
                bool result = enemyBase.Damaged(damage); // �_���[�W����
                                                         // �_���[�W��^����ꂽ�Ȃ�e�I�u�W�F�N�g�폜
                if (result)
                {
                    // �ǉ�����
                    OnDamagedEnemy(enemyBase);
                    // ���̒e���폜
                    Destroy(gameObject);
                }
            }
        }
        else if (tag == "Ground")
        {// �n�ʁE�ǂɖ���
            Destroy(gameObject);
        }
    }
}