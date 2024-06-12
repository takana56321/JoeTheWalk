using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// データマネージャー(シングルトン)
/// </summary>
public class Data : MonoBehaviour
{
    #region シングルトン維持用処理(変更不要)
    // シングルトン維持用
    public static Data instance { get; private set; }

    // Awake
    private void Awake()
    {
        // シングルトン用処理
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // その他起動時処理
        InitialProcess();
    }
    #endregion

    // シーン間で保持するデータデータ
    public bool isTitleDisplayed; // タイトル画面タップ済み
    public bool[] stageClearedFlags; // ステージ別クリアフラグ
    public int nowStageID; // 現在攻略中のステージID
    public bool[] weaponUnlocks; // 特殊武器の開放データ

    // 定数定義
    public const int StageNum_Normal = 7; // 通常ステージ数 
    public const int StageNum_All = 8;  // ラスボス面も含めたステージ数
    public const int LastStageID = 7; // ラスボス面のステージ番号
    // PlayerPrefsキー
    private const string Key_StageClearedFlags = "Key_StageClearedFlags";
    private const string Key_WeaponUnlocks = "Key_WeaponUnlocks";

    /// <summary>
    /// ゲーム開始時(インスタンス生成完了時)に一度だけ実行される処理
    /// </summary>
    private void InitialProcess()
    {
        // 乱数シード値初期化
        Random.InitState(System.DateTime.Now.Millisecond);

        // ステージクリアフラグ初期化
        stageClearedFlags = new bool[StageNum_All];
        // PlayerPrefsからロード
        for (int i = 0; i < StageNum_All; i++)
        {
            // データに1が格納されていたならステージクリア済みフラグを立てる
            if (PlayerPrefs.GetInt(Key_StageClearedFlags + i, 0) == 1)
                stageClearedFlags[i] = true;
        }

        // 特殊武器開放データ初期化
        weaponUnlocks = new bool[(int)ActorController.ActorWeaponType._Max];
        weaponUnlocks[0] = true; // 1個目の初期武器は自動開放
                                 // PlayerPrefsからロード
        for (int i = 0; i < (int)ActorController.ActorWeaponType._Max; i++)
        {
            // データに1が格納されていたなら武器の開放済みフラグを立てる
            if (PlayerPrefs.GetInt(Key_WeaponUnlocks + i, 0) == 1)
                weaponUnlocks[i] = true;
        }
    }

    /// <summary>
	/// ステージクリアデータと武器解放データをそれぞれ保存する
	/// </summary>
	public void SaveDatas()
    {
        // ステージクリアフラグ保存
        for (int i = 0; i < StageNum_All; i++)
        {
            // 該当ステージのクリア済みフラグが立っているなら1を、未クリアなら0を保存
            if (stageClearedFlags[i])
                PlayerPrefs.SetInt(Key_StageClearedFlags + i, 1);
            else
                PlayerPrefs.SetInt(Key_StageClearedFlags + i, 0);
        }

        // 武器開放フラグ保存
        for (int i = 0; i < (int)ActorController.ActorWeaponType._Max; i++)
        {
            // 該当武器の開放済みフラグが立っているなら1を、未開放なら0を保存
            if (weaponUnlocks[i])
                PlayerPrefs.SetInt(Key_WeaponUnlocks + i, 1);
            else
                PlayerPrefs.SetInt(Key_WeaponUnlocks + i, 0);
        }

        // PlayerPrefsへの変更を保存
        PlayerPrefs.Save();
    }
}