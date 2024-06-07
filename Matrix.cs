public class Matrix
{
    public int r;
    public int c;
    public double[,] matrix;

    public Matrix(double[,] matrix)
    {
        r = matrix.GetLength(0);
        c = matrix.GetLength(1);
        this.matrix = matrix;
    }

    public static Matrix Zero(int n)
    {
        double[,] matrix = new double[n, n];
        return new Matrix(matrix);
    }

    public static Matrix Identity(int n)
    {
        double[,] matrix = new double[n, n];
        for (int i = 0; i < n; i++)
        {
            matrix[i, i] = 1;
        }
        return new Matrix(matrix);
    }

    public string ToTsv()
    {
        string result = $"{r}\t{c}\n";
        for (int i = 0; i < r; i++)
        {
            for (int j = 0; j < c; j++)
            {
                result += $"{matrix[i, j]}\t";
            }
            result = result.TrimEnd('\t') + "\n";
        }
        return result.TrimEnd('\n');
    }

    public override string ToString()
    {
        string result = "";
        for (int i = 0; i < r; i++)
        {
            for (int j = 0; j < c; j++)
            {
                result += $"{matrix[i, j]} ";
            }
            result = result.TrimEnd() + "\n";
        }
        return result.TrimEnd('\n');
    }

    public override bool Equals(object obj)
    {
        if (obj is Matrix other)
        {
            if (r != other.r || c != other.c)
                return false;

            for (int i = 0; i < r; i++)
            {
                for (int j = 0; j < c; j++)
                {
                    if (matrix[i, j] != other.matrix[i, j])
                        return false;
                }
            }
            return true;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return (r, c, matrix).GetHashCode();
    }

    //----------------------------------jperators----------------------------------
/*    public static Matrix operator *(Matrix matrix, double scalar)
    {
        double[,] result = new double[matrix.r, matrix.c];
        for (int i = 0; i < matrix.r; i++)
        {
            for (int j = 0; j < matrix.c; j++)
            {
                result[i, j] = matrix.matrix[i, j] * scalar;
            }
        }
        return new Matrix(result);
    }

    public static Matrix operator *(double scalar, Matrix matrix)
    {
        return matrix * scalar;
    }

    // Оператор "Унарный плюс / минус"
    public static Matrix operator +(Matrix a)
    {
        return a;
    }

    public static Matrix operator -(Matrix a)
    {
        double[,] result = new double[a.r, a.c];
        for (int i = 0; i < a.r; i++)
        {
            for (int j = 0; j < a.c; j++)
            {
                result[i, j] = -a.matrix[i, j];
            }
        }
        return new Matrix(result);
    }

    // Операторы "Сложение" / "Вычитание"
    public static Matrix operator +(Matrix a, Matrix b)
    {
        if (a.r != b.r || a.c != b.c)
            throw new ArgumentException("Matrix dimensions must be the same for addition.");

        double[,] result = new double[a.r, a.c];
        for (int i = 0; i < a.r; i++)
        {
            for (int j = 0; j < a.c; j++)
            {
                result[i, j] = a.matrix[i, j] + b.matrix[i, j];
            }
        }
        return new Matrix(result);
    }

    //транспонирование :(
*/
}
