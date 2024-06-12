using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// �ʓG�N���X(�{�X)�FJellyfish
/// 
/// �ړ������ˌ��U��
/// </summary>
public class Boss_Crocodile : EnemyBase
{
    // �摜�f��
    [Header("�摜�f��")]
    [SerializeField] private Sprite sprite_Wait = null; // �ҋ@
    [SerializeField] private Sprite sprite_OneArm = null; // �Ў茂��
    [SerializeField] private Sprite sprite_TwoArms = null; // ���茂��
    [SerializeField] private Sprite sprite_Anger = null; // �{��

    // �e�ۃv���n�u���X�g
    [Header("�G�l�~�[�e�ۃv���n�u���X�g(�F��)")]
    public List<GameObject> bulletPrefabs;

    // �ݒ荀��
    [Header("�U���Ԋu(�ŏ�)")]
    public float attackInterval_Min;
    [Header("�U���Ԋu(�ő�)")]
    public float attackInterval_Max;

    // �e��ϐ�
    private Sequence actionSequence; // �s��Sequence
    private float timeCount;
    private float nextActionTime;

    // �s���p�^�[��
    private ActionMode nowMode;
    // �s���p�^�[����`
    private enum ActionMode
    {
        Shot_A,
        Shot_B,
        Shot_C,
        Shot_D,
        _MAX,
    }

    /// <summary>
    /// ���̃����X�^�[�̋���G���A�ɃA�N�^�[���i���������̏���(�G���A�A�N�e�B�u��������)
    /// </summary>
    public override void OnAreaActivated()
    {
        base.OnAreaActivated();

        // �ϐ�������
        nowMode = ActionMode.Shot_A;
        timeCount = 0.0f;
        nextActionTime = attackInterval_Max;
    }

    // Update
    void Update()
    {
        // ���Œ��Ȃ珈�����Ȃ�
        if (isVanishing)
            return;

        // ���Ԍo��
        timeCount += Time.deltaTime;
        // �܂��s���^�C�~���O�łȂ��ꍇ
        if (timeCount < nextActionTime)
        {
            // �U������0.5�b�ȏ�o�߂Ȃ�X�v���C�g��ҋ@��Ԃɖ߂�
            if (timeCount >= 0.5f)
                spriteRenderer.sprite = sprite_Wait;
            // �����I��
            return;
        }

        // �s���J�n����
        timeCount = 0.0f;
        nextActionTime = Random.Range(attackInterval_Min, attackInterval_Max);
        // �e�p�^�[���ʂ̍s��
        switch (nowMode)
        {
            case ActionMode.Shot_A:
                // �ˌ�
                ShotBullet_ToActor();
                // �X�v���C�g�K�p
                spriteRenderer.sprite = sprite_OneArm;
                break;

            case ActionMode.Shot_B:
                // �ˌ�
                ShotBullet_ToActor3Way();
                // �X�v���C�g�K�p
                spriteRenderer.sprite = sprite_OneArm;
                break;

            case ActionMode.Shot_C:
                // �ˌ�
                ShotBullet_Random();
                // �X�v���C�g�K�p
                spriteRenderer.sprite = sprite_TwoArms;
                break;

            case ActionMode.Shot_D:
                // �ˌ�
                ShotBullet_ActorPierce();
                // �X�v���C�g�K�p
                spriteRenderer.sprite = sprite_Anger;
                break;
        }
        // �p�^�[���؂�ւ�
        nowMode++;
        if (nowMode == ActionMode._MAX)
            nowMode = ActionMode.Shot_A;
    }

    /// <summary>
    /// �A�N�^�[�Ɍ����Ďˌ����鏈��
    /// (�A�N�^�[�֑��d�ˌ�)
    /// </summary>
    private void ShotBullet_ToActor()
    {
        // �e�ۃI�u�W�F�N�g�����E�ݒ�
        // �ˌ��ʒu
        Vector2 startPos = transform.position;
        // ���ˊp�x�v�Z
        Vector2 calcVec = actorTransform.position - transform.position;
        float angle = Mathf.Atan2(calcVec.y, calcVec.x);
        angle *= Mathf.Rad2Deg;
        // ���ː�
        int bulletNum_Layer = 8; // ���d���ː�
                                 // �A�N�^�[�֔��ˏ���
        for (int i = 0; i < bulletNum_Layer; i++)
        {
            GameObject obj = Instantiate(bulletPrefabs[0], startPos, Quaternion.identity);
            obj.GetComponent<EnemyShot>().Init(
                2.0f + (i * 1.0f), // ���x
                angle, // �p�x
                3, // �_���[�W��
                5.0f, // ���ݎ���
                true); // �n�ʂɓ�����Ə�����
        }
    }
    /// <summary>
    /// �A�N�^�[�Ɍ����Ďˌ����鏈��
    /// (�A�N�^�[��3Way���d�ˌ�)
    /// </summary>
    private void ShotBullet_ToActor3Way()
    {
        // �e�ۃI�u�W�F�N�g�����E�ݒ�
        // �ˌ��ʒu
        Vector2 startPos = transform.position;
        // ���ˊp�x�v�Z
        Vector2 calcVec = actorTransform.position - transform.position;
        float angle = Mathf.Atan2(calcVec.y, calcVec.x);
        angle *= Mathf.Rad2Deg;
        // ���ː�
        int bulletNum_Layer = 8; // ���d���ː�
        int bulletAngleWidth = 30; // ���˕����ʂ̊p�x��
                                   // �A�N�^�[�֔��ˏ���
        for (int i = 0; i < bulletNum_Layer; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                GameObject obj = Instantiate(bulletPrefabs[1], startPos, Quaternion.identity);
                obj.GetComponent<EnemyShot>().Init(
                    2.0f + (i * 1.2f), // ���x
                    angle + (bulletAngleWidth * j), // �p�x
                    3, // �_���[�W��
                    5.0f, // ���ݎ���
                    true); // �n�ʂɓ�����Ə�����
            }
        }
    }
    /// <summary>
    /// �A�N�^�[�Ɍ����Ďˌ����鏈��
    /// (�����_�����d�ˌ�)
    /// </summary>
    private void ShotBullet_Random()
    {
        // �e�ۃI�u�W�F�N�g�����E�ݒ�
        // �ˌ��ʒu
        Vector2 startPos = transform.position;
        // ���ː�
        int bulletNum = 30; // ���ː�
                            // �A�N�^�[�֔��ˏ���
        for (int i = 0; i < bulletNum; i++)
        {
            GameObject obj = Instantiate(bulletPrefabs[2], startPos, Quaternion.identity);
            obj.GetComponent<EnemyShot>().Init(
                Random.Range(1.6f, 8.0f), // ���x
                Random.Range(0.0f, 360.0f), // �p�x
                3, // �_���[�W��
                5.0f, // ���ݎ���
                true); // �n�ʂɓ�����Ə�����
        }
    }

    /// <summary>
    /// �A�N�^�[�Ɍ����Ďˌ����鏈��
    /// (�A�N�^�[�֊ђʎˌ�)
    /// </summary>
    private void ShotBullet_ActorPierce()
    {
        // �e�ۃI�u�W�F�N�g�����E�ݒ�
        // �ˌ��ʒu
        Vector2 startPos = transform.position;
        // ���ˊp�x�v�Z
        Vector2 calcVec = actorTransform.position - transform.position;
        float angle = Mathf.Atan2(calcVec.y, calcVec.x);
        angle *= Mathf.Rad2Deg;
        // �A�N�^�[�֔��ˏ���
        GameObject obj = Instantiate(bulletPrefabs[3], startPos, Quaternion.identity);
        obj.transform.localScale *= 1.8f; // �e���剻
        obj.GetComponent<EnemyShot>().Init(
            1.8f, // ���x
            angle, // �p�x
            4, // �_���[�W��
            10.0f, // ���ݎ���
            false); // �n�ʂɓ������Ă������Ȃ�
    }
}