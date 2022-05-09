using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrawWalls : MonoBehaviour
{
    //public List<Vector2> pairsOfWalls;
    public GameObject room;
    public GameObject door;
    public HierarchyTreemap hierarchy;

    //float width, height, aspectRatio;
    //TreeNode<RoomData> roomTree = new TreeNode<RoomData>(new RoomData { room = Room.LivingRoom, roomSize = 10.0f});

    // Start is called before the first frame update
    void Start()
    {
        //GenerateHouseShape();
        //GenerateRoomTree();
        GenerateTreeMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //void GenerateHouseShape()
    //{
    //    width = Random.Range(0.3f, 0.4f);
    //    height = Random.Range(0.2f, 0.3f);
    //    aspectRatio = Mathf.Max(width / height, height / width);
    //    //pairsOfWalls.Add(pairsOfWalls[0] + new Vector2(width, height));
    //}

    //void GenerateRoomTree()
    //{
    //    roomTree.AddChild(new RoomData { room = Room.Kitchen, roomSize = 8.0f }).AddChild(new RoomData { room = Room.Pantry, roomSize = 6.0f });
    //    roomTree.AddChild(new RoomData { room = Room.Bedroom, roomSize = 9.0f });
    //    roomTree.AddChild(new RoomData { room = Room.Bathroom, roomSize = 5.0f });
    //    roomTree.AddChild(new RoomData { room = Room.Bedroom, roomSize = 7.0f }).AddChild(new RoomData { room = Room.Toilet, roomSize = 4.0f });
    //}

    //void OnPostRender()
    //{
    //    if (!mat)
    //    {
    //        Debug.LogError("Please Assign a material on the inspector");
    //        return;
    //    }
    //    GL.PushMatrix();
    //    mat.SetPass(0);
    //    GL.LoadOrtho();
    //    GL.Begin(GL.LINES);
    //    GL.Color(Color.red);

    //    for (int i = 0; i < pairsOfWalls.Count - 1; i += 2)
    //    {
    //        // Skala?
    //        GL.Vertex(pairsOfWalls[i]);
    //        GL.Vertex(pairsOfWalls[(i + 1) % pairsOfWalls.Count]);
    //    }
    //    GL.End();

    //    GL.PopMatrix();
    //}

    //void GenerateTreeMap()
    //{
    //    int Width = (int)(width * 1000);
    //    int Height = (int)(height * 1000);
    //    const double MinSliceRatio = 0.35;

    //    var elements = new[] { Random.Range(1, 50), Random.Range(1, 50), Random.Range(1, 50), Random.Range(1, 50), Random.Range(1, 50), Random.Range(1, 50) }
    //     .Select(x => new SquarifyTreeMap.Element<string> { Object = x.ToString(), Value = x })
    //     .OrderByDescending(x => x.Value)
    //     .ToList();

    //    var slice = SquarifyTreeMap.GetSlice(elements, 1, MinSliceRatio);
    //    SetRooms(slice);

    //    var rectangles = SquarifyTreeMap.GetRectangles(slice, Width, Height).ToList();

    //    SquarifyTreeMap.DrawTreemap(rectangles, Width, Height, room);
    //}

    void GenerateTreeMap()
    {
        List<TreemapItem> map = new List<TreemapItem>();
        map.Add(new TreemapItem("living room", (int)(700 + 500 * Random.Range(0f, 1f))));
        hierarchy = new HierarchyTreemap("living room");

        HierarchyTreemap masterBedroomHierarchy = new HierarchyTreemap("bedroom");
        hierarchy.Children.Add(masterBedroomHierarchy);

        List<TreemapItem> sideArea = new List<TreemapItem>();
        float bathroomProbability = Random.Range(0f, 1f);
        if (Menu._difficulty * bathroomProbability > .5f)
        {
            sideArea.Add(new TreemapItem("master bathroom", (int)(40 + 60 * bathroomProbability)));
            masterBedroomHierarchy.Children.Add(new HierarchyTreemap("master bathroom"));
        }
        if (Menu._difficulty * bathroomProbability > .96f)
        {
            sideArea.Add(new TreemapItem("second master bathroom", (int)(40 + 60 * bathroomProbability)));
            masterBedroomHierarchy.Children.Add(new HierarchyTreemap("second master bathroom"));
        }

        List<TreemapItem> masterBedroom = new List<TreemapItem>();
        masterBedroom.Add(new TreemapItem("bedroom", (int)(500 + 200 * Random.Range(0f, 1f))));
        if (sideArea.Count > 0)
        {
            TreemapItem sideAreaItem = new TreemapItem("side rooms", sideArea);
            masterBedroom.Add(sideAreaItem);
        }

        map.Add(new TreemapItem("master bedroom", masterBedroom));

        List<TreemapItem> kitchen = new List<TreemapItem>();
        kitchen.Add(new TreemapItem("kitchen", (int)(200 + 300 * Random.Range(0f, 1f))));
        HierarchyTreemap kitchenHierarchy = new HierarchyTreemap("kitchen");
        hierarchy.Children.Add(kitchenHierarchy);
        if (Menu._difficulty * Random.Range(0f, 1f) > 1.9f)
        {
            kitchen.Add(new TreemapItem("pantry", (int)(35 + 50 * Random.Range(0f, 1f))));
            kitchenHierarchy.Children.Add(new HierarchyTreemap("pantry"));
        }

        map.Add(new TreemapItem("kitchen area", kitchen));

        map.Add(new TreemapItem("main bathroom", (int)(75 + 100 * Random.Range(0f, 1f))));
        hierarchy.Children.Add(new HierarchyTreemap("main bathroom"));

        //if (Menu._difficulty * Random.Range(0f, 1f) > .9f)
        //{
        //    List<TreemapItem> secondBedroom = new List<TreemapItem>();
        //    secondBedroom.Add(new TreemapItem("second bedroom", (int)(300 + 200 * Random.Range(0f, 1f))));
        //    map.Add(new TreemapItem("second bedroom", secondBedroom));
        //    hierarchy.Children.Add(new HierarchyTreemap("second bedroom"));
        //}

        //var map = new[]
        //{
        //    new TreemapItem("ItemA", new[]
        //        {
        //            new TreemapItem("ItemA-1", new[]
        //            {
        //                new TreemapItem("ItemA-11", 100),
        //                new TreemapItem("ItemA-12", 100),
        //            }),
        //            new TreemapItem("ItemA-2", 500),
        //            new TreemapItem("ItemA-3", 600),
        //        }),
        //    new TreemapItem("ItemB", 1000),
        //    new TreemapItem("ItemC", new[]
        //        {
        //            new TreemapItem("ItemC-1", 200),
        //            new TreemapItem("ItemC-2", 500),
        //            new TreemapItem("ItemC-3", 600),
        //        }),
        //    new TreemapItem("ItemD", 2400) { },
        //    new TreemapItem("ItemE", new[]
        //        {
        //            new TreemapItem("ItemE-1", 200),
        //            new TreemapItem("ItemE-2", 500),
        //            new TreemapItem("ItemE-3", 600),
        //        })
        //};
        new Treemap().Build(map.ToArray(), Random.Range(0, 21) * Menu._size + 100, Random.Range(0, 21) * Menu._size + 100, room, door);
    }

    void SetRooms(SquarifyTreeMap.Slice<string> slice)
    {
        if (slice.SubSlices != null)
        {
            foreach (SquarifyTreeMap.Slice<string> subSlice in slice.SubSlices)
            {
                SetRooms(subSlice);
            }
        }
        slice.room = (Room)Random.Range(0, (int)Room.LaundryRoom);
    }
}
