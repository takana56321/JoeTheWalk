using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �f�[�^�}�l�[�W���[(�V���O���g��)
/// </summary>
public class Data : MonoBehaviour
{
    #region �V���O���g���ێ��p����(�ύX�s�v)
    // �V���O���g���ێ��p
    public static Data instance { get; private set; }

    // Awake
    private void Awake()
    {
        // �V���O���g���p����
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // ���̑��N��������
        InitialProcess();
    }
    #endregion

    // �V�[���Ԃŕێ�����f�[�^�f�[�^
    public bool isTitleDisplayed; // �^�C�g����ʃ^�b�v�ς�
    public bool[] stageClearedFlags; // �X�e�[�W�ʃN���A�t���O
    public int nowStageID; // ���ݍU�����̃X�e�[�WID
    public bool[] weaponUnlocks; // ���ꕐ��̊J���f�[�^

    // �萔��`
    public const int StageNum_Normal = 7; // �ʏ�X�e�[�W�� 
    public const int StageNum_All = 8;  // ���X�{�X�ʂ��܂߂��X�e�[�W��
    public const int LastStageID = 7; // ���X�{�X�ʂ̃X�e�[�W�ԍ�
    // PlayerPrefs�L�[
    private const string Key_StageClearedFlags = "Key_StageClearedFlags";
    private const string Key_WeaponUnlocks = "Key_WeaponUnlocks";

    /// <summary>
    /// �Q�[���J�n��(�C���X�^���X����������)�Ɉ�x�������s����鏈��
    /// </summary>
    private void InitialProcess()
    {
        // �����V�[�h�l������
        Random.InitState(System.DateTime.Now.Millisecond);

        // �X�e�[�W�N���A�t���O������
        stageClearedFlags = new bool[StageNum_All];
        // PlayerPrefs���烍�[�h
        for (int i = 0; i < StageNum_All; i++)
        {
            // �f�[�^��1���i�[����Ă����Ȃ�X�e�[�W�N���A�ς݃t���O�𗧂Ă�
            if (PlayerPrefs.GetInt(Key_StageClearedFlags + i, 0) == 1)
                stageClearedFlags[i] = true;
        }

        // ���ꕐ��J���f�[�^������
        weaponUnlocks = new bool[(int)ActorController.ActorWeaponType._Max];
        weaponUnlocks[0] = true; // 1�ڂ̏�������͎����J��
                                 // PlayerPrefs���烍�[�h
        for (int i = 0; i < (int)ActorController.ActorWeaponType._Max; i++)
        {
            // �f�[�^��1���i�[����Ă����Ȃ畐��̊J���ς݃t���O�𗧂Ă�
            if (PlayerPrefs.GetInt(Key_WeaponUnlocks + i, 0) == 1)
                weaponUnlocks[i] = true;
        }
    }

    /// <summary>
	/// �X�e�[�W�N���A�f�[�^�ƕ������f�[�^�����ꂼ��ۑ�����
	/// </summary>
	public void SaveDatas()
    {
        // �X�e�[�W�N���A�t���O�ۑ�
        for (int i = 0; i < StageNum_All; i++)
        {
            // �Y���X�e�[�W�̃N���A�ς݃t���O�������Ă���Ȃ�1���A���N���A�Ȃ�0��ۑ�
            if (stageClearedFlags[i])
                PlayerPrefs.SetInt(Key_StageClearedFlags + i, 1);
            else
                PlayerPrefs.SetInt(Key_StageClearedFlags + i, 0);
        }

        // ����J���t���O�ۑ�
        for (int i = 0; i < (int)ActorController.ActorWeaponType._Max; i++)
        {
            // �Y������̊J���ς݃t���O�������Ă���Ȃ�1���A���J���Ȃ�0��ۑ�
            if (weaponUnlocks[i])
                PlayerPrefs.SetInt(Key_WeaponUnlocks + i, 1);
            else
                PlayerPrefs.SetInt(Key_WeaponUnlocks + i, 0);
        }

        // PlayerPrefs�ւ̕ύX��ۑ�
        PlayerPrefs.Save();
    }
}