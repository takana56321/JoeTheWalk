using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// WeaponGetAnimationシーン進行クラス
/// アクターが１回特殊武器を発射する演出を行う
/// </summary>
public class WeaponGetAnimation : MonoBehaviour
{
    private ActorController actorController; // アクター制御クラス

    // Start
    void Start()
    {
        // 参照取得
        actorController = GetComponentInChildren<ActorController>();

        // 武器装備
        actorController.nowWeapon = (ActorController.ActorWeaponType)(Data.instance.nowStageID + 1);
        actorController.unmovableMode = true;
        actorController.ApplyWeaponChange();

        // 新武器紹介アニメーション(Sequence)
        // Sequence初期設定
        Sequence animationSequence = DOTween.Sequence();
        animationSequence.AppendInterval(2.0f);

        // ジャンプ移動
        for (int i = 0; i < 2; i++)
        {
            animationSequence.AppendCallback(() =>
            {
                actorController.StartShotActionImmediately();
            });
            animationSequence.AppendInterval(1.5f);
        }

        // ボタンのクリックイベントにリスナーを追加
       // backButton.onClick.AddListener(OnBackButtonClick);
    }

    public void OnBackButton()
    {
        // 指定されたシーンに切り替え
        SceneManager.LoadScene("StageSelect");
    }
}
