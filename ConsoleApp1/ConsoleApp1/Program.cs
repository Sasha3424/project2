using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp1
{
    public struct GeneticData
    {
        public string name; //protein name
        public string organism;
        public string formula; //formula
    }

    class Program
    {
        // Статический список для хранения данных о белках
        static List<GeneticData> data = new List<GeneticData>();

        // Статическая переменная для отслеживания количества белков (или других целей)
        static int count = 1;

        // Метод, который принимает имя белка и возвращает его формулу
        static string GetFormula(string proteinName)
        {
            // Проходим по каждому элементу в списке данных
            foreach (GeneticData item in data)
            {
                // Проверяем, совпадает ли имя белка с искомым
                if (item.name.Equals(proteinName))
                    // Если совпадает, возвращаем формулу этого белка
                    return item.formula;
            }

            // Если белок не найден, возвращаем null
            return null;
        }


        // Метод для чтения данных о белках из файла
        static void ReadGeneticData(string filename)
        {
            // Создаем объект StreamReader для чтения файла с указанным именем
            StreamReader reader = new StreamReader(filename);

            // Читаем файл построчно, пока не достигнем конца потока
            while (!reader.EndOfStream)
            {
                // Читаем текущую строку
                string line = reader.ReadLine();

                // Разбиваем строку на фрагменты по табуляции
                string[] fragments = line.Split('\t');

                // Создаем новый объект GeneticData для хранения информации о белке
                GeneticData protein = new GeneticData();

                // Заполняем поля объекта данными из фрагментов
                protein.name = fragments[0];      // Имя белка
                protein.organism = fragments[1];   // Организм, из которого получен белок
                protein.formula = fragments[2];    // Формула белка

                // Добавляем объект в статический список данных
                data.Add(protein);

                // Увеличиваем счетчик на 1
                count++;
            }

            // Закрываем StreamReader после завершения чтения файла
            reader.Close();
        }

        // Метод для обработки команд из файла
        static void ReadHandleCommands(string filename)
        {
            // Создаем объект StreamReader для чтения из указанного файла
            StreamReader reader = new StreamReader(filename);

            // Создаем объект StreamWriter для записи результатов в файл "answer.txt"
            StreamWriter sw = new StreamWriter("answer.txt");

            // Записываем заголовок в файл и выводим его на консоль
            sw.WriteLine("Yanushevskaya Sasha \nGenetic Searching");
            Console.WriteLine("Yanushevskaya Sasha \nGenetic Searching");

            // Записываем разделитель в файл и на консоль 
            Console.WriteLine("================================================");
            sw.WriteLine("================================================");

            int counter = 0; // Счетчик для отслеживания номера команды

            // Читаем файл построчно, пока не достигнем конца потока
            while (!reader.EndOfStream)
            {
                // Считываем текущую строку и увеличиваем счетчик
                string line = reader.ReadLine();
                counter++;

                // Разбиваем строку на команды по табуляции
                string[] command = line.Split('\t');

                // Обработка команды "search"
                if (command[0].Equals("search"))
                {
                    // Выводим команду на консоль и записываем в файл
                    Console.WriteLine($"{counter.ToString("D3")}   {"search"}   {Decoding(command[1])}");
                    sw.WriteLine($"{counter.ToString("D3")}   {"search"}   {Decoding(command[1])}");

                    // Ищем белок по заданному имени
                    int index = Search(command[1]);
                    if (index != -1) // Если белок найден
                    {
                        Console.WriteLine($"{data[index].organism}    {data[index].name}");
                        sw.WriteLine($"{data[index].organism}    {data[index].name}");
                    }
                    else // Если белок не найден
                    {
                        Console.WriteLine("NOT FOUND");
                        sw.WriteLine("NOT FOUND");
                    }

                    // Записываем разделитель
                    Console.WriteLine("================================================");
                    sw.WriteLine("================================================");
                }

                // Обработка команды "diff"
                if (command[0].Equals("diff"))
                {
                    // Выводим команду на консоль и записываем в файл
                    Console.WriteLine($"{counter.ToString("D3")}   {"diff"}   {command[1]} \t {command[2]}");
                    sw.WriteLine($"{counter.ToString("D3")}   {"diff"}   {command[1]} \t {command[2]}");

                    // Сравниваем два белка и получаем количество различий
                    int cou = Diff(command[1], command[2]);
                    if (cou != -1) // Если различия найдены
                    {
                        Console.WriteLine($"{"amino - acids difference:"} \n {cou}");
                        sw.WriteLine($"{"amino - acids difference:"} \n {cou}");
                    }
                    else // Если белки не найдены
                    {
                        Console.WriteLine("NOT FOUND");
                        sw.WriteLine("NOT FOUND");
                    }

                    // Записываем разделитель
                    sw.WriteLine("================================================");
                    Console.WriteLine("================================================");
                }

                // Обработка команды "mode"
                if (command[0].Equals("mode"))
                {
                    // Выводим команду на консоль и записываем в файл
                    Console.WriteLine($"{counter.ToString("D3")}   {"mode"}   {Decoding(command[1])}");
                    sw.WriteLine($"{counter.ToString("D3")}   {"mode"}   {Decoding(command[1])}");

                    // Вызываем метод Mode для обработки соответствующей команды
                    Mode(command[1], sw);
                }
            }

            // Закрываем StreamReader и StreamWriter после завершения работы
            reader.Close();
            sw.Close();
        }

        // Метод для проверки валидности формулы
        static bool IsValid(string formula)
        {
            // Список допустимых символов (аминокислот)
            List<char> letters = new List<char>() { 'A', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'Y' };

            // Перебираем каждый символ в формуле
            foreach (char ch in formula)
            {
                // Если символ не находится в списке допустимых, возвращаем false
                if (!letters.Contains(ch)) return false;
            }

            // Если все символы валидны, возвращаем true
            return true;
        }

        // Метод для кодирования формулы
        static string Encoding(string formula)
        {
            string encoded = String.Empty; // Инициализируем строку для закодированной формулы

            // Перебираем каждый символ в формуле
            for (int i = 0; i < formula.Length; i++)
            {
                char ch = formula[i]; // Текущий символ
                int count = 1; // Счетчик для подсчета повторений текущего символа

                // Подсчитываем количество повторений текущего символа
                while (i < formula.Length - 1 && formula[i + 1] == ch)
                {
                    count++; // Увеличиваем счетчик
                    i++; // Переходим к следующему символу
                }

                // Кодируем символ в зависимости от его количества
                if (count > 2)
                    encoded = encoded + count + ch; // Если больше 2, добавляем количество и символ
                if (count == 1)
                    encoded = encoded + ch; // Если 1, добавляем только символ
                if (count == 2)
                    encoded = encoded + ch + ch; // Если 2, добавляем символ дважды
            }

            return encoded; // Возвращаем закодированную строку
        }

        // Метод для декодирования закодированной формулы
        static string Decoding(string formula)
        {
            string decoded = String.Empty; // Инициализируем строку для декодированной формулы

            // Перебираем каждый символ в закодированной формуле
            for (int i = 0; i < formula.Length; i++)
            {
                // Проверяем, является ли текущий символ цифрой
                if (char.IsDigit(formula[i]))
                {
                    // Следующий символ - это аминокислота
                    char letter = formula[i + 1];

                    // Преобразуем цифру в целое число
                    int conversion = formula[i] - '0';

                    // Добавляем аминокислоту в декодированную строку (количество - 1, так как одна уже добавлена)
                    for (int j = 0; j < conversion - 1; j++)
                        decoded = decoded + letter;
                }
                else
                {
                    // Если символ не цифра, просто добавляем его в декодированную строку
                    decoded = decoded + formula[i];
                }
            }

            return decoded; // Возвращаем декодированную строку
        }

        // Метод для поиска аминокислоты в данных
        static int Search(string amino_acid)
        {
            // Декодируем переданную аминокислоту
            string decoded = Decoding(amino_acid);

            // Перебираем все элементы в данных
            for (int i = 0; i < data.Count; i++)
            {
                // Если декодированная формула содержится в элементе данных, возвращаем индекс
                if (data[i].formula.Contains(decoded))
                    return i;
            }

            return -1; // Если не найдено, возвращаем -1
        }

        // Метод для вычисления различий между двумя белками
        static int Diff(string protein1, string protein2)
        {
            // Декодируем формулы двух белков
            string decoded1 = Decoding(GetFormula(protein1));
            string decoded2 = Decoding(GetFormula(protein2));

            int counter = 0; // Счетчик различий

            // Проверяем на случай, если одна из декодированных строк равна null
            if (decoded1 is null || decoded2 is null)
            {
                return -1; // Возвращаем -1 в случае ошибки
            }

            // Перебираем символы в обеих декодированных строках
            for (int i = 0; i < decoded1.Length || i < decoded2.Length; i++)
            {
                try
                {
                    // Если символы не совпадают, увеличиваем счетчик различий
                    if (decoded1[i] != decoded2[i])
                        counter++;
                }
                catch (IndexOutOfRangeException)
                {
                    // Если вышли за пределы одной из строк, увеличиваем счетчик различий
                    counter++;
                }
            }

            return counter; // Возвращаем количество различий
        }

        // Метод для определения наиболее часто встречающейся аминокислоты в белке
        static void Mode(string protein, StreamWriter sw)
        {
            // Декодируем формулу белка
            string decoded = Decoding(GetFormula(protein));

            // Массив с символами аминокислот
            char[] arr = new char[20] { 'A', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'Y' };

            char res = '0'; // Переменная для хранения наиболее частой аминокислоты
            int c = 0; // Счетчик для хранения максимального количества повторений

            // Перебираем все аминокислоты из массива
            for (int i = 0; i < arr.Length; i++)
            {
                int c1 = 0; // Счетчик для текущей аминокислоты

                // Перебираем декодированную строку
                for (int j = 0; j < decoded.Length; j++)
                {
                    // Если текущая аминокислота совпадает с символом в декодированной строке, увеличиваем счетчик
                    if (arr[i] == decoded[j]) c1++;
                }

                // Если текущий счетчик больше максимального, обновляем результаты
                if (c < c1)
                {
                    res = arr[i]; // Сохраняем символ аминокислоты
                    c = c1; // Обновляем максимальное количество повторений
                }
            }

            // Если была найдена аминокислота, выводим и записываем результат
            if (res != '0')
            {
                Console.WriteLine($"{"amino - acids occurs:"} \n {res} \t {c}");
                sw.WriteLine($"{"amino - acids occurs:"} \n {res} \t {c}");
            }
            else // Если аминокислоты не найдены, выводим сообщение
            {
                Console.WriteLine("NOT FOUND");
                sw.WriteLine("NOT FOUND");
            }

            // Разделительная линия для удобства чтения
            Console.WriteLine("================================================");
            sw.WriteLine("================================================");
        }

        // Основной метод программы
        static void Main(string[] args)
        {
            // Чтение генетических данных из файла
            ReadGeneticData("sequences.0.txt");

            // Обработка команд из файла
            ReadHandleCommands("commands.0.txt");
        }
    }
}