using System.Reflection;

namespace AreaCalculatorUsing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Данный код показывает вариант работы с библиотекой AreaCalculator.dll при позднем связывании
            // (Вычисление площади фигуры без знания типа фигуры в compile-time).
            //
            // Обратите внимание, что перед запуском данной программы необходимо поместить файл библиотеки AreaCalculator.dll в ту же директорию, где
            // находится исполняемый файл данной программы.

            // Загрузка сборки
            Console.WriteLine("Hello, World! AreaCalculator.dll is loading...");
            var areaCalculatorAssembly = Assembly.LoadFrom("AreaCalculator.dll");
            Console.WriteLine("Loading is done!");

            // Получение интерфейса IAreaCalculator
            var iAreaCalculatorType = areaCalculatorAssembly.ExportedTypes.FirstOrDefault(t => t.Name == "IAreaCalculator");
            // Получение классов, реализующих интерфейс IAreaCalculator - именно они обладают функциональностью по определению площади конкретной фигуры
            var areaCalculatorTypes = areaCalculatorAssembly.ExportedTypes.Where(t => t.GetInterfaces().Contains(iAreaCalculatorType)).ToArray();

            // Выбираем, с каким калькулятором площади (с какой фигурой) будем работать
            Console.WriteLine($"Choose number: ");
            for (int i = 0; i < areaCalculatorTypes.Count(); i++)
            {
                Console.WriteLine($"{i + 1}. {areaCalculatorTypes[i].Name}");
            }
            var choosenAreaCalculatorTypeIndex_String = Console.ReadLine();

            if (int.TryParse(choosenAreaCalculatorTypeIndex_String, out int choosenAreaCalculatorTypeIndex) 
                && areaCalculatorTypes[choosenAreaCalculatorTypeIndex - 1] is not null)
            {
                var areaCalculatorType = areaCalculatorTypes[choosenAreaCalculatorTypeIndex - 1];
                // Получаем параметры конструктора, которые необходимо заполнить для того, чтобы выполнить вычисление.
                var constructorParameters = areaCalculatorType.GetConstructors().FirstOrDefault()?.GetParameters();
                if (constructorParameters is not null)
                {
                    var parametersValues = new object?[constructorParameters.Length];
                    var parameterValue_String = string.Empty;
                    for (int i = 0; i < constructorParameters.Length; i++)
                    {
                        Console.WriteLine($"Enter {constructorParameters[i].Name} ({constructorParameters[i].ParameterType}):");
                        parameterValue_String = Console.ReadLine();
                        parametersValues[i] = Convert.ChangeType(parameterValue_String, constructorParameters[i].ParameterType);
                    }
                    // Создаём экземпляр вычислителя площади. В конструктор передадутся заполненные параметры
                    var areaCalculatorInstance = Activator.CreateInstance(areaCalculatorType, parametersValues);
                    var areaCalculateMethod = areaCalculatorType.GetMethod("Calculate");
                    
                    // Вычисляем площадь
                    var area = areaCalculateMethod?.Invoke(areaCalculatorInstance, null);
                    
                    Console.WriteLine($"Area: {area}");
                    Console.ReadKey(true);
                }                    
            }
        }
    }
}
