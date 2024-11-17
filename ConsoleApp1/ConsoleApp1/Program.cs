using System;
using System.Collections.Generic;
using System.IO;

class HashDictionary
{
    private const int SegmentCount = 10; // Количество сегментов (для хеширования)
    private readonly List<List<KeyValuePair<string, string>>> _segments;

    public HashDictionary()
    {
        // Инициализируем список сегментов, каждый из которых представляет связный список
        _segments = new List<List<KeyValuePair<string, string>>>();
        for (int i = 0; i < SegmentCount; i++)
        {
            _segments.Add(new List<KeyValuePair<string, string>>());
        }
    }

    // Хеш-функция: определяет сегмент для данного ключа
    private int HashFunction(string key)
    {
        return Math.Abs(key.GetHashCode() % SegmentCount);
    }

    // Метод для добавления новой записи в словарь или обновления существующей
    public void Insert(string key, string value)
    {
        int segmentIndex = HashFunction(key); // Определяем сегмент
        var segment = _segments[segmentIndex];

        // Проверяем, существует ли уже запись с данным ключом
        for (int i = 0; i < segment.Count; i++)
        {
            if (segment[i].Key == key)
            {
                // Если ключ найден, обновляем значение
                segment[i] = new KeyValuePair<string, string>(key, value);
                return;
            }
        }

        // Если ключ не найден, добавляем новую запись
        segment.Add(new KeyValuePair<string, string>(key, value));
    }

    // Метод для удаления записи по ключу
    public bool Remove(string key)
    {
        int segmentIndex = HashFunction(key); // Определяем сегмент
        var segment = _segments[segmentIndex];

        // Ищем запись с данным ключом
        for (int i = 0; i < segment.Count; i++)
        {
            if (segment[i].Key == key)
            {
                segment.RemoveAt(i); // Удаляем запись
                return true; // Удаление успешно
            }
        }

        return false; // Ключ не найден
    }

    // Метод для поиска значения по ключу
    public string Search(string key)
    {
        int segmentIndex = HashFunction(key); // Определяем сегмент
        var segment = _segments[segmentIndex];

        // Ищем запись с данным ключом
        foreach (var pair in segment)
        {
            if (pair.Key == key)
            {
                return pair.Value; // Возвращаем значение
            }
        }

        return null; // Ключ не найден
    }

    // Метод для сохранения всех данных словаря в файл
    public void SaveToFile(string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Сохраняем данные каждого сегмента
            for (int i = 0; i < _segments.Count; i++)
            {
                foreach (var pair in _segments[i])
                {
                    writer.WriteLine($"{pair.Key}:{pair.Value}"); // Формат записи "ключ:значение"
                }
            }
        }
    }

    // Метод для загрузки данных словаря из файла
    public void LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Файл не найден!");

        // Очищаем текущие данные словаря
        foreach (var segment in _segments)
        {
            segment.Clear();
        }

        // Загружаем данные из файла
        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split(':');
                if (parts.Length == 2)
                {
                    Insert(parts[0], parts[1]); // Добавляем запись в словарь
                }
            }
        }
    }

    // Метод для вывода всех данных словаря с хешированными сегментами
    public void PrintAll()
    {
        Console.WriteLine("\nВсе данные в словаре:");
        for (int i = 0; i < _segments.Count; i++)
        {
            if (_segments[i].Count == 0)
                continue; // Пропускаем пустые сегменты

            Console.WriteLine($"Сегмент {i}:");
            foreach (var pair in _segments[i])
            {
                Console.WriteLine($"  Ключ: {pair.Key}, Значение: {pair.Value}, Хеш: {HashFunction(pair.Key)}");
            }
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        var dictionary = new HashDictionary(); // Инициализация словаря

        while (true)
        {
            // Отображение меню
            Console.WriteLine("\nВыберите действие:");
            Console.WriteLine("1. Вставить значение");
            Console.WriteLine("2. Удалить значение");
            Console.WriteLine("3. Найти значение");
            Console.WriteLine("4. Сохранить словарь в файл");
            Console.WriteLine("5. Загрузить словарь из файла");
            Console.WriteLine("6. Автоматический ввод данных");
            Console.WriteLine("7. Вывести все данные");
            Console.WriteLine("8. Выход");

            // Чтение выбора пользователя
            Console.Write("Введите номер действия: ");
            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Ошибка: Введите числовое значение.");
                continue;
            }

            switch (choice)
            {
                case 1:
                    // Ввод данных для добавления
                    Console.Write("Введите ключ: ");
                    string key = Console.ReadLine();
                    Console.Write("Введите значение: ");
                    string value = Console.ReadLine();
                    dictionary.Insert(key, value);
                    Console.WriteLine($"Добавлено: {key} -> {value}");
                    break;

                case 2:
                    // Удаление данных
                    Console.Write("Введите ключ для удаления: ");
                    key = Console.ReadLine();
                    if (dictionary.Remove(key))
                        Console.WriteLine($"Ключ '{key}' удален.");
                    else
                        Console.WriteLine($"Ключ '{key}' не найден.");
                    break;

                case 3:
                    // Поиск данных
                    Console.Write("Введите ключ для поиска: ");
                    key = Console.ReadLine();
                    value = dictionary.Search(key);
                    if (value != null)
                        Console.WriteLine($"Найдено: {key} -> {value}");
                    else
                        Console.WriteLine($"Ключ '{key}' не найден.");
                    break;

                case 4:
                    // Сохранение данных в файл
                    Console.Write("Введите путь для сохранения файла: ");
                    string savePath = Console.ReadLine();
                    try
                    {
                        dictionary.SaveToFile(savePath);
                        Console.WriteLine("Словарь успешно сохранен.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при сохранении файла: {ex.Message}");
                    }
                    break;

                case 5:
                    // Загрузка данных из файла
                    Console.Write("Введите путь для загрузки файла: ");
                    string loadPath = Console.ReadLine();
                    try
                    {
                        dictionary.LoadFromFile(loadPath);
                        Console.WriteLine("Словарь успешно загружен.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при загрузке файла: {ex.Message}");
                    }
                    break;

                case 6:
                    // Автоматический ввод данных
                    dictionary.Insert("apple", "яблоко");
                    dictionary.Insert("banana", "банан");
                    dictionary.Insert("car", "машина");
                    Console.WriteLine("Автоматические данные добавлены.");
                    break;

                case 7:
                    // Вывод всех данных
                    dictionary.PrintAll();
                    break;

                case 8:
                    // Выход из программы
                    Console.WriteLine("Выход из программы.");
                    return;

                default:
                    // Обработка некорректного выбора
                    Console.WriteLine("Ошибка: Неверный выбор.");
                    break;
            }
        }
    }
}
