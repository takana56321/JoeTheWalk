using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アクター突風弾クラス
/// </summary>
public class ActorWindblowShot : ActorNormalShot
{
    /// <summary>
    /// (継承して使用)この弾が敵にダメージを与えた時の追加処理
    /// </summary>
    protected override void OnDamagedEnemy(EnemyBase enemyBase)
    {
        Vector2 blowVector = new Vector2(10.0f, 7.0f);
        // 弾が左に進んでいるならベクトルも左右反転
        if (angle > 90)
            blowVector.x *= -1.0f;

        // 敵を吹き飛ばす(対ザコのみ)
        if (!enemyBase.isBoss)
            enemyBase.GetComponent<Rigidbody2D>().velocity += blowVector;
    }
}