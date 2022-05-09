using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum Direction { up, down, left, right }

public class Treemap
{
    GameObject door;

    public void Build(TreemapItem[] items, float width, float height, GameObject gameObject, GameObject doorGM)
    {
        door = doorGM;

        var map = BuildMultidimensional(items, width, height, 0, 0);
        //var bmp = new Bitmap(width, height);

        //var g = Graphics.FromImage(bmp);
        //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
        HierarchyTreemap hierarchyNode = Camera.main.GetComponent<DrawWalls>().hierarchy;
        foreach (var kv in map)
        {
            var item = kv.Key;
            var rect = kv.Value;
            // fill rectangle
            GameObject treeObject = GameObject.Instantiate(gameObject);
            treeObject.transform.position = new Vector3(rect.x + rect.width * 0.5f, 0f, rect.y + rect.height * 0.5f);
            treeObject.transform.localScale = new Vector3(rect.width, 1f, rect.height);
            treeObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material.color = Random.ColorHSV();
            string roomName = kv.Key.Label.ToString();
            treeObject.transform.Find("Canvas").Find("Text").GetComponent<Text>().text = roomName;

            HierarchyTreemap currentNode = hierarchyNode.Find(roomName);
            currentNode.Rect = rect;
            currentNode.GameObject = treeObject;

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
        Adjust(hierarchyNode);
    }

    void Adjust(HierarchyTreemap node)
    {
        if (node.Children != null)
        {
            foreach (HierarchyTreemap child in node.Children)
            {
                Rect parentRect = node.Rect;
                Rect childRect = child.Rect;

                // check if rooms are adjacent
                if (node.GameObject.transform.position.x - parentRect.height * 0.5f - 1 <= child.GameObject.transform.position.x + childRect.width * .5f + 1
                    && node.GameObject.transform.position.x + parentRect.width * .5f + 1 >= child.GameObject.transform.position.x - childRect.width * .5f - 1
                    && node.GameObject.transform.position.z - parentRect.height * .5f - 1 <= child.GameObject.transform.position.z + childRect.height * .5f + 1
                    && node.GameObject.transform.position.z + parentRect.height * .5f + 1 >= child.GameObject.transform.position.z - childRect.height * .5f - 1)
                {
                    PlaceDoor(node, child);
                }
                else // move it if not
                {
                    Vector3 newPosition = node.GameObject.transform.position;

                    if (childRect.width > childRect.height)
                    {
                        if (TestSpace(node, childRect, Direction.up))
                        {
                            newPosition += Vector3.forward * (node.GameObject.transform.lossyScale.z * .5f + child.GameObject.transform.lossyScale.z * .5f);
                        }
                        else if (TestSpace(node, childRect, Direction.down))
                        {
                            newPosition += Vector3.back * (node.GameObject.transform.lossyScale.z * .5f + child.GameObject.transform.lossyScale.z * .5f);
                        }
                        else if (TestSpace(node, childRect, Direction.left))
                        {
                            newPosition += Vector3.left * (node.GameObject.transform.lossyScale.x * .5f + child.GameObject.transform.lossyScale.x * .5f);
                        }
                        else if (TestSpace(node, childRect, Direction.right))
                        {
                            newPosition += Vector3.right * (node.GameObject.transform.lossyScale.x * .5f + child.GameObject.transform.lossyScale.x * .5f);
                        }
                        else
                        {
                            newPosition = child.GameObject.transform.position;
                        }
                    }
                    else
                    {
                        if (TestSpace(node, childRect, Direction.left))
                        {
                            newPosition += Vector3.left * (node.GameObject.transform.lossyScale.x * .5f + child.GameObject.transform.lossyScale.x * .5f);
                        }
                        else if (TestSpace(node, childRect, Direction.right))
                        {
                            newPosition += Vector3.right * (node.GameObject.transform.lossyScale.x * .5f + child.GameObject.transform.lossyScale.x * .5f);
                        }
                        else if (TestSpace(node, childRect, Direction.up))
                        {
                            newPosition += Vector3.forward * (node.GameObject.transform.lossyScale.z * .5f + child.GameObject.transform.lossyScale.z * .5f);
                        }
                        else if (TestSpace(node, childRect, Direction.down))
                        {
                            newPosition += Vector3.back * (node.GameObject.transform.lossyScale.z * .5f + child.GameObject.transform.lossyScale.z * .5f);
                        }
                        else
                        {
                            newPosition = child.GameObject.transform.position;
                        }
                    }

                    child.GameObject.transform.position = newPosition;
                    PlaceDoor(node, child);
                }

                Adjust(child);
            }
        }
    }

    void PlaceDoor(HierarchyTreemap parentRoom, HierarchyTreemap childRoom)
    {
        Vector3 parentCenter = parentRoom.GameObject.transform.position;// new Vector3(parentRoom.GameObject.transform.position.x, 0, parentRoom.GameObject.transform.position.z + parentRoom.Rect.height * .5f);
        Vector3 childCenter = childRoom.GameObject.transform.position; //new Vector3(childRoom.GameObject.transform.position.x, 0, childRoom.GameObject.transform.position.z + childRoom.Rect.height * .5f);

        Vector3 offset = Vector3.zero;
        Quaternion rotation = Quaternion.identity;

        if (Mathf.Abs(childCenter.x - parentCenter.x) > Mathf.Abs(childCenter.z - parentCenter.z))
        {
            if (childCenter.x < parentCenter.x)
                offset = Vector3.right * childRoom.Rect.width * .5f;
            else
                offset = Vector3.left * childRoom.Rect.width * .5f;

            offset += new Vector3(0, 0, (parentCenter.z - childCenter.z) * .5f);
            rotation.eulerAngles = new Vector3(0, 90, 0);
        }
        else
        {
            if (childCenter.z < parentCenter.z)
            {
                offset = Vector3.forward * childRoom.Rect.height * .5f;
            }
            else
                offset = Vector3.back * childRoom.Rect.height * .5f;

            offset += new Vector3((parentCenter.x - childCenter.x) * .5f, 0, 0);

        }

        Vector3 position = childCenter + offset;

        GameObject newDoor = GameObject.Instantiate(door, position, rotation);
        parentRoom.Doors.Add(newDoor);
        childRoom.Doors.Add(newDoor);
    }

    bool TestSpace(HierarchyTreemap parentNode, Rect roomToMove, Direction direction)
    {
        switch (direction)
        {
            case Direction.up:
                foreach (GameObject door in parentNode.Doors)
                {
                    if (door.transform.rotation == Quaternion.identity)
                    {
                        if (parentNode.GameObject.transform.position.z < door.transform.position.z)
                            return false;
                    }
                }
                break;
            case Direction.down:
                foreach (GameObject door in parentNode.Doors)
                {
                    if (door.transform.rotation == Quaternion.identity)
                    {
                        if (parentNode.GameObject.transform.position.z > door.transform.position.z)
                            return false;
                    }
                }
                break;
            case Direction.left:
                foreach (GameObject door in parentNode.Doors)
                {
                    if (door.transform.rotation == Quaternion.Euler(new Vector3(0, 90, 0)))
                    {
                        if (parentNode.GameObject.transform.position.x > door.transform.position.x)
                            return false;
                    }
                }
                break;
            case Direction.right:
                foreach (GameObject door in parentNode.Doors)
                {
                    if (door.transform.rotation == Quaternion.Euler(new Vector3(0, 90, 0)))
                    {
                        if (parentNode.GameObject.transform.position.x < door.transform.position.x)
                            return false;
                    }
                }
                break;
        }
        return true;
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