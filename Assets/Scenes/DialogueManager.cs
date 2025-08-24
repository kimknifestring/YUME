using System.Collections.Generic;
using UnityEngine;
using TMPro; 

[System.Serializable] // 이 줄이 있어야 인스펙터에서 보입니다.
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

    // 인스펙터에서 직접 연결할 UI 요소들
    public GameObject dialogueBoxObject; // 대화창 전체를 담는 최상위 오브젝트
    public TextMeshProUGUI nameText;      // 이름을 표시할 텍스트
    public TextMeshProUGUI dialogueText;  // 대사를 표시할 텍스트

    private Queue<DialogueLine> lines; // 대사 큐, 이제 DialogueLine 구조체를 사용

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
        // 게임 시작 시 대화창은 확실하게 비활성화
        if (dialogueBoxObject != null)
        {
            dialogueBoxObject.SetActive(false);
        }
    }

    void Update()
    {
        // 대화창이 활성화되어 있을 때 엔터(리턴) 키를 누르면 다음 대사 진행
        if (dialogueBoxObject.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            DisplayNextSentence();
        }
    }

    // 대화 시작 함수
    public void StartDialogue(DialogueLine[] dialogueLines)
    {
        if (isAble) {
            lines.Clear(); // 이전 대화 내용 초기화

            foreach (DialogueLine line in dialogueLines)
            {
                lines.Enqueue(line);
            }

            // 대화창 활성화
            dialogueBoxObject.SetActive(true);

            DisplayNextSentence(); // 첫 대사 표시
        }
    }

    // 다음 대사 표시 함수
    public void DisplayNextSentence()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = lines.Dequeue();

        // 이름 텍스트 업데이트
        nameText.text = currentLine.name;
        // 대사 텍스트 업데이트
        dialogueText.text = currentLine.sentence;
    }

    // 대화 종료 함수
    void EndDialogue()
    {
        BgmManager.instance.Stop(2.0f);
        SceneFader fader = FindAnyObjectByType<SceneFader>();
        fader.StartFadeOut();
        // 대화창 비활성화
        dialogueBoxObject.SetActive(false);
        isAble = false;
    }
}