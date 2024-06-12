using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �A�N�^�[�̃X�v���C�g��ݒ肷��N���X
/// </summary>
public class ActorSprite : MonoBehaviour
{
    private ActorController actorController; // �A�N�^�[����N���X
    private SpriteRenderer spriteRenderer; // �A�N�^�[��SpriteRenderer
    public GameObject defeatParticlePrefab = null; // �팂�j�p�[�e�B�N��Prefab

    // �摜�f�ގQ��
    public List<Sprite> walkAnimationRes; // ���s�A�j���[�V����(������*�R�}��)
    public List<Sprite> stuckSpriteRes; // �X�^���X�v���C�g(������)
    public List<Sprite> swimAnimationRes; // ���j�A�j���[�V����(�R�}��)
    public Sprite doggySpriteRes; // ���R��X�v���C�g

    // �e��ϐ�
    private float walkAnimationTime; // ���s�A�j���[�V�����o�ߎ���
    private int walkAnimationFrame; // ���s�A�j���[�V�����̌��݂̃R�}�ԍ�
    private Tween blinkTween;   // �_�ŏ���Tween
    private Tween defeatTween;	// �팂�jTween
    public bool stuckMode;      // �X�^���摜�\�����[�h

    // �萔��`
    private const int WalkAnimationNum = 3; // ���s�A�j���[�V������1��ނ�����̖���
    private const float WalkAnimationSpan = 0.3f; // ���s�A�j���[�V�����̃X�v���C�g�؂�ւ�����

    // �������֐�(ActorController.cs����ďo)
    public void Init(ActorController _actorController)
    {
        // �Q�Ǝ擾
        actorController = _actorController;
        spriteRenderer = actorController.GetComponent<SpriteRenderer>();
    }

    // Update
    void Update()
    {
        // �팂�j���Ȃ�I��
        if (actorController.isDefeat)
            return;

        // ���R��摜�\�����[�h���Ȃ猢�R��摜��\��
        if (actorController.doggyMode)
        {
            spriteRenderer.sprite = doggySpriteRes;
            return;
        }

        // �X�^���摜�\�����[�h���Ȃ�X�^���摜��\��
        if (stuckMode)
        {
            spriteRenderer.sprite = stuckSpriteRes[(int)actorController.nowWeapon];
            return;
        }

        // ���s�A�j���[�V�������Ԃ��o��(���ړ����Ă���Ԃ̂�)
        if (Mathf.Abs(actorController.xSpeed) > 0.0f)
            walkAnimationTime += Time.deltaTime;
        // ���s�A�j���[�V�����R�}�����v�Z
        if (walkAnimationTime >= WalkAnimationSpan)
        {
            walkAnimationTime -= WalkAnimationSpan;
            // �R�}���𑝉�
            walkAnimationFrame++;
            // �R�}�������s�A�j���[�V�����������z���Ă���Ȃ�0�ɖ߂�
            if (walkAnimationFrame >= WalkAnimationNum)
                walkAnimationFrame = 0;
        }

        // ���s�A�j���[�V�����X�V
        if (!actorController.inWaterMode)
        {// �n��
            spriteRenderer.sprite = walkAnimationRes[(int)actorController.nowWeapon * WalkAnimationNum + walkAnimationFrame];
        }
        else
        {// ����
            spriteRenderer.sprite = swimAnimationRes[walkAnimationFrame];
        }
    }

    /// <summary>
	/// �_�ŊJ�n����
	/// </summary>
	public void StartBlinking()
    {
        // DoTween���g�����_�ŏ���
        blinkTween = spriteRenderer.DOFade(0.0f, 0.15f) // 1�񕪂̍Đ�����:0.15�b
            .SetDelay(0.3f) // 0.3�b�x��
            .SetEase(Ease.Linear) // ���`�ω�
            .SetLoops(-1, LoopType.Yoyo);    // �������[�v�Đ�(������͋t�Đ�)
    }
    /// <summary>
    /// �팂�j���o�J�n
    /// </summary>
    public void StartDefeatAnim()
    {
        // �팂�j�p�[�e�B�N���𐶐�
        var obj = Instantiate(defeatParticlePrefab);
        obj.transform.position = transform.position;
        // �팂�j�X�v���C�g�\��
        spriteRenderer.sprite = stuckSpriteRes[0];
        // �_�ŉ��o�I��
        if (blinkTween != null)
            blinkTween.Kill();
        // �X�v���C�g��\�����A�j���[�V����(DOTween)
        defeatTween = spriteRenderer.DOFade(0.0f, 2.0f); // 2.0�b�����ăX�v���C�g�̔񓧖��x��0.0f�ɂ���
    }
    /// <summary>
    /// �팂�j���o�I��
    /// </summary>
    public void StopDefeatAnim()
    {
        // �팂�jTween�I��
        if (defeatTween != null)
            defeatTween.Kill();
        defeatTween = null;
        // �\���F��߂�
        spriteRenderer.color = Color.white;
    }

    /// <summary>
    /// �_�ŏI������
    /// </summary>
    public void EndBlinking()
    {
        // DoTween�̓_�ŏ������I��������
        if (blinkTween != null)
        {
            blinkTween.Kill(); // Tween���I��
            spriteRenderer.color = Color.white; // �F�����ɖ߂�
            blinkTween = null;  // Tween����������
        }
    }
}
