using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アイテム：HP回復
/// </summary>
public class Item_LifeUp : MonoBehaviour
{
    // 設定項目
    [Header("HP回復量")]
    public int healValue;
    [Header("持続時間")]
    public float maxLifetime;

    // 残り持続時間
    float remainLifeTime;

    // Start
    void Start()
    {
        // 変数初期化
        remainLifeTime = maxLifetime;
    }

    // Update
    void Update()
    {
        // 残り持続時間経過処理
        remainLifeTime -= Time.deltaTime;
        if (remainLifeTime < 0.0f)
            Destroy(gameObject);
    }

    // アクター接触時処理(Trigger)
    private void OnTriggerStay2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;

        if (tag == "Actor")
        {// アクターと接触した
         // アクターを回復する
            collision.gameObject.GetComponent<ActorController>().HealHP(healValue);
            Destroy(gameObject);
            // SE再生
            SEPlayer.instance.PlaySE(SEPlayer.SEName.Item_LifeUp);
        }
    }
}