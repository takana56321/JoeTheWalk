using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// �A�N�^�[����E����N���X
/// </summary>
public class ActorController : MonoBehaviour
{
    // �I�u�W�F�N�g�E�R���|�[�l���g�Q��
    private StageManager stageManager;
    private Rigidbody2D rigidbody2D; // Rigidbody2D�R���|�[�l���g�ւ̎Q��
    private SpriteRenderer spriteRenderer;
    private ActorGroundSensor groundSensor; // �A�N�^�[�ڒn����N���X
    private ActorSprite actorSprite; // �A�N�^�[�X�v���C�g�ݒ�N���X
    public CameraController cameraController; // �J��������N���X
    public Image hpGage; // HP�Q�[�W
    public Image energyGage = null; // ����G�l���M�[�Q�[�W
    public Image energyGageIcon = null; // ����G�l���M�[�Q�[�W�A�C�R��

    // �ݒ荀��
    [Header("true:���ꂪ���郂�[�h")]
    public bool icyGroundMode;
    [Header("�e����Ŏg�p����v���n�u���X�g(��`�̏��Ԃɐݒ�)")]
    public List<GameObject> weaponBulletPrefabs;
    [Header("�e����̃G�l���M�[�Q�[�W�̉摜")]
    public List<Sprite> weaponIconSprites;
    [Header("�e����̃G�l���M�[�Q�[�W�̐F")]
    public List<Color> weaponGageColors;
    [Header("�e����̏���G�l���M�[��")]
    public List<int> weaponEnergyCosts;
    [Header("�e����̘A�ˊԊu(�b)")]
    public List<float> weaponIntervals;

    // �̗͕ϐ�
    [HideInInspector] public int nowHP; // ����HP
    [HideInInspector] public int maxHP; // �ő�HP

    // �����ϐ�
    [HideInInspector] public ActorWeaponType nowWeapon;
    private int[] weaponEnergies; // ����̎c��G�l���M�[�f�[�^(���ꂼ��ő�l��MaxEnergy)
    private float weaponRemainInterval; // ���킪���ɔ��ˉ\�ɂȂ�܂ł̎c�莞��(�b)

    // �ړ��֘A�ϐ�
    [HideInInspector] public float xSpeed; // X�����ړ����x
    [HideInInspector] public bool rightFacing; // �����Ă������(true.�E���� false:������)
    private float remainJumpTime;   // �󒆂ł̃W�����v���͎c���t����
    private bool canDoubleJump; // ��i�W�����v�\�t���O

    // ���̑��ϐ�
    private float remainStuckTime; // �c��d������(0�ȏゾ�ƍs���ł��Ȃ�)
    private float invincibleTime;   // �c�薳�G����(�b)
    [HideInInspector] public bool isDefeat; // true:���j���ꂽ(�Q�[���I�[�o�[)
    [HideInInspector] public bool inWaterMode; // true:�������[�h(���\�b�h����ύX)
    [HideInInspector] public bool doggyMode; // ���R�惂�[�h
    [HideInInspector] public bool unmovableMode; // �s���֎~���[�h

    // �萔��`
    private const int InitialHP = 20;           // ����HP(�ő�HP)
    private const int MaxEnergy = 20;			// ����G�l���M�[�̍ő�l
    private const float InvicibleTime = 2.0f;   // ��_���[�W����̖��G����(�b)
    private const float StuckTime = 0.5f;       // ��_���[�W����̍d������(�b)
    private const float KnockBack_X = 2.5f;     // ��_���[�W���m�b�N�o�b�N��(x����)
    private const float WaterModeDecelerate_X = 0.8f;// �����ł�X�������x�{��
    private const float WaterModeDecelerate_Y = 0.92f;// �����ł�X�������x�{��

    // �A�N�^�[������`
    public enum ActorWeaponType
    {
        Normal,     // (�ʏ�)
        Doggy,      // ���R��
        Tackle,     // �^�b�N��
        Windblow,   // �˕�
        IceBall,    // ���
        Lightning,  // ���
        WaterRing,  // ���̗�
        Laser,      // ���[�U�[
        _Max,
    }

