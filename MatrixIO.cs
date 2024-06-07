using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

public static class MatrixIO
{
 /*   public static async Task WriteTextAsync(Matrix matrix, Stream stream, string sep = " ")
    {
        using (StreamWriter writer = new StreamWriter(stream))
        {
            await writer.WriteLineAsync($"{matrix.r} {matrix.c}");
            for (int i = 0; i < matrix.r; i++)
            {
                for (int j = 0; j < matrix.c; j++)
                {
                    await writer.WriteAsync(matrix[i, j].ToString());
                    if (j < matrix.c - 1)
                        await writer.WriteAsync(sep);
                }
                await writer.WriteLineAsync();
            }
        }
    }
*/
    public static async Task<Matrix> ReadTextAsync(Stream stream, string sep = " ")
    {
        using (StreamReader reader = new StreamReader(stream))
        {
            string line = await reader.ReadLineAsync();
            string[] dimensions = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int rows = int.Parse(dimensions[0]);
            int columns = int.Parse(dimensions[1]);

            double[,] values = new double[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                line = await reader.ReadLineAsync();
                string[] elements = line.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < columns; j++)
                {
                    values[i, j] = double.Parse(elements[j]);
                }
            }

            return new Matrix(values);
        }
    }
/*
    public static void WriteBinary(Matrix matrix, Stream stream)
    {
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            writer.Write(matrix.r);
            writer.Write(matrix.c);
            for (int i = 0; i < matrix.r; i++)
            {
                for (int j = 0; j < matrix.c; j++)
                {
                    writer.Write(matrix[i, j]);
                }
            }
        }
    }
*/
    public static Matrix ReadBinary(Stream stream)
    {
        using (BinaryReader reader = new BinaryReader(stream))
        {
            int rows = reader.ReadInt32();
            int columns = reader.ReadInt32();

            double[,] values = new double[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    values[i, j] = reader.ReadDouble();
                }
            }

            return new Matrix(values);
        }
    }
/*
    public static async Task WriteJsonAsync(Matrix matrix, Stream stream)
    {
        double[][] array = new double[matrix.r][];
        for (int i = 0; i < matrix.r; i++)
        {
            array[i] = new double[matrix.c];
            for (int j = 0; j < matrix.c; j++)
            {
                array[i][j] = matrix[i, j];
            }
        }

        await JsonSerializer.SerializeAsync(stream, array);
    }
*/
    public static async Task<Matrix> ReadJsonAsync(Stream stream)
    {
        double[][] array = await JsonSerializer.DeserializeAsync<double[][]>(stream);
        double[,] values = new double[array.Length, array[0].Length];
        for (int i = 0; i < array.Length; i++)
        {
            for (int j = 0; j < array[0].Length; j++)
            {
                values[i, j] = array[i][j];
            }
        }

        return new Matrix(values);
    }

    public static void WriteToFile(string directory, string fileName, Matrix matrix, Action<Matrix, Stream> writeMethod)
    {
        using (FileStream stream = new FileStream(Path.Combine(directory, fileName), FileMode.Create))
        {
            writeMethod(matrix, stream);
        }
    }

    public static async Task WriteToFileAsync(string directory, string fileName, Matrix matrix, Func<Matrix, Stream, Task> writeMethod)
    {
        using (FileStream stream = new FileStream(Path.Combine(directory, fileName), FileMode.Create))
        {
            await writeMethod(matrix, stream);
        }
    }

    public static Matrix ReadFromFile(string file, Func<Stream, Matrix> readMethod)
    {
        using (FileStream stream = new FileStream(file, FileMode.Open))
        {
            return readMethod(stream);
        }
    }

    public static async Task<Matrix> ReadFromFileAsync(string file, Func<Stream, Task<Matrix>> readMethod)
    {
        using (FileStream stream = new FileStream(file, FileMode.Open))
        {
            return await readMethod(stream);
        }
    }
}
