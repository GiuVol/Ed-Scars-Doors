using UnityEngine;

public class PhysicalItem : MonoBehaviour
{
    [SerializeField]
    private Item _item;

    public Item ItemData
    {
        get
        {
            return _item;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer(GameFormulas.CollidesWithGroundOnlyLayerName);

        foreach (Transform child in transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer(GameFormulas.ItemLayerName);
        }
    }
}