    // Start�i�I�u�W�F�N�g�L��������1�x���s�j
    void Start()
    {
        // �R���|�[�l���g�Q�Ǝ擾
        stageManager = GetComponentInParent<StageManager>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        groundSensor = GetComponentInChildren<ActorGroundSensor>();
        actorSprite = GetComponent<ActorSprite>();

        // �z���R���|�[�l���g������
        actorSprite.Init(this);

        // �J���������ʒu
        cameraController.SetPosition(transform.position);

        // ����G�l���M�[������
        weaponEnergies = new int[(int)ActorWeaponType._Max];
        for (int i = 0; i < (int)ActorWeaponType._Max; i++)
            weaponEnergies[i] = MaxEnergy;
        ApplyWeaponChange(); // ���������𔽉f

        // �ϐ�������
        rightFacing = true; // �ŏ��͉E����
        nowHP = maxHP = InitialHP; // ����HP
        hpGage.fillAmount = 1.0f; // HP�Q�[�W�̏���FillAmount
        canDoubleJump = true; // ��i�W�����v�\
    }

    // Update�i1�t���[�����Ƃ�1�x�����s�j
    void Update()
    {
        // �s���֎~���[�h�Ȃ�I��
        if (unmovableMode)
            return;
        // ���j���ꂽ��Ȃ�I��
        if (isDefeat)
            return;

        // ���G���Ԃ��c���Ă���Ȃ猸��
        if (invincibleTime > 0.0f)
        {
            invincibleTime -= Time.deltaTime;
            if (invincibleTime <= 0.0f)
            {// ���G���ԏI��������
                actorSprite.EndBlinking(); // �_�ŏI��
            }
        }
        // �d�����ԏ���
        if (remainStuckTime > 0.0f)
        {// �d�����Ԍ���
            remainStuckTime -= Time.deltaTime;
            if (remainStuckTime <= 0.0f)
            {// �X�^�����ԏI��������
                actorSprite.stuckMode = false;
            }
            else
                return;
        }

        // ���E�ړ�����
        MoveUpdate();
        // �W�����v���͏���
        JumpUpdate();

        // ����؂�ւ�����
        ChangeWeaponUpdate();

        // �U���\�܂ł̎c�莞�Ԍ���
        if (weaponRemainInterval > 0.0f)
            weaponRemainInterval -= Time.deltaTime;
        // �U�����͏���
        StartShotAction();

        // �⓹�Ŋ���Ȃ����鏈��
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation; // Rigidbody�̋@�\�̂�����]�����͏�ɒ�~
        if (groundSensor.isGround && !(Input.GetKey(KeyCode.UpArrow) || stageManager.virtualButton_Jump.input))
        {
            // �⓹��o���Ă��鎞�㏸�͂������Ȃ��悤�ɂ��鏈��
            if (rigidbody2D.velocity.y > 0.0f)
            {
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0.0f);
            }
            // �⓹�ɗ����Ă��鎞���藎���Ȃ��悤�ɂ��鏈��
            if (Mathf.Abs(xSpeed) < 0.1f && !icyGroundMode)
            {
                // Rigidbody�̋@�\�̂����ړ��Ɖ�]���~
                rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            }
        }

