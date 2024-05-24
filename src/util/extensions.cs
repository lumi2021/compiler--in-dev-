namespace Compiler;

public static class Extensions
{

    public static T Shift<T>(this List<T> list)
    {
        var item = list[0];
        list.RemoveAt(0);
        return item;
    }

    public static string Shift(this List<char> list)
    {
        var item = list[0];
        list.RemoveAt(0);
        return "" + item;
    }

}