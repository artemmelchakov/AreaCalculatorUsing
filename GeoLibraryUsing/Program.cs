using System.Reflection;

namespace AreaCalculatorUsing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Данный код показывает вариант работы с библиотекой GeoLibrary при позднем связывании
            // (Вычисление площади фигуры без знания типа фигуры в compile-time).
            //
            // Обратите внимание, что перед запуском данной программы необходимо поместить файлы библиотеки в директорию GeoLibrary
            // внутри директории, где находится исполняемый файл данной программы.
            //
            // Например, если данная прогамма находится в C:/GeoLibraryUsing,
            // то файлы библиотеки GeoLibrary необходимо поместить в C:/GeoLibraryUsing/GeoLibrary

            // Загрузка сборки
            Console.WriteLine("Hello, World! GeoLibrary is loading...");
            var areaCalculatorAssembly = Assembly.LoadFrom("GeoLibrary/AreaCalculators.dll");
            Console.WriteLine("Loading is done!");

            // Получение классов, которые обладают функциональностью по определению площади конкретной фигуры
            var areaCalculatorTypes = areaCalculatorAssembly.ExportedTypes.Where(t => t.BaseType?.Name == "AreaCalculator`1").ToArray();

            // Выбираем, с каким калькулятором площади будем работать
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
                // Получаем тип фигуры
                var figureType = areaCalculatorType.GetConstructors().FirstOrDefault()?.GetParameters().FirstOrDefault()?.ParameterType;
                // Информация о параметрах конструктора 
                var figureParameters = figureType?.GetConstructors().FirstOrDefault()?.GetParameters();                
                // Значение параметров конструктора будут в данном массиве
                var figureParametersValues = new object?[figureParameters?.Length ?? 0];

                // Заполняем значения параметров
                var parameterValue_String = string.Empty;
                for (int i = 0; i < (figureParameters?.Length ?? 0); i++)
                {
                    Console.WriteLine($"Enter {figureParameters[i].Name} ({figureParameters[i].ParameterType}):");
                    parameterValue_String = Console.ReadLine();
                    figureParametersValues[i] = Convert.ChangeType(parameterValue_String, figureParameters[i].ParameterType);
                }
                // Создаем экземпляр фигуры
                var figureInstance = figureType is not null ? Activator.CreateInstance(figureType, figureParametersValues) : null;
                // Создаём экземпляр вычислителя площади
                var areaCalculatorInstance = Activator.CreateInstance(areaCalculatorType, figureInstance);
                // Получаем метод, выполяющий вычисление
                var areaCalculateMethod = areaCalculatorType.GetMethod("Calculate");

                // Вычисляем площадь
                var area = areaCalculateMethod?.Invoke(areaCalculatorInstance, null);

                Console.WriteLine($"Area: {area}");
                Console.ReadKey(true);
            }
        }
    }
}
