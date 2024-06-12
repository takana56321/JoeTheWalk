using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �}�b�v�^�C���@�\�F����
/// �G�ꂽ�A�N�^�[�𐅒����[�h�ɂ���
/// </summary>
public class Map_WaterTile : MonoBehaviour
{
    // �g���K�[�؍ݎ��Ɍďo
    private void OnTriggerStay2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;
        // �G�ꂽ�A�N�^�[�𐅒����[�h�ɂ���
        if (tag == "Actor")
        {

            collision.gameObject.GetComponent<ActorController>().SetWaterMode(true);
        }
    }

    // �g���K�[�ޏo���Ɍďo
    private void OnTriggerExit2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;
        // �������[�h����
        if (tag == "Actor")
        {
            collision.gameObject.GetComponent<ActorController>().SetWaterMode(false);
        }
    }
}