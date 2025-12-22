using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ImageListPopup : MonoBehaviour
{
    public List<GameObject> popupParents;

    public float startingDelay = 1f;
    public float delayDecrease = 0.2f;
    public float minimumDelay = 0.2f;
    public float stayVisibleTime = 3f;

    private int currentParentIndex = 0;
    private bool isPlaying = false;

    private void Start()
    {
        foreach (GameObject parent in popupParents)
        {
            Canvas[] canvases = parent.GetComponentsInChildren<Canvas>(true);
            foreach (Canvas canvas in canvases)
            {
                canvas.gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        //if (Keyboard.current.enterKey.wasPressedThisFrame)
        //{
        //    if (isPlaying)
        //        return;

        //    if (currentParentIndex < popupParents.Count)
        //    {
        //        StartCoroutine(ShowCanvasesCoroutine(popupParents[currentParentIndex]));
        //        currentParentIndex++;
        //    }
        //}
    }

    public IEnumerator ShowCanvasesCoroutine(GameObject parent)
    {
        isPlaying = true;

        Canvas[] canvases = parent.GetComponentsInChildren<Canvas>(true);

        float currentDelay = startingDelay;

        foreach (Canvas canvas in canvases)
        {
            canvas.gameObject.SetActive(true);

            yield return new WaitForSeconds(currentDelay);

            currentDelay = Mathf.Max(minimumDelay, currentDelay - delayDecrease);
        }

        yield return new WaitForSeconds(stayVisibleTime);

        foreach (Canvas canvas in canvases)
        {
            canvas.gameObject.SetActive(false);
        }

        isPlaying = false;
    }
}
