using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ʓG�N���X(�{�X)�FPenguin
/// 
/// ���ړ��E����E�e�U��
/// </summary>
public class Boss_Penguin : EnemyBase
{
    // �摜�f��
    [Header("�摜�f��")]
    [SerializeField] private Sprite[] spriteList_Walk = null; // ���s�A�j���[�V����
    [SerializeField] private Sprite sprite_Skating = null; // ����
    [SerializeField] private Sprite sprite_Charge = null; // �`���[�W
    [SerializeField] private Sprite sprite_Splash = null; // �e�g�U�U��

    // �e�ۃv���n�u
    [Header("�G�l�~�[�e�ۃv���n�u")]
    public GameObject bulletPrefab;

    // �ݒ荀��
    [Header("�ړ����x")]
    public float movingSpeed;
    [Header("�ړ�����")]
    public float movingTime;
    [Header("���莞��")]
    public float skatingTime;
    [Header("�`���[�W����")]
    public float chargeTime;
    [Header("�U�����[�V��������")]
    public float attackingTime;

    // �e���[�h�ʂ̌o�ߎ���
    private float movingCount; // �ړ�
    private float skatingCount; // ���蒆
    private float chargeCount; // �`���[�W
    private float attackingCount; // �U�����[�V����

    /// <summary>
    /// ���̃����X�^�[�̋���G���A�ɃA�N�^�[���i���������̏���(�G���A�A�N�e�B�u��������)
    /// </summary>
    public override void OnAreaActivated()
    {
        base.OnAreaActivated();

        // �ϐ�������
        skatingCount = chargeCount = attackingCount = -1.0f;
        movingCount = 0.0f;
    }

    // FixedUpdate
    void FixedUpdate()
    {
        // ���Œ��Ȃ珈�����Ȃ�
        if (isVanishing)
        {
            rigidbody2D.velocity = Vector2.zero;
            rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            return;
        }

        // ���ړ�����
        if (movingCount > -1.0f)
        {
            movingCount += Time.fixedDeltaTime;

            // ���ړ�
            float xSpeed = movingSpeed;
            // �A�N�^�[�Ƃ̈ʒu�֌W�������������
            if (transform.position.x > actorTransform.position.x)
            {// ������
                SetFacingRight(false);
                xSpeed *= -1.0f;
            }
            else
            {// �E����
                SetFacingRight(true);
            }
            rigidbody2D.velocity += new Vector2(xSpeed * Time.deltaTime, 0.0f);

            // ����ڍs
            if (movingCount >= movingTime)
            {
                movingCount = -1.0f;
                skatingCount = 0.0f;
            }

            // ���s�A�j���[�V����
            int animationFrame = (int)(movingCount * 4.0f);
            animationFrame %= spriteList_Walk.Length;
            if (animationFrame < 0)
                animationFrame = 0;
            spriteRenderer.sprite = spriteList_Walk[animationFrame];
        }
        // ���蒆����
        else if (skatingCount > -1.0f)
        {
            skatingCount += Time.fixedDeltaTime;

            // �`���[�W�ڍs
            if (skatingCount >= skatingTime)
            {
                skatingCount = -1.0f;
                chargeCount = 0.0f;
            }
            // �X�v���C�g�K�p
            spriteRenderer.sprite = sprite_Skating;
        }
        // �`���[�W������
        else if (chargeCount > -1.0f)
        {
            chargeCount += Time.fixedDeltaTime;

            // �ړ���~
            rigidbody2D.velocity = Vector2.zero;

            // �`���[�W�ڍs
            if (chargeCount >= chargeTime)
            {
                chargeCount = -1.0f;
                attackingCount = 0.0f;
                // �ˌ�
                ShotBullet_Splash();
            }
            // �X�v���C�g�K�p
            spriteRenderer.sprite = sprite_Charge;
        }
        // �U�����o������
        else if (attackingCount > -1.0f)
        {
            attackingCount += Time.fixedDeltaTime;

            // �`���[�W�ڍs
            if (attackingCount >= attackingTime)
            {
                attackingCount = -1.0f;
                movingCount = 0.0f;
            }
            // �X�v���C�g�K�p
            spriteRenderer.sprite = sprite_Splash;
        }
    }

    /// <summary>
    /// �A�N�^�[�Ɍ����Ďˌ����鏈��
    /// (�S���ʂ֎ˌ�)
    /// </summary>
    private void ShotBullet_Splash()
    {
        // �e�ۃI�u�W�F�N�g�����E�ݒ�
        // �ˌ��ʒu
        Vector2 startPos = transform.position;
        // ���ː�
        int bulletNum = 12;
        // �S���ʔ��ˏ���
        for (int i = 0; i < bulletNum; i++)
        {
            GameObject obj = Instantiate(bulletPrefab, startPos, Quaternion.identity);
            obj.GetComponent<EnemyShot>().Init(
                5.5f, // ���x
                (360 / bulletNum) * i, // �p�x
                3, // �_���[�W��
                3.0f, // ���ݎ���
                true); // �n�ʂɓ�����Ə�����
        }
    }
}