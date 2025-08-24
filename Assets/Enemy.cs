using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("기본 스탯")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("넉백 & 피격 효과")]
    public float knockbackForce = 5f;
    public float knockbackImmunityDuration = 0.5f;
    public float flashDuration = 0.1f;

    [Header("UI 및 컴포넌트")]
    public BossHealthBar healthBar; 
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Rigidbody2D rb;
    public AudioSource normalBgm;
    public AudioSource bossBgm;
    public AudioSource sfxAudioSource;
    public AudioClip dieS;

    [Header("시퀀스용 오브젝트")]
    public Canvas canvasToHide;          // 숨길 캔버스
    public GameObject objectToSpawn;     // 소환할 오브젝트 프리팹
    public Transform player;             // 플레이어 위치 
    public float spawnHeightOffset = 1.5f; // 소환 높이

    // 상태 확인 변수
    private bool isDead = false;
    private bool isKnockbackImmune = false;

    void Start()
    {
        // 초기화
        currentHealth = maxHealth;

        // 컴포넌트 가져오기
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        // 원래 색상 저장
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // 시작 시 체력 바 초기화
        if (healthBar != null)
        {
            healthBar.HandleHealthChanged((float)currentHealth / maxHealth);
        }
    }

    // 데미지를 받는 메인 함수
    public void TakeDamage(int damage, Vector2 attackerPosition)
    {
        if (isDead) return; // 이미 죽었으면 아무것도 안 함

        currentHealth -= damage;

        // 체력 바 업데이트
        if (healthBar != null)
        {
            float currentHealthPct = (float)currentHealth / maxHealth;
            healthBar.HandleHealthChanged(currentHealthPct);
        }

        // 피격 효과 실행
        StartCoroutine(FlashWhite());

        // 넉백 면역 상태가 아닐 때만 넉백 실행
        if (!isKnockbackImmune)
        {
            Knockback(attackerPosition);
        }

        // 체력이 0 이하이면 사망 처리
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 수평 넉백 처리 함수
    void Knockback(Vector2 attackerPosition)
    {
        if (rb == null) return;

        // 넉백 방향 계산 (수평으로만)
        Vector2 knockbackDirection;
        if (attackerPosition.x < transform.position.x)
        {
            knockbackDirection = Vector2.right; // 오른쪽으로
        }
        else
        {
            knockbackDirection = Vector2.left;  // 왼쪽으로
        }

        // 넉백 실행
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        // 넉백 면역 코루틴 시작
        StartCoroutine(KnockbackImmunityRoutine());
    }

    // 넉백 면역을 처리하는 코루틴
    IEnumerator KnockbackImmunityRoutine()
    {
        isKnockbackImmune = true;
        yield return new WaitForSeconds(knockbackImmunityDuration);
        isKnockbackImmune = false;
    }

    // 피격 시 하얗게 빛나는 효과 코루틴
    IEnumerator FlashWhite()
    {
        spriteRenderer.color = new Color(1, 1, 1);
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    // 사망 처리 함수
    void Die()
    {
        PlaySoundEffect(dieS);
        SwitchBGM(normalBgm, bossBgm);
        // 사망 애니메이션 재생
        anim.Play("사망"); // 또는 anim.SetTrigger("Die");
        isDead = true;

        // 물리적 상호작용 및 충돌 중지
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
        }
        GetComponent<Collider2D>().enabled = false;


    }

    // 애니메이션 이벤트에서 호출할 파괴 함수
    public void DestroyObject()
    {
        Destroy(gameObject);
    }

    // BGM을 전환할 때 이 함수를 호출합니다.
    public void SwitchBGM(AudioSource musicToPlay, AudioSource musicToStop)
    {
        // 현재 재생 중인 음악을 멈춥니다.
        if (musicToStop != null && musicToStop.isPlaying)
        {
            musicToStop.Stop();
        }

        // 새로 재생할 음악을 재생합니다.
        if (musicToPlay != null && !musicToPlay.isPlaying)
        {
            musicToPlay.Play();
        }
    }

    public void PlaySoundEffect(AudioClip clipToPlay)
    {
        // 오디오 소스나 클립이 비어있으면 오류 방지를 위해 실행하지 않음
        if (sfxAudioSource == null || clipToPlay == null)
        {
            Debug.LogWarning("오디오 소스 또는 클립이 비어있습니다.");
            return;
        }

        // 지정된 클립을 한 번 재생합니다.
        sfxAudioSource.PlayOneShot(clipToPlay);
    }

    public void StartSpawnSequence()
    {
        // 1. 자신의 스프라이트 렌더러 끄기
        SpriteRenderer myRenderer = GetComponent<SpriteRenderer>();
        if (myRenderer != null)
        {
            myRenderer.enabled = false;
        }

        // 2. 캔버스의 렌더링 해제
        if (canvasToHide != null)
        {
            canvasToHide.enabled = false;
        }

        // 3. 플레이어 조작 비활성화
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = false;
            }
        }
        Invoke("SpawnObject", 3f);
    }

    private void SpawnObject()
    {
        if (objectToSpawn != null && player != null)
        {
            Vector3 spawnPosition = player.position + new Vector3(0, spawnHeightOffset, 0);
            Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
        }
    }
}