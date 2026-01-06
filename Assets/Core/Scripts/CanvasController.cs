using UnityEngine;
using System.Collections;

public class CanvasController : MonoBehaviour
{
    public Canvas UI;

    void Start()
    {
        StartCoroutine(DisableCanvasAfterSeconds(15f));
    }

    IEnumerator DisableCanvasAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        UI.enabled = false;
    }
}