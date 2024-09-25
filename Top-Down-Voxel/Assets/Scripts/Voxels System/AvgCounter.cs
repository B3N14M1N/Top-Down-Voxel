using System.Collections.Generic;

public static class AvgCounter
{
    private class Counter
    {
        public float Time;
        public int Count;

        public Counter() 
        { 
            Time = 0;
            Count = 0;
        }

        public Counter(float time, int count)
        {
            Time = time;
            Count = count;
        }
    }

    private static Dictionary<string, Counter> kvp = new Dictionary<string, Counter>();

    public static void AddTimer(string name)
    {
        kvp.TryAdd(name, new Counter());
    }

    public static void UpdateTimer(string name, float time)
    {
        if (kvp.TryGetValue(name, out Counter counter))
        {
            counter.Time += time;
            counter.Count++;
        }
        else
        {
            kvp.TryAdd(name, new Counter());
        }
    }

    public static float GetTimer(string name)
    {
        if (kvp.TryGetValue(name, out Counter counter))
        {
            return counter.Time / counter.Count;
        }
        return 0;
    }

    /// <summary>
    /// Removes the timer if it exists
    /// </summary>
    /// <param name="name"></param>
    public static void RemoveTimer(string name)
    {
        kvp.Remove(name);
    }

}
