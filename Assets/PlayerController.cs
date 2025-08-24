using System.Collections;

using UnityEngine;

using UnityEngine.Rendering; 

using UnityEngine.Rendering.Universal; 
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour

{
    [Header("사운드 설정")]
    public AudioSource movementAudioSource;
    public AudioSource audioSource;         // 소리를 재생할 오디오 소스
    public AudioClip[] slashSounds;         // 칼질 소리 파일들을 담을 배열
    private int slashSoundIndex = 0;        // 다음에 재생할 소리
    public AudioClip walkSound;             // 걷는 소리



    [Header("플레이어 스탯")]

    public int maxHealth = 100;

    public float knockbackForce = 10f;

    public float stunDuration = 0.2f;      // 넉백 후 입력 불가 시간

    public float invincibilityDuration = 1f; // 총 무적 시간



    [Header("UI 및 컴포넌트")]

    public PlayerHealthBar healthBar;



    [Header("이동")]

    public float moveSpeed = 5f;

    public float jumpForce = 10f;



    [Header("대시")]

    public float dashSpeed = 15f;

    public float dashTime = 0.2f;

    public float dashCooldown = 1f;



    [Header("공격")]

    public Transform attackPoint;

    public float attackRange = 0.5f;

    public int attackDamage = 20;

    public LayerMask enemyLayers;

    public float attackRate = 1f;



    [Header("피격 효과")]

    public Volume volume; // URP의 포스트 프로세싱 볼륨

    public float shakeDuration = 0.2f;

    public float shakeMagnitude = 0.05f;

    public float effectReturnDuration = 0.5f; // 효과가 원래대로 돌아오는 시간



    // --- 내부 변수들 ---

    private Rigidbody2D rb;

    private Animator animator;

    private SpriteRenderer spriteRenderer;

    private Color originalColor;

    private LayerMask groundLayer;

    private float groundRayDistance = 0.2f;

    private float nextAttackTime = 0f;

    private bool facingRight = true;

    private bool canDash = true;

    private int currentHealth;



    // 상태 변수

    private bool isDashing = false;

    private bool isStunned = false;

    private bool isInvincible = false;



    // URP 포스트 프로세싱 효과 변수

    private Vignette vignette;

    private ChromaticAberration chromaticAberration;

    private LensDistortion lensDistortion;



    void Start()

    {

        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        originalColor = spriteRenderer.color;



        // 보스와 물리적 충돌만 무시 (레이어 번호는 직접 확인 후 설정하세요)

        Physics2D.IgnoreLayerCollision(8, 9, true);



        // 체력 바 초기화

        if (healthBar != null)

        {

            healthBar.HandleHealthChanged((float)currentHealth / maxHealth);

        }



        // URP 포스트 프로세싱 효과 찾아오기

        if (volume != null)

        {

            volume.profile.TryGet<Vignette>(out vignette);

            volume.profile.TryGet<ChromaticAberration>(out chromaticAberration);

            volume.profile.TryGet<LensDistortion>(out lensDistortion);

        }

    }



    void Update()

    {

        // 대시 중이거나 스턴 상태일 때는 모든 조작 비활성화

        if (isDashing || isStunned)

        {

            return;

        }
        PlayWalkSound();


        // 공격

        if (Input.GetKeyDown(KeyCode.O) && Time.time >= nextAttackTime)

        {

            Attack();

            nextAttackTime = Time.time + 1f / attackRate;

        }



        // 이동

        float moveInput = Input.GetAxisRaw("Horizontal");

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        animator.SetBool("isMoving", moveInput != 0 && IsGrounded());



        // 방향 전환

        if (moveInput > 0 && !facingRight) Flip();

        else if (moveInput < 0 && facingRight) Flip();



        // 점프

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())

        {

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        }

        animator.SetBool("IsJumping", !IsGrounded());



        // 대시

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && moveInput != 0)

        {

            StartCoroutine(Dash());

        }

    }



    // --- 메인 피격 함수 ---

    public void TakeDamage(int damage, Vector2 attackerPosition)

    {

        if (isInvincible) return; // 무적 상태면 함수 종료



        currentHealth -= damage;

        if (healthBar != null)

        {

            healthBar.HandleHealthChanged((float)currentHealth / maxHealth);

        }



        if (currentHealth <= 0)

        {

            Die();

        }

        else

        {

            // 모든 피격 관련 효과를 처리하는 통합 코루틴 시작

            StartCoroutine(HitSequence(attackerPosition));

        }

    }



    // --- 모든 피격 효과를 순서대로 처리하는 통합 코루틴 ---

    IEnumerator HitSequence(Vector2 attackerPosition)

    {

        // 1. 피격 상태 시작 (무적 & 조작 불가)

        isInvincible = true;

        isStunned = true;



        // 2. 즉발 효과 실행 (넉백, 카메라 셰이크, 포스트 프로세싱)

        StartCoroutine(CameraShake());

        HandlePostProcessingHitEffect();



        Vector2 knockbackDirection;

        if (attackerPosition.x < transform.position.x) { knockbackDirection = Vector2.right; }

        else { knockbackDirection = Vector2.left; }

        rb.linearVelocity = Vector2.zero;

        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);



        // 3. 스턴 시간(조작 불가 시간)만큼 대기

        yield return new WaitForSeconds(stunDuration);

        isStunned = false; // 스턴 해제, 조작 가능



        // 4. 총 무적 시간 동안 깜빡임 효과

        float remainingInvincibilityTime = invincibilityDuration - stunDuration;

        float flashInterval = 0.1f;

        int flashCount = Mathf.RoundToInt(remainingInvincibilityTime / (flashInterval * 2));



        for (int i = 0; i < flashCount; i++)

        {

            spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);

            yield return new WaitForSeconds(flashInterval);

            spriteRenderer.color = originalColor;

            yield return new WaitForSeconds(flashInterval);

        }



        // 5. 피격 상태 완전 종료

        isInvincible = false;

    }



    void HandlePostProcessingHitEffect()

    {

        if (volume == null) return;

        // 다른 코루틴을 시작해서 부드럽게 원래대로 돌아오도록 처리

        StartCoroutine(PostProcessingRoutine());

    }

    void PlayWalkSound()
    {
        // 땅에 있고, 움직이는 중이며, 걷는 소리가 현재 재생 중이 아닐 때만
        if (IsGrounded() && rb.linearVelocity.x != 0 && !movementAudioSource.isPlaying)
        {
            // 걷는 소리를 반복 재생하도록 설정하고 재생
            movementAudioSource.clip = walkSound;
            movementAudioSource.loop = true;
            movementAudioSource.Play();
        }
        // 움직이지 않으면 걷는 소리 멈춤
        else if (rb.linearVelocity.x == 0 || !IsGrounded())
        {
            movementAudioSource.Stop();
        }
    }

    IEnumerator PostProcessingRoutine()

    {

        // 효과 즉시 적용

        vignette.intensity.Override(0.7f);

        chromaticAberration.intensity.Override(1f);

        lensDistortion.intensity.Override(-0.3f);



        // 지정된 시간 동안 서서히 원래 값으로 복구

        float elapsed = 0f;

        while (elapsed < effectReturnDuration)

        {

            elapsed += Time.deltaTime;

            float progress = elapsed / effectReturnDuration;



            vignette.intensity.Override(Mathf.Lerp(0.8f, 0.4f, progress));

            chromaticAberration.intensity.Override(Mathf.Lerp(1f, 0f, progress));

            lensDistortion.intensity.Override(Mathf.Lerp(-0.4f, 0f, progress));

            yield return null;

        }



        // 값 보정

        vignette.intensity.Override(0.4f);

        chromaticAberration.intensity.Override(0f);

        lensDistortion.intensity.Override(0f);

    }



    IEnumerator CameraShake()

    {

        Transform camTransform = Camera.main.transform;

        Vector3 originalPos = camTransform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < shakeDuration)

        {

            float x = Random.Range(-1f, 1f) * shakeMagnitude;

            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            camTransform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;

        }

        camTransform.localPosition = originalPos;

    }



    void Die()

    {

        Debug.Log("플레이어 사망");

        SceneManager.LoadScene("리트");

    }

    public void PlaySlashSound()
    {
        if (audioSource == null || slashSounds.Length == 0) return;

        audioSource.Stop();

        AudioClip clipToPlay = slashSounds[slashSoundIndex];

        audioSource.PlayOneShot(clipToPlay);

        slashSoundIndex++;

        if (slashSoundIndex >= slashSounds.Length)
        {
            slashSoundIndex = 0;
        }
    }


    // --- 기타 유틸리티 함수 ---

    bool IsGrounded() { RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.2f, LayerMask.GetMask("Ground")); return hit.collider != null; }

    IEnumerator Dash()
    {
        // 1. 대시 시작과 동시에 무적 상태 & 쿨다운 시작
        canDash = false;
        isDashing = true;
        isInvincible = true;

        // 2. 대시 이동 실행
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(facingRight ? dashSpeed : -dashSpeed, 0);

        // 3. 대시 이동 시간(dashTime)만큼 대기
        yield return new WaitForSeconds(dashTime);

        // 4. 대시 이동 종료 및 조작 가능 상태로 복귀
        isDashing = false;
        rb.gravityScale = originalGravity;
        // 대시 후 속도를 0으로 만들어 미끄러짐 방지 
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        // 5. 총 무적 시간(0.5초)을 채우기 위해 남은 시간만큼 더 대기
        float remainingInvincibilityTime = 0.5f - dashTime;
        if (remainingInvincibilityTime > 0)
        {
            yield return new WaitForSeconds(remainingInvincibilityTime);
        }

        // 6. 무적 상태 종료
        isInvincible = false;

        // 7. 대시 쿨다운 대기
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void Attack() { animator.SetTrigger("Attack"); Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers); foreach (Collider2D enemyCollider in hitEnemies) { Enemy enemy = enemyCollider.GetComponent<Enemy>(); if (enemy != null) { enemy.TakeDamage(attackDamage, transform.position); } } }

    void Flip() { facingRight = !facingRight; transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z); }

}