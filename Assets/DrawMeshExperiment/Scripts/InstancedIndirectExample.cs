using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstancedIndirectExample : MonoBehaviour
{
    public int instanceCount = 100000;
    public Mesh instanceMesh;
    public Material instanceMaterial;

    private int _cachedInstanceCount = -1;
    private ComputeBuffer _positionBuffer;
    private ComputeBuffer _argsBuffer;
    private ComputeBuffer _colorBuffer;

    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

    void Start()
    {
        _argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        UpdateBuffers();
    }

    void Update()
    {
        Graphics.DrawMeshInstancedIndirect(instanceMesh, 0, instanceMaterial, new Bounds(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f)), _argsBuffer);
    }

    void UpdateBuffers()
    {
        if (_positionBuffer != null) {
            _positionBuffer.Release();
        }

        if (_colorBuffer != null) {
            _colorBuffer.Release();
        }

        _positionBuffer = new ComputeBuffer(instanceCount, sizeof(float) * 4);
        _colorBuffer = new ComputeBuffer(instanceCount, sizeof(float) * 4);

        Vector4[] _positions = new Vector4[instanceCount];
        Vector4[] _colors = new Vector4[instanceCount];
        float radius = 300.0f;
        for (int i = 0; i < instanceCount; i++) {
            Vector3 _p = Random.insideUnitSphere * radius;
            _positions[i] = new Vector4(_p.x, _p.y, _p.z, Random.RandomRange(10.0f, 30.0f));
            float _c = Random.value;
            _colors[i] = new Vector4(_c, _c, _c, 1.0f);

        }

        _positionBuffer.SetData(_positions);
        _colorBuffer.SetData(_colors);


        //send shader'suniform
        instanceMaterial.SetBuffer("positionBuffer", _positionBuffer);
        instanceMaterial.SetBuffer("colorBuffer", _colorBuffer);

        uint numIndices = (instanceMesh != null) ? (uint)instanceMesh.GetIndexCount(0) : 0;
    
        args[0] = numIndices;
        args[1] = (uint)instanceCount;
        _argsBuffer.SetData(args);

        _cachedInstanceCount = instanceCount;
    }

    void OnDisable()
    {
        if (_positionBuffer != null)
            _positionBuffer.Release();
        _positionBuffer = null;

        if (_colorBuffer != null)
            _colorBuffer.Release();
        _colorBuffer = null;

        if (_argsBuffer != null)
            _argsBuffer.Release();
        _argsBuffer = null;
    }
}
