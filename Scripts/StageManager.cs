using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// ステージマネージャクラス
/// </summary>
public class StageManager : MonoBehaviour
{
    [HideInInspector] public ActorController actorController; // アクター制御クラス
    [HideInInspector] public CameraController cameraController; // カメラ制御クラス
    public Image bossHPGage; // ボス用HPゲージImage

    [Header("初期エリアのAreaManager")]
    public AreaManager initArea; // ステージ内の最初のエリア(初期エリア)

    [Header("ボス戦用BGMのAudioClip")]
    public AudioClip bossBGMClip;

    [Header("ザコ敵がドロップするアイテムのプレハブリスト")]
    public GameObject[] dropItemPrefabs;

    // ステージ内の全エリアの配列(Startで取得)
    private AreaManager[] inStageAreas;

    // 仮想ボタン一覧
    public VirtualButton virtualButton_Left;
    public VirtualButton virtualButton_Right;
    public VirtualButton virtualButton_Jump;
    public VirtualButton virtualButton_Fire;
    public VirtualButton virtualButton_ChangeWeaponCW;
    public VirtualButton virtualButton_ChangeWeaponCCW;

    // 復活ボタンのGameObject
    public GameObject revivalButtonObject;
    // ゲームオーバー時Tween
    private Tween gameOverTween;


    // Start
    void Start()
    {
        // 参照取得
        actorController = GetComponentInChildren<ActorController>();
        cameraController = GetComponentInChildren<CameraController>();

        // ステージ内の全エリアを取得・初期化
        inStageAreas = GetComponentsInChildren<AreaManager>();
        foreach (var targetAreaManager in inStageAreas)
            targetAreaManager.Init(this);

        // 初期エリアをアクティブ化(その他のエリアは全て無効化)
        initArea.ActiveArea();

        // UI初期化
        bossHPGage.transform.parent.gameObject.SetActive(false);
        // 復活ボタン非表示
        revivalButtonObject.SetActive(false);

        // 全仮想ボタンを非表示にする(エディターおよびwindows上でのみ)
#if UNITY_EDITOR || UNITY_STANDALONE
        virtualButton_Left.gameObject.SetActive(false);
        virtualButton_Right.gameObject.SetActive(false);
        virtualButton_Jump.gameObject.SetActive(false);
        virtualButton_Fire.gameObject.SetActive(false);
        virtualButton_ChangeWeaponCW.gameObject.SetActive(false);
        virtualButton_ChangeWeaponCCW.gameObject.SetActive(false);
#endif

        
    }

    /// <summary>
    /// ステージ内の全エリアを無効化する
    /// </summary>
    public void DeactivateAllAreas()
    {
        foreach (var targetAreaManager in inStageAreas)
            targetAreaManager.gameObject.SetActive(false);
    }

    /// <summary>
	/// ボス戦用BGMを再生する
	/// </summary>
	public void PlayBossBGM()
    {
        // BGMを変更する
        GetComponent<AudioSource>().clip = bossBGMClip;
        GetComponent<AudioSource>().Play();
    }

    /// <summary>
    /// ステージクリア時処理
    /// </summary>
    public void StageClear()
    {
        // ステージクリアフラグ記録
        Data.instance.stageClearedFlags[Data.instance.nowStageID] = true;
        // 特殊武器解放
        bool gainNewWeapon = false; // 新武器獲得フラグ
        if (Data.instance.weaponUnlocks.Length > Data.instance.nowStageID + 1)
        {
            if (!Data.instance.weaponUnlocks[Data.instance.nowStageID + 1])
            {
                Data.instance.weaponUnlocks[Data.instance.nowStageID + 1] = true;
                gainNewWeapon = true;
            }
        }

        // データ保存
        Data.instance.SaveDatas();

        // 指定秒数経過後に処理を実行
        DOVirtual.DelayedCall(
            5.0f,   // 5.0秒遅延
            () => {
                // シーン切り替え
                SceneManager.LoadScene(0);
                if (gainNewWeapon)
					SceneManager.LoadScene ("WeaponGetAnimation");
				else
					SceneManager.LoadScene ("StageSelect");
            }
        );
    }

    /// <summary>
    /// ゲームオーバー処理
    /// </summary>
    public void GameOver()
    {
        // ボス戦闘時のみ復活ボタンを表示する
        // (ボスHPゲージが表示中ならボス戦闘時であると判断する)
        if (bossHPGage.gameObject.activeInHierarchy)
        {
            // 復活ボタン表示
            revivalButtonObject.SetActive(true);
        }
        // 指定秒数経過後に処理を実行
        gameOverTween = DOVirtual.DelayedCall(
            5.0f,   // 5.0秒遅延
            () => {
                // シーン切り替え
                SceneManager.LoadScene(0);
            }
        );
    }


    /// <summary>
    /// 復活ボタン押下時処理
    /// </summary>
    public void OnRevivalButtonPressed()
    {

        // 復活ボタン非表示
        revivalButtonObject.SetActive(false);

        // ゲームオーバー処理を停止
        if (gameOverTween != null)
        {
            gameOverTween.Kill();
            gameOverTween = null;
        }

        // アクターを復活させる
        actorController.RevivalActor();

    }

 
}