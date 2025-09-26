using System.Collections.Generic;

public static class ListRotator
{
    public static List<T> RotateList<T>(List<T> list, bool clockwise, int positions = 1)
    {
        if (list == null || list.Count == 0 || positions % list.Count == 0) return list;

        int count = list.Count;
        positions = positions % count;
        if (!clockwise) positions = count - positions;

        T[] temp = new T[positions];
        for (int i = 0; i < positions; i++)
        {
            temp[i] = list[i];
        }

        for (int i = 0; i < count - positions; i++)
        {
            list[i] = list[i + positions];
        }

        for (int i = 0; i < positions; i++)
        {
            list[count - positions + i] = temp[i];
        }

        return list;
    }
}
