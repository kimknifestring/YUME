using UnityEngine;

public class StageManager : MonoBehaviour
{
    public AudioClip stageBgm; // �ν����Ϳ��� �������� BGM ������ ����

    void Start()
    {
        // 2�ʿ� ���� �ε巴�� �������� BGM�� ���
        BgmManager.instance.Play(stageBgm, 2.0f);
    }
}