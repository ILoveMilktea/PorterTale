using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextMeshSortingLayer : MonoBehaviour
{
    public string sortingLayerName;
    public int sortingOrder;

    void Start()
    {
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        mesh.sortingLayerName = sortingLayerName;
        mesh.sortingOrder = sortingOrder;
    }
}
