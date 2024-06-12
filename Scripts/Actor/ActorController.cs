using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// アクター操作・制御クラス
/// </summary>
public class ActorController : MonoBehaviour
{
    // オブジェクト・コンポーネント参照
    private StageManager stageManager;
    private Rigidbody2D rigidbody2D; // Rigidbody2Dコンポーネントへの参照
    private SpriteRenderer spriteRenderer;
    private ActorGroundSensor groundSensor; // アクター接地判定クラス
    private ActorSprite actorSprite; // アクタースプライト設定クラス
    public CameraController cameraController; // カメラ制御クラス
    public Image hpGage; // HPゲージ
    public Image energyGage = null; // 武器エネルギーゲージ
    public Image energyGageIcon = null; // 武器エネルギーゲージアイコン

    // 設定項目
    [Header("true:足場が滑るモード")]
    public bool icyGroundMode;
    [Header("各武器で使用するプレハブリスト(定義の順番に設定)")]
    public List<GameObject> weaponBulletPrefabs;
    [Header("各武器のエネルギーゲージの画像")]
    public List<Sprite> weaponIconSprites;
    [Header("各武器のエネルギーゲージの色")]
    public List<Color> weaponGageColors;
    [Header("各武器の消費エネルギー量")]
    public List<int> weaponEnergyCosts;
    [Header("各武器の連射間隔(秒)")]
    public List<float> weaponIntervals;

    // 体力変数
    [HideInInspector] public int nowHP; // 現在HP
    [HideInInspector] public int maxHP; // 最大HP

    // 装備変数
    [HideInInspector] public ActorWeaponType nowWeapon;
    private int[] weaponEnergies; // 武器の残りエネルギーデータ(それぞれ最大値がMaxEnergy)
    private float weaponRemainInterval; // 武器が次に発射可能になるまでの残り時間(秒)

    // 移動関連変数
    [HideInInspector] public float xSpeed; // X方向移動速度
    [HideInInspector] public bool rightFacing; // 向いている方向(true.右向き false:左向き)
    private float remainJumpTime;   // 空中でのジャンプ入力残り受付時間
    private bool canDoubleJump; // 二段ジャンプ可能フラグ

    // その他変数
    private float remainStuckTime; // 残り硬直時間(0以上だと行動できない)
    private float invincibleTime;   // 残り無敵時間(秒)
    [HideInInspector] public bool isDefeat; // true:撃破された(ゲームオーバー)
    [HideInInspector] public bool inWaterMode; // true:水中モード(メソッドから変更)
    [HideInInspector] public bool doggyMode; // 犬騎乗モード
    [HideInInspector] public bool unmovableMode; // 行動禁止モード

    // 定数定義
    private const int InitialHP = 20;           // 初期HP(最大HP)
    private const int MaxEnergy = 20;			// 武器エネルギーの最大値
    private const float InvicibleTime = 2.0f;   // 被ダメージ直後の無敵時間(秒)
    private const float StuckTime = 0.5f;       // 被ダメージ直後の硬直時間(秒)
    private const float KnockBack_X = 2.5f;     // 被ダメージ時ノックバック力(x方向)
    private const float WaterModeDecelerate_X = 0.8f;// 水中でのX方向速度倍率
    private const float WaterModeDecelerate_Y = 0.92f;// 水中でのX方向速度倍率

    // アクター装備定義
    public enum ActorWeaponType
    {
        Normal,     // (通常)
        Doggy,      // 犬騎乗
        Tackle,     // タックル
        Windblow,   // 突風
        IceBall,    // 雪玉
        Lightning,  // 稲妻
        WaterRing,  // 水の輪
        Laser,      // レーザー
        _Max,
    }

    // Start（オブジェクト有効化時に1度実行）
    void Start()
    {
        // コンポーネント参照取得
        stageManager = GetComponentInParent<StageManager>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        groundSensor = GetComponentInChildren<ActorGroundSensor>();
        actorSprite = GetComponent<ActorSprite>();

        // 配下コンポーネント初期化
        actorSprite.Init(this);

        // カメラ初期位置
        cameraController.SetPosition(transform.position);

        // 武器エネルギー初期化
        weaponEnergies = new int[(int)ActorWeaponType._Max];
        for (int i = 0; i < (int)ActorWeaponType._Max; i++)
            weaponEnergies[i] = MaxEnergy;
        ApplyWeaponChange(); // 初期装備を反映

        // 変数初期化
        rightFacing = true; // 最初は右向き
        nowHP = maxHP = InitialHP; // 初期HP
        hpGage.fillAmount = 1.0f; // HPゲージの初期FillAmount
        canDoubleJump = true; // 二段ジャンプ可能
    }

