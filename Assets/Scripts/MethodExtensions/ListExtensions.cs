using System.Collections.Generic;

public static class ListExtensions
{
    private static System.Random rng = new System.Random();

    // 전체 리스트 섞기
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }

    // 지정된 위치 기준으로 나눠서 각각 섞기
    public static void ShuffleBySplit<T>(this IList<T> list, int splitIndex)
    {
        int count = list.Count;
        splitIndex = System.Math.Clamp(splitIndex, 0, count); // 제한

        // 앞쪽 섞기
        for (int i = 0; i < splitIndex - 1; i++)
        {
            int j = rng.Next(i, splitIndex);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        // 뒷쪽 섞기
        for (int i = splitIndex; i < count - 1; i++)
        {
            int j = rng.Next(i, count);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}