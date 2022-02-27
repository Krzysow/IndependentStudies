using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public struct RoomData
{
    public Room room;
    public float roomSize;
}

public class TreeNode<T>
{
    private readonly T _value;
    private readonly List<TreeNode<T>> _children = new List<TreeNode<T>>();

    public TreeNode(T value)
    {
        _value = value;
    }

    public TreeNode<T> this[int i]
    {
        get { return _children[i]; }
    }

    public TreeNode<T> Parent { get; private set; }

    public T Value { get { return _value; } }

    public ReadOnlyCollection<TreeNode<T>> Children
    {
        get { return _children.AsReadOnly(); }
    }

    public TreeNode<T> AddChild(T value)
    {
        var node = new TreeNode<T>(value) { Parent = this };
        _children.Add(node);
        return node;
    }

    public TreeNode<T>[] AddChildren(params T[] values)
    {
        return values.Select(AddChild).ToArray();
    }

    public bool RemoveChild(TreeNode<T> node)
    {
        return _children.Remove(node);
    }

    public void Traverse(Action<T> action)
    {
        action(Value);
        foreach (var child in _children)
            child.Traverse(action);
    }

    //IEnumerator IEnumerable.GetEnumerator()
    //{
    //    return (IEnumerator)GetEnumerator();
    //}

    //public TreeEnum GetEnumerator()
    //{
    //    return new TreeEnum(_children.ToArray());
    //}

    //public class TreeEnum : IEnumerator
    //{
    //    public List<TreeNode<T>>[] _tree;

    //    // Enumerators are positioned before the first element
    //    // until the first MoveNext() call.
    //    int position = -1;

    //    public TreeEnum(List<TreeNode<T>>[] list)
    //    {
    //        _tree = list;
    //    }

    //    public bool MoveNext()
    //    {
    //        position++;
    //        return (position < _tree.Length);
    //    }

    //    public void Reset()
    //    {
    //        position = -1;
    //    }

    //    object IEnumerator.Current
    //    {
    //        get
    //        {
    //            return Current;
    //        }
    //    }

    //    public T Current
    //    {
    //        get
    //        {
    //            try
    //            {
    //                return _tree[position];
    //            }
    //            catch (IndexOutOfRangeException)
    //            {
    //                throw new InvalidOperationException();
    //            }
    //        }
    //    }
    //}

}
