using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[ExecuteAlways]
[RequireComponent(typeof(Camera), typeof(PixelPerfectCamera))]
public class TiledCamera : MonoBehaviour
{
    [SerializeField]
    [HideInInspector]
    Camera _camera;
    public Camera Camera => _camera;

    [SerializeField]
    [HideInInspector]
    GameObject _clearCameraGO;

    [SerializeField]
    [HideInInspector]
    Camera _clearCamera;

    [SerializeField]
    [HideInInspector]
    PixelPerfectCamera _pixelCam;

    [SerializeField]
    int2 _tileCount = new int2(48, 25);
    [SerializeField]
    int2 _tileSize = new int2(8, 8);

    [SerializeField]
    Color _backgroundColor = Color.black;

    bool _changed;
    
    private void OnEnable()
    {
        if(_camera == null)
        {
            _camera = GetComponent<Camera>();
        }

        _camera.orthographic = true;

        if(_clearCamera == null)
        {
            if(_clearCameraGO == null )
            {
                _clearCameraGO = new GameObject("ClearCamera");
                _clearCameraGO.transform.SetParent(transform);
            }
            _clearCamera = _clearCameraGO.AddComponent<Camera>();
            _clearCamera.orthographic = true;
            _clearCamera.cullingMask = 0;
            _clearCameraGO.hideFlags = HideFlags.HideInHierarchy;
        }

        if(_pixelCam == null)
        {
            _pixelCam = GetComponent<PixelPerfectCamera>();
        }

        _pixelCam.runInEditMode = true;
        _pixelCam.upscaleRT = true;
        _pixelCam.cropFrameX = true;
        _pixelCam.cropFrameY = true;
    }

    private void Update()
    {
        if (_changed)
        {
            _changed = false;

            _pixelCam.refResolutionX = _tileCount.x * _tileSize.x;
            _pixelCam.refResolutionY = _tileCount.y * _tileSize.y;

            _clearCamera.backgroundColor = _backgroundColor;
            _clearCamera.depth = _camera.depth - 1;

            _clearCamera.orthographicSize = _camera.orthographicSize;
            _pixelCam.assetsPPU = _tileSize.y;
        }
    }

#if UNITY_EDITOR
    [Header("Grid")]

    [SerializeField]
    bool _drawGrid = false;

    [SerializeField]
    Color _gridColorOdd = new Color(.75f, 0, 0, .35f);

    [SerializeField]
    Color _gridColorEven = new Color(.1f, .1f, .1f, .1f);


    private void OnValidate()
    {
        _changed = true;

        _tileCount = math.max(1, _tileCount);
        _tileSize = math.max(1, _tileSize);
    }

    private void OnDrawGizmosSelected()
    {
        if (!_drawGrid || Camera.current != _camera)
            return;

        float camDistance = _camera.farClipPlane;
        float3 bl = _camera.ViewportToWorldPoint(new Vector3(0, 0, camDistance));

        for (int x = 0; x < _tileCount.x; ++x)
            for (int y = 0; y < _tileCount.y; ++y)
            {
                float3 p = bl + (new float3(x, y, 0));

                float2 xy = math.floor(p.xy);
                xy /= 2;
                float checker = math.frac(xy.x + xy.y) * 2;

                Color gridColor = Color.Lerp(_gridColorEven, _gridColorOdd, checker);

                Gizmos.color = gridColor;
                Gizmos.DrawCube(p + .5f, Vector3.one);
            }
    }
#endif
}
