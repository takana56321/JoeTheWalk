using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SE(効果音)再生クラス
/// </summary>
public class SEPlayer : MonoBehaviour
{
    // 静的参照
    public static SEPlayer instance { get; private set; }

    // SE再生用AudioSource
    private AudioSource audioSource;

    // 登録効果音定義リスト
    public enum SEName
    {
        // (システム)
        DecideA,
        // (ゲーム)
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
    // 登録効果音参照リスト(上の定義リストと同じ順番でインスペクタから格納)
    public List<AudioClip> seClips = null;

    // Start
    void Start()
    {
        // 参照取得
        instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// 指定したSEを再生する
    /// </summary>
    public void PlaySE(SEName seName)
    {
        // SEを再生
        var targetAudioSource = audioSource;
        targetAudioSource.PlayOneShot(seClips[(int)seName]); // 再生
    }
}