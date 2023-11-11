using System;

class Sample
{
    public static void Main()
    {
    
        Console.SetWindowSize(50, 20);
        Console.SetBufferSize(50, 20);

        string o = "O";
        for(int i = 0; i < 50*20; i++)
        {
            Console.Write(o);
        }
        // Console.WriteLine("HELLLLLLO00000000000000000000000000000000000000000000000000000000000000000000000000000000000");
        Console.ReadKey();
    }
}
