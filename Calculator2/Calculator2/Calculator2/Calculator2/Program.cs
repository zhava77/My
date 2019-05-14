using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator2
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Введите выражение");
                try
                {
                    PolishNotation handler = new PolishNotation(Console.ReadLine());
                    handler.GeneratePolishString();
                    Console.WriteLine(handler.CalculatePolishString());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
        }
    }
}
