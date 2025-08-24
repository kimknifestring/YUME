using UnityEngine;

public class StarGenerator : MonoBehaviour
{
    [Header("������ �� ������")]
    public GameObject starPrefab; // ���� �⺻�� �� ���� ������Ʈ (������)

    [Header("�� �̹��� �ҽ� �迭")]
    public Sprite[] starSprites; // ����� �� �̹�����

    [Header("���� ��ġ ����")]
    public Transform startPoint; // X��ǥ ������
    public Transform endPoint;   // X��ǥ ����

    [Header("���� �ɼ�")]
    public int numberOfStars = 50; // ������ ���� ����
    [Range(10f, 50f)]
    public float minY = 10f; // �ּ� Y��ǥ
    [Range(10f, 50f)]
    public float maxY = 50f; // �ִ� Y��ǥ

    [Header("ũ�� ���� (1 ~ 6)")]
    [Range(1f, 10f)]
    public float minSize = 1f; // �ּ� ũ��
    [Range(1f, 10f)]
    public float maxSize = 20f; // �ִ� ũ��

    [Header("������ ���� ���� �θ� ������Ʈ")]
    public Transform starParent; // ������ ������ �����ϱ� ���� �θ� Ʈ������ (���� ����)

    // �ν����� â���� �� ��ũ��Ʈ�� �޴�(�� 3��)�� ������ "Generate Stars" �׸��� ���Դϴ�.
    [ContextMenu("Generate Stars")]
    private void GenerateStars()
    {
        // �ʼ� ������ �����Ǿ����� Ȯ��
        if (starPrefab == null || starSprites.Length == 0 || startPoint == null || endPoint == null)
        {
            Debug.LogError("�ʼ� �׸�(������, �� �̹���, ����/����)�� ��� �����Ǿ����� Ȯ�����ּ���.");
            return;
        }

        // ������ ������ŭ �� ���� �ݺ�
        for (int i = 0; i < numberOfStars; i++)
        {
            // 1. ���� �̹��� ����
            Sprite randomSprite = starSprites[Random.Range(0, starSprites.Length)];

            // 2. ���� ��ġ ���
            float randomX = Random.Range(startPoint.position.x, endPoint.position.x);
            float randomY = Random.Range(minY, maxY);
            float randomZ = Random.Range(1, 30);
            Vector3 spawnPosition = new Vector3(randomX, randomY, randomZ);

            // 3. �� ������Ʈ ����(Instantiate)
            GameObject newStar = Instantiate(starPrefab, spawnPosition, Quaternion.identity);

            // 4. ������ ���� ���� �̹��� ����
            SpriteRenderer renderer = newStar.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sprite = randomSprite;
            }

            // 5. ���� ũ�� ����
            float randomSize = Random.Range(minSize, maxSize);
            newStar.transform.localScale = new Vector3(randomSize, randomSize, 1f);

            // 6. �θ� ������Ʈ�� �����Ǿ��ٸ� �� �ڽ����� �ֱ� (�� ������)
            if (starParent != null)
            {
                newStar.transform.SetParent(starParent);
            }
        }

        Debug.Log($"{numberOfStars}���� �� ������ �Ϸ�Ǿ����ϴ�! ���� ����(Ctrl+S)�ϴ� ���� ���� ������.");
    }
}