using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;// Marshal.SizeOf() 使えるようになる

public class Instancing : MonoBehaviour
{

    public int instanceCount = 100000;
    public Mesh instanceMesh;
    public Material instanceMaterial;
    public ComputeShader insObjShader;

    private int cachedInstanceCount = -1;
    private ComputeBuffer insObjBuffer;
    private ComputeBuffer argsBuffer;
    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

    struct InsObj
    {
        Vector3 pos;
        Vector3 vel;
        Vector3 rot;
        Vector3 angVel;
        Vector3 acc;
        Vector3 scale;
        Vector4 col;
    }

    void Start()
    {

        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        InitInsObjBuffer();
    }

    void Update()
    {

        // Update starting position buffer
        if (cachedInstanceCount != instanceCount)
            UpdateBuffers();

        // Pad input
        if (Input.GetAxisRaw("Horizontal") != 0.0f)
            instanceCount = (int)Mathf.Clamp(instanceCount + Input.GetAxis("Horizontal") * 40000, 1.0f, 5000000.0f);

        // Render
        Graphics.DrawMeshInstancedIndirect(instanceMesh, 0, instanceMaterial, new Bounds(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f)), argsBuffer);
    }

    void OnGUI()
    {

        GUI.Label(new Rect(265, 25, 200, 30), "Instance Count: " + instanceCount.ToString());
        instanceCount = (int)GUI.HorizontalSlider(new Rect(25, 20, 200, 30), (float)instanceCount, 1.0f, 5000000.0f);
    }

    void InitInsObjBuffer()
    {
        if (insObjBuffer != null)
            insObjBuffer.Release();
        insObjBuffer = new ComputeBuffer(instanceCount, Marshal.SizeOf(typeof(InsObj)));

        var initKernel = insObjShader.FindKernel("Init");
        insObjShader.SetBuffer(initKernel, "InsObjs", insObjBuffer);

        var posRange = new Vector3(1000.0f, 500.0f, 1000.0f);
        insObjShader.SetVector("PosRange", posRange);
        insObjShader.Dispatch(initKernel, instanceCount, 1, 1);
    }

    void UpdateBuffers()
    {

        // positions
        InitInsObjBuffer();
        instanceMaterial.SetBuffer("InsObjs", insObjBuffer);

        // indirect args
        uint numIndices = (instanceMesh != null) ? (uint)instanceMesh.GetIndexCount(0) : 0;
        args[0] = numIndices;
        args[1] = (uint)instanceCount;
        argsBuffer.SetData(args);

        cachedInstanceCount = instanceCount;
    }

    void OnDisable()
    {
        if (argsBuffer != null)
            argsBuffer.Release();
        argsBuffer = null;

        if (insObjBuffer != null)
            insObjBuffer.Release();
        insObjBuffer = null;
    }
}