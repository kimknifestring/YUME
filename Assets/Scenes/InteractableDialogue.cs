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
        // 대화창이 꺼져 있을 때(아직 대화 시작 전일 때)만 대화를 시작하도록 조건을 추가
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
                // 참고: 색상 값을 1.0f 이상으로 올리면 그냥 하얀색이 됩니다. 
                // 기존 값에 더하기보다 특정 색으로 바꾸거나, 1.0f를 넘지 않도록 조절하는 것이 좋습니다.
                // 예: spriteRenderer.color = Color.yellow;
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