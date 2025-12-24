using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;

[AddComponentMenu("UI/Effects/Hand Drawn Outline")]
public class HandDrawnOutline : Shadow
{
    [Range(0f, 1f)]
    public float jitterAmount = 0.4f;

    [Range(0.5f, 6f)]
    public float noiseScale = 3f;

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
            return;

        var verts = ListPool<UIVertex>.Get();
        vh.GetUIVertexStream(verts);

        int count = verts.Count;
        if (verts.Capacity < count * 5)
            verts.Capacity = count * 5;

        ApplyJitteredShadow(verts, count,  effectDistance.x,  effectDistance.y);
        ApplyJitteredShadow(verts, count,  effectDistance.x, -effectDistance.y);
        ApplyJitteredShadow(verts, count, -effectDistance.x,  effectDistance.y);
        ApplyJitteredShadow(verts, count, -effectDistance.x, -effectDistance.y);

        vh.Clear();
        vh.AddUIVertexTriangleStream(verts);
        ListPool<UIVertex>.Release(verts);
    }

    private void ApplyJitteredShadow(
        System.Collections.Generic.List<UIVertex> verts,
        int originalCount,
        float dx,
        float dy)
    {
        for (int i = 0; i < originalCount; i++)
        {
            var v = verts[i];
            Vector3 p = v.position;

            float nx = Mathf.PerlinNoise(p.x * noiseScale, p.y * noiseScale) - 0.5f;
            float ny = Mathf.PerlinNoise(p.y * noiseScale, p.x * noiseScale) - 0.5f;

            Vector2 jitter = new Vector2(nx, ny) * jitterAmount;

            v.position.x += dx + jitter.x;
            v.position.y += dy + jitter.y;

            Color32 c = v.color;
v.color = new Color32(
    (byte)(effectColor.r * 255f),
    (byte)(effectColor.g * 255f),
    (byte)(effectColor.b * 255f),
    (byte)(c.a * effectColor.a * 255f)
);


            verts.Add(v);
        }
    }
}
