using UnityEngine;

public class CanvasController : MonoBehaviour
{
    // Метод для включения одного канваса
    public void EnableCanvas(Canvas canvasToEnable)
    {
        if (canvasToEnable != null)
        {
            bool currentState = canvasToEnable.gameObject.activeSelf;
            bool newState = !currentState;
            canvasToEnable.gameObject.SetActive(newState);
            Debug.Log($"Canvas '{canvasToEnable.name}' state changed: {currentState} -> {newState}");
        }
        else
        {
            Debug.LogError("The canvas to enable is null.");
        }
    }
}
