using UnityEngine;

public class StageManager : MonoBehaviour
{
    public AudioClip stageBgm; // 인스펙터에서 스테이지 BGM 파일을 연결

    void Start()
    {
        // 2초에 걸쳐 부드럽게 스테이지 BGM을 재생
        BgmManager.instance.Play(stageBgm, 2.0f);
    }
}