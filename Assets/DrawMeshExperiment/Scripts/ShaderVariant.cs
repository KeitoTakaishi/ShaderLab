using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderVariant : MonoBehaviour
{
    public Material material;
    void Start()
    {
        material.EnableKeyword("GREEN");
    }

    void Update()
    {
        
    }
}
