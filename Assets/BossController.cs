using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public enum BossState { Deactivated, WakingUp, Idle, Walk, Attack1, Attack2, JumpAttack, FallAttack, Dead }
    private BossState currentState;

    [Header("�÷��̾�")]
    public Transform player;

    [Header("���� ����")]
    public int maxHealth = 1000;
    private int currentHealth;
    public int attackDamage = 24;
    public float walkSpeed = 2f;
    public float attackRange = 2f;
    public float �������� = 10f;

    [Header("UI ���")]
    public RectTransform bossUIPanel;
    public float uiMoveDistance = 200f;
    public float uiMoveSpeed = 300f;
    private Vector2 uiOriginalPos;
    private Vector2 uiTargetPos;

    [Header("BGM �� ���� ����")]
    public AudioSource normalBgmSource;
    public AudioSource bossBgmSource;
    public AudioSource sfxAudioSource;      // �ܹ߼� ȿ������
    public AudioSource movementAudioSource; // �ȴ� �Ҹ� �� �������� (�߰�)
    public AudioClip walkSound;             // �ȴ� �Ҹ� Ŭ��
    // A1S, A2S �� ���� ���� Ŭ��
    public AudioClip A1S;
    public AudioClip A2S;
    public AudioClip A3S;
    public AudioClip A4S;
    public float bgmFadeDuration = 2f;
    private bool isBgmFading = false;

    [Header("���� ������")]
    public Collider2D activationTrigger;
    public BoxCollider2D[] ����������;

    [Header("���� ������Ʈ")]
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private bool isAttack = false;
    private bool isJump = false;
    private bool hasEncountered = false; 

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;

        if (bossUIPanel != null)
        {
            uiOriginalPos = bossUIPanel.anchoredPosition;
            uiTargetPos = uiOriginalPos + new Vector2(0, uiMoveDistance);
        }
        currentState = BossState.Deactivated;
    }

    void Update()
    {
        if (currentState == BossState.Deactivated || currentState == BossState.Dead) return;

        if (currentState == BossState.Idle) { Idle(); }
        else if (currentState == BossState.Walk) { Walk(); }
    }

    void ChangeState(BossState newState)
    {
        if (currentState == BossState.Dead && newState != BossState.Dead) return;

        // --- �ȴ� �Ҹ� ���� ���� ---
        if (currentState == BossState.Walk && newState != BossState.Walk)
        {
            if (movementAudioSource != null && movementAudioSource.isPlaying)
            {
                movementAudioSource.Stop();
            }
        }

        currentState = newState;
        anim.Play(newState.ToString());

        // --- �ȴ� �Ҹ� �ѱ� ���� ---
        if (newState == BossState.Walk)
        {
            if (movementAudioSource != null && !movementAudioSource.isPlaying && walkSound != null)
            {
                movementAudioSource.clip = walkSound;
                movementAudioSource.loop = true;
                movementAudioSource.Play();
            }
        }
    }

    // --- Ȱ��ȭ, ������, ��� ó�� ---
    void ActivateBoss()
    {
        if (currentState != BossState.Deactivated) return;
        spriteRenderer.enabled = true;
        if (activationTrigger != null) { activationTrigger.enabled = false; }
        ChangeState(BossState.FallAttack); 
        Invoke("StartAI", 0.5f); 
    }


    void StartAI()
    {
        if (bossUIPanel != null) StartCoroutine(MoveUI(uiTargetPos, uiMoveSpeed));
        ChangeToBossBGM();
        ChangeState(BossState.Idle);
    }

    public void TakeDamage(int damage)
    {
        if (currentState == BossState.Dead) return;
        currentHealth -= damage;
        // ���� ü�� �� ������Ʈ...
        if (currentHealth <= 0) { Die(); }
    }

    void Die()
    {
        ChangeState(BossState.Dead);
        rb.linearVelocity = Vector2.zero;
        foreach (var col in GetComponentsInChildren<Collider2D>()) { col.enabled = false; }
        if (bossUIPanel != null) StartCoroutine(MoveUI(uiOriginalPos, uiMoveSpeed * 5f));
        RevertToNormalBGM();
        Destroy(gameObject, 3f);
    }

    void Idle()
    {
        if (!hasEncountered)
        {
            hasEncountered = true;
        }
        ChangeState(BossState.Walk);
    }

    void Walk() { NoAttackCollider(); Vector2 direction = (player.position - transform.position).normalized; if (direction.x < 0) { transform.localScale = new Vector3(-1 * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); } else { transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); } rb.linearVelocity = new Vector2(direction.x * walkSpeed, rb.linearVelocity.y); if (Vector2.Distance(transform.position, player.position) < attackRange) { float choiceAttack = Random.Range(0f, 1f); if (choiceAttack <= 0.3f) { ChangeState(BossState.Attack1); PlaySoundEffect(A1S); } else if (choiceAttack <= 0.6f) { ChangeState(BossState.Attack2); PlaySoundEffect(A2S); } else if (!isJump) { ChangeState(BossState.JumpAttack); PlaySoundEffect(A3S); } } }
    public void A1() { ����������[0].enabled = true; }
    public void A2() { ����������[1].enabled = true; }
    void Attack1() { if (!isAttack) { NoAttackCollider(); rb.linearVelocity = Vector2.zero; isAttack = true; PlaySoundEffect(A1S); } }
    void Attack2() { if (!isAttack) { NoAttackCollider(); rb.linearVelocity = Vector2.zero; isAttack = true; PlaySoundEffect(A2S); } }
    void JumpAttack() { if (!isAttack) { NoAttackCollider(); rb.linearVelocity = Vector2.zero; anim.Play("JumpAttack"); isAttack = true; PlaySoundEffect(A3S); } }
    void FallAttack() { if (!isJump) { ChangeState(BossState.FallAttack); PlaySoundEffect(A4S); ����������[2].enabled = true; Vector2 direction = (player.position - transform.position).normalized; if (direction.x < 0) { transform.localScale = new Vector3(-1 * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); } else { transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); } rb.linearVelocity = Vector2.zero; isJump = true; } }
    void AnimStop() { anim.Play("stop"); }
    void AnimIdle() { anim.speed = 0; anim.Play("Idle"); anim.speed = 1; ChangeState(BossState.Walk); ReturnToIdle(); }
    void AnimRestart() { this.transform.position = new Vector3(player.position.x, this.transform.position.y, this.transform.position.z); Invoke("FallAttack", 0.2f); }
    void ReturnToIdle() { isAttack = false; isJump = false; ChangeState(BossState.Idle); }
    void NoAttackCollider() { for (int i = 0; i < ����������.Length; i++) { ����������[i].enabled = false; } }
    void ChangeToBossBGM() { if (!isBgmFading && normalBgmSource != null && bossBgmSource != null) { StartCoroutine(CrossfadeBGM(normalBgmSource, bossBgmSource)); } }
    void RevertToNormalBGM() { if (normalBgmSource != null && bossBgmSource != null) { StartCoroutine(CrossfadeBGM(bossBgmSource, normalBgmSource)); } }
    IEnumerator CrossfadeBGM(AudioSource fadeOutSource, AudioSource fadeInSource) { isBgmFading = true; float startVolume = fadeOutSource.volume; fadeInSource.volume = 0; fadeInSource.Play(); float elapsed = 0f; while (elapsed < bgmFadeDuration) { elapsed += Time.deltaTime; float progress = elapsed / bgmFadeDuration; fadeOutSource.volume = Mathf.Lerp(startVolume, 0f, progress); fadeInSource.volume = Mathf.Lerp(0f, startVolume, progress); yield return null; } fadeOutSource.Stop(); fadeInSource.volume = startVolume; isBgmFading = false; }
    IEnumerator MoveUI(Vector2 targetPos, float speed) { while (Vector2.Distance(bossUIPanel.anchoredPosition, targetPos) > 0.1f) { bossUIPanel.anchoredPosition = Vector2.MoveTowards(bossUIPanel.anchoredPosition, targetPos, speed * Time.deltaTime); yield return null; } bossUIPanel.anchoredPosition = targetPos; }
    public void PlaySoundEffect(AudioClip clipToPlay)
    {
        // ����� �ҽ��� Ŭ���� ��������� ���� ������ ���� �������� ����
        if (sfxAudioSource == null || clipToPlay == null)
        {
            Debug.LogWarning("����� �ҽ� �Ǵ� Ŭ���� ����ֽ��ϴ�.");
            return;
        }

        // ������ Ŭ���� �� �� ���
        sfxAudioSource.PlayOneShot(clipToPlay);
    }
    private void OnTriggerEnter2D(Collider2D other) { if (!other.CompareTag("Player")) return; if (currentState == BossState.Deactivated) { ActivateBoss(); return; } PlayerController playerController = other.GetComponent<PlayerController>(); if (playerController != null) { playerController.TakeDamage(attackDamage, transform.position); NoAttackCollider(); } }
}