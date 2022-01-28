using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrawWalls : MonoBehaviour
{
    public Material mat;
    public List<Vector2> pairsOfWalls;
    public GameObject room;
    
    float width, height, aspectRatio;
    TreeNode<RoomData> roomTree = new TreeNode<RoomData>(new RoomData { room = Room.LivingRoom, roomSize = 10.0f});

    // Start is called before the first frame update
    void Start()
    {
        GenerateHouseShape();
        GenerateRoomTree();
        GenerateTreeMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateHouseShape()
    {
        width = Random.Range(0.3f, 0.4f);
        height = Random.Range(0.2f, 0.3f);
        aspectRatio = Mathf.Max(width / height, height / width);
        //pairsOfWalls.Add(pairsOfWalls[0] + new Vector2(width, height));
    }

    void GenerateRoomTree()
    {
        roomTree.AddChild(new RoomData { room = Room.Kitchen, roomSize = 8.0f }).AddChild(new RoomData { room = Room.Pantry, roomSize = 6.0f });
        roomTree.AddChild(new RoomData { room = Room.Bedroom, roomSize = 9.0f });
        roomTree.AddChild(new RoomData { room = Room.Bathroom, roomSize = 5.0f });
        roomTree.AddChild(new RoomData { room = Room.Bedroom, roomSize = 7.0f }).AddChild(new RoomData { room = Room.Toilet, roomSize = 4.0f });
    }

    void OnPostRender()
    {
        if (!mat)
        {
            Debug.LogError("Please Assign a material on the inspector");
            return;
        }
        GL.PushMatrix();
        mat.SetPass(0);
        GL.LoadOrtho();
        GL.Begin(GL.LINES);
        GL.Color(Color.red);

        for (int i = 0; i < pairsOfWalls.Count - 1; i += 2)
        {
            // Skala?
            GL.Vertex(pairsOfWalls[i]);
            GL.Vertex(pairsOfWalls[(i + 1) % pairsOfWalls.Count]);
        }
        GL.End();

        GL.PopMatrix();
    }

    void GenerateTreeMap()
    {
        int Width = (int)(width * 1000);
        int Height = (int)(height * 1000);
        const double MinSliceRatio = 0.35;

        var elements = new[] { Random.Range(1, 50), Random.Range(1, 50), Random.Range(1, 50), Random.Range(1, 50), Random.Range(1, 50), Random.Range(1, 50) }
         .Select(x => new SquarifyTreeMap.Element<string> { Object = x.ToString(), Value = x })
         .OrderByDescending(x => x.Value)
         .ToList();

        var slice = SquarifyTreeMap.GetSlice(elements, 1, MinSliceRatio);
        SetRooms(slice);

        var rectangles = SquarifyTreeMap.GetRectangles(slice, Width, Height).ToList();

        SquarifyTreeMap.DrawTreemap(rectangles, Width, Height, room);
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
