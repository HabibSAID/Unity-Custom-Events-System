using UnityEngine;

public class CustomEventInvoker : MonoBehaviour
{
    [Header("Custom Events work exactly like UnityEvents, but these can accept methods with more than 1 parameter.")]
    public CustomEvent Events;

    /// <summary>
    /// Call this to invoke all assigned custom events.
    /// </summary>
    public void InvokeCustomEvents()
    {
        if (Events != null)
        {
            Events.Invoke();
        }
    }

    // --------------------------
    // Example methods (for testing)
    // --------------------------

    /// <summary>
    /// Example with int, float, and bool parameters.
    /// </summary>
    public void TestMethodOne(int number, float speed, bool isActive)
    {
        Debug.Log($"[TestMethodOne] number={number}, speed={speed}, isActive={isActive}");
    }

    /// <summary>
    /// Example with AudioClip, MeshFilter, and Material parameters.
    /// </summary>
    public void TestMethodTwo(AudioClip audio, MeshFilter meshFilter, Material material)
    {
        string audioName = audio ? audio.name : "null";
        string meshName = meshFilter ? meshFilter.name : "null";
        string matName = material ? material.name : "null";

        Debug.Log($"[TestMethodTwo] AudioClip={audioName}, MeshFilter={meshName}, Material={matName}");
    }

    // --------------------------
    // Context Menu (Inspector)
    // --------------------------

    [ContextMenu("Invoke Custom Events")]
    private void ContextInvokeCustomEvents()
    {
        InvokeCustomEvents();
    }
}
