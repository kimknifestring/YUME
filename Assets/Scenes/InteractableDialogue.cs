// InteractableDialogue.cs
using UnityEngine;

public class InteractableDialogue : MonoBehaviour
{
    public DialogueLine[] dialogue;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool playerInRange = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    void Update()
    {
        // ��ȭâ�� ���� ���� ��(���� ��ȭ ���� ���� ��)�� ��ȭ�� �����ϵ��� ������ �߰�
        if (playerInRange && Input.GetKeyDown(KeyCode.Return) && !DialogueManager.instance.dialogueBoxObject.activeSelf)
        {
            DialogueManager.instance.StartDialogue(dialogue);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (spriteRenderer != null)
            {
                // ����: ���� ���� 1.0f �̻����� �ø��� �׳� �Ͼ���� �˴ϴ�. 
                // ���� ���� ���ϱ⺸�� Ư�� ������ �ٲٰų�, 1.0f�� ���� �ʵ��� �����ϴ� ���� �����ϴ�.
                // ��: spriteRenderer.color = Color.yellow;
                spriteRenderer.color = new Color(originalColor.r + 0.2f, originalColor.g + 0.2f, originalColor.b + 0.2f);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }
        }
    }
}