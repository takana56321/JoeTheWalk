using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// �X�e�[�W�Z���N�g��ʊǗ��N���X
/// </summary>
public class StageSelectManager : MonoBehaviour
{
    // �X�e�[�W�I���{�^�����X�g
    [SerializeField] private List<Image> stageSelectButtonImages = null;
    // �^�C�g��Image�I�u�W�F�N�g
    [SerializeField] private GameObject titlePictureObject = null;
    // �I���{�^��
    [SerializeField] private Button exitButton = null;

    // Start
    void Start()
    {
        // �^�C�g����ʃ^�b�v�ς݃t���O���L���Ȃ�^�C�g����ʂ�\�����Ȃ�
        if (Data.instance.isTitleDisplayed)
            titlePictureObject.SetActive(false);

        // �X�e�[�W�Z���N�g�{�^���̐F�ύX
        for (int i = 0; i < stageSelectButtonImages.Count; i++)
        {
            if (Data.instance.stageClearedFlags[i])
                stageSelectButtonImages[i].color = Color.gray;
        }

        // ���X�{�X�X�e�[�W�I���{�^���L�����E������
        bool isReady = true;
        for (int i = 0; i < Data.StageNum_Normal; i++)
        {
            if (!Data.instance.stageClearedFlags[i])
            {
                isReady = false;
                break;
            }
        }
        stageSelectButtonImages[Data.StageNum_All - 1].gameObject.SetActive(isReady);

        // �I���{�^���Ƀ��\�b�h��ǉ�
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitButtonPressed);
        }
    }

    /// <summary>
    /// �X�e�[�W�I���{�^������������
    /// </summary>
    /// <param name="bossID">�Y���{�XID</param>
    public void OnStageSelectButtonPressed(int bossID)
    {
        // �X�e�[�WID�L��
        Data.instance.nowStageID = bossID;
        // �V�[���؂�ւ�
        SceneManager.LoadScene(bossID + 1);
    }

    /// <summary>
	/// �^�C�g���摜�I�u�W�F�N�g�^�b�v���ɌĂяo��
	/// </summary>
	public void OnTitlePictureTapped()
    {
        // �^�C�g���摜�I�u�W�F�N�g�𖳌���
        titlePictureObject.SetActive(false);
        // �^�C�g����ʃ^�b�v�ς݃t���O���Z�b�g
        Data.instance.isTitleDisplayed = true;
    }

    /// <summary>
    /// �I���{�^������������
    /// </summary>
    public void OnExitButtonPressed()
    {
        // �A�v���P�[�V�����I��
        Application.Quit();

        // �G�f�B�^�Ŏ��s���Ă���ꍇ�A��~������
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
