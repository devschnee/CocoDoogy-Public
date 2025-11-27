using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ground Renderer의 Bounds를 기준으로
/// - 중앙 그리드 없이
/// - 바깥 테두리 4줄만 LineRenderer로 그려주는 오버레이
/// </summary>
[DisallowMultipleComponent]
public class GridOverlay : MonoBehaviour
{
    [Header("Target Ground")]
    [SerializeField] private Renderer groundRenderer;   // 선을 둘러줄 Ground Mesh/Renderer

    [Header("Line")]
    [SerializeField] private Material lineMaterial;     // URP: Unlit/Color 같은 머티리얼
    [SerializeField] private Color lineColor = new Color(1f, 1f, 1f, 0.35f);
    [SerializeField] private float lineWidth = 0.03f;
    [SerializeField, Tooltip("Ground 위로 조금 띄울 높이(m)")]
    private float yOffset = 0.02f;

    private readonly List<LineRenderer> _lines = new();
    private bool _visible = false;

    private void Awake()
    {
        // 시작할 때 테두리 라인 생성만 해두고
        RebuildBorder();

        // 처음 씬 진입할 땐 항상 안 보이게 (편집모드 On에서 Show 호출)
        Hide();
    }

    // 외부에서 호출
    public void Show()
    {
        if (_visible) return;
        _visible = true;
        SetLinesActive(true);
    }

    public void Hide()
    {
        if (!_visible && _lines.Count == 0) return;
        _visible = false;
        SetLinesActive(false);
    }

    /// <summary>
    /// Ground Bounds가 바뀌었을 때(스케일 조정 등) 다시 호출
    /// </summary>
    public void RebuildBorder()
    {
        ClearLines();

        if (!groundRenderer)
        {
            Debug.LogWarning("[GridOverlay] groundRenderer가 할당되지 않았습니다.");
            return;
        }

        if (!lineMaterial)
        {
            Debug.LogWarning("[GridOverlay] lineMaterial이 비어 있어서 그리드를 그릴 수 없습니다.");
            return;
        }

        Bounds b = groundRenderer.bounds;
        float minX = b.min.x;
        float maxX = b.max.x;
        float minZ = b.min.z;
        float maxZ = b.max.z;
        float y = b.min.y + yOffset;

        // ─────────────────────────────────────
        // 4개의 모서리 점
        // ─────────────────────────────────────
        Vector3 p00 = new Vector3(minX, y, minZ); // 왼아래
        Vector3 p01 = new Vector3(minX, y, maxZ); // 왼위
        Vector3 p11 = new Vector3(maxX, y, maxZ); // 오른위
        Vector3 p10 = new Vector3(maxX, y, minZ); // 오른아래

        // 테두리 4줄
        CreateLine(p00, p01); // 왼쪽 세로
        CreateLine(p01, p11); // 위쪽 가로
        CreateLine(p11, p10); // 오른쪽 세로
        CreateLine(p10, p00); // 아래쪽 가로

        // 현재 보이는 상태에 맞춰 활성/비활성
        SetLinesActive(_visible);
    }

    // ─────────────────────────────────────────────
    // 내부 Helper
    // ─────────────────────────────────────────────

    private void CreateLine(Vector3 a, Vector3 b)
    {
        var go = new GameObject("GridBorderLine");
        go.transform.SetParent(transform, worldPositionStays: false);

        var lr = go.AddComponent<LineRenderer>();

        lr.positionCount = 2;
        lr.useWorldSpace = true;

        lr.sharedMaterial = lineMaterial;
        lr.startColor = lineColor;
        lr.endColor = lineColor;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;

        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.receiveShadows = false;
        lr.alignment = LineAlignment.View;
        lr.textureMode = LineTextureMode.Stretch;

        lr.SetPosition(0, a);
        lr.SetPosition(1, b);

        _lines.Add(lr);
    }

    private void SetLinesActive(bool on)
    {
        for (int i = 0; i < _lines.Count; i++)
        {
            if (_lines[i])
                _lines[i].gameObject.SetActive(on);
        }
    }

    private void ClearLines()
    {
        for (int i = 0; i < _lines.Count; i++)
        {
            if (_lines[i])
                Destroy(_lines[i].gameObject);
        }
        _lines.Clear();
    }
}
