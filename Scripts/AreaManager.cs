using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �X�e�[�W���̊e�G���A�Ǘ��N���X
/// </summary>
public class AreaManager : MonoBehaviour
{
    // �I�u�W�F�N�g�E�R���|�[�l���g
    [HideInInspector] public StageManager stageManager; // �X�e�[�W�Ǘ��N���X
    private CameraMovingLimitter movingLimitter; // ���̃G���A�̃J�����ړ��͈�

    // �G���A���G�f�[�^�z��
    EnemyBase[] inAreaEnemies;

    // �������֐�(StageManager.cs����ďo)
    public void Init(StageManager _stageManager)
    {
        // �Q�Ǝ擾
        stageManager = _stageManager;
        movingLimitter = GetComponentInChildren<CameraMovingLimitter>();

        // �G���A���̓G���擾&������
        inAreaEnemies = GetComponentsInChildren<EnemyBase>();
        foreach (var targetEnemyBase in inAreaEnemies)
            targetEnemyBase.Init(this);

        // �A�N�^�[���i������܂ł��̃G���A�𖳌���
        gameObject.SetActive(false);
    }

    /// <summary>
    /// ���̃G���A���A�N�e�B�u������
    /// </summary>
    public void ActiveArea()
    {
        // ��U�S�G���A���A�N�e�B�u��
        stageManager.DeactivateAllAreas();

        // �I�u�W�F�N�g�L����
        gameObject.SetActive(true);

        // �G���A���̓G���A�N�e�B�u��
        foreach (var targetEnemyBase in inAreaEnemies)
            targetEnemyBase.OnAreaActivated();

        // �J�����ړ��͈͂�ύX
        stageManager.cameraController.ChangeMovingLimitter(movingLimitter);
    }
}