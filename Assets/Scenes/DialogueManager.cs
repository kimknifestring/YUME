using System.Collections.Generic;
using UnityEngine;
using TMPro; 

[System.Serializable] // �� ���� �־�� �ν����Ϳ��� ���Դϴ�.
public class DialogueLine
{
    public string name;
    [TextArea(2, 5)]
    public string sentence;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    private bool isAble = true;

    // �ν����Ϳ��� ���� ������ UI ��ҵ�
    public GameObject dialogueBoxObject; // ��ȭâ ��ü�� ��� �ֻ��� ������Ʈ
    public TextMeshProUGUI nameText;      // �̸��� ǥ���� �ؽ�Ʈ
    public TextMeshProUGUI dialogueText;  // ��縦 ǥ���� �ؽ�Ʈ

    private Queue<DialogueLine> lines; // ��� ť, ���� DialogueLine ����ü�� ���

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        lines = new Queue<DialogueLine>();
    }

    void Start()
    {
        // ���� ���� �� ��ȭâ�� Ȯ���ϰ� ��Ȱ��ȭ
        if (dialogueBoxObject != null)
        {
            dialogueBoxObject.SetActive(false);
        }
    }

    void Update()
    {
        // ��ȭâ�� Ȱ��ȭ�Ǿ� ���� �� ����(����) Ű�� ������ ���� ��� ����
        if (dialogueBoxObject.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            DisplayNextSentence();
        }
    }

    // ��ȭ ���� �Լ�
    public void StartDialogue(DialogueLine[] dialogueLines)
    {
        if (isAble) {
            lines.Clear(); // ���� ��ȭ ���� �ʱ�ȭ

            foreach (DialogueLine line in dialogueLines)
            {
                lines.Enqueue(line);
            }

            // ��ȭâ Ȱ��ȭ
            dialogueBoxObject.SetActive(true);

            DisplayNextSentence(); // ù ��� ǥ��
        }
    }

    // ���� ��� ǥ�� �Լ�
    public void DisplayNextSentence()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = lines.Dequeue();

        // �̸� �ؽ�Ʈ ������Ʈ
        nameText.text = currentLine.name;
        // ��� �ؽ�Ʈ ������Ʈ
        dialogueText.text = currentLine.sentence;
    }

    // ��ȭ ���� �Լ�
    void EndDialogue()
    {
        BgmManager.instance.Stop(2.0f);
        SceneFader fader = FindAnyObjectByType<SceneFader>();
        fader.StartFadeOut();
        // ��ȭâ ��Ȱ��ȭ
        dialogueBoxObject.SetActive(false);
        isAble = false;
    }
}