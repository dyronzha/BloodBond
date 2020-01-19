using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particle_cache : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer _karol = null;
    [SerializeField] RenderTexture _testMap = null;

    Mesh _mesh;
    List<Vector3> _positionList = new List<Vector3>();
    // Start is called before the first frame update
    void Start()
    {
        _mesh = new Mesh();
    }

    // Update is called once per frame
    void Update()
    {
        if (_karol == null)
            return;

        _karol.BakeMesh(_mesh); //獲得當前skinmesh下的mesh網格(去除掉計算骨骼啥的計算
        _mesh.GetVertices(_positionList); //從mesh中取出網格

        Test();
    }

    void Test()
    {
        var mapWidth = _testMap.width;
        var mapHeight = _testMap.height;

        var vcount = _positionList.Count;
        var vcount_x3 = vcount * 3; //每點有三個座標
    }

    //大神用來檢查的函式 ??
    //bool CheckConsistency()
    //{
    //    if (_warned) return false;

    //    if (_positionMap.width % 8 != 0 || _positionMap.height % 8 != 0)
    //    {
    //        Debug.LogError("Position map dimensions should be a multiple of 8.");
    //        _warned = true;
    //    }

    //    if (_normalMap.width != _positionMap.width ||
    //        _normalMap.height != _positionMap.height)
    //    {
    //        Debug.LogError("Position/normal map dimensions should match.");
    //        _warned = true;
    //    }

    //    if (_positionMap.format != RenderTextureFormat.ARGBHalf &&
    //        _positionMap.format != RenderTextureFormat.ARGBFloat)
    //    {
    //        Debug.LogError("Position map format should be ARGBHalf or ARGBFloat.");
    //        _warned = true;
    //    }

    //    if (_normalMap.format != RenderTextureFormat.ARGBHalf &&
    //        _normalMap.format != RenderTextureFormat.ARGBFloat)
    //    {
    //        Debug.LogError("Normal map format should be ARGBHalf or ARGBFloat.");
    //        _warned = true;
    //    }

    //    return !_warned;
    //}
}
