using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// �ʓG�N���X(�{�X)�FJellyfish
/// 
/// ���V�ړ��E�e�U��
/// </summary>
public class Boss_Jellyfish : EnemyBase
{
    // �摜�f��
    [Header("�摜�f��")]
    [SerializeField] private Sprite[] spriteList_Move = null; // �ړ��A�j���[�V����
    [SerializeField] private Sprite sprite_Charge = null; // �`���[�W
    [SerializeField] private Sprite sprite_Spread = null; // �g�U�e�U��

    // �e�ۃv���n�u
    [Header("�G�l�~�[�e�ۃv���n�u")]
    public GameObject bulletPrefab;

    // �ݒ荀��
    [Header("�ړ����x")]
    public float movingSpeed;
    [Header("�ړ�����")]
    public float movingTime;
    [Header("�`���[�W����")]
    public float chargeTime;
    [Header("�U�����[�V��������")]
    public float attackingTime;

    // �e��ϐ�
    private Sequence actionSequence; // �s��Sequence
    private float timeCount; // �o�ߎ���

    // �s���p�^�[��
    private ActionMode nowMode;
    // �s���p�^�[����`
    private enum ActionMode
    {
        Moving,
        Charge,
        Attacking,
        _MAX,
    }

    /// <summary>
    /// ���̃����X�^�[�̋���G���A�ɃA�N�^�[���i���������̏���(�G���A�A�N�e�B�u��������)
    /// </summary>
    public override void OnAreaActivated()
    {
        base.OnAreaActivated();

        // �ϐ�������
        nowMode = ActionMode.Moving;
        timeCount = 0.0f;
    }

    // Update
    void Update()
    {
        // ���Œ��Ȃ珈�����Ȃ�
        if (isVanishing)
            return;

        // ���Ԍo��
        timeCount += Time.deltaTime;

        // �A�N�^�[�Ƃ̈ʒu�֌W�������������
        if (transform.position.x > actorTransform.position.x)
        {// ������
            SetFacingRight(false);
        }
        else
        {// �E����
            SetFacingRight(true);
        }

        // �e�p�^�[���ʂ̍s��
        switch (nowMode)
        {
            case ActionMode.Moving: // �ړ���
                                    // �A�N�^�[�Ɍ������Ĉړ�
                Vector2 calcVec = actorTransform.position - transform.position;
                calcVec *= 0.2f * Time.deltaTime * movingSpeed;
                rigidbody2D.velocity += calcVec;

                // �X�v���C�g�K�p
                int animationFrame = (int)(timeCount * 2.0f);
                animationFrame %= spriteList_Move.Length;
                if (animationFrame < 0)
                    animationFrame = 0;
                spriteRenderer.sprite = spriteList_Move[animationFrame];
                // �����[�h�ؑ�
                if (timeCount >= movingTime)
                {
                    timeCount = 0.0f;
                    nowMode = ActionMode.Charge;
                }
                break;

            case ActionMode.Charge: // �`���[�W��
                                    // ����
                rigidbody2D.velocity *= Time.deltaTime;

                // �X�v���C�g�K�p
                spriteRenderer.sprite = sprite_Charge;
                // �����[�h�ؑ�
                if (timeCount >= chargeTime)
                {
                    timeCount = 0.0f;
                    nowMode = ActionMode.Attacking;
                    // �g�U�e����
                    ShotBullet_Spread();
                }
                break;

            case ActionMode.Attacking: // �U����
                                       // �X�v���C�g�K�p
                spriteRenderer.sprite = sprite_Spread;
                // �����[�h�ؑ�
                if (timeCount >= attackingTime)
                {
                    timeCount = 0.0f;
                    nowMode = ActionMode.Moving;
                }
                break;
        }
    }

    /// <summary>
    /// �S���ʂ֑��d�ˌ����鏈��
    /// </summary>
    private void ShotBullet_Spread()
    {
        // �e�ۃI�u�W�F�N�g�����E�ݒ�
        // �ˌ��ʒu
        Vector2 startPos = transform.position;
        // ���ː�
        int bulletNum_Angle = 8; // ���˕�����
        int bulletNum_Layer = 6; // ���d���ː�
                                 // �S���ʔ��ˏ���
        for (int i = 0; i < bulletNum_Angle; i++)
        {
            for (int j = 0; j < bulletNum_Layer; j++)
            {
                GameObject obj = Instantiate(bulletPrefab, startPos, Quaternion.identity);
                obj.GetComponent<EnemyShot>().Init(
                    2.4f + (j * 0.8f), // ���x
                    (360 / bulletNum_Angle) * i, // �p�x
                    3, // �_���[�W��
                    3.6f, // ���ݎ���
                    true); // �n�ʂɓ�����Ə�����
            }
        }
    }
}