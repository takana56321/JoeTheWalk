using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ʓG�N���X(�{�X)�FBear
/// 
/// �Ǔo��E���n�Ռ��g
/// </summary>
public class Boss_Bear : EnemyBase
{
    // �摜�f��
    [Header("�摜�f��")]
    [SerializeField] private Sprite[] spriteList_Walk = null; // ���s�A�j���[�V����
    [SerializeField] private Sprite[] spriteList_Climb = null; // �Ǔo��A�j���[�V����
    [SerializeField] private Sprite sprite_Jump = null; // �W�����v��
    [SerializeField] private Sprite sprite_Land = null; // ���n��

    // �e�ۃv���n�u
    [Header("�G�l�~�[�e�ۃv���n�u")]
    public GameObject bulletPrefab;

    // �ݒ荀��
    [Header("�ړ����x")]
    public float movingSpeed;
    [Header("�Ǔo�葬�x")]
    public float climbSpeed;
    [Header("�Ǔo�莞��")]
    public float climbTime;
    [Header("�W�����v��")]
    public Vector2 jumpPower;
    [Header("���n����")]
    public float landTime;

    // �e��ϐ�
    private bool isFalling; // �������t���O
                            // �e���[�h�ʂ̌o�ߎ���
    private float walkCount = -1.0f; // ���s
    private float climbCount = -1.0f; // �Ǔo��
    private float jumpCount = -1.0f; // �W�����v
    private float landCount = -1.0f; // ���n

    // Start
    void Start()
    {
        // �ϐ�������
        walkCount = 0.0f;
    }
    /// <summary>
    /// ���̃����X�^�[�̋���G���A�ɃA�N�^�[���i���������̏���(�G���A�A�N�e�B�u��������)
    /// </summary>
    public override void OnAreaActivated()
    {
        base.OnAreaActivated();
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

        // �Ǔo�菈��
        if (climbCount > -1.0f)
        {
            climbCount += Time.fixedDeltaTime;
            // �c�ړ�
            float ySpeed = climbSpeed;
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, ySpeed);
            // �Ǔo��A�j���[�V����
            int animationFrame = (int)(climbCount * 3.0f);
            animationFrame %= spriteList_Climb.Length;
            spriteRenderer.sprite = spriteList_Climb[animationFrame];

            // �W�����v����
            if (climbCount >= climbTime)
            {
                climbCount = -1.0f;
                jumpCount = 0.0f;
                isFalling = false;

                // �W�����v�͌v�Z�E���f
                Vector2 jumpVec = jumpPower;
                if (!rightFacing)
                    jumpVec.x *= -1.0f;
                rigidbody2D.velocity = jumpVec;
            }
        }
        // ���ړ�����
        else if (walkCount > -1.0f)
        {
            walkCount += Time.fixedDeltaTime;

            // �ǂɂԂ�����������ύX�E�Ǔo��
            bool isStartClimb = false;
            if (rightFacing && rigidbody2D.velocity.x <= 0.0f)
            {
                SetFacingRight(false);
                isStartClimb = true;
            }
            else if (!rightFacing && rigidbody2D.velocity.x >= 0.0f)
            {
                SetFacingRight(true);
                isStartClimb = true;
            }
            // �Ǔo��J�n(�����n�߂Ă��班�Ȃ��Ƃ�1.0�b�ȏ�K�v)
            if (isStartClimb && walkCount > 1.0f)
            {
                walkCount = -1.0f;
                climbCount = 0.0f;
                return;
            }

            // ���ړ�
            float xSpeed = movingSpeed;
            if (!rightFacing)
                xSpeed *= -1.0f;
            rigidbody2D.velocity = new Vector2(xSpeed, rigidbody2D.velocity.y);
            // ���s�A�j���[�V����
            int animationFrame = (int)(walkCount * 3.0f);
            animationFrame %= spriteList_Walk.Length;
            if (animationFrame < 0)
                animationFrame = 0;
            spriteRenderer.sprite = spriteList_Walk[animationFrame];
        }
        // �W�����v������
        else if (jumpCount > -1.0f)
        {
            jumpCount += Time.fixedDeltaTime;

            // ����������
            if (!isFalling)
            {
                if (rigidbody2D.velocity.y < -Mathf.Epsilon)
                    isFalling = true;
            }
            // ���n�ڍs
            else if (isFalling && rigidbody2D.velocity.y >= -0.01f)
            {
                jumpCount = -1.0f;
                landCount = 0.0f;
                // �ˌ�����
                ShotBullet_TwoSideOnGround();
            }
            // �W�����v�X�v���C�g
            spriteRenderer.sprite = sprite_Jump;
        }
        // ���n������
        else if (landCount > -1.0f)
        {
            landCount += Time.fixedDeltaTime;

            // �ړ���~
            rigidbody2D.velocity = Vector2.zero;
            // ���ړ��ڍs
            if (landCount >= landTime)
            {
                landCount = -1.0f;
                walkCount = 0.0f;
            }
            // ���n�X�v���C�g
            spriteRenderer.sprite = sprite_Land;
        }
    }

    /// <summary>
    /// ���n���̏Ռ��g���ˌ����鏈��
    /// (�Ⴂ�ʒu�ō��E2�����֎ˌ�)
    /// </summary>
    private void ShotBullet_TwoSideOnGround()
    {
        // �e�ۃI�u�W�F�N�g�����E�ݒ�
        // �ˌ��ʒu
        Vector2 startPos = transform.position;
        startPos.y -= 0.8f; // ���ˈʒu��Y�����ɔ�����
                            // �@
        GameObject obj = Instantiate(bulletPrefab, startPos, Quaternion.identity);
        obj.GetComponent<EnemyShot>().Init(
            8.0f, // ���x
            0, // �p�x
            3, // �_���[�W��
            0.9f, // ���ݎ���
            false); // �n�ʂɓ������Ă������Ȃ�
        obj.GetComponent<SpriteRenderer>().flipX = true; // �X�v���C�g�����E���]
                                                         // �A
        obj = Instantiate(bulletPrefab, startPos, Quaternion.identity);
        obj.GetComponent<EnemyShot>().Init(
            8.0f, // ���x
            180, // �p�x
            3, // �_���[�W��
            0.9f, // ���ݎ���
            false); // �n�ʂɓ������Ă������Ȃ�
    }
}