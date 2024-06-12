using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SE(���ʉ�)�Đ��N���X
/// </summary>
public class SEPlayer : MonoBehaviour
{
    // �ÓI�Q��
    public static SEPlayer instance { get; private set; }

    // SE�Đ��pAudioSource
    private AudioSource audioSource;

    // �o�^���ʉ���`���X�g
    public enum SEName
    {
        // (�V�X�e��)
        DecideA,
        // (�Q�[��)
        ActorShot_Normal,
        Actor_Damaged,
        Actor_Defeat,
        Actor_Jump,
        Enemy_Damaged,
        Enemy_Defeat,
        Boss_Defeat,
        Item_LifeUp,
        Item_EnergyUp,
    }
    // �o�^���ʉ��Q�ƃ��X�g(��̒�`���X�g�Ɠ������ԂŃC���X�y�N�^����i�[)
    public List<AudioClip> seClips = null;

    // Start
    void Start()
    {
        // �Q�Ǝ擾
        instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// �w�肵��SE���Đ�����
    /// </summary>
    public void PlaySE(SEName seName)
    {
        // SE���Đ�
        var targetAudioSource = audioSource;
        targetAudioSource.PlayOneShot(seClips[(int)seName]); // �Đ�
    }
}