using System.Reflection;

namespace AreaCalculatorUsing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Данный код показывает вариант работы с библиотекой AreaCalculator.dll при позднем связывании
            // (Вычисление площади фигуры без знания типа фигуры в compile-time).

            Console.WriteLine("Hello, World! AreaCalculator.dll is loading...");
            var areaCalculatorAssembly = Assembly.LoadFrom("AreaCalculator.dll");
            Console.WriteLine("Loading is done!");

            var iAreaCalculatorType = areaCalculatorAssembly.ExportedTypes.FirstOrDefault(t => t.Name == "IAreaCalculator");
            var areaCalculatorTypes = areaCalculatorAssembly.ExportedTypes.Where(t => t.GetInterfaces().Contains(iAreaCalculatorType)).ToArray();

            Console.WriteLine($"Choose number: ");
            for (int i = 0; i < areaCalculatorTypes.Count(); i++)
            {
                Console.WriteLine($"{i + 1}. {areaCalculatorTypes[i].Name}");
            }
            var answer = Console.ReadLine();
        }
    }
}
