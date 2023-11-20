using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class VolumeControllerScript : MonoBehaviour
{
    public Volume v;
    private Bloom b;
    private Vignette vg;
    private float VignetteValue;
    // Start is called before the first frame update
    void Start()
    {
        v.profile.TryGet(out vg);
        VignetteValue = 0.24f;
    }

    [ContextMenu("test")]
    IEnumerator VignetteInside()
    {
        vg.intensity.value = Mathf.Lerp(0.3f, 0.8f, 20f);
        yield return null;
    }

    IEnumerator VignetteOutside()
    {
        vg.intensity.value = Mathf.Lerp(0.8f, 0.3f, 20f);
        yield return null;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Doors") VignetteValue = 0.42f;
        //vg.intensity.value = Mathf.Lerp(0.3f, 0.8f, 20f);
        //StartCoroutine(VignetteInside());
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Doors") VignetteValue = 0.24f;
        //vg.intensity.value = Mathf.Lerp(0.8f, 0.3f, 20f);
        //StartCoroutine(VignetteOutside());
    }

    private void Update()
    {
        vg.intensity.value = Mathf.Lerp(vg.intensity.value, VignetteValue, 7f * Time.deltaTime);
    }
}
