using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Treemap
{
    public void Build(TreemapItem[] items, float width, float height, GameObject gameObject)
    {
        var map = BuildMultidimensional(items, width, height, 0, 0);
        //var bmp = new Bitmap(width, height);

        //var g = Graphics.FromImage(bmp);
        //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
        foreach (var kv in map)
        {
            var item = kv.Key;
            var rect = kv.Value;
            // fill rectangle
            GameObject treeObject = GameObject.Instantiate(gameObject);
            treeObject.transform.position = new Vector3(rect.x + rect.width * 0.5f, 0f, rect.y + rect.height * 0.5f);
            treeObject.transform.localScale = new Vector3(rect.width, 1f, rect.height);
            treeObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material.color = Random.ColorHSV();
            treeObject.transform.Find("Canvas").Find("Text").GetComponent<Text>().text = kv.Key.Label.ToString();
            //g.FillRectangle(item.FillBrush, rect);
            // draw border
            //g.DrawRectangle(new Pen(item.BorderBrush, 1), rect);
            //if (!String.IsNullOrWhiteSpace(item.Label))
            //{
            //    // draw text
            //    var format = new StringFormat();
            //    format.Alignment = StringAlignment.Center;
            //    format.LineAlignment = StringAlignment.Center;
            //    var font = new Font("Arial", 16);
            //    g.DrawString(item.Label, font, item.TextBrush, new RectangleF(rect.X, rect.Y, rect.Width, rect.Height), format);
            //}
        }
        //return bmp;
    }

    private Dictionary<TreemapItem, Rect> BuildMultidimensional(TreemapItem[] items, float width, float height, float x, float y)
    {
        var results = new Dictionary<TreemapItem, Rect>();
        var mergedData = new TreemapItem[items.Length];
        for (int i = 0; i < items.Length; i++)
        {
            // calculate total area of children - current item's area is ignored
            mergedData[i] = SumChildren(items[i]);
        }
        // build a map for this merged items (merged because their area is sum of areas of their children)                
        var mergedMap = BuildFlat(mergedData, width, height, x, y);
        for (int i = 0; i < items.Length; i++)
        {
            var mergedChild = mergedMap[mergedData[i]];
            // inspect children of children in the same way
            if (items[i].Children != null)
            {
                //var headerRect = new Rect(mergedChild.x, mergedChild.y, mergedChild.width, 20);
                //results.Add(mergedData[i], headerRect);
                // reserve 20 pixels of height for header
                foreach (var kv in BuildMultidimensional(items[i].Children.ToArray(), mergedChild.width, mergedChild.height, mergedChild.x, mergedChild.y))
                {
                    results.Add(kv.Key, kv.Value);
                }
            }
            else
            {
                results.Add(mergedData[i], mergedChild);
            }
        }
        return results;
    }

    private Dictionary<TreemapItem, Rect> BuildFlat(TreemapItem[] items, float width, float height, float x, float y)
    {
        // normalize all area values for given width and height
        Normalize(items, width * height);
        var result = new Dictionary<TreemapItem, Rect>();
        Squarify(items, new TreemapItem[0], new Container(x, y, width, height), result);
        return result;
    }

    private void Normalize(TreemapItem[] data, float area)
    {
        var sum = data.Select(c => c.Area).Sum();
        var multi = area / (float)sum;
        foreach (var item in data)
        {
            item.Area = (int)(item.Area * multi);
        }
    }

    private void Squarify(TreemapItem[] data, TreemapItem[] currentRow, Container container, Dictionary<TreemapItem, Rect> stack)
    {
        if (data.Length == 0)
        {
            foreach (var kv in container.GetCoordinates(currentRow))
            {
                stack.Add(kv.Key, kv.Value);
            }
            return;
        }
        var length = container.ShortestEdge;
        var nextPoint = data[0];
        if (ImprovesRatio(currentRow, nextPoint, length))
        {
            currentRow = currentRow.Concat(new[] { nextPoint }).ToArray();
            Squarify(data.Skip(1).ToArray(), currentRow, container, stack);
        }
        else
        {
            var newContainer = container.CutArea(currentRow.Select(c => c.Area).Sum());
            foreach (var kv in container.GetCoordinates(currentRow))
            {
                stack.Add(kv.Key, kv.Value);
            }
            Squarify(data, new TreemapItem[0], newContainer, stack);
        }
    }

    private bool ImprovesRatio(TreemapItem[] currentRow, TreemapItem nextNode, float length)
    {
        // if adding nextNode 
        if (currentRow.Length == 0)
            return true;
        var newRow = currentRow.Concat(new[] { nextNode }).ToArray();
        var currentRatio = CalculateRatio(currentRow, length);
        var newRatio = CalculateRatio(newRow, length);
        return currentRatio >= newRatio;
    }

    private float CalculateRatio(TreemapItem[] row, float length)
    {
        var min = row.Select(c => c.Area).Min();
        var max = row.Select(c => c.Area).Max();
        var sum = row.Select(c => c.Area).Sum();
        return Mathf.Max(Mathf.Pow(length, 2) * max / Mathf.Pow(sum, 2), Mathf.Pow(sum, 2) / (Mathf.Pow(length, 2) * min));
    }

    private TreemapItem SumChildren(TreemapItem item)
    {
        int total = 0;
        if (item.Children?.Count > 0)
        {
            total += item.Children.Sum(c => c.Area);
            foreach (var child in item.Children)
            {
                total += SumChildren(child).Area;
            }
        }
        else
        {
            total = item.Area;
        }
        return new TreemapItem(item.Label, total/*, item.FillBrush*/);
    }
}