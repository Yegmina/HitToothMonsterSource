using UnityEngine;

public class PrefabOnClick : MonoBehaviour
{
    private money moneyScript;

    void Start()
    {
        // Поиск объекта с скриптом money на сцене
        moneyScript = FindObjectOfType<money>();
        if (moneyScript == null)
        {
            Debug.LogError("money script not found on any GameObject in the scene.");
        }
        else
        {
            // Загружаем начальное значение moneyToAdd из PlayFab
            moneyScript.LoadMoneyToAdd();
        }
    }

    void Update()
    {
        // Проверка на нажатие мыши
        if (Input.GetMouseButtonDown(0))
        {
            CheckClick(Input.mousePosition);
        }

        // Проверка на касание экрана
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                CheckClick(touch.position);
            }
        }
    }

    void CheckClick(Vector2 position)
    {
        // Конвертируем экранные координаты в мировые координаты
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(position);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            if (moneyScript != null)
            {
                moneyScript.AddMoney(moneyScript.moneyToAdd);
                Destroy(gameObject); // Удаление префаба после клика
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Проверка на попадание в триггер
        if (other.CompareTag("Player")) // Можно задать подходящий тэг
        {
            if (moneyScript != null)
            {
                moneyScript.AddMoney(moneyScript.moneyToAdd);
                Destroy(gameObject); // Удаление префаба после столкновения
            }
        }
    }

    public void AddMoneyPerCoin(int x)
    {
        if (moneyScript != null)
        {
            moneyScript.UpdateMoneyToAdd(moneyScript.moneyToAdd + x);
        }
    }
}
