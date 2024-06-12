using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// WeaponGetAnimation�V�[���i�s�N���X
/// �A�N�^�[���P����ꕐ��𔭎˂��鉉�o���s��
/// </summary>
public class WeaponGetAnimation : MonoBehaviour
{
    private ActorController actorController; // �A�N�^�[����N���X

    // Start
    void Start()
    {
        // �Q�Ǝ擾
        actorController = GetComponentInChildren<ActorController>();

        // ���푕��
        actorController.nowWeapon = (ActorController.ActorWeaponType)(Data.instance.nowStageID + 1);
        actorController.unmovableMode = true;
        actorController.ApplyWeaponChange();

        // �V����Љ�A�j���[�V����(Sequence)
        // Sequence�����ݒ�
        Sequence animationSequence = DOTween.Sequence();
        animationSequence.AppendInterval(2.0f);

        // �W�����v�ړ�
        for (int i = 0; i < 2; i++)
        {
            animationSequence.AppendCallback(() =>
            {
                actorController.StartShotActionImmediately();
            });
            animationSequence.AppendInterval(1.5f);
        }

        // �{�^���̃N���b�N�C�x���g�Ƀ��X�i�[��ǉ�
       // backButton.onClick.AddListener(OnBackButtonClick);
    }

    public void OnBackButton()
    {
        // �w�肳�ꂽ�V�[���ɐ؂�ւ�
        SceneManager.LoadScene("StageSelect");
    }
}
