using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �A�C�e���F����G�l���M�[��
/// </summary>
public class Item_EnergyUp : MonoBehaviour
{
    // �ݒ荀��
    [Header("�G�l���M�[�񕜗�")]
    public int chargeValue;
    [Header("��������")]
    public float maxLifetime;

    // �c�莝������
    float remainLifeTime;

    // Start
    void Start()
    {
        // �ϐ�������
        remainLifeTime = maxLifetime;
    }

    // Update
    void Update()
    {
        // �c�莝�����Ԍo�ߏ���
        remainLifeTime -= Time.deltaTime;
        if (remainLifeTime < 0.0f)
            Destroy(gameObject);
    }

    // �A�N�^�[�ڐG������(Trigger)
    private void OnTriggerStay2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;

        if (tag == "Actor")
        {// �A�N�^�[�ƐڐG����
         // �G�l���M�[���񕜂���
            collision.gameObject.GetComponent<ActorController>().ChargeEnergy(chargeValue);
            Destroy(gameObject);
            // SE�Đ�
            SEPlayer.instance.PlaySE(SEPlayer.SEName.Item_EnergyUp);
        }
    }
}