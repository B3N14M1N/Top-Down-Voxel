using UnityEngine;

public class DrawRendererBounds : MonoBehaviour
{
    private Renderer cachedRenderer;
    public void OnDrawGizmosSelected()
    {
        if(cachedRenderer == null)
            cachedRenderer = GetComponent<Renderer>();
        if (cachedRenderer == null)
            return;
        var bounds = cachedRenderer.bounds;
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(bounds.center, bounds.extents * 2);
    }
}