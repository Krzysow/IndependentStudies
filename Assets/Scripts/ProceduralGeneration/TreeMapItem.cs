using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreemapItem
{
    private TreemapItem(string label)
    {
        Label = label;
        //FillBrush = Brushes.White;
        //BorderBrush = Brushes.Black;
        //TextBrush = Brushes.Black;
    }

    public TreemapItem(string label, int area/*, Brush fillBrush*/) : this(label)
    {
        Area = area;
        //FillBrush = fillBrush;
        Children = null;
    }

    public TreemapItem(string label, List<TreemapItem> children) : this(label)
    {
        // in this implementation if there are children - all other properies are ignored
        // but this can be changed in future
        Children = children;
    }

    // Label to write on rectangle
    public string Label { get; set; }
    //// color to fill rectangle with
    //public Brush FillBrush { get; set; }
    //// color to fill rectangle border with
    //public Brush BorderBrush { get; set; }
    //// color of label
    //public Brush TextBrush { get; set; }
    // area
    public int Area { get; set; }
    // children
    public List<TreemapItem> Children { get; set; }
}
