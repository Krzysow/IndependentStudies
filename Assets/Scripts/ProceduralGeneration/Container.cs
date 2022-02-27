using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class Container
{
    public Container(float x, float y, float width, float height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public float X { get; }
    public float Y { get; }
    public float Width { get; }
    public float Height { get; }

    public float ShortestEdge => Mathf.Min(Width, Height);

    public IDictionary<TreemapItem, Rect> GetCoordinates(TreemapItem[] row)
    {
        // getCoordinates - for a row of boxes which we've placed 
        //                  return an array of their cartesian coordinates
        var coordinates = new Dictionary<TreemapItem, Rect>();
        var subx = this.X;
        var suby = this.Y;
        var areaWidth = row.Select(c => c.Area).Sum() / Height;
        var areaHeight = row.Select(c => c.Area).Sum() / Width;
        if (Width >= Height)
        {
            for (int i = 0; i < row.Length; i++)
            {
                var rect = new Rect(subx, suby, areaWidth, row[i].Area / areaWidth);
                coordinates.Add(row[i], rect);
                suby += row[i].Area / areaWidth;
            }
        }
        else
        {
            for (int i = 0; i < row.Length; i++)
            {
                var rect = new Rect(subx, suby, row[i].Area / areaHeight, areaHeight);
                coordinates.Add(row[i], rect);
                subx += row[i].Area / areaHeight;
            }
        }
        return coordinates;
    }

    public Container CutArea(int area)
    {
        // cutArea - once we've placed some boxes into an row we then need to identify the remaining area, 
        //           this function takes the area of the boxes we've placed and calculates the location and
        //           dimensions of the remaining space and returns a container box defined by the remaining area
        if (Width >= Height)
        {
            var areaWidth = area / Height;
            var newWidth = Width - areaWidth;
            return new Container(X + areaWidth, Y, newWidth, Height);
        }
        else
        {
            var areaHeight = area / Width;
            var newHeight = Height - areaHeight;
            return new Container(X, Y + areaHeight, Width, newHeight);
        }
    }
}
