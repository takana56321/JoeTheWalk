using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// �X�e�[�W�M�~�b�N�F�d�C�X�C�b�`
/// �v���C���[���g�܂��̓V���b�g���G���Ɠd�C�h�A����������
/// </summary>
public class Gimmic_ElectricalSwitch : MonoBehaviour
{
    // �I�u�W�F�N�g�E�R���|�[�l���g
    private SpriteRenderer iconSpriteRenderer; // �d�C�A�C�R����SpriteRenderer

    // �ݒ荀��
    [Header("�����Ώۃh�A�I�u�W�F�N�g")]
    public GameObject TargetDoor;
    [Header("�I����Ԃ̓d�C�A�C�R��")]
    public Sprite iconSprite_On;

    // �쓮�ς݃t���O
    bool isActived;

    // Start
    void Start()
    {
        // 0�Ԗڂ̎q�I�u�W�F�N�g������SpriteRenderer�ւ̎Q�Ƃ��擾
        iconSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    // �g���K�[�؍ݎ��Ɍďo
    private void OnTriggerStay2D(Collider2D collision)
    {
        // ���ɍ쓮�ς݂Ȃ�I��
        if (isActived)
            return;

        // �ڂ��Ă���̂��A�N�^�[�܂��̓A�N�^�[�̃V���b�g�̎�����
        string tag = collision.gameObject.tag;
        if (tag == "Actor" || tag == "Actor_Shot")
        {
            // �X�C�b�`�쓮
            isActived = true;
            iconSpriteRenderer.sprite = iconSprite_On;
            // �����Ώۃh�A�����A�j���[�V����
            TargetDoor.transform.DOScale(0.0f, 1.0f)
                .OnComplete(() => Destroy(TargetDoor));
        }
    }
}