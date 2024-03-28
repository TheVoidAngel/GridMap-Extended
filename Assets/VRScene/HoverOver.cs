using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; 
public class HoverOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public AudioSource Audio;
    public float ScaleTime = 0.1f, ScaleAmount = 1.2f;
    // Start is called before the first frame update

    //OnPointerDown
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Audio)
            Audio.Play();

            StartCoroutine(ScaleButton(ScaleAmount));
    }
    public void OnPointerExit(PointerEventData eventData)
    {
            StartCoroutine(ScaleButton(1f));
    }
    public IEnumerator ScaleButton(float Scale)
    {
        float InitScale = (transform.localScale.x);
        float iterator = 0;

       
        while (iterator < ScaleTime)
        {
            yield return new WaitForEndOfFrame();
            iterator += Time.deltaTime;
            transform.localScale = Vector3.one * Mathf.Lerp(InitScale, Scale, iterator / ScaleTime);
        }
    }
   // public void OnPointerDown
    public IEnumerator ScaleButtonY(float Scale)
    {
        float InitScale = (transform.localScale.y);
        float iterator = 0;
        while (iterator < ScaleTime)
        {
            yield return new WaitForEndOfFrame();
            iterator += Time.deltaTime;
            transform.localScale = new Vector3(1, Mathf.Lerp(InitScale, Scale, iterator / ScaleTime), 1);
        }
    }

}
