using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// �X�e�[�W�}�l�[�W���N���X
/// </summary>
public class StageManager : MonoBehaviour
{
    [HideInInspector] public ActorController actorController; // �A�N�^�[����N���X
    [HideInInspector] public CameraController cameraController; // �J��������N���X
    public Image bossHPGage; // �{�X�pHP�Q�[�WImage

    [Header("�����G���A��AreaManager")]
    public AreaManager initArea; // �X�e�[�W���̍ŏ��̃G���A(�����G���A)

    [Header("�{�X��pBGM��AudioClip")]
    public AudioClip bossBGMClip;

    [Header("�U�R�G���h���b�v����A�C�e���̃v���n�u���X�g")]
    public GameObject[] dropItemPrefabs;

    // �X�e�[�W���̑S�G���A�̔z��(Start�Ŏ擾)
    private AreaManager[] inStageAreas;

    // ���z�{�^���ꗗ
    public VirtualButton virtualButton_Left;
    public VirtualButton virtualButton_Right;
    public VirtualButton virtualButton_Jump;
    public VirtualButton virtualButton_Fire;
    public VirtualButton virtualButton_ChangeWeaponCW;
    public VirtualButton virtualButton_ChangeWeaponCCW;

    // �����{�^����GameObject
    public GameObject revivalButtonObject;
    // �Q�[���I�[�o�[��Tween
    private Tween gameOverTween;


    // Start
    void Start()
    {
        // �Q�Ǝ擾
        actorController = GetComponentInChildren<ActorController>();
        cameraController = GetComponentInChildren<CameraController>();

        // �X�e�[�W���̑S�G���A���擾�E������
        inStageAreas = GetComponentsInChildren<AreaManager>();
        foreach (var targetAreaManager in inStageAreas)
            targetAreaManager.Init(this);

        // �����G���A���A�N�e�B�u��(���̑��̃G���A�͑S�Ė�����)
        initArea.ActiveArea();

        // UI������
        bossHPGage.transform.parent.gameObject.SetActive(false);
        // �����{�^����\��
        revivalButtonObject.SetActive(false);

        // �S���z�{�^�����\���ɂ���(�G�f�B�^�[�����windows��ł̂�)
#if UNITY_EDITOR || UNITY_STANDALONE
        virtualButton_Left.gameObject.SetActive(false);
        virtualButton_Right.gameObject.SetActive(false);
        virtualButton_Jump.gameObject.SetActive(false);
        virtualButton_Fire.gameObject.SetActive(false);
        virtualButton_ChangeWeaponCW.gameObject.SetActive(false);
        virtualButton_ChangeWeaponCCW.gameObject.SetActive(false);
#endif

        
    }

    /// <summary>
    /// �X�e�[�W���̑S�G���A�𖳌�������
    /// </summary>
    public void DeactivateAllAreas()
    {
        foreach (var targetAreaManager in inStageAreas)
            targetAreaManager.gameObject.SetActive(false);
    }

    /// <summary>
	/// �{�X��pBGM���Đ�����
	/// </summary>
	public void PlayBossBGM()
    {
        // BGM��ύX����
        GetComponent<AudioSource>().clip = bossBGMClip;
        GetComponent<AudioSource>().Play();
    }

    /// <summary>
    /// �X�e�[�W�N���A������
    /// </summary>
    public void StageClear()
    {
        // �X�e�[�W�N���A�t���O�L�^
        Data.instance.stageClearedFlags[Data.instance.nowStageID] = true;
        // ���ꕐ����
        bool gainNewWeapon = false; // �V����l���t���O
        if (Data.instance.weaponUnlocks.Length > Data.instance.nowStageID + 1)
        {
            if (!Data.instance.weaponUnlocks[Data.instance.nowStageID + 1])
            {
                Data.instance.weaponUnlocks[Data.instance.nowStageID + 1] = true;
                gainNewWeapon = true;
            }
        }

        // �f�[�^�ۑ�
        Data.instance.SaveDatas();

        // �w��b���o�ߌ�ɏ��������s
        DOVirtual.DelayedCall(
            5.0f,   // 5.0�b�x��
            () => {
                // �V�[���؂�ւ�
                SceneManager.LoadScene(0);
                if (gainNewWeapon)
					SceneManager.LoadScene ("WeaponGetAnimation");
				else
					SceneManager.LoadScene ("StageSelect");
            }
        );
    }

    /// <summary>
    /// �Q�[���I�[�o�[����
    /// </summary>
    public void GameOver()
    {
        // �{�X�퓬���̂ݕ����{�^����\������
        // (�{�XHP�Q�[�W���\�����Ȃ�{�X�퓬���ł���Ɣ��f����)
        if (bossHPGage.gameObject.activeInHierarchy)
        {
            // �����{�^���\��
            revivalButtonObject.SetActive(true);
        }
        // �w��b���o�ߌ�ɏ��������s
        gameOverTween = DOVirtual.DelayedCall(
            5.0f,   // 5.0�b�x��
            () => {
                // �V�[���؂�ւ�
                SceneManager.LoadScene(0);
            }
        );
    }


    /// <summary>
    /// �����{�^������������
    /// </summary>
    public void OnRevivalButtonPressed()
    {

        // �����{�^����\��
        revivalButtonObject.SetActive(false);

        // �Q�[���I�[�o�[�������~
        if (gameOverTween != null)
        {
            gameOverTween.Kill();
            gameOverTween = null;
        }

        // �A�N�^�[�𕜊�������
        actorController.RevivalActor();

    }

 
}