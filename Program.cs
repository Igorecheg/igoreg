using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        // Шаг 1: Создать массивы матриц a и b
        Matrix[] a = CreateMatrices(50, 500, 100);
        Matrix[] b = CreateMatrices(50, 100, 500);

        // Шаг 2: Умножение матриц и сохранение результатов
        string resultDirectory = "Results";
        if (Directory.Exists(resultDirectory))
        {
            Directory.Delete(resultDirectory, true);
        }
        Directory.CreateDirectory(resultDirectory);

        Task[] tasks = new Task[4];
        tasks[0] = Task.Run(() => MultiplyAndSaveMatrices(a, b, resultDirectory, "ab_multiplication.tsv"));
        tasks[1] = Task.Run(() => MultiplyAndSaveMatrices(b, a, resultDirectory, "ba_multiplication.tsv"));
        tasks[2] = Task.Run(() => ScalarMultiplyAndSaveMatrices(a, b, resultDirectory, "ab_scalar.tsv"));
        tasks[3] = Task.Run(() => ScalarMultiplyAndSaveMatrices(b, a, resultDirectory, "ba_scalar.tsv"));

        await Task.WhenAll(tasks);
        Console.WriteLine("Matrix multiplications and scalar multiplications saved.");

        // Шаг 3: Сохранение матриц в разных форматах
        string binaryDirectory = "BinaryFormat";
        string textDirectory = "TextFormat";
        string jsonDirectory = "JsonFormat";

        Directory.CreateDirectory(binaryDirectory);
        Directory.CreateDirectory(textDirectory);
        Directory.CreateDirectory(jsonDirectory);

        Task[] saveTasks = new Task[6];
        saveTasks[0] = Task.Run(() => SaveMatricesAsync(a, textDirectory, "a_text_", ".csv", MatrixIO.WriteTextAsync));
        saveTasks[1] = Task.Run(() => SaveMatricesAsync(b, textDirectory, "b_text_", ".csv", MatrixIO.WriteTextAsync));
        saveTasks[2] = Task.Run(() => SaveMatricesAsync(a, jsonDirectory, "a_json_", ".json", MatrixIO.WriteJsonAsync));
        saveTasks[3] = Task.Run(() => SaveMatricesAsync(b, jsonDirectory, "b_json_", ".json", MatrixIO.WriteJsonAsync));
        saveTasks[4] = Task.Run(() => SaveMatrices(a, binaryDirectory, "a_binary_", ".bin", MatrixIO.WriteBinary));
        saveTasks[5] = Task.Run(() => SaveMatrices(b, binaryDirectory, "b_binary_", ".bin", MatrixIO.WriteBinary));

        await Task.WhenAll(saveTasks);
        Console.WriteLine("Matrices saved in different formats.");

        // Шаг 4: Чтение и сравнение матриц
        Task<Matrix[]> textReadTask = ReadMatricesAsync(textDirectory, "a_text_", ".csv", MatrixIO.ReadTextAsync);
        Task<Matrix[]> jsonReadTask = ReadMatricesAsync(jsonDirectory, "a_json_", ".json", MatrixIO.ReadJsonAsync);

        Matrix[] textReadMatrices = await Task.WhenAny(textReadTask, jsonReadTask).Result;
        string readType = textReadTask.IsCompleted ? "text" : "json";
        Console.WriteLine($"Matrices read from {readType} format.");

        Task<bool> compareTask = Task.Run(() => CompareMatrices(a, textReadMatrices));
        bool areEqual = await compareTask;
        Console.WriteLine($"Matrices comparison result: {areEqual}");

        // Синхронное чтение бинарных файлов и сравнение
        Matrix[] binaryReadMatrices = ReadMatrices(binaryDirectory, "a_binary_", ".bin", MatrixIO.ReadBinary);
        bool binaryCompareResult = CompareMatrices(a, binaryReadMatrices);
        Console.WriteLine($"Binary matrices comparison result: {binaryCompareResult}");
    }

    static Matrix[] CreateMatrices(int count, int rows, int columns)
    {
        Random rand = new Random();
        Matrix[] matrices = new Matrix[count];

        for (int i = 0; i < count; i++)
        {
            double[,] values = new double[rows, columns];
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    values[r, c] = rand.NextDouble() * 20 - 10;
                }
            }
            matrices[i] = new Matrix(values);
        }

        return matrices;
    }

    static void MultiplyAndSaveMatrices(Matrix[] first, Matrix[] second, string directory, string fileName)
    {
        using (StreamWriter writer = new StreamWriter(Path.Combine(directory, fileName)))
        {
            for (int i = 0; i < first.Length; i++)
            {
                Matrix result = MatrixOperations.Multiply(first[i], second[i]);
                writer.WriteLine(result.ToTsv());
            }
        }
    }

    static void ScalarMultiplyAndSaveMatrices(Matrix[] first, Matrix[] second, string directory, string fileName)
    {
        using (StreamWriter writer = new StreamWriter(Path.Combine(directory, fileName)))
        {
            for (int i = 0; i < first.Length; i++)
            {
                Matrix result = MatrixOperations.ScalarMultiply(first[i], second[i].matrix[0, 0]);
                writer.WriteLine(result.ToTsv());
            }
        }
    }

    static async Task SaveMatricesAsync(Matrix[] matrices, string directory, string prefix, string extension, Func<Matrix, Stream, Task> writeMethod)
    {
        for (int i = 0; i < matrices.Length; i++)
        {
            string fileName = $"{prefix}{i}{extension}";
            using (FileStream stream = new FileStream(Path.Combine(directory, fileName), FileMode.Create))
            {
                await writeMethod(matrices[i], stream);
            }
        }
    }

    static void SaveMatrices(Matrix[] matrices, string directory, string prefix, string extension, Action<Matrix, Stream> writeMethod)
    {
        for (int i = 0; i < matrices.Length; i++)
        {
            string fileName = $"{prefix}{i}{extension}";
            using (FileStream stream = new FileStream(Path.Combine(directory, fileName), FileMode.Create))
            {
                writeMethod(matrices[i], stream);
            }
        }
    }

    static async Task<Matrix[]> ReadMatricesAsync(string directory, string prefix, string extension, Func<Stream, Task<Matrix>> readMethod)
    {
        Matrix[] matrices = new Matrix[50];
        for (int i = 0; i < matrices.Length; i++)
        {
            string fileName = $"{prefix}{i}{extension}";
            using (FileStream stream = new FileStream(Path.Combine(directory, fileName), FileMode.Open))
            {
                matrices[i] = await readMethod(stream);
            }
        }
        return matrices;
    }

    static Matrix[] ReadMatrices(string directory, string prefix, string extension, Func<Stream, Matrix> readMethod)
    {
        Matrix[] matrices = new Matrix[50];
        for (int i = 0; i < matrices.Length; i++)
        {
            string fileName = $"{prefix}{i}{extension}";
            using (FileStream stream = new FileStream(Path.Combine(directory, fileName), FileMode.Open))
            {
                matrices[i] = readMethod(stream);
            }
        }
        return matrices;
    }

    static bool CompareMatrices(Matrix[] first, Matrix[] second)
    {
        if (first.Length != second.Length)
            throw new ArgumentException("Arrays must have the same length.");

        for (int i = 0; i < first.Length; i++)
        {
            if (!first[i].Equals(second[i]))
            {
                return false;
            }
        }
        return true;
    }
}
