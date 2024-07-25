using UnityEngine;
using System.Collections;

public class PrefabSpawner : MonoBehaviour
{
    public GameObject prefab;  // Префаб, который будет появляться
    public Vector2 spawnRangeX = new Vector2(-10f, 10f);  // Диапазон по оси X
    public Vector2 spawnRangeY = new Vector2(-10f, 10f);  // Диапазон по оси Y
    public int sortingOrder = 10;  // Порядок отрисовки для заспавненного префаба

    void Start()
    {
        // Запуск корутины для спавна префабов
        StartCoroutine(SpawnPrefabCoroutine());
    }

    // Корутин для спавна префабов в случайное время
    IEnumerator SpawnPrefabCoroutine()
    {
        while (true)
        {
            // Ждать случайное время от 0 до 4 секунд
            float waitTime = Random.Range(0f, 7f);
            yield return new WaitForSeconds(waitTime);

            // Спавн префаба
            SpawnPrefab();
        }
    }

    // Метод для спавна префаба
    public void SpawnPrefab()
    {
        // Генерация случайных координат в заданных диапазонах
        float randomX = Random.Range(spawnRangeX.x, spawnRangeX.y);
        float randomY = Random.Range(spawnRangeY.x, spawnRangeY.y);
        Vector2 spawnPosition = new Vector2(randomX, randomY);

        // Создание префаба на случайных координатах
        GameObject spawnedPrefab = Instantiate(prefab, spawnPosition, Quaternion.identity);

        // Установка порядка отрисовки для префаба
        SpriteRenderer spriteRenderer = spawnedPrefab.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = sortingOrder;
        }

        // Удаление префаба через 1 секунду
        Destroy(spawnedPrefab, 1f);
    }
}
