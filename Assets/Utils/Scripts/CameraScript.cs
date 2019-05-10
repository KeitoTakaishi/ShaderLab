using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject target;
    public float radius = 500.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target.transform);
        float t = Time.realtimeSinceStartup;
        t /= 10.0f;
        radius = 500.0f * Mathf.Sin(3.0f*t);
        this.transform.position = new Vector3(radius * Mathf.Sin(t), 300.0f*Mathf.Sin(t), radius * Mathf.Cos(t));
    }
}
