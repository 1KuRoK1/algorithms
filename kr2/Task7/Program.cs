using System;
using System.Diagnostics;
using System.Text;

// Дерево Фенвика — своя реализация.
// Принцип: tree[i] хранит сумму отрезка V[i-lowbit(i)+1..i]; обновление
// идёт вверх по i += lowbit(i), префиксная сумма — вниз по i -= lowbit(i).
// Запрос суммы на [L,R] = Prefix(R+1) - Prefix(L).
struct FenwickTree
{
    public long[] tree;
    public int n;

    public void Init(int n)
    {
        this.n = n;
        tree = new long[n + 1];      // 1-индексация
    }

    // прибавить delta к V[idx] (0-индексация)
    public void Update(int idx, long delta)
    {
        int i = idx + 1;
        while (i <= n)
        {
            tree[i] += delta;
            i += i & (-i);           // вверх по дереву
        }
    }

    // сумма V[0..idx] (0-индексация), idx включительно
    public long PrefixSum(int idx)
    {
        long s = 0;
        int i = idx + 1;
        while (i > 0)
        {
            s += tree[i];
            i -= i & (-i);           // вниз по дереву
        }
        return s;
    }

    // сумма V[L..R] (0-индексация, L и R включительно)
    public long RangeSum(int L, int R)
    {
        if (L > R) return 0;
        if (L == 0) return PrefixSum(R);
        return PrefixSum(R) - PrefixSum(L - 1);
    }
}

class Program
{
    static void Main()
    {
        Stopwatch sw = Stopwatch.StartNew();

        string[] first = Console.ReadLine()
                                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        int N = int.Parse(first[0]);
        int M = int.Parse(first[1]);

        long[] v = new long[N];
        string[] vparts = Console.ReadLine()
                                 .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < N; i++) v[i] = long.Parse(vparts[i]);

        long memStart = GC.GetTotalMemory(false);

        // строим дерево Фенвика за O(N): каждый V[i] прибавляем в tree
        FenwickTree ft = default;
        ft.Init(N);
        for (int i = 0; i < N; i++) ft.Update(i, v[i]);

        StringBuilder output = new StringBuilder();

        for (int q = 0; q < M; q++)
        {
            string[] parts = Console.ReadLine()
                                    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int code = int.Parse(parts[0]);

            if (code == 1)
            {
                int L = int.Parse(parts[1]);
                int R = int.Parse(parts[2]);
                output.Append(ft.RangeSum(L, R)).Append('\n');
            }
            else // code == 2
            {
                int idx = int.Parse(parts[1]);
                long newVal = long.Parse(parts[2]);
                long delta = newVal - v[idx];
                v[idx] = newVal;
                ft.Update(idx, delta);
            }
        }

        sw.Stop();
        long memEnd = GC.GetTotalMemory(false);

        Console.Write(output.ToString());

        Console.Error.WriteLine($"TIME_MS={sw.Elapsed.TotalMilliseconds:F2}");
        Console.Error.WriteLine($"MEM_MB={(memEnd - memStart) / 1024.0 / 1024.0:F2}");
    }
}