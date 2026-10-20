using System;
using System.Diagnostics;

class Program
{
    // Свой BFS по состояниям (x, y) — z = N - x - y — своя реализация.
    // Принцип: очередь на массиве; из каждого состояния 6 переливаний
    // (i→j для i≠j), каждое даёт newX, newY; если visited[newId] == -1 —
    // обновляем dist и кладём в очередь.
    static void Main()
    {
        Stopwatch swTotal = Stopwatch.StartNew();

        string[] parts = Console.ReadLine()
                                .Split(new char[] { ' ', '\t' },
                                       StringSplitOptions.RemoveEmptyEntries);
        int N = int.Parse(parts[0]);
        int M = int.Parse(parts[1]);
        int K = int.Parse(parts[2]);
        int L = int.Parse(parts[3]);

        long memStart = GC.GetTotalMemory(false);
        Stopwatch swAlgo = Stopwatch.StartNew();

        // проверка невозможности: L не может быть больше max ёмкости
        if (L > N && L > M && L > K)
        {
            swAlgo.Stop();
            long memEnd0 = GC.GetTotalMemory(false);
            swTotal.Stop();
            Console.WriteLine("OOPS");
            Console.Error.WriteLine($"TIME_TOTAL_MS={swTotal.Elapsed.TotalMilliseconds:F2}");
            Console.Error.WriteLine($"TIME_ALGO_MS={swAlgo.Elapsed.TotalMilliseconds:F2}");
            Console.Error.WriteLine($"MEM_MB={(memEnd0 - memStart) / 1024.0 / 1024.0:F2}");
            return;
        }

        // состояние (x, y), z = N - x - y
        int cols = M + 1;
        int totalStates = (N + 1) * (M + 1);
        int[] dist = new int[totalStates];
        for (int i = 0; i < totalStates; i++) dist[i] = -1;

        // своя очередь на массиве
        int[] queue = new int[totalStates];
        int head = 0, tail = 0;

        int startId = N * cols + 0;   // x = N, y = 0, z = 0
        dist[startId] = 0;
        queue[tail++] = startId;

        int[] caps = { N, M, K };
        int answer = -1;

        while (head < tail)
        {
            int id = queue[head++];
            int x = id / cols;
            int y = id % cols;
            int z = N - x - y;
            int d = dist[id];

            if (x == L || y == L || z == L) { answer = d; break; }

            int[] v = { x, y, z };

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i == j) continue;
                    int pour = Math.Min(v[i], caps[j] - v[j]);
                    if (pour <= 0) continue;
                    int[] nv = { v[0], v[1], v[2] };
                    nv[i] -= pour;
                    nv[j] += pour;

                    int nx = nv[0], ny = nv[1];
                    int nid = nx * cols + ny;
                    if (dist[nid] == -1)
                    {
                        dist[nid] = d + 1;
                        queue[tail++] = nid;
                    }
                }
            }
        }

        swAlgo.Stop();
        long memEnd = GC.GetTotalMemory(false);
        swTotal.Stop();

        if (answer == -1) Console.WriteLine("OOPS");
        else              Console.WriteLine(answer);

        Console.Error.WriteLine($"TIME_TOTAL_MS={swTotal.Elapsed.TotalMilliseconds:F2}");
        Console.Error.WriteLine($"TIME_ALGO_MS={swAlgo.Elapsed.TotalMilliseconds:F2}");
        Console.Error.WriteLine($"MEM_MB={(memEnd - memStart) / 1024.0 / 1024.0:F2}");
    }
}