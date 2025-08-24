using UnityEngine;

public class StarGenerator : MonoBehaviour
{
    [Header("생성할 별 프리팹")]
    public GameObject starPrefab; // 별의 기본이 될 게임 오브젝트 (프리팹)

    [Header("별 이미지 소스 배열")]
    public Sprite[] starSprites; // 사용할 별 이미지들

    [Header("생성 위치 범위")]
    public Transform startPoint; // X좌표 시작점
    public Transform endPoint;   // X좌표 끝점

    [Header("생성 옵션")]
    public int numberOfStars = 50; // 생성할 별의 개수
    [Range(10f, 50f)]
    public float minY = 10f; // 최소 Y좌표
    [Range(10f, 50f)]
    public float maxY = 50f; // 최대 Y좌표

    [Header("크기 범위 (1 ~ 6)")]
    [Range(1f, 10f)]
    public float minSize = 1f; // 최소 크기
    [Range(1f, 10f)]
    public float maxSize = 20f; // 최대 크기

    [Header("생성된 별을 담을 부모 오브젝트")]
    public Transform starParent; // 생성된 별들을 정리하기 위한 부모 트랜스폼 (선택 사항)

    // 인스펙터 창에서 이 스크립트의 메뉴(점 3개)를 누르면 "Generate Stars" 항목이 보입니다.
    [ContextMenu("Generate Stars")]
    private void GenerateStars()
    {
        // 필수 값들이 설정되었는지 확인
        if (starPrefab == null || starSprites.Length == 0 || startPoint == null || endPoint == null)
        {
            Debug.LogError("필수 항목(프리팹, 별 이미지, 시작/끝점)이 모두 설정되었는지 확인해주세요.");
            return;
        }

        // 지정된 개수만큼 별 생성 반복
        for (int i = 0; i < numberOfStars; i++)
        {
            // 1. 랜덤 이미지 선택
            Sprite randomSprite = starSprites[Random.Range(0, starSprites.Length)];

            // 2. 랜덤 위치 계산
            float randomX = Random.Range(startPoint.position.x, endPoint.position.x);
            float randomY = Random.Range(minY, maxY);
            float randomZ = Random.Range(1, 30);
            Vector3 spawnPosition = new Vector3(randomX, randomY, randomZ);

            // 3. 별 오브젝트 생성(Instantiate)
            GameObject newStar = Instantiate(starPrefab, spawnPosition, Quaternion.identity);

            // 4. 생성된 별에 랜덤 이미지 적용
            SpriteRenderer renderer = newStar.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sprite = randomSprite;
            }

            // 5. 랜덤 크기 적용
            float randomSize = Random.Range(minSize, maxSize);
            newStar.transform.localScale = new Vector3(randomSize, randomSize, 1f);

            // 6. 부모 오브젝트가 지정되었다면 그 자식으로 넣기 (씬 정리용)
            if (starParent != null)
            {
                newStar.transform.SetParent(starParent);
            }
        }

        Debug.Log($"{numberOfStars}개의 별 생성이 완료되었습니다! 씬을 저장(Ctrl+S)하는 것을 잊지 마세요.");
    }
}