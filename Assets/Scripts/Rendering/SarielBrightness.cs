using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SarielBrightness : MonoBehaviour
{
    [SerializeField] public bool scaleIncrease;
    [SerializeField] public float scaleFactor;
    [SerializeField] public float baseIntensity;
    [SerializeField] public float maxIntensity;
    [SerializeField] public GameObject light;
    [SerializeField] public float sliderSize = 100f;
    [SerializeField] private Material material;
    [SerializeField] public Vector4 start = new Vector4(0/256, 0/256, 0/256, 0/256);
    [SerializeField] public Vector4 midpoint = new Vector4(256/256, 0/256, 192/256, 0/256);
    [SerializeField] public Vector4 end = new Vector4(256/256, 256/256, 256/256, 0/256);
    private Vector3 pos;
    private Vector3 scale;
    private float halfSliderSize;
    private Light l;
    private Color color;
    // Start is called before the first frame update
    void Start()
    {
        if(scaleFactor <= 0.25f) scaleFactor = 0.25f;
        if(scaleFactor >= 500f) scaleFactor = 1e8f;
        SpriteRenderer sr = this.gameObject.GetComponent<SpriteRenderer>();
        material = sr.material;
        halfSliderSize = sliderSize / 2;
        pos = this.gameObject.transform.position;
        scale = this.gameObject.transform.localScale;
        l = light.GetComponent<Light>();
        color = l.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetColor(int m)
    {
        if(scaleIncrease) {
            this.gameObject.transform.localScale = scale * (1 + m/scaleFactor);
            this.gameObject.transform.position = new Vector3(pos.x, pos.y * (1 + m / scaleFactor), pos.z);
        }
        
        float n = (float) m;
        bool lowerHalf = n <= halfSliderSize;
        n = (lowerHalf) ? n/halfSliderSize : (n - halfSliderSize) / halfSliderSize;
        Debug.Log(n);
        Vector4 lerpRes;
        if(lowerHalf)
        {
            lerpRes = Vector4.Lerp(start, midpoint, n);
        }
        else
        {
            lerpRes = Vector4.Lerp(midpoint, end, n);
        }
        l.color = new Color(lerpRes.x, lerpRes.y, lerpRes.z, lerpRes.w) + color;
        l.intensity = (baseIntensity) + ((maxIntensity - baseIntensity)/100 * m); 
        Debug.Log("Setting color to " + lerpRes);
        material.SetVector("_Color", lerpRes);
    }

}
