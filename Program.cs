using System;
using Pegasus;

namespace PegasusExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new Parsers.Parser();
            var result = parser.Parse("5,7+8,9*42");
            Console.WriteLine(result);
        }
    }
}
