using UnityEngine;
using System.Collections.Generic;
using System.Collections;

// This script is adapted from these,
// but has been heavily modified in some areas:
// http://www.redblobgames.com/pathfinding/a-star/implementation.html#csharp
// https://gist.github.com/DanBrooker/1f8855367ae4add40792

// I'm continuing to optimize and change things here. I would not use this
// in production as it is.

// Note that Floor, Forest and Wall are somewhat arbitrary,
// but also represent three differnt types of tiles, which are
// all handled differently by A*. Floors are Passable, walls are not,
// and forests are passable but with a higher movement cost.
public enum TileType
{
    Floor = 1,
    Forest = 5,
    Forest1 = 10,
    Forest2 = 15,
    Forest3 = 20,
    Forest4 = 25,
    Forest5 = 30,
    Wall = System.Int32.MaxValue
}

public class Location
{

    public readonly int x, z;

    public Location(int x, int y)
    {
        this.x = x;
        this.z = y;
    }

    public Location(float x, float y)
    {
        this.x = Mathf.FloorToInt(x);
        this.z = Mathf.FloorToInt(y);
    }

    public Location(Vector3 position)
    {
        this.x = Mathf.RoundToInt(position.x);
        this.z = Mathf.RoundToInt(position.z);
        //Debug.Log("x = " + x);
        //Debug.Log("z = " + z);
    }

    public Vector3 vector3()
    {
        return new Vector3(this.x, 0, this.z);
    }

    public override bool Equals(System.Object obj)
    {
        Location loc = obj as Location;
        return this.x == loc.x && this.z == loc.z;
    }

    // This is creating collisions. How do I solve this?
    public override int GetHashCode()
    {
        return (x * 597) ^ (z * 1173);
    }
}

public class Cell
{
    public TileType tile;
    public int corridorIdx = -1;
    public int corridorYRot = -1;
    public int childEulerZ = -1;
    public int childEulerX = -1;
    public Location[] disallowedDIRS = new Location[0];
}

public class SquareGrid
{
    // DIRS is directions
    // I added diagonals to this but noticed it can create problems--
    // like the path will go through obstacles that are diagonal from each other.
    public static readonly Location[] DIRS = new[] {
    new Location(1, 0), // to right of tile
    new Location(0, -1), // below tile
    new Location(-1, 0), // to left of tile
    new Location(0, 1), // above tile
    //new Location(1, 1), // diagonal top right
    //new Location(-1, 1), // diagonal top left
    //new Location(1, -1), // diagonal bottom right
    //new Location(-1, -1) // diagonal bottom left
  };

    // The x and y here represent the grid's starting point, 0,0.
    // And width and height are how many units wide and high the grid is.
    // See how I use this in TileManager.cs to see how you can keep
    // your Unity GameObjects in sync with this abstracted grid.
    public SquareGrid(int x, int y, int width, int height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }

    public int x, y, width, height;

    // This is a 2d array that stores each tile's type and movement cost
    // using the TileType enum defined above
    public Cell[,] tiles;// = new Cell[40, 40];

    // Check if a location is within the bounds of this grid.
    public bool InBounds(Location id)
    {
        return (/*x*/0 <= id.x) && (id.x < width) && (/*y*/0 <= id.z) && (id.z < height);
    }

    // Everything that isn't a Wall is Passable
    public bool Passable(Location id)
    {
        return (int)tiles[id.x, id.z].tile < System.Int32.MaxValue;
    }

    //To disallow certain moves, for vents mainly
    public bool IsDIRAllowed(Location next, Location curr)
    {
        //Debug.Log("IsDIRAllowed = " + tiles[curr.x, curr.z]);
        //Debug.Log("IsDIRAllowed = " + tiles[curr.x, curr.z].allowedDIRS.Length);
        
        for (int i = 0; i < tiles[next.x, next.z].disallowedDIRS.Length; i++)
        {
            if (tiles[next.x, next.z].disallowedDIRS[i].vector3() + next.vector3() == curr.vector3())
            {
                //Debug.Log("isDirsallowed = " + (tiles[next.x, next.z].allowedDIRS[0].x + next.x));
                //Debug.Log("isDirsallowed = " + (tiles[next.x, next.z].allowedDIRS[1].x + next.x));
                //Debug.Log("isDirsallowed = " + curr.x);
                //Debug.Log("isDirsallowed = " + (tiles[next.x, next.z].allowedDIRS[0].z + next.z));
                //Debug.Log("isDirsallowed = " + (tiles[next.x, next.z].allowedDIRS[1].z + next.z));
                //Debug.Log("isDirsallowed = " + curr.z);

                //Debug.Log("isDirsallowed v3 = " + (tiles[next.x, next.z].allowedDIRS[i].vector3() + next.vector3()) * -4);
                //Debug.Log("isDirsallowed v3 = " + curr.vector3() * -4);

                //Debug.Log("good its false");
                return false;
            }
        }
        for (int i = 0; i < tiles[curr.x, curr.z].disallowedDIRS.Length; i++)
        {
            if (tiles[curr.x, curr.z].disallowedDIRS[i].vector3() + curr.vector3() == next.vector3())
            {

                //Debug.Log("good its false");
                return false;
            }
        }
        //if(tiles[next.x, next.z].allowedDIRS.Length > 0)
        //{
        //    //Debug.Log("isDirsallowed = " + (tiles[next.x, next.z].allowedDIRS[0].x + next.x));
        //    //Debug.Log("isDirsallowed = " + (tiles[next.x, next.z].allowedDIRS[1].x + next.x));
        //    //Debug.Log("isDirsallowed = " + curr.x);
        //    //Debug.Log("isDirsallowed = " + (tiles[next.x, next.z].allowedDIRS[0].z + next.z));
        //    //Debug.Log("isDirsallowed = " + (tiles[next.x, next.z].allowedDIRS[1].z + next.z));
        //    //Debug.Log("isDirsallowed = " + curr.z);
        //    //Debug.Log("isDirsallowed v3 = " + (tiles[next.x, next.z].allowedDIRS[0].vector3() + next.vector3()) * -4);
        //    //Debug.Log("isDirsallowed v3 = " + (tiles[next.x, next.z].allowedDIRS[1].vector3() + next.vector3()) * -4);
        //}
        //Debug.Log("good its true... => curr = " + curr.vector3() * -4 + " next = " + next.vector3() * -4);
        return true;
    }

