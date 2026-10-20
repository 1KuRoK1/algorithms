using System;
using System.Diagnostics;
using System.IO;

class Program
{
    // Быстрый парсер целых чисел из потока байтов — своя реализация.
    // Принцип: читаем байты, пропускаем нецифры, копим значение, пока цифры;
    // учитываем минус. Возвращает int (влезает по условию).
    static class FastInput
    {
        public static Stream In = Console.OpenStandardInput();
        static byte[] buffer = new byte[1 << 16];
        static int bufLen = 0, bufPos = 0;

        static int ReadByte()
        {
            if (bufPos >= bufLen)
            {
                bufLen = In.Read(buffer, 0, buffer.Length);
                bufPos = 0;
                if (bufLen <= 0) return -1;
            }
            return buffer[bufPos++];
        }

        public static int ReadInt()
        {
            int b = ReadByte();
            while (b != -1 && (b < '0' || b > '9') && b != '-') b = ReadByte();
            if (b == -1) return 0;
            bool neg = (b == '-');
            if (neg) b = ReadByte();
            int x = 0;
            while (b >= '0' && b <= '9')
            {
                x = x * 10 + (b - '0');
                b = ReadByte();
            }
            return neg ? -x : x;
        }
    }

    static void Main()
    {
        Stopwatch swTotal = Stopwatch.StartNew();

        int N = FastInput.ReadInt();

        long memStart = GC.GetTotalMemory(false);
        Stopwatch swAlgo = Stopwatch.StartNew();

        // Алгоритм Кадане — своя реализация.
        // Принцип: идём по массиву, поддерживаем cur (лучшая сумма,
        // заканчивающаяся здесь) и best (глобальный максимум); если cur
        // становится отрицательным — начинаем новый подмассив с нуля.
        long best = long.MinValue;
        long cur = 0;

        for (int i = 0; i < N; i++)
        {
            int x = FastInput.ReadInt();
            cur += x;
            if (cur > best) best = cur;
            if (cur < 0) cur = 0;
        }

        swAlgo.Stop();
        long memEnd = GC.GetTotalMemory(false);
        swTotal.Stop();

        Console.WriteLine(best);

        Console.Error.WriteLine($"TIME_TOTAL_MS={swTotal.Elapsed.TotalMilliseconds:F2}");
        Console.Error.WriteLine($"TIME_ALGO_MS={swAlgo.Elapsed.TotalMilliseconds:F2}");
        Console.Error.WriteLine($"MEM_MB={(memEnd - memStart) / 1024.0 / 1024.0:F2}");
    }
}