        // �J�����Ɏ��g�̍��W��n��
        cameraController.SetPosition(transform.position);
    }

    #region �ړ��֘A
    /// <summary>
	/// Update����Ăяo����鍶�E�ړ����͏���
	/// </summary>
	private void MoveUpdate()
    {
        // X�����ړ�����
        if (Input.GetKey(KeyCode.RightArrow) || stageManager.virtualButton_Right.input)
        {// �E�����̈ړ�����
         // X�����ړ����x���v���X�ɐݒ�
            xSpeed = 6.0f;

            // �E�����t���Oon
            rightFacing = true;

            // �X�v���C�g��ʏ�̌����ŕ\��
            spriteRenderer.flipX = false;
        }
        else if (Input.GetKey(KeyCode.LeftArrow) || stageManager.virtualButton_Left.input)
        {// �������̈ړ�����
         // X�����ړ����x���}�C�i�X�ɐݒ�
            xSpeed = -6.0f;

            // �E�����t���Ooff
            rightFacing = false;

            // �X�v���C�g�����E���]���ĕ\��
            spriteRenderer.flipX = true;
        }
        else
        {// ���͂Ȃ�
         // X�����̈ړ����~
            xSpeed = 0.0f;
        }
    }

    /// <summary>
	/// Update����Ăяo�����W�����v���͏���
	/// </summary>
	private void JumpUpdate()
    {
        // �󒆂ł̃W�����v���͎�t���Ԍ���
        if (remainJumpTime > 0.0f)
            remainJumpTime -= Time.deltaTime;

        // �W�����v����
        if (Input.GetKeyDown(KeyCode.UpArrow) || stageManager.virtualButton_Jump.down)
        {
            if (groundSensor.isGround || inWaterMode)
            {
                // �n��܂��͐����ł̃W�����v�J�n
                JumpAction();
                canDoubleJump = true; // ��i�W�����v�����Z�b�g
            }
            else if (canDoubleJump)
            {
                // ��i�W�����v�J�n
                JumpAction();
                canDoubleJump = false; // ��i�W�����v������
            }
        }
        else if (Input.GetKey(KeyCode.UpArrow) || stageManager.virtualButton_Jump.input)
        {// �W�����v���i�W�����v���͂𒷉�������ƌp�����ď㏸���鏈���j
         // �󒆂ł̃W�����v���͎󂯕t�����Ԃ��c���ĂȂ��Ȃ�I��
            if (remainJumpTime <= 0.0f)
                return;
            // �ڒn���Ă���Ȃ�I��
            if (groundSensor.isGround)
                return;

            // �W�����v�͉��Z�ʂ��v�Z
            float jumpAddPower = 30.0f * Time.deltaTime; // Update()�͌Ăяo���Ԋu���قȂ�̂�Time.deltaTime���K�v
            // �W�����v�͉��Z��K�p
            rigidbody2D.velocity += new Vector2(0.0f, jumpAddPower);
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow) || stageManager.virtualButton_Jump.up)
        {// �W�����v���͏I��
            remainJumpTime = -1.0f;
        }
    }

    /// <summary>
    /// �W�����v�A�N�V�����̎��s
    /// </summary>
    private void JumpAction()
    {
        // �W�����v�͂��v�Z
        float jumpPower = 10.0f;
        // �W�����v�͂�K�p
        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpPower);

        // �󒆂ł̃W�����v���͎󂯕t�����Ԑݒ�
        remainJumpTime = 0.25f;
    }

    // FixedUpdate�i��莞�Ԃ��Ƃ�1�x�����s�E�������Z�p�j
    private void FixedUpdate()
    {
        // �ړ����x�x�N�g�������ݒl����擾
        Vector2 velocity = rigidbody2D.velocity;
        // X�����̑��x����͂��猈��
        velocity.x = xSpeed;
        // �X���X�e�[�W�Ȃ�ڒn���Ɋ���悤�ȑ��x�ݒ�ɂ���
        if (icyGroundMode && groundSensor.isGround)
            velocity.x = Mathf.Lerp(xSpeed, rigidbody2D.velocity.x, 0.99f);

        // �������[�h�ł̑��x
        if (inWaterMode)
        {
            velocity.x *= WaterModeDecelerate_X;
            velocity.y *= WaterModeDecelerate_Y;
        }

        // �v�Z�����ړ����x�x�N�g����Rigidbody2D�ɔ��f
        rigidbody2D.velocity = velocity;
    }

    /// <summary>
	/// �������[�h���Z�b�g����
	/// </summary>
	/// <param name="mode">true:�����ɂ���</param>
	public void SetWaterMode(bool mode)
    {
        // �������[�h
        inWaterMode = mode;
        // ���R�惂�[�h�Ȃ�gravityScale��ς��Ȃ�
        if (doggyMode)
            return;
        // �����ł̏d��
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

    #region �����֘A
    /// <summary>
    /// Update����Ăяo����镐��؂�ւ�����
    /// </summary>
    private void ChangeWeaponUpdate()
    {
        // ����؂�ւ�
        if (Input.GetKeyDown(KeyCode.A) || stageManager.virtualButton_ChangeWeaponCCW.down)
        {// 1�O�ɐ؂�ւ�
         // �O�̉���ςݕ��킪������܂őI�𕐊���f�N�������g
            do
            {
                if (nowWeapon == ActorWeaponType.Normal)
                    nowWeapon = ActorWeaponType._Max;
                nowWeapon--;
            }
            while (!Data.instance.weaponUnlocks[(int)nowWeapon]);
            // ���R�撆�Ȃ����
            if (doggyMode)
                ShotAction_Doggy();
            // ����ύX�𔽉f
            ApplyWeaponChange();
        }
        else if (Input.GetKeyDown(KeyCode.S) || stageManager.virtualButton_ChangeWeaponCW.down)
        {// 1���ɐ؂�ւ�
         // ���̉���ςݕ��킪������܂őI�𕐊���C���N�������g
            do
            {
                nowWeapon++;
                if (nowWeapon == ActorWeaponType._Max)
                    nowWeapon = ActorWeaponType.Normal;
            }
            while (!Data.instance.weaponUnlocks[(int)nowWeapon]);
            // ���R�撆�Ȃ����
            if (doggyMode)
                ShotAction_Doggy();
            // ����ύX�𔽉f
            ApplyWeaponChange();
        }
    }

    /// <summary>
    /// ���ꕐ��̕ύX�𔽉f����
    /// </summary>
    public void ApplyWeaponChange()
    {
        // �G�l���M�[�Q�[�W�\��(�ʏ핐��ȊO)
        if (nowWeapon == ActorWeaponType.Normal)
            energyGage.transform.parent.gameObject.SetActive(false);
        else
            energyGage.transform.parent.gameObject.SetActive(true);

        // �Q�[�W�̐F�𔽉f
        energyGage.color = weaponGageColors[(int)nowWeapon];
        // �Q�[�W�̗ʂ𔽉f
        energyGage.fillAmount = (float)weaponEnergies[(int)nowWeapon] / MaxEnergy;
        // �Q�[�W�̃A�C�R����ݒ�
        energyGageIcon.sprite = weaponIconSprites[(int)nowWeapon];
    }
    #endregion

    #region �퓬�֘A
    /// <summary>
    /// �_���[�W���󂯂�ۂɌĂяo�����
    /// </summary>
    /// <param name="damage">�_���[�W��</param>
    public void Damaged(int damage)
    {
        // ���j���ꂽ��Ȃ�I��
        if (isDefeat)
            return;

        // �������G���Ԓ��Ȃ�_���[�W����
        if (invincibleTime > 0.0f)
            return;
        // �Q�[���I�[�o�[���Ȃ�I��
        //if (gameManager.isGameOver)
        //	return;

        // �_���[�W����
        nowHP -= damage;
        // HP�Q�[�W�̕\�����X�V����
        float hpRatio = (float)nowHP / maxHP;
        hpGage.DOFillAmount(hpRatio, 0.5f);

        // HP0�Ȃ�Q�[���I�[�o�[����
        if (nowHP <= 0)
        {
            isDefeat = true;
            // �팂�j���o�J�n
            actorSprite.StartDefeatAnim();
            // �������Z���~
            rigidbody2D.velocity = Vector2.zero;
            xSpeed = 0.0f;
            rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            // �Q�[���I�[�o�[����
            GetComponentInParent<StageManager>().GameOver();
            return;
        }

        // �X�^���d��
        remainStuckTime = StuckTime;
        actorSprite.stuckMode = true;

        // �m�b�N�o�b�N����
        // �m�b�N�o�b�N�́E��������
        float knockBackPower = KnockBack_X;
        if (rightFacing)
            knockBackPower *= -1.0f;
        // �m�b�N�o�b�N�K�p
        xSpeed = knockBackPower;

        // ���G���Ԕ���
        invincibleTime = InvicibleTime;
        if (invincibleTime > 0.0f)
            actorSprite.StartBlinking(); // �_�ŊJ�n
    }

    /// <summary>
    /// �U���{�^�����͎�����
    /// </summary>
    public void StartShotAction()
    {
        // �U���{�^�������͂���Ă��Ȃ��Ȃ�I��
        if (!(Input.GetKeyDown(KeyCode.Z) || stageManager.virtualButton_Fire.down))
            return;
        // ����G�l���M�[������Ȃ��Ȃ�U�����Ȃ�
        if (weaponEnergies[(int)nowWeapon] <= 0)
            return;
        // �U���\�܂ł̎��Ԃ��c���Ă���Ȃ�I��
        if (weaponRemainInterval > 0.0f)
            return;

        // ����G�l���M�[����
        weaponEnergies[(int)nowWeapon] -= weaponEnergyCosts[(int)nowWeapon];
        if (weaponEnergies[(int)nowWeapon] < 0)
            weaponEnergies[(int)nowWeapon] = 0;
        // ����G�l���M�[�Q�[�W�\���X�V
        energyGage.fillAmount = (float)weaponEnergies[(int)nowWeapon] / MaxEnergy;
        // ���e���ˉ\�܂ł̎c�莞�Ԑݒ�
        weaponRemainInterval = weaponIntervals[(int)nowWeapon];

        // �U���𔭎�
        switch (nowWeapon)
        {
            case ActorWeaponType.Normal:
                // �ʏ�U��
                ShotAction_Normal();
                break;
            case ActorWeaponType.Doggy:
                // ���R��
                ShotAction_Doggy();
                break;
            case ActorWeaponType.Tackle:
                // �^�b�N��
                ShotAction_Tackle();
                break;
            case ActorWeaponType.Windblow:
                // �˕�
                ShotAction_Windblow();
                break;
            case ActorWeaponType.IceBall:
                // ���
                ShotAction_IceBall();
                break;
            case ActorWeaponType.Lightning:
                // ���
                ShotAction_Lightning();
                break;
            case ActorWeaponType.WaterRing:
                // ���̗�
                ShotAction_WaterRing();
                break;
            case ActorWeaponType.Laser:
                // ���[�U�[
                ShotAction_Laser();
                break;
        }
    }

    /// <summary>
	/// �ˌ��A�N�V�����F�ʏ�U��
	/// </summary>
	private void ShotAction_Normal()
    {
        // �e�̕������擾
        float bulletAngle = 0.0f; // �E����
                                  // �A�N�^�[���������Ȃ�e���������ɐi��
        if (!rightFacing)
            bulletAngle = 180.0f;

        // �e�ۃI�u�W�F�N�g�����E�ݒ�
        GameObject obj = Instantiate(weaponBulletPrefabs[(int)ActorWeaponType.Normal], transform.position, Quaternion.identity);
        obj.GetComponent<ActorNormalShot>().Init(
            12.0f, // ���x
            bulletAngle, // �p�x
            1, // �_���[�W��
            5.0f, // ���ݎ���
            nowWeapon); // �g�p����

        // SE�Đ�
        SEPlayer.instance.PlaySE(SEPlayer.SEName.ActorShot_Normal);
    }
    /// <summary>
    /// �ˌ��A�N�V�����F���R��
    /// </summary>
    private void ShotAction_Doggy()
    {
        if (!doggyMode)
        {// ���ɏ��
            doggyMode = true;
            rigidbody2D.gravityScale = 0.5f;
        }
        else
        {// ������~���
            doggyMode = false;
            rigidbody2D.gravityScale = 1.0f;
        }
    }
    /// <summary>
    /// �ˌ��A�N�V�����F�^�b�N��
    /// </summary>
    private void ShotAction_Tackle()
    {
        // �e�̕������擾
        float bulletAngle = 0.0f; // �E����
                                  // �A�N�^�[���������Ȃ�e���������ɐi��
        if (!rightFacing)
            bulletAngle = 180.0f;

        // �e�ۃI�u�W�F�N�g�����E�ݒ�
        GameObject obj = Instantiate(weaponBulletPrefabs[(int)ActorWeaponType.Tackle], transform.position, Quaternion.identity);
        obj.GetComponent<ActorNormalShot>().Init(
            20.0f, // ���x
            bulletAngle, // �p�x
            1, // �_���[�W��
            0.3f, // ���ݎ���
            nowWeapon); // �g�p����
        if (!rightFacing)
            obj.GetComponent<SpriteRenderer>().flipX = true;

        // ��l���̓ːi�ړ�
        Vector3 moveVector = new Vector3(1.2f, 0.25f, 0.0f);
        if (!rightFacing)
            moveVector.x *= -1.0f;
        rigidbody2D.MovePosition(transform.position + moveVector);
        groundSensor.isGround = false;

        // ���G���Ԕ���
        invincibleTime = 0.6f;
    }
    /// <summary>
    /// �ˌ��A�N�V�����F�˕�
    /// </summary>
    private void ShotAction_Windblow()
    {
        // �e�̕������擾
        float bulletAngle = 0.0f; // �E����
                                  // �A�N�^�[���������Ȃ�e���������ɐi��
        if (!rightFacing)
            bulletAngle = 180.0f;

        // �e�ۃI�u�W�F�N�g�����E�ݒ�
        GameObject obj = Instantiate(weaponBulletPrefabs[(int)ActorWeaponType.Windblow], transform.position, Quaternion.identity);
        obj.GetComponent<ActorNormalShot>().Init(
            16.0f, // ���x
            bulletAngle, // �p�x
            1, // �_���[�W��
            3.0f, // ���ݎ���
            nowWeapon); // �g�p����
    }
    /// <summary>
    /// �ˌ��A�N�V�����F���
    /// </summary>
    private void ShotAction_IceBall()
    {
        // �e�̏����x�N�g����ݒ�
        Vector2 velocity = new Vector2(14.0f, 8.0f);
        if (!rightFacing)
            velocity.x *= -1.0f;

        // �e�ۃI�u�W�F�N�g�����E�ݒ�
        GameObject obj = Instantiate(weaponBulletPrefabs[(int)ActorWeaponType.IceBall], transform.position, Quaternion.identity);
        obj.GetComponent<ActorNormalShot>().Init(
            0.0f, // ���x(rigidbody�Œe�𓮂����̂Őݒ�s�v)
            0.0f, // �p�x(rigidbody�Œe�𓮂����̂Őݒ�s�v)
            1, // �_���[�W��
            5.0f, // ���ݎ���
            nowWeapon); // �g�p����
        obj.GetComponent<Rigidbody2D>().velocity += velocity;
    }
    /// <summary>
    /// �ˌ��A�N�V�����F���
    /// </summary>
    private void ShotAction_Lightning()
    {
        // �e�̔��ˈʒu��ݒ�(��l���̉E��or����)
        Vector3 fixPos = new Vector3(4.0f, 5.0f, 0.0f);
        if (!rightFacing)
            fixPos.x *= -1.0f;

        // �e�ۃI�u�W�F�N�g�����E�ݒ�
        GameObject obj = Instantiate(weaponBulletPrefabs[(int)ActorWeaponType.Lightning], transform.position + fixPos, Quaternion.identity);
        obj.GetComponent<ActorNormalShot>().Init(
            14.0f, // ���x
            270, // �p�x
            2, // �_���[�W��
            5.0f, // ���ݎ���
            nowWeapon); // �g�p����
    }
    /// <summary>
    /// �ˌ��A�N�V�����F���̗�
    /// </summary>
    private void ShotAction_WaterRing()
    {
        // �e�ۃI�u�W�F�N�g�����E�ݒ�
        int bulletNum_Angle = 8; // ���˕�����
        for (int i = 0; i < bulletNum_Angle; i++)
        {
            GameObject obj = Instantiate(weaponBulletPrefabs[(int)ActorWeaponType.WaterRing], transform.position, Quaternion.identity);
            obj.GetComponent<ActorNormalShot>().Init(
                3.0f, // ���x
                (360 / bulletNum_Angle) * i, // �p�x
                1, // �_���[�W��
                2.0f, // ���ݎ���
                nowWeapon); // �g�p����
        }
    }
    /// <summary>
    /// �ˌ��A�N�V�����F���[�U�[
    /// </summary>
    private void ShotAction_Laser()
    {
        // ���[�U�[�I�u�W�F�N�g�����E�ݒ�
        GameObject obj = Instantiate(weaponBulletPrefabs[(int)ActorWeaponType.Laser], transform.position, Quaternion.identity);
        obj.GetComponent<ActorLaser>().Init(
            1, // �_���[�W��
            1.0f); // ���ݎ���
    }
    #endregion


    /// <summary>
	/// �A�N�^�[��HP���񕜂���
	/// </summary>
	/// <param name="healValue">�񕜗�</param>
	public void HealHP(int healValue)
    {
        // ���j���ꂽ��Ȃ�I��
        if (isDefeat)
            return;

        // �_���[�W����
        nowHP += healValue;
        if (nowHP > maxHP)
            nowHP = maxHP;
        // HP�Q�[�W�̕\�����X�V����
        float hpRatio = (float)nowHP / maxHP;
        hpGage.DOFillAmount(hpRatio, 0.5f);
    }

    /// <summary>
	/// �A�N�^�[�̕���G�l���M�[���񕜂���
	/// </summary>
	/// <param name="chargeValue">�񕜗�</param>
	public void ChargeEnergy(int chargeValue)
    {
        // ���j���ꂽ��Ȃ�I��
        if (isDefeat)
            return;

        // �_���[�W����
        weaponEnergies[(int)nowWeapon] += chargeValue;
        if (weaponEnergies[(int)nowWeapon] > MaxEnergy)
            weaponEnergies[(int)nowWeapon] = MaxEnergy;
        // ����G�l���M�[�Q�[�W�\���X�V
        energyGage.fillAmount = (float)weaponEnergies[(int)nowWeapon] / MaxEnergy;
    }

    /// <summary>
	/// �A�N�^�[�����̏�ŕ���������
	/// </summary>
	public void RevivalActor()
    {
        // HP��
        nowHP = maxHP;
        // HP�Q�[�W�̕\�����X�V����
        float hpRatio = (float)nowHP / maxHP;
        hpGage.DOFillAmount(hpRatio, 0.5f);

        // ���G���Ԕ���
        invincibleTime = InvicibleTime;
        if (invincibleTime > 0.0f)
            actorSprite.StartBlinking(); // �_�ŊJ�n

        // �팂�j���ɕς���������߂�
        isDefeat = false;
        rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        // �팂�j���o�J�n
        actorSprite.StopDefeatAnim();
    }

    /// <summary>
	/// �U���{�^�����͎�����(�����Ăяo���p)
	/// </summary>
	public void StartShotActionImmediately()
    {
        // �U���𔭎�
        switch (nowWeapon)
        {
            case ActorWeaponType.Normal:
                // �ʏ�U��
                ShotAction_Normal();
                break;
            case ActorWeaponType.Doggy:
                // ���R��
                ShotAction_Doggy();
                break;
            case ActorWeaponType.Tackle:
                // �^�b�N��
                ShotAction_Tackle();
                break;
            case ActorWeaponType.Windblow:
                // �˕�
                ShotAction_Windblow();
                break;
            case ActorWeaponType.IceBall:
                // ���
                ShotAction_IceBall();
                break;
            case ActorWeaponType.Lightning:
                // ���
                ShotAction_Lightning();
                break;
            case ActorWeaponType.WaterRing:
                // ���̗�
                ShotAction_WaterRing();
                break;
            case ActorWeaponType.Laser:
                // ���[�U�[
                ShotAction_Laser();
                break;
        }
    }
}
