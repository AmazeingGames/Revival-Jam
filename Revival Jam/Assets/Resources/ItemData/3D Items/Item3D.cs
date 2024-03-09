using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item3D : MonoBehaviour
{
    [SerializeField] LayerMask layermask;
    public static event Action<ItemData3D> GainItem3D;
    public ItemData3D ItemData3D { get; private set; }

    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] BoxCollider boxCollider;

    public void InitializeData(ItemData3D itemData3D)
    {
        ItemData3D = itemData3D;

        meshRenderer.material = itemData3D.Material;
        meshFilter.mesh = itemData3D.Mesh;

        transform.localScale = new Vector3(ItemData3D.Scale, ItemData3D.Scale, ItemData3D.Scale);
        transform.localPosition = Vector3.zero;
        throw new NotImplementedException("SET COLLISION SIZE");

    }

    private void Update()
    {
# if DEBUG
        if (ItemData3D != null)
            InitializeData(ItemData3D);
# endif
        bool isMouseOver = IsMouseOver();
        if (isMouseOver)
            Debug.Log("Mouse over!");
        if (Input.GetMouseButtonDown(0) && isMouseOver)
            GrabItem();
    }

    void GrabItem()
    {
        Debug.Log("mouse click!");
        GainItem3D?.Invoke(ItemData3D);
        gameObject.SetActive(false);
    }

    bool IsMouseOver()
    {
        if (Camera.main == null)
            return false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit, 5, layermask))
            return hit.transform == transform;
        return false;
    }
}
