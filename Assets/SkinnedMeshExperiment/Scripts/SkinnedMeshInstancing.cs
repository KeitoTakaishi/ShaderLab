using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class SkinnedMeshInstancing : MonoBehaviour
{
    [SerializeField] protected SkinnedMeshRenderer skinned;
    protected Mesh mesh;

    public int instanceCount;
    public Mesh instanceMesh;
    public Material instanceMaterial;
    private int cachedInstanceCount = -1;
    private ComputeBuffer vertexBuffer;
    private ComputeBuffer argsBuffer;
    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

    void Start()
    {
        skinned.BakeMesh(mesh);
        var vertices = mesh.vertices;
        var vertBuffer = new ComputeBuffer(vertices.Length, Marshal.SizeOf(typeof(Vector4)));
        vertBuffer.SetData(vertices);
        //computeShader.SetBuffer(0, "_VertBuffer", vertBuffer);

        instanceCount = vertices.Length;
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        UpdateBuffer();
    }

    void Update()
    {
        Graphics.DrawMeshInstancedIndirect(instanceMesh, 0, instanceMaterial, new Bounds(Vector3.zero, new Vector3(10000.0f, 10000.0f, 10000.0f)), argsBuffer);

    }

    void UpdateBuffer()
    {
        if (vertexBuffer != null) {
            vertexBuffer.Release();
        }

        var vertices = mesh.vertices;
        
        List<Vector4> v = new List<Vector4>();
        for(int i = 0; i < vertices.Length; i++) {
            float size = Random.value;
            v.Add(new Vector4(vertices[i].x, vertices[i].y, vertices[i].z, size));
        }
        vertexBuffer = new ComputeBuffer(instanceCount, Marshal.SizeOf(typeof(Vector4)));
        vertexBuffer.SetData(v.ToArray());

        instanceMaterial.SetBuffer("vertexBuffer", vertexBuffer);

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

        if (vertexBuffer != null)
            vertexBuffer.Release();
        vertexBuffer = null;
    }
}
