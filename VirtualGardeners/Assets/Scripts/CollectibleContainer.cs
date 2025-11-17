using UnityEngine;

public class CollectibleContainer : MonoBehaviour
{
    // Script to power a generic collectible container
    // Functions similar to a composter from minecraft, with a rising layer
    // proportional to percent of available items collected.

    public string itemTag;   // Tag for collectible item
    public Transform layer;         // Visual layer that rises
    public float minHeight = 0f;    // Lowest position of the layer
    public float maxHeight = 1f;    // Highest position of the layer

    [HideInInspector] public int itemsAmount;       // Total items in scene
    [HideInInspector] public int itemsCollected;    // Items collected so far

    // Calculate percent done (items collected out of total possible)
    public float GetPercentDone()
    { return Mathf.Clamp01((float)itemsCollected / itemsAmount); }
    
    // Return amounts done (amount done and total amount needed)
    public (int, int) GetAmountDone()
    { return (itemsCollected, itemsAmount); }

    void Start()
    {
        // Count all item items in the scene at start
        layer.localPosition = new Vector3(layer.localPosition.x, minHeight, layer.localPosition.z);
        itemsAmount = GameObject.FindGameObjectsWithTag(itemTag).Length;
        if (itemsAmount == 0) this.enabled = false;
        itemsCollected = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is item
        if (other.CompareTag(itemTag))
        {
            itemsCollected++;
            Destroy(other.gameObject);
            UpdateLayer();
        }
    }

    void UpdateLayer()
    {
        if (itemsAmount <= 0) return;

        // Calculate fraction filled
        float fraction = GetPercentDone();

        // Lerp between min and max layer height
        Vector3 pos = layer.localPosition;
        pos.y = Mathf.Lerp(minHeight, maxHeight, fraction);
        layer.localPosition = pos;
    }
}
