using System;

namespace BackgroundRunner.Runner
{
    public class ConsoleOutput : IOutput
    {
        public void Write(string value)
        {
            Console.WriteLine(value);
        }
    }
}