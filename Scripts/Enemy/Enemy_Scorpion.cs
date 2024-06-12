using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ʓG�N���X�FScorpion
/// 
/// �ړ����������U��
/// </summary>
public class Enemy_Scorpion : EnemyBase
{
    // �I�u�W�F�N�g�E�R���|�[�l���g
    public GameObject tornadoBulletPrefab; // �����e�v���n�u

    // �摜�f��
    [SerializeField] private List<Sprite> spriteList = null; // �U���A�j���[�V����

    // �ݒ荀��
    [Header("�U���Ԋu")]
    public float attackInterval;

    // �e��ϐ�
    private float timeCount;
    private float nextActionTime;

    // Start
    void Start()
    {
        // �ϐ�������
        timeCount = -1.0f;
        nextActionTime = attackInterval;
    }

    // Update
    void Update()
    {
        // ���Œ��Ȃ珈�����Ȃ�
        if (isVanishing)
            return;

        // �A�N�^�[�Ƃ̈ʒu�֌W�������������
        if (transform.position.x > actorTransform.position.x)
        {// ������
            SetFacingRight(false);
        }
        else
        {// �E����
            SetFacingRight(true);
        }

        // ���Ԍo��
        timeCount += Time.deltaTime;
        // �X�v���C�g�K�p
        int animationFrame = (int)(timeCount * spriteList.Count / attackInterval);
        animationFrame = Mathf.Clamp(animationFrame, 0, spriteList.Count - 1);
        spriteRenderer.sprite = spriteList[animationFrame];
        // �܂��s���^�C�~���O�łȂ��ꍇ�͏����I��
        if (timeCount < attackInterval)
            return;
        // �s���J�n
        timeCount = 0.0f;

        // �����U��
        ShotBullet_Tornado();
    }

    /// <summary>
    /// ���ɗ����e���ˌ����鏈��
    /// </summary>
    private void ShotBullet_Tornado()
    {
        // �e�ۃI�u�W�F�N�g�����E�ݒ�
        // �ˌ��ʒu
        Vector2 startPos = transform.position;
        // �S���ʔ��ˏ���
        GameObject obj = Instantiate(tornadoBulletPrefab, startPos, Quaternion.identity);
        obj.GetComponent<EnemyShot>().Init(
            2.4f, // ���x
            rightFacing ? 0 : 180, // �p�x(rightFacing���I���̎��͉E���A�I�t�Ȃ獶���̊p�x�ɂ��鎖���Q�l���Z�q�Ŏ���)
            3, // �_���[�W��
            4.2f, // ���ݎ���
            true); // �n�ʂɓ�����Ə�����
    }
}