    // Update（1フレームごとに1度ずつ実行）
    void Update()
    {
        // 行動禁止モードなら終了
        if (unmovableMode)
            return;
        // 撃破された後なら終了
        if (isDefeat)
            return;

        // 無敵時間が残っているなら減少
        if (invincibleTime > 0.0f)
        {
            invincibleTime -= Time.deltaTime;
            if (invincibleTime <= 0.0f)
            {// 無敵時間終了時処理
                actorSprite.EndBlinking(); // 点滅終了
            }
        }
        // 硬直時間処理
        if (remainStuckTime > 0.0f)
        {// 硬直時間減少
            remainStuckTime -= Time.deltaTime;
            if (remainStuckTime <= 0.0f)
            {// スタン時間終了時処理
                actorSprite.stuckMode = false;
            }
            else
                return;
        }

        // 左右移動処理
        MoveUpdate();
        // ジャンプ入力処理
        JumpUpdate();

        // 武器切り替え処理
        ChangeWeaponUpdate();

        // 攻撃可能までの残り時間減少
        if (weaponRemainInterval > 0.0f)
            weaponRemainInterval -= Time.deltaTime;
        // 攻撃入力処理
        StartShotAction();

        // 坂道で滑らなくする処理
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation; // Rigidbodyの機能のうち回転だけは常に停止
        if (groundSensor.isGround && !(Input.GetKey(KeyCode.UpArrow) || stageManager.virtualButton_Jump.input))
        {
            // 坂道を登っている時上昇力が働かないようにする処理
            if (rigidbody2D.velocity.y > 0.0f)
            {
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0.0f);
            }
            // 坂道に立っている時滑り落ちないようにする処理
            if (Mathf.Abs(xSpeed) < 0.1f && !icyGroundMode)
            {
                // Rigidbodyの機能のうち移動と回転を停止
                rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            }
        }

        // カメラに自身の座標を渡す
        cameraController.SetPosition(transform.position);
    }

    #region 移動関連
    /// <summary>
	/// Updateから呼び出される左右移動入力処理
	/// </summary>
	private void MoveUpdate()
    {
        // X方向移動入力
        if (Input.GetKey(KeyCode.RightArrow) || stageManager.virtualButton_Right.input)
        {// 右方向の移動入力
         // X方向移動速度をプラスに設定
            xSpeed = 6.0f;

            // 右向きフラグon
            rightFacing = true;

            // スプライトを通常の向きで表示
            spriteRenderer.flipX = false;
        }
        else if (Input.GetKey(KeyCode.LeftArrow) || stageManager.virtualButton_Left.input)
        {// 左方向の移動入力
         // X方向移動速度をマイナスに設定
            xSpeed = -6.0f;

            // 右向きフラグoff
            rightFacing = false;

            // スプライトを左右反転して表示
            spriteRenderer.flipX = true;
        }
        else
        {// 入力なし
         // X方向の移動を停止
            xSpeed = 0.0f;
        }
    }

    /// <summary>
	/// Updateから呼び出されるジャンプ入力処理
	/// </summary>
	private void JumpUpdate()
    {
        // 空中でのジャンプ入力受付時間減少
        if (remainJumpTime > 0.0f)
            remainJumpTime -= Time.deltaTime;

        // ジャンプ操作
        if (Input.GetKeyDown(KeyCode.UpArrow) || stageManager.virtualButton_Jump.down)
        {
            if (groundSensor.isGround || inWaterMode)
            {
                // 地上または水中でのジャンプ開始
                JumpAction();
                canDoubleJump = true; // 二段ジャンプをリセット
            }
            else if (canDoubleJump)
            {
                // 二段ジャンプ開始
                JumpAction();
                canDoubleJump = false; // 二段ジャンプを消費
            }
        }
        else if (Input.GetKey(KeyCode.UpArrow) || stageManager.virtualButton_Jump.input)
        {// ジャンプ中（ジャンプ入力を長押しすると継続して上昇する処理）
         // 空中でのジャンプ入力受け付け時間が残ってないなら終了
            if (remainJumpTime <= 0.0f)
                return;
            // 接地しているなら終了
            if (groundSensor.isGround)
                return;

            // ジャンプ力加算量を計算
            float jumpAddPower = 30.0f * Time.deltaTime; // Update()は呼び出し間隔が異なるのでTime.deltaTimeが必要
            // ジャンプ力加算を適用
            rigidbody2D.velocity += new Vector2(0.0f, jumpAddPower);
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow) || stageManager.virtualButton_Jump.up)
        {// ジャンプ入力終了
            remainJumpTime = -1.0f;
        }
    }

    /// <summary>
    /// ジャンプアクションの実行
    /// </summary>
    private void JumpAction()
    {
        // ジャンプ力を計算
        float jumpPower = 10.0f;
        // ジャンプ力を適用
        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpPower);

        // 空中でのジャンプ入力受け付け時間設定
        remainJumpTime = 0.25f;
    }

    // FixedUpdate（一定時間ごとに1度ずつ実行・物理演算用）
    private void FixedUpdate()
    {
        // 移動速度ベクトルを現在値から取得
        Vector2 velocity = rigidbody2D.velocity;
        // X方向の速度を入力から決定
        velocity.x = xSpeed;
        // 氷床ステージなら接地時に滑るような速度設定にする
        if (icyGroundMode && groundSensor.isGround)
            velocity.x = Mathf.Lerp(xSpeed, rigidbody2D.velocity.x, 0.99f);

        // 水中モードでの速度
        if (inWaterMode)
        {
            velocity.x *= WaterModeDecelerate_X;
            velocity.y *= WaterModeDecelerate_Y;
        }

        // 計算した移動速度ベクトルをRigidbody2Dに反映
        rigidbody2D.velocity = velocity;
    }

    /// <summary>
	/// 水中モードをセットする
	/// </summary>
	/// <param name="mode">true:水中にいる</param>
	public void SetWaterMode(bool mode)
    {
        // 水中モード
        inWaterMode = mode;
        // 犬騎乗モードならgravityScaleを変えない
        if (doggyMode)
            return;
        // 水中での重力
        if (inWaterMode)
        {
            rigidbody2D.gravityScale = 0.3f;
        }
        else
        {
            rigidbody2D.gravityScale = 1.0f;
        }
    }
    #endregion

    #region 装備関連
    /// <summary>
    /// Updateから呼び出される武器切り替え処理
    /// </summary>
    private void ChangeWeaponUpdate()
    {
        // 武器切り替え
        if (Input.GetKeyDown(KeyCode.A) || stageManager.virtualButton_ChangeWeaponCCW.down)
        {// 1つ前に切り替え
         // 前の解放済み武器が見つかるまで選択武器をデクリメント
            do
            {
                if (nowWeapon == ActorWeaponType.Normal)
                    nowWeapon = ActorWeaponType._Max;
                nowWeapon--;
            }
            while (!Data.instance.weaponUnlocks[(int)nowWeapon]);
            // 犬騎乗中なら解除
            if (doggyMode)
                ShotAction_Doggy();
            // 武器変更を反映
            ApplyWeaponChange();
        }
        else if (Input.GetKeyDown(KeyCode.S) || stageManager.virtualButton_ChangeWeaponCW.down)
        {// 1つ次に切り替え
         // 次の解放済み武器が見つかるまで選択武器をインクリメント
            do
            {
                nowWeapon++;
                if (nowWeapon == ActorWeaponType._Max)
                    nowWeapon = ActorWeaponType.Normal;
            }
            while (!Data.instance.weaponUnlocks[(int)nowWeapon]);
            // 犬騎乗中なら解除
            if (doggyMode)
                ShotAction_Doggy();
            // 武器変更を反映
            ApplyWeaponChange();
        }
    }

    /// <summary>
    /// 特殊武器の変更を反映する
    /// </summary>
    public void ApplyWeaponChange()
    {
        // エネルギーゲージ表示(通常武器以外)
        if (nowWeapon == ActorWeaponType.Normal)
            energyGage.transform.parent.gameObject.SetActive(false);
        else
            energyGage.transform.parent.gameObject.SetActive(true);

        // ゲージの色を反映
        energyGage.color = weaponGageColors[(int)nowWeapon];
        // ゲージの量を反映
        energyGage.fillAmount = (float)weaponEnergies[(int)nowWeapon] / MaxEnergy;
        // ゲージのアイコンを設定
        energyGageIcon.sprite = weaponIconSprites[(int)nowWeapon];
    }
    #endregion

    #region 戦闘関連
    /// <summary>
    /// ダメージを受ける際に呼び出される
    /// </summary>
    /// <param name="damage">ダメージ量</param>
    public void Damaged(int damage)
    {
        // 撃破された後なら終了
        if (isDefeat)
            return;

        // もし無敵時間中ならダメージ無効
        if (invincibleTime > 0.0f)
            return;
        // ゲームオーバー中なら終了
        //if (gameManager.isGameOver)
        //	return;

        // ダメージ処理
        nowHP -= damage;
        // HPゲージの表示を更新する
        float hpRatio = (float)nowHP / maxHP;
        hpGage.DOFillAmount(hpRatio, 0.5f);

        // HP0ならゲームオーバー処理
        if (nowHP <= 0)
        {
            isDefeat = true;
            // 被撃破演出開始
            actorSprite.StartDefeatAnim();
            // 物理演算を停止
            rigidbody2D.velocity = Vector2.zero;
            xSpeed = 0.0f;
            rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            // ゲームオーバー処理
            GetComponentInParent<StageManager>().GameOver();
            return;
        }

        // スタン硬直
        remainStuckTime = StuckTime;
        actorSprite.stuckMode = true;

        // ノックバック処理
        // ノックバック力・方向決定
        float knockBackPower = KnockBack_X;
        if (rightFacing)
            knockBackPower *= -1.0f;
        // ノックバック適用
        xSpeed = knockBackPower;

        // 無敵時間発生
        invincibleTime = InvicibleTime;
        if (invincibleTime > 0.0f)
            actorSprite.StartBlinking(); // 点滅開始
    }

    /// <summary>
    /// 攻撃ボタン入力時処理
    /// </summary>
    public void StartShotAction()
    {
        // 攻撃ボタンが入力されていないなら終了
        if (!(Input.GetKeyDown(KeyCode.Z) || stageManager.virtualButton_Fire.down))
            return;
        // 武器エネルギーが足りないなら攻撃しない
        if (weaponEnergies[(int)nowWeapon] <= 0)
            return;
        // 攻撃可能までの時間が残っているなら終了
        if (weaponRemainInterval > 0.0f)
            return;

        // 武器エネルギー減少
        weaponEnergies[(int)nowWeapon] -= weaponEnergyCosts[(int)nowWeapon];
        if (weaponEnergies[(int)nowWeapon] < 0)
            weaponEnergies[(int)nowWeapon] = 0;
        // 武器エネルギーゲージ表示更新
        energyGage.fillAmount = (float)weaponEnergies[(int)nowWeapon] / MaxEnergy;
        // 次弾発射可能までの残り時間設定
        weaponRemainInterval = weaponIntervals[(int)nowWeapon];

        // 攻撃を発射
        switch (nowWeapon)
        {
            case ActorWeaponType.Normal:
                // 通常攻撃
                ShotAction_Normal();
                break;
            case ActorWeaponType.Doggy:
                // 犬騎乗
                ShotAction_Doggy();
                break;
            case ActorWeaponType.Tackle:
                // タックル
                ShotAction_Tackle();
                break;
            case ActorWeaponType.Windblow:
                // 突風
                ShotAction_Windblow();
                break;
            case ActorWeaponType.IceBall:
                // 雪玉
                ShotAction_IceBall();
                break;
            case ActorWeaponType.Lightning:
                // 稲妻
                ShotAction_Lightning();
                break;
            case ActorWeaponType.WaterRing:
                // 水の輪
                ShotAction_WaterRing();
                break;
            case ActorWeaponType.Laser:
                // レーザー
                ShotAction_Laser();
                break;
        }
    }

    /// <summary>
	/// 射撃アクション：通常攻撃
	/// </summary>
	private void ShotAction_Normal()
    {
        // 弾の方向を取得
        float bulletAngle = 0.0f; // 右向き
                                  // アクターが左向きなら弾も左向きに進む
        if (!rightFacing)
            bulletAngle = 180.0f;

        // 弾丸オブジェクト生成・設定
        GameObject obj = Instantiate(weaponBulletPrefabs[(int)ActorWeaponType.Normal], transform.position, Quaternion.identity);
        obj.GetComponent<ActorNormalShot>().Init(
            12.0f, // 速度
            bulletAngle, // 角度
            1, // ダメージ量
            5.0f, // 存在時間
            nowWeapon); // 使用武器

        // SE再生
        SEPlayer.instance.PlaySE(SEPlayer.SEName.ActorShot_Normal);
    }
    /// <summary>
    /// 射撃アクション：犬騎乗
    /// </summary>
    private void ShotAction_Doggy()
    {
        if (!doggyMode)
        {// 犬に乗る
            doggyMode = true;
            rigidbody2D.gravityScale = 0.5f;
        }
        else
        {// 犬から降りる
            doggyMode = false;
            rigidbody2D.gravityScale = 1.0f;
        }
    }
    /// <summary>
    /// 射撃アクション：タックル
    /// </summary>
    private void ShotAction_Tackle()
    {
        // 弾の方向を取得
        float bulletAngle = 0.0f; // 右向き
                                  // アクターが左向きなら弾も左向きに進む
        if (!rightFacing)
            bulletAngle = 180.0f;

        // 弾丸オブジェクト生成・設定
        GameObject obj = Instantiate(weaponBulletPrefabs[(int)ActorWeaponType.Tackle], transform.position, Quaternion.identity);
        obj.GetComponent<ActorNormalShot>().Init(
            20.0f, // 速度
            bulletAngle, // 角度
            1, // ダメージ量
            0.3f, // 存在時間
            nowWeapon); // 使用武器
        if (!rightFacing)
            obj.GetComponent<SpriteRenderer>().flipX = true;

        // 主人公の突進移動
        Vector3 moveVector = new Vector3(1.2f, 0.25f, 0.0f);
        if (!rightFacing)
            moveVector.x *= -1.0f;
        rigidbody2D.MovePosition(transform.position + moveVector);
        groundSensor.isGround = false;

        // 無敵時間発生
        invincibleTime = 0.6f;
    }
    /// <summary>
    /// 射撃アクション：突風
    /// </summary>
    private void ShotAction_Windblow()
    {
        // 弾の方向を取得
        float bulletAngle = 0.0f; // 右向き
                                  // アクターが左向きなら弾も左向きに進む
        if (!rightFacing)
            bulletAngle = 180.0f;

        // 弾丸オブジェクト生成・設定
        GameObject obj = Instantiate(weaponBulletPrefabs[(int)ActorWeaponType.Windblow], transform.position, Quaternion.identity);
        obj.GetComponent<ActorNormalShot>().Init(
            16.0f, // 速度
            bulletAngle, // 角度
            1, // ダメージ量
            3.0f, // 存在時間
            nowWeapon); // 使用武器
    }
    /// <summary>
    /// 射撃アクション：雪玉
    /// </summary>
    private void ShotAction_IceBall()
    {
        // 弾の初速ベクトルを設定
        Vector2 velocity = new Vector2(14.0f, 8.0f);
        if (!rightFacing)
            velocity.x *= -1.0f;

        // 弾丸オブジェクト生成・設定
        GameObject obj = Instantiate(weaponBulletPrefabs[(int)ActorWeaponType.IceBall], transform.position, Quaternion.identity);
        obj.GetComponent<ActorNormalShot>().Init(
            0.0f, // 速度(rigidbodyで弾を動かすので設定不要)
            0.0f, // 角度(rigidbodyで弾を動かすので設定不要)
            1, // ダメージ量
            5.0f, // 存在時間
            nowWeapon); // 使用武器
        obj.GetComponent<Rigidbody2D>().velocity += velocity;
    }
    /// <summary>
    /// 射撃アクション：稲妻
    /// </summary>
    private void ShotAction_Lightning()
    {
        // 弾の発射位置を設定(主人公の右上or左上)
        Vector3 fixPos = new Vector3(4.0f, 5.0f, 0.0f);
        if (!rightFacing)
            fixPos.x *= -1.0f;

        // 弾丸オブジェクト生成・設定
        GameObject obj = Instantiate(weaponBulletPrefabs[(int)ActorWeaponType.Lightning], transform.position + fixPos, Quaternion.identity);
        obj.GetComponent<ActorNormalShot>().Init(
            14.0f, // 速度
            270, // 角度
            2, // ダメージ量
            5.0f, // 存在時間
            nowWeapon); // 使用武器
    }
    /// <summary>
    /// 射撃アクション：水の輪
    /// </summary>
    private void ShotAction_WaterRing()
    {
        // 弾丸オブジェクト生成・設定
        int bulletNum_Angle = 8; // 発射方向数
        for (int i = 0; i < bulletNum_Angle; i++)
        {
            GameObject obj = Instantiate(weaponBulletPrefabs[(int)ActorWeaponType.WaterRing], transform.position, Quaternion.identity);
            obj.GetComponent<ActorNormalShot>().Init(
                3.0f, // 速度
                (360 / bulletNum_Angle) * i, // 角度
                1, // ダメージ量
                2.0f, // 存在時間
                nowWeapon); // 使用武器
        }
    }
    /// <summary>
    /// 射撃アクション：レーザー
    /// </summary>
    private void ShotAction_Laser()
    {
        // レーザーオブジェクト生成・設定
        GameObject obj = Instantiate(weaponBulletPrefabs[(int)ActorWeaponType.Laser], transform.position, Quaternion.identity);
        obj.GetComponent<ActorLaser>().Init(
            1, // ダメージ量
            1.0f); // 存在時間
    }
    #endregion


    /// <summary>
	/// アクターのHPを回復する
	/// </summary>
	/// <param name="healValue">回復量</param>
	public void HealHP(int healValue)
    {
        // 撃破された後なら終了
        if (isDefeat)
            return;

        // ダメージ処理
        nowHP += healValue;
        if (nowHP > maxHP)
            nowHP = maxHP;
        // HPゲージの表示を更新する
        float hpRatio = (float)nowHP / maxHP;
        hpGage.DOFillAmount(hpRatio, 0.5f);
    }

    /// <summary>
	/// アクターの武器エネルギーを回復する
	/// </summary>
	/// <param name="chargeValue">回復量</param>
	public void ChargeEnergy(int chargeValue)
    {
        // 撃破された後なら終了
        if (isDefeat)
            return;

        // ダメージ処理
        weaponEnergies[(int)nowWeapon] += chargeValue;
        if (weaponEnergies[(int)nowWeapon] > MaxEnergy)
            weaponEnergies[(int)nowWeapon] = MaxEnergy;
        // 武器エネルギーゲージ表示更新
        energyGage.fillAmount = (float)weaponEnergies[(int)nowWeapon] / MaxEnergy;
    }

    /// <summary>
	/// アクターをその場で復活させる
	/// </summary>
	public void RevivalActor()
    {
        // HP回復
        nowHP = maxHP;
        // HPゲージの表示を更新する
        float hpRatio = (float)nowHP / maxHP;
        hpGage.DOFillAmount(hpRatio, 0.5f);

        // 無敵時間発生
        invincibleTime = InvicibleTime;
        if (invincibleTime > 0.0f)
            actorSprite.StartBlinking(); // 点滅開始

        // 被撃破時に変えた部分を戻す
        isDefeat = false;
        rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        // 被撃破演出開始
        actorSprite.StopDefeatAnim();
    }

    /// <summary>
	/// 攻撃ボタン入力時処理(強制呼び出し用)
	/// </summary>
	public void StartShotActionImmediately()
    {
        // 攻撃を発射
        switch (nowWeapon)
        {
            case ActorWeaponType.Normal:
                // 通常攻撃
                ShotAction_Normal();
                break;
            case ActorWeaponType.Doggy:
                // 犬騎乗
                ShotAction_Doggy();
                break;
            case ActorWeaponType.Tackle:
                // タックル
                ShotAction_Tackle();
                break;
            case ActorWeaponType.Windblow:
                // 突風
                ShotAction_Windblow();
                break;
            case ActorWeaponType.IceBall:
                // 雪玉
                ShotAction_IceBall();
                break;
            case ActorWeaponType.Lightning:
                // 稲妻
                ShotAction_Lightning();
                break;
            case ActorWeaponType.WaterRing:
                // 水の輪
                ShotAction_WaterRing();
                break;
            case ActorWeaponType.Laser:
                // レーザー
                ShotAction_Laser();
                break;
        }
    }
}
