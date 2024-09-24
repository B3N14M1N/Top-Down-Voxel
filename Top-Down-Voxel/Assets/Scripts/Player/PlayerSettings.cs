public static class PlayerSettings
{
    public static int RenderDistance = 20;
    public static int LoadDistance = 25;

    // limits how many chunks to generate - it dilutes the workload and uses less memory per frame
    public static int ChunksProcessed = 2;

    public static int ChunksToLoad = 2;
    public static float TimeToLoadNextChunks = 0.016f;//seconds
}