    // If the heuristic = 2f, the movement is diagonal
    public float Cost(Location a, Location b)
    {
        if (AStarSearch.Heuristic(a, b) == 2f)
        {
            return (float)(int)tiles[b.x, b.z].tile * Mathf.Sqrt(2f);
        }
        return (float)(int)tiles[b.x, b.z].tile;
    }

    // Check the tiles that are next to, above, below, or diagonal to
    // this tile, and return them if they're within the game bounds and passable
    public IEnumerable<Location> Neighbors(Location id)
    {
        foreach (var dir in DIRS)
        {
            Location next = new Location(id.x + dir.x, id.z + dir.z);
            //Debug.Log("width = " + width);
            //Debug.Log("height = " + height);
            //Debug.Log("next = " + next.vector3());
            //Debug.Log("(0 <= id.x) = " + (0 <= next.x));
            //Debug.Log("(id.x < width) = " + (next.x < width));
            //Debug.Log("(0 <= id.z) = " + (0 <= next.z));
            ////Debug.Log("(y) = " + (y));
            //Debug.Log("(id.z) = " + (next.z));
            //Debug.Log("(id.x) = " + (next.x));
            //Debug.Log("(id.z < height) = " + (next.z < height));
            //Debug.Log("InBounds(next) = " + InBounds(next));
            if (InBounds(next))
            {
                //Debug.Log("Passable(next) = " + Passable(next));
                if (Passable(next))
                {
                    if (IsDIRAllowed(next, id))
                    {
                        //Debug.Log("accepted " + (next.vector3() * -4));
                        yield return next;
                    }
                }
            }
            //Debug.Log("kicked " + (next.vector3() * -4));
        }
    }
}

public class PriorityQueue<T>
{
    // From Red Blob: I'm using an unsorted array for this example, but ideally this
    // would be a binary heap. Find a binary heap class:
    // * https://bitbucket.org/BlueRaja/high-speed-priority-queue-for-c/wiki/Home
    // * http://visualstudiomagazine.com/articles/2012/11/01/priority-queues-with-c.aspx
    // * http://xfleury.github.io/graphsearch.html
    // * http://stackoverflow.com/questions/102398/priority-queue-in-net

    private List<KeyValuePair<T, float>> elements = new List<KeyValuePair<T, float>>();

    public int Count
    {
        get { return elements.Count; }
    }

    public void Enqueue(T item, float priority)
    {
        elements.Add(new KeyValuePair<T, float>(item, priority));
    }

    // Returns the Location that has the lowest priority
    public T Dequeue()
    {
        int bestIndex = 0;

        for (int i = 0; i < elements.Count; i++)
        {
            if (elements[i].Value < elements[bestIndex].Value)
            {
                bestIndex = i;
            }
        }

        T bestItem = elements[bestIndex].Key;
        elements.RemoveAt(bestIndex);
        return bestItem;
    }
}

// Now that all of our classes are in place, we get get
// down to the business of actually finding a path.
public class AStarSearch
{
    public static SquareGrid InitialiseSquareGrid(float xSize, float zSize, int mapSizeX, int mapSizeZ, out int xOverall, out int zOverall)
    {
        xOverall = (int)xSize * mapSizeX / 4;
        zOverall = (int)zSize * mapSizeZ / 4;
        //Debug.Log("xOverall = " + xOverall);
        //Debug.Log("zOverall = " + zOverall);
        SquareGrid squareGrid = new SquareGrid(0, 0, xOverall, zOverall)
        {
            tiles = new Cell[xOverall, zOverall]
        };
        for (int i = 0; i < xOverall; i++)
        {
            for (int j = 0; j < zOverall; j++)
            {
                squareGrid.tiles[i, j] = new Cell
                {
                    tile = TileType.Floor,
                    corridorIdx = -1,
                    corridorYRot = -1,
                    childEulerZ = -1,
                    childEulerX = -1
                };
                //if (isDevMode)
                //{
                //    Instantiate(testGridPlane, new Vector3(i * -4, 0, j * -4), Quaternion.identity, testGridPlaneHolder);
                //}
            }
        }
        return squareGrid;
    }

    // Someone suggested making this a 2d field.
    // That will be worth looking at if you run into performance issues.
    public Dictionary<Location, Location> cameFrom = new Dictionary<Location, Location>();
    public Dictionary<Location, float> costSoFar = new Dictionary<Location, float>();

    private Location start;
    private Location goal;


    SquareGrid graph;
    Transform testGridPlaneHolder;
    int zOverall;
    float aStarVisualisationTime;
    bool isDevMode;

    static public float Heuristic(Location a, Location b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.z - b.z);
    }

    // Conduct the A* search
    public AStarSearch(SquareGrid graph, Location start, Location goal, Transform testGridPlaneHolder, int zOverall, float aStarVisualisationTime, bool isDevMode)
    {
        // start is current sprite Location
        this.start = start;
        // goal is sprite destination eg tile user clicked on
        this.goal = goal;
        //Debug.Log("start.vector3() = " + start.vector3());
        //Debug.Log("goal.vector3() = " + goal.vector3());
        this.graph = graph;
        this.testGridPlaneHolder = testGridPlaneHolder;
        this.zOverall = zOverall;
        this.aStarVisualisationTime = aStarVisualisationTime;
        this.isDevMode = isDevMode;
    }

    public IEnumerator ShowAStar()
    {
        // add the cross product of the start to goal and tile to goal vectors
        // Vector3 startToGoalV = Vector3.Cross(start.vector3,goal.vector3);
        // Location startToGoal = new Location(startToGoalV);
        // Vector3 neighborToGoalV = Vector3.Cross(neighbor.vector3,goal.vector3);

        // frontier is a List of key-value pairs:
        // Location, (float) priority
        var frontier = new PriorityQueue<Location>();
        // Add the starting location to the frontier with a priority of 0
        frontier.Enqueue(start, 0f);

        cameFrom.Add(start, start); // is set to start, None in example
        costSoFar.Add(start, 0f);
        //Debug.Log("-1");

        while (frontier.Count > 0f)
        {
            // Get the Location from the frontier that has the lowest
            // priority, then remove that Location from the frontier
            //Debug.Log("0 => " + frontier.Count);
            Location current = frontier.Dequeue();

            // If we're at the goal Location, stop looking.
            if (current.Equals(goal)) break;
            //Debug.Log("0 => " + frontier.Count);

            // Neighbors will return a List of valid tile Locations
            // that are next to, diagonal to, above or below current
            IEnumerable<Location> a = graph.Neighbors(current);
            //Debug.Log()
            foreach (var neighbor in a)
            {
                //Debug.Log("1");
                // If neighbor is diagonal to current, graph.Cost(current,neighbor)
                // will return Sqrt(2). Otherwise it will return only the cost of
                // the neighbor, which depends on its type, as set in the TileType enum.
                // So if this is a normal floor tile (1) and it's neighbor is an
                // adjacent (not diagonal) floor tile (1), newCost will be 2,
                // or if the neighbor is diagonal, 1+Sqrt(2). And that will be the
                // value assigned to costSoFar[neighbor] below.
                float newCost = costSoFar[current] + graph.Cost(current, neighbor);

                // If there's no cost assigned to the neighbor yet, or if the new
                // cost is lower than the assigned one, add newCost for this neighbor
                if (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor])
                {
                    //Debug.Log("2");
                    int idx = neighbor.x * zOverall + neighbor.z;
                    //Debug.Log(idx + " = idx");
                    if (testGridPlaneHolder != null)
                    {
                        testGridPlaneHolder.GetChild(idx).GetComponent<Renderer>().material.color = Color.gray;
                    }
                    //Debug.Log(neighbor.vector3() * -4);

                    // If we're replacing the previous cost, remove it
                    if (costSoFar.ContainsKey(neighbor))
                    {
                        //Debug.Log("replacing lower cost");
                        costSoFar.Remove(neighbor);
                        cameFrom.Remove(neighbor);
                    }

                    costSoFar.Add(neighbor, newCost);
                    cameFrom.Add(neighbor, current);
                    float priority = newCost + Heuristic(neighbor, goal);
                    frontier.Enqueue(neighbor, priority);
                }
                if (isDevMode && aStarVisualisationTime != 0)
                {
                    yield return new WaitForSeconds(aStarVisualisationTime);
                }
            }
        }
        Data.instance.isDoneConnectTwoRooms = true;
        yield return null;
    }

    // Return a List of Locations representing the found path
    public List<Location> FindPath()
    {

        List<Location> path = new List<Location>();
        Location current = goal;
        // path.Add(current);

        while (!current.Equals(start))
        {
            if (!cameFrom.ContainsKey(current))
            {
                MonoBehaviour.print("cameFrom does not contain current.");
                return new List<Location>();
            }
            path.Add(current);
            current = cameFrom[current];
        }
        // path.Add(start);
        path.Reverse();
        return path;
    }
}