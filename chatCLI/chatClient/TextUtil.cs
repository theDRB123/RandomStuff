
// testing
public static class TextUtil
{
    private static readonly Dictionary<int, (ConsoleColor, ConsoleColor)> Theme = new(){
    {0 , (ConsoleColor.Gray , ConsoleColor.Black)},
    {1 , (ConsoleColor.Black , ConsoleColor.DarkBlue)},
    {2 , (ConsoleColor.White , ConsoleColor.Black)},
    {3 , (ConsoleColor.Red , ConsoleColor.Black)},
    {4 , (ConsoleColor.Black , ConsoleColor.DarkRed)},
    {5 , (ConsoleColor.Green , ConsoleColor.Black)},
    {6 , (ConsoleColor.Cyan , ConsoleColor.Black) },
    {7 , (ConsoleColor.Yellow , ConsoleColor.Black)}
    };

    public static void WriteLine(List<(string message, (ConsoleColor foreground, ConsoleColor background))> collection)
    {
        foreach (var item in collection)
        {
            Console.ForegroundColor = item.Item2.foreground;
            Console.BackgroundColor = item.Item2.background;
            Console.WriteLine(item.message);
            Console.ForegroundColor = Theme[0].Item1;
            Console.BackgroundColor = Theme[0].Item2;
        }
    }
    public static void WriteLine(string message, (ConsoleColor foreground, ConsoleColor background) color)
    {
        Console.ForegroundColor = color.foreground;
        Console.BackgroundColor = color.background;
        Console.WriteLine(message);
        Console.ForegroundColor = Theme[0].Item1;
        Console.BackgroundColor = Theme[0].Item2;
    }
    public static void WriteLine(string message, int theme)
    {
        Console.ForegroundColor = Theme[theme].Item1;
        Console.BackgroundColor = Theme[theme].Item2;
        Console.WriteLine(message);
        Console.ForegroundColor = Theme[0].Item1;
        Console.BackgroundColor = Theme[0].Item2;
    }
    public static void Write(string message, int theme)
    {
        Console.ForegroundColor = Theme[theme].Item1;
        Console.BackgroundColor = Theme[theme].Item2;
        Console.Write(message);
        Console.ForegroundColor = Theme[0].Item1;
        Console.BackgroundColor = Theme[0].Item2;
    }
    public static void Write(string message, (ConsoleColor foreground, ConsoleColor background) color)
    {
        Console.ForegroundColor = color.foreground;
        Console.BackgroundColor = color.background;
        Console.Write(message);
        Console.ForegroundColor = Theme[0].Item1;
        Console.BackgroundColor = Theme[0].Item2;
    }
    public static void changeTheme(int theme)
    {
        Console.ForegroundColor = Theme[theme].Item1;
        Console.BackgroundColor = Theme[theme].Item2;
    }
    public static void changeTheme(ConsoleColor forground, ConsoleColor background)
    {
        Console.ForegroundColor = forground;
        Console.BackgroundColor = background;
    }

    public static void ClearCurrentConsoleLine()
    {
        int currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, Console.CursorTop - 1);
        Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
        Console.SetCursorPosition(0, currentLineCursor - 1);
        Console.ForegroundColor = Theme[0].Item1;
        Console.BackgroundColor = Theme[0].Item2;
    }
};
