using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �X�e�[�W�M�~�b�N�F�B����
/// �A�N�^�[���ʂ��ė~�����Ȃ��ꏊ�ɐݒu����
/// </summary>
public class Gimmic_HiddenWall : MonoBehaviour
{
    // Start
    void Start()
    {
        GetComponent<SpriteRenderer>().color = Color.clear;
    }
}