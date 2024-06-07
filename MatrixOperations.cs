using System;
using System.Threading.Tasks;

public static class MatrixOperations
{
    public static Matrix Transpose(Matrix matrix)
    {
        double[,] transposed = new double[matrix.r, matrix.c];
        for (int i = 0; i < matrix.r; i++)
        {
            for (int j = 0; j < matrix.c; j++)
            {
                transposed[j, i] = matrix.matrix[i, j];
            }
        }
        return new Matrix(transposed);
    }

    public static Matrix Multiply(Matrix a, Matrix b)
    {
        if (a.c != b.r)
            throw new ArgumentException("Matrix dimensions are not valid for multiplication.");

        double[,] result = new double[a.r, b.c];
        for (int i = 0; i < a.r; i++)
        {
            for (int j = 0; j < b.c; j++)
            {
                result[i, j] = 0;
                for (int k = 0; k < a.c; k++)
                {
                    result[i, j] += a.matrix[i, k] * b.matrix[k, j];
                }
            }
        }
        return new Matrix(result);
    }

    
    public static Matrix ScalarMultiply(Matrix a, double scalar)
    {
        double[,] result = new double[a.r, a.c];
        for (int i = 0; i < a.r; i++)
        {
            for (int j = 0; j < a.c; j++)
            {
                result[i, j] = a.matrix[i, j] * scalar;
            }
        }
        return new Matrix(result);
    }

    public static Matrix Add(Matrix a, Matrix b)
    {
        if (a.r != b.r || a.c != b.c)
            throw new MatrixOperationException("Matrix dimensions must be the same for addition.");

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

    public static Matrix Subtract(Matrix a, Matrix b)
    {
        if (a.r != b.r || a.c != b.c)
            throw new MatrixOperationException("Matrix dimensions must be the same for subtraction.");

        double[,] result = new double[a.r, a.c];
        for (int i = 0; i < a.r; i++)
        {
            for (int j = 0; j < a.c; j++)
            {
                result[i, j] = a.matrix[i, j] - b.matrix[i, j];
            }
        }
        return new Matrix(result);
    }
/*
    public static Matrix Multiply(Matrix a, Matrix b)
    {
        if (a.c != b.r)
            throw new MatrixOperationException("Number of columns in the first matrix must be equal to the number of rows in the second matrix for multiplication.");

        b = Transpose(b); 

        double[,] result = new double[a.r, b.r];
        Parallel.For(0, a.r, i =>
        {
            for (int j = 0; j < b.r; j++)
            {
                double sum = 0;
                for (int k = 0; k < a.c; k++)
                {
                    sum += a.matrix[i, k] * b.matrix[j, k];
                }
                result[i, j] = sum;
            }
        });
        return new Matrix(result);
    }
*/
    public static (Matrix inverse, double determinant) Inverse(Matrix matrix)
    {
        throw new NotImplementedException(); 
    }
}

public class MatrixOperationException : Exception
{
    public MatrixOperationException(string message) : base(message) { }
}