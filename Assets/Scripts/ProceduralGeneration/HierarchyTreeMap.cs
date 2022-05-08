using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HierarchyTreemap
{
    public HierarchyTreemap(string label)
    {
        Label = label;
        Children = new List<HierarchyTreemap>();
        Doors = new List<GameObject> ();
    }

    public HierarchyTreemap(string label, List<HierarchyTreemap> children)
    {
        Label = label;
        Children = children;
        Doors = new List<GameObject>();
    }

    public string Label { get; set; }
    public List<HierarchyTreemap> Children { get; set; }
    public Rect Rect { get; set; }
    public GameObject GameObject { get; set; }
    public List<GameObject> Doors { get; set; }
    public HierarchyTreemap Find(string label)
    {
        HierarchyTreemap result = null;
        if (label == Label)
        {
            result = this;
        }
        else
        {
            foreach (HierarchyTreemap child in Children)
            {
                result = child.Find(label);
                if (result != null && result.Label == label)
                {
                    break;
                }
            }
        }
        return result;
    }
}
