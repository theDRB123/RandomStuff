public class ConsoleUtils()
{
    public class ConsoleWindowManager()
    {
        public List<ConsoleWindow> Windows = [];
        public void AddWindow(ref ConsoleWindow window)
        {
            Windows.Add(window);
        }
        public void RemoveWindow(ref ConsoleWindow window)
        {
            Windows.Remove(window);
        }
        public void DrawWindows()
        {
            Console.Clear();
            foreach (ConsoleWindow window in Windows)
            {
                window.DrawWindow();
                window.DrawText();
            }
        }
    }
    public class ConsoleWindow(int xpos, int ypos, int width, int height)
    {
        //window will have a border and a text area;
        private readonly int xpos = xpos;
        private readonly int ypos = ypos;
        private readonly int width = width;
        private readonly int height = height;
        private readonly int textSlots = height - 2;
        private List<string> textBuffer = [];
        private int offset = 0;
        public event EventHandler? OnWindowChange;
        public void AddText(string text)
        {
            textBuffer.Add(text);
        }
        public void AddText(List<string> text)
        {
            textBuffer.AddRange(text);
        }
        public void AddParsedText(string text)
        {
            text.Split("||").ToList().ForEach(x => textBuffer.Add(x));
        }
        public void ClearText()
        {
            textBuffer.Clear();
        }
        public void ReplaceBuffer(List<string> buffer)
        {
            textBuffer = buffer;
        }
        public void ScrollBuffer(int direction)
        {
            if (direction == 1)
            {
                if (offset < textBuffer.Count - textSlots)
                {
                    offset++;
                }
            }
            else if (direction == -1)
            {
                if (offset > 0)
                {
                    offset--;
                }
            }
        }
        private int borderSize = 2;
        public int BorderSize
        {
            get => borderSize;
            set
            {
                if (value > 0 && value < 5)
                {
                    borderSize = value;
                }
            }
        }
        private char borderChar = '/';
        public char BorderChar
        {
            get => borderChar;
            set
            {
                if (value != default(char))
                {
                    borderChar = value;
                }
            }
        }
        private string header = "";
        public string Header
        {
            get => header;
            set
            {
                if (value != default(string))
                {
                    header = value;
                }
            }
        }
        public void DrawWindow()
        {
            Console.SetCursorPosition(xpos, ypos);
            int halfwidth = (width + 1) / 2 - (header.Length + 1) / 2;
            string topBorder = new string(borderChar, halfwidth) + header + new string(borderChar, halfwidth);
            Console.WriteLine(topBorder);
            for (int i = 1; i < height - 1; i++)
            {
                Console.SetCursorPosition(xpos, ypos + i);
                Console.WriteLine(new string(borderChar, borderSize) + new string(' ', width - 2 * borderSize) + new string(borderChar, borderSize));
            }
            Console.SetCursorPosition(xpos, ypos + height - 1);
            Console.WriteLine(new string(borderChar, width));
            Console.SetCursorPosition(0, 0);
        }
        public void DrawText()
        {
            for (int i = 0; i < textSlots && i < textBuffer.Count; i++)
            {
                Console.SetCursorPosition(xpos + borderSize, ypos + 1 + i);
                TextUtil.WriteFormatParsed(textBuffer[i + offset]);
                Console.SetCursorPosition(textBuffer[i + offset].Length, ypos + 1 + i);
            }
            // Console.SetCursorPosition(0, ypos + height + 1);
        }
    }



}