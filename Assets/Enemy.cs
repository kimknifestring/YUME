using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("�⺻ ����")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("�˹� & �ǰ� ȿ��")]
    public float knockbackForce = 5f;
    public float knockbackImmunityDuration = 0.5f;
    public float flashDuration = 0.1f;

    [Header("UI �� ������Ʈ")]
    public BossHealthBar healthBar; 
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Rigidbody2D rb;
    public AudioSource normalBgm;
    public AudioSource bossBgm;
    public AudioSource sfxAudioSource;
    public AudioClip dieS;

    [Header("�������� ������Ʈ")]
    public Canvas canvasToHide;          // ���� ĵ����
    public GameObject objectToSpawn;     // ��ȯ�� ������Ʈ ������
    public Transform player;             // �÷��̾� ��ġ 
    public float spawnHeightOffset = 1.5f; // ��ȯ ����

    // ���� Ȯ�� ����
    private bool isDead = false;
    private bool isKnockbackImmune = false;

    void Start()
    {
        // �ʱ�ȭ
        currentHealth = maxHealth;

        // ������Ʈ ��������
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        // ���� ���� ����
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // ���� �� ü�� �� �ʱ�ȭ
        if (healthBar != null)
        {
            healthBar.HandleHealthChanged((float)currentHealth / maxHealth);
        }
    }

    // �������� �޴� ���� �Լ�
    public void TakeDamage(int damage, Vector2 attackerPosition)
    {
        if (isDead) return; // �̹� �׾����� �ƹ��͵� �� ��

        currentHealth -= damage;

        // ü�� �� ������Ʈ
        if (healthBar != null)
        {
            float currentHealthPct = (float)currentHealth / maxHealth;
            healthBar.HandleHealthChanged(currentHealthPct);
        }

        // �ǰ� ȿ�� ����
        StartCoroutine(FlashWhite());

        // �˹� �鿪 ���°� �ƴ� ���� �˹� ����
        if (!isKnockbackImmune)
        {
            Knockback(attackerPosition);
        }

        // ü���� 0 �����̸� ��� ó��
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ���� �˹� ó�� �Լ�
    void Knockback(Vector2 attackerPosition)
    {
        if (rb == null) return;

        // �˹� ���� ��� (�������θ�)
        Vector2 knockbackDirection;
        if (attackerPosition.x < transform.position.x)
        {
            knockbackDirection = Vector2.right; // ����������
        }
        else
        {
            knockbackDirection = Vector2.left;  // ��������
        }

        // �˹� ����
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        // �˹� �鿪 �ڷ�ƾ ����
        StartCoroutine(KnockbackImmunityRoutine());
    }

    // �˹� �鿪�� ó���ϴ� �ڷ�ƾ
    IEnumerator KnockbackImmunityRoutine()
    {
        isKnockbackImmune = true;
        yield return new WaitForSeconds(knockbackImmunityDuration);
        isKnockbackImmune = false;
    }

    // �ǰ� �� �Ͼ�� ������ ȿ�� �ڷ�ƾ
    IEnumerator FlashWhite()
    {
        spriteRenderer.color = new Color(1, 1, 1);
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    // ��� ó�� �Լ�
    void Die()
    {
        PlaySoundEffect(dieS);
        SwitchBGM(normalBgm, bossBgm);
        // ��� �ִϸ��̼� ���
        anim.Play("���"); // �Ǵ� anim.SetTrigger("Die");
        isDead = true;

        // ������ ��ȣ�ۿ� �� �浹 ����
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
        }
        GetComponent<Collider2D>().enabled = false;


    }

    // �ִϸ��̼� �̺�Ʈ���� ȣ���� �ı� �Լ�
    public void DestroyObject()
    {
        Destroy(gameObject);
    }

    // BGM�� ��ȯ�� �� �� �Լ��� ȣ���մϴ�.
    public void SwitchBGM(AudioSource musicToPlay, AudioSource musicToStop)
    {
        // ���� ��� ���� ������ ����ϴ�.
        if (musicToStop != null && musicToStop.isPlaying)
        {
            musicToStop.Stop();
        }

        // ���� ����� ������ ����մϴ�.
        if (musicToPlay != null && !musicToPlay.isPlaying)
        {
            musicToPlay.Play();
        }
    }

    public void PlaySoundEffect(AudioClip clipToPlay)
    {
        // ����� �ҽ��� Ŭ���� ��������� ���� ������ ���� �������� ����
        if (sfxAudioSource == null || clipToPlay == null)
        {
            Debug.LogWarning("����� �ҽ� �Ǵ� Ŭ���� ����ֽ��ϴ�.");
            return;
        }

        // ������ Ŭ���� �� �� ����մϴ�.
        sfxAudioSource.PlayOneShot(clipToPlay);
    }

    public void StartSpawnSequence()
    {
        // 1. �ڽ��� ��������Ʈ ������ ����
        SpriteRenderer myRenderer = GetComponent<SpriteRenderer>();
        if (myRenderer != null)
        {
            myRenderer.enabled = false;
        }

        // 2. ĵ������ ������ ����
        if (canvasToHide != null)
        {
            canvasToHide.enabled = false;
        }

        // 3. �÷��̾� ���� ��Ȱ��ȭ
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