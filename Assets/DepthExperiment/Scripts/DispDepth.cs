using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DispDepth : MonoBehaviour
{
    public Material mat;

    void Start()
    {
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;    
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, mat);
    }
}
