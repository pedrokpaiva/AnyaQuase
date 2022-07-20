using Anya_2d;
using System;
using UnityEngine;

public class Anya
{
    private static int RES = 10000;

    private static AnyaSearch anya = null;
    private static GridGraph storedGraph = null;

    private Path<Node> pathStartNode = null;
    protected GridGraph graph;

    protected int[] parent;
    protected int sizeX;
    protected int sizeXplusOne;
    protected int sizeY;

    protected int sx;
    protected int sy;
    protected int ex;
    protected int ey;

    private int ticketNumber = -1;

    private bool recordingMode;
    private bool usingStaticMemory = false;

    private static void Initialise(GridGraph graph)
    {
        if (graph == storedGraph)
        {
            return;
        }

        try
        {
            GridGraph grid = graph;
            anya = new AnyaSearch(new AnyaExpansionPolicy(grid));
            storedGraph = graph;

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message); ;
        }

        anya.isRecording = false;
    }

    public Anya(GridGraph graph, int sizeX, int sizeY,
            int sx, int sy, int ex, int ey)
    {
        this.graph = graph;
        this.sizeX = sizeX;
        sizeXplusOne = sizeX + 1;
        this.sizeY = sizeY;
        this.sx = sx;
        this.sy = sy;
        this.ex = ex;
        this.ey = ey;
        Initialise(graph);
        anya.isRecording = false;

        Node start = new Node(null, new Interval(0, 0, 0), 0, 0);
        Node target = new Node(null, new Interval(0, 0, 0), 0, 0);
        anya.mb_start_ = start;
        anya.mb_target_ = target;

        start.root.Set(sx, sy);
        start.interval.Init(sx, sx, sy);
        target.root.Set(ex, ey);
        target.interval.Init(ex, ex, ey);
    }

    /**
     * Call this to compute the path.
     */
    public void ComputePath()
    {
        pathStartNode = anya.Search(anya.mb_start_, anya.mb_target_);
        //pathLength = anya.mb_cost_;
    }

    /**
     * @return retrieve the path computed by the algorithm
     */
    public int[][] GetPath()
    {
        int length = 0;
        Path<Node> current = pathStartNode;
        while (current != null)
        {
            current = current.GetNext();
            ++length;
        }
        int[][] path = new int[length][];

        current = pathStartNode;
        int i = 0;
        while (current != null)
        {
            Vector2 p = current.GetVertex().root;
            path[i] = new int[] { (int)p.x, (int)p.y };
            current = current.GetNext();
            ++i;
        }

        return path;
    }

    /**
     * @return directly get path length without computing path.
     * Has to run fast, unlike getPath.
     */
    public float GetPathLength()
    {
        return (float)anya.mb_cost_;
    }
    /*
        @Override
        public void startRecording()
        {
            super.startRecording();
            anya.isRecording = true;
        }

        @Override
        public void stopRecording()
        {
            super.stopRecording();
            anya.isRecording = false;
        }


        private final void snapshotInsert(Node Node)
        {
            AnyaInterval in = Node.interval;

            Integer[] line = new Integer[7];
            line[0] = in.getRow();
            line[1] = (int)(in.getLeft() * RES);
            line[2] = RES;
            line[3] = (int)(in.getRight() * RES);
            line[4] = RES;
            line[5] = (int)Node.root.getX();
            line[6] = (int)Node.root.getY();
            currSnapshot.add(SnapshotItem.generate(line));

            maybeSaveSearchSnapshot();
        }

        private final void snapshotExpand(Node Node)
        {
            AnyaInterval in = Node.interval;

            Integer[] line = new Integer[5];
            line[0] = in.getRow();
            line[1] = (int)(in.getLeft() * RES);
            line[2] = RES;
            line[3] = (int)(in.getRight() * RES);
            line[4] = RES;
            currSnapshot.add(SnapshotItem.generate(line));

            maybeSaveSearchSnapshot();
        }

        @Override
        protected List<SnapshotItem> computeSearchSnapshot()
        {
            return new ArrayList<>(currSnapshot);
        }

        public static void clearMemory()
        {
            anya = null;
            storedGraph = null;
            System.gc();
        }
    */
}
