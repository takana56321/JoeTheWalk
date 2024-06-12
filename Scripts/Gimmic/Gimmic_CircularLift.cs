using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �X�e�[�W�M�~�b�N�F�~�^�����t�g
/// �~��`���悤�Ɉړ����郊�t�g
/// </summary>
public class Gimmic_CircularLift : MonoBehaviour
{
    // ����Lift��Rigidbody2D
    private Rigidbody2D rigidbody2D;
    // �A�N�^�[��Transform(�A�N�^�[����������Ɏ擾)
    private Transform actorTrs;
    private ActorGroundSensor actorGroundSensor;

    // �ݒ荀��
    [Header("�ړ�����(���������Ԃ�������)")]
    public float MovingTime;
    [Header("���a")]
    public float Radius;
    [Header("�����p�x(0-360)")]
    public float InitialAngle;
    [Header("��]�������]�t���O")]
    public bool Reverse;

    //- �e��ϐ�
    private Vector3 defaultPos; // �������W
    private Vector3 moveVec; // �ړ��ʃx�N�g��
    private float time; // �o�ߎ���
    private bool isActorRiding; // �A�N�^�[������Ă���

    // Start
    void Start()
    {
        // �Q�Ǝ擾
        rigidbody2D = GetComponent<Rigidbody2D>();

        // �ϐ�������
        defaultPos = transform.position;
        rigidbody2D.freezeRotation = true; // ��]�𖳌���
        moveVec = Vector3.zero;
        time = 0.0f;

        // �G���[���
        if (MovingTime < 0.1f)
            MovingTime = 0.1f;

        // �����̈ړ��i�s�����f
        float fixValue = InitialAngle * Mathf.Deg2Rad;
        time = MovingTime * fixValue;
    }

    // FixedUpdate
    void FixedUpdate()
    {
        // ���Ԍo��
        time += Time.fixedDeltaTime;

        // �ړ��x�N�g���v�Z
        Vector3 vec;
        float moveValue = time / MovingTime;
        if (Reverse)
            moveValue *= -1.0f;
        vec = new Vector3(
            Mathf.Sin(moveValue) * Radius,
            Mathf.Cos(moveValue) * Radius,
            0.0f);

        // �O�t���[������̈ʒu�ω��ʂ��擾
        // (������A�N�^�[���ړ������鏈���Ɏg�p����)
        moveVec = (defaultPos + vec) - transform.position;

        // �ړ��K�p
        rigidbody2D.MovePosition(defaultPos + vec);

        // �A�N�^�[������Ă���Ȃ�A�N�^�[���ړ�
        if (isActorRiding)
        {
            isActorRiding = false;
            actorTrs.Translate(moveVec);
            actorGroundSensor.isGround = true;
        }
    }

    // �g���K�[�؍ݎ��Ɍďo
    private void OnTriggerStay2D(Collider2D collision)
    {
        // �ڂ��Ă���̂��A�N�^�[�̐ڒn����I�u�W�F�N�g�łȂ��ꍇ�͏I��
        var component = collision.gameObject.GetComponent<ActorGroundSensor>();
        if (component == null)
            return;

        actorGroundSensor = component;
        actorGroundSensor.isGround = true;
        // �A�N�^�[��Transform���擾
        actorTrs = collision.gameObject.transform.parent;
        // �A�N�^�[���ړ������鏈���̗\��
        isActorRiding = true;
    }
}