using System.Collections.Generic;
using UnityEngine;

// Adapted from code posted by Acme Nerd Games on gamedev.stackexchange.com
// https://gamedev.stackexchange.com/questions/197313/show-colliders-in-a-build-game-in-unity

[RequireComponent(typeof(Collider2D))]
public class DrawCollider2D : MonoBehaviour
{
    private const bool DRAW_COLLIDERS = true;

    [SerializeField] private GameObject _linePrefab;
    [SerializeField] private Color _lineColor = Color.white;

    private List<LineRenderer> _lineRenderers = new List<LineRenderer>();
    Collider2D _collider2D;

    void Start()
    {
        if (!DRAW_COLLIDERS)
        {
            Destroy(this);
            return;
        }

        // Prioritize CompositeCollider2D if it exists, as it holds the final geometry.
        _collider2D = GetComponent<CompositeCollider2D>();
        if (_collider2D == null)
        {
            _collider2D = GetComponent<Collider2D>();
        }
    }

    void Update()
    {
        HiliteCollider();
    }

    void HiliteCollider()
    {
        // Disable all line renderers if the main collider is disabled.
        if (!_collider2D.enabled)
        {
            foreach (var lr in _lineRenderers)
            {
                lr.enabled = false;
            }
            return;
        }

        if (_collider2D is PolygonCollider2D polygonCollider) DrawPolygonCollider2D(polygonCollider);
        else if (_collider2D is BoxCollider2D boxCollider) DrawBoxCollider2D(boxCollider);
        else if (_collider2D is CapsuleCollider2D capsuleCollider) DrawCapsuleCollider2D(capsuleCollider);
        else if (_collider2D is CompositeCollider2D compositeCollider) DrawCompositeCollider2D(compositeCollider);
        else Debug.LogError($"Unsupported collider type for {nameof(DrawCollider2D)}: {_collider2D.GetType().Name}");
    }

    private LineRenderer GetLineRenderer(int index)
    {
        while (_lineRenderers.Count <= index)
        {
            var lineRenderer = Instantiate(_linePrefab).GetComponent<LineRenderer>();
            lineRenderer.transform.SetParent(transform, false);
            lineRenderer.transform.localPosition = Vector3.zero;
            lineRenderer.useWorldSpace = false;
            _lineRenderers.Add(lineRenderer);
        }
        var lr = _lineRenderers[index];
        lr.enabled = true;
        lr.startColor = _lineColor;
        lr.endColor = _lineColor;
        return lr;
    }

    private void DrawPolygonCollider2D(PolygonCollider2D polygonCollider2D)
    {
        var points = polygonCollider2D.GetPath(0); // Assuming only one path
        var lineRenderer = GetLineRenderer(0);

        Vector3[] positions = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            positions[i] = points[i];
        }

        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(positions);
    }

    private void DrawBoxCollider2D(BoxCollider2D boxCollider2D)
    {
        Vector3[] positions = new Vector3[4];
        var lineRenderer = GetLineRenderer(0);
        var size = boxCollider2D.size;
        var offset = boxCollider2D.offset;

        positions[0] = new Vector2(size.x / 2, size.y / 2) + offset;
        positions[1] = new Vector2(-size.x / 2, size.y / 2) + offset;
        positions[2] = new Vector2(-size.x / 2, -size.y / 2) + offset;
        positions[3] = new Vector2(size.x / 2, -size.y / 2) + offset;

        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }

    private void DrawCapsuleCollider2D(CapsuleCollider2D capsuleCollider)
    {
        const int segments = 20; // Number of segments for the entire capsule outline
        var lineRenderer = GetLineRenderer(0);
        Vector2 size = capsuleCollider.size;
        Vector2 offset = capsuleCollider.offset;
        CapsuleDirection2D direction = capsuleCollider.direction;

        float radius;
        Vector2 center1, center2;

        if (direction == CapsuleDirection2D.Vertical)
        {
            radius = size.x / 2f;
            float straightLength = Mathf.Max(0, size.y - size.x);
            center1 = new Vector2(0, straightLength / 2f) + offset;
            center2 = new Vector2(0, -straightLength / 2f) + offset;
        }
        else // Horizontal
        {
            radius = size.y / 2f;
            float straightLength = Mathf.Max(0, size.x - size.y);
            center1 = new Vector2(straightLength / 2f, 0) + offset;
            center2 = new Vector2(-straightLength / 2f, 0) + offset;
        }

        Vector3[] positions = new Vector3[segments + 1];
        int semiCircleSegments = segments / 2;

        // First semi-circle
        for (int i = 0; i <= semiCircleSegments; i++)
        {
            float angle = (float)i / semiCircleSegments * Mathf.PI;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            positions[i] = (direction == CapsuleDirection2D.Vertical)
                ? center1 + new Vector2(x, y)
                : center1 + new Vector2(y, x);
        }

        // Second semi-circle
        for (int i = 0; i <= semiCircleSegments; i++)
        {
            float angle = (float)i / semiCircleSegments * Mathf.PI + Mathf.PI; // Add PI to start on the other side
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            positions[semiCircleSegments + i] = (direction == CapsuleDirection2D.Vertical)
                ? center2 + new Vector2(x, y)
                : center2 + new Vector2(y, x);
        }

        lineRenderer.positionCount = segments + 1;
        lineRenderer.SetPositions(positions);
    }

    private void DrawCompositeCollider2D(CompositeCollider2D compositeCollider)
    {
        int pathCount = compositeCollider.pathCount;
        var points = new List<Vector2>();

        for (int i = 0; i < pathCount; i++)
        {
            points.Clear();
            compositeCollider.GetPath(i, points);

            var lineRenderer = GetLineRenderer(i);
            Vector3[] positions = new Vector3[points.Count];
            for (int j = 0; j < points.Count; j++)
            {
                positions[j] = points[j];
            }

            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(positions);
        }

        // Disable any extra line renderers that are not being used
        for (int i = pathCount; i < _lineRenderers.Count; i++)
        {
            _lineRenderers[i].enabled = false;
        }
    }
}
