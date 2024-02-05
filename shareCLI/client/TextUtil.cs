
// testing
public class TextUtil
{
    private static Dictionary<int, (ConsoleColor, ConsoleColor)> Theme = new(){
    {0 , (ConsoleColor.Gray , ConsoleColor.Black)},
    {1 , (ConsoleColor.Black , ConsoleColor.DarkBlue)},
    {2 , (ConsoleColor.White , ConsoleColor.Black)},
    {3 , (ConsoleColor.Red , ConsoleColor.Black)},
    {4 , (ConsoleColor.Black , ConsoleColor.DarkRed)},
    {5 , (ConsoleColor.Green , ConsoleColor.Black)},
    {6 , (ConsoleColor.Cyan , ConsoleColor.Black) },
    {7 , (ConsoleColor.Yellow , ConsoleColor.Black)}
    };

    public static void AddTheme(int index, (ConsoleColor, ConsoleColor) color)
    {
        Theme.Add(index, color);
    }

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
    public static void ChangeTheme(int theme)
    {
        Console.ForegroundColor = Theme[theme].Item1;
        Console.BackgroundColor = Theme[theme].Item2;
    }
    public static void ChangeTheme(ConsoleColor forground, ConsoleColor background)
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

    // format :- "[0]"message"[\] 
    public static void WriteFormatParsed(List<string> collection)
    {

    }
    public static void WriteFormatParsed(string message)
    {
        List<(int, string)> collection = Parser(message);
        // Console.WriteLine("The length of collection is :-" + collection.Count);
        foreach (var item in collection)
        {
            Write(item.Item2, item.Item1);
            Console.Write(" ");
        }
        Console.Write("\n");
    }

    private static List<(int, string)> Parser(string message)
    {
        List<(int, string)> result = [];
        int startIndex = 0;
        int currentTheme = 0;
        for (int i = 0; i < message.Length - 2; i++)
        {
            if(IsStartingTag(i)){
                if(i != 0){
                    result.Add((currentTheme, message[startIndex..i]));
                }
                currentTheme = int.Parse(message[i + 1].ToString());
                startIndex = i + 3;
            }
            else if(IsEndingTag(i)){
                result.Add((currentTheme, message[startIndex..i]));
                startIndex = i + 3;
            }
        }

        bool IsStartingTag(int index) => message[index] == '[' && message[index + 2] == ']' && IsValidTheme(index);
        bool IsEndingTag(int index) => message[index] == '[' && message[index + 2] == ']' && message[index + 1] == '/';
        bool IsValidTheme(int index) => message[index + 1] != '/' && message[index + 1] != ' ';
        return result;
    }
};
