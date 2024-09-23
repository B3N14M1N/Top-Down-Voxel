public static class PlayerSettings
{
    public static int RenderDistance = 12;
    public static int LoadDistance = 15;

    // limits how many chunks to generate - it dilutes the workload and uses less memory per frame
    public static int ChunksProcessed = 3;

    public static int ChunksToLoad = 2;
    public static float TimeToLoadNextChunks = 1f/120f;//seconds
}
