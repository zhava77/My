using System;
using System.Collections.Generic;
using System.Linq;

namespace Calculator2
{
    class Matrix
    {
       public double[,] matrix;

        public Matrix(string input)
        {
            string[] rows = input.Split(';');
            int rowsCount = rows.Length;
            IEnumerable<string> tempRows = rows.Select(x => x.Trim());
            rows = tempRows.ToArray();
            int columnCount = rows[0].Split(' ').Length;
            matrix = new double[rowsCount, columnCount];
            for (int i = 0; i < rowsCount; i++)
            {
                string[] elements = rows[i].Split(' ');
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    matrix[i, j] = Convert.ToDouble(elements[j]);
                }
            }
        }

        public Matrix(double[,] m)
        {
            matrix = m;
        }
        public override string ToString()
        {
            string result = " ";
            if (matrix != null)
            {
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    for (int j = 0; j < matrix.GetLength(1); j++)
                    {
                        result += matrix[i, j] + " ";
                    }
                    if (i != matrix.GetLength(0) - 1)
                        result += "; ";
                }
            }
            return result;
        }

        public static Matrix operator +(Matrix m1, Matrix m2)
        {

            if (m1.matrix.GetLength(0) == m2.matrix.GetLength(0)
                && m1.matrix.GetLength(1) == m2.matrix.GetLength(1))
            {
                double[,] resultMatrix = new double[m1.matrix.GetLength(0), m1.matrix.GetLength(1)];
                for (int i = 0; i < m1.matrix.GetLength(0); i++)
                {
                    for (int j = 0; j < m2.matrix.GetLength(1); j++)
                    {
                        resultMatrix[i, j] = m1.matrix[i, j] + m2.matrix[i, j];
                    }
                }
                return new Matrix(resultMatrix);
            }
            throw new Exception("С матрицами не так, они разные: " + m1.matrix.GetLength(0) + " " + m2.matrix.GetLength(0));
        }

        public static Matrix operator -(Matrix m1, Matrix m2)
        {

            if (m1.matrix.GetLength(0) == m2.matrix.GetLength(0)
                && m1.matrix.GetLength(1) == m2.matrix.GetLength(1))
            {
                double[,] resultMatrix = new double[m1.matrix.GetLength(0), m1.matrix.GetLength(1)];
                for (int i = 0; i < m1.matrix.GetLength(0); i++)
                {
                    for (int j = 0; j < m2.matrix.GetLength(1); j++)
                    {
                        resultMatrix[i, j] = m1.matrix[i, j] - m2.matrix[i, j];
                    }
                }
                return new Matrix(resultMatrix);
            }
            throw new Exception("С матрицами не так, они разные");
        }

        public static Matrix operator *(double number, Matrix m)
        {

            double[,] resultMatrix = new double[m.matrix.GetLength(0), m.matrix.GetLength(1)];
            for (int i = 0; i < m.matrix.GetLength(0); i++)
            {
                for (int j = 0; j < m.matrix.GetLength(1); j++)
                {
                    resultMatrix[i, j] = m.matrix[i, j] * number;
                }
            }
            return new Matrix(resultMatrix);
        }

        public static Matrix operator *(Matrix m, double number)
        {

            double[,] resultMatrix = new double[m.matrix.GetLength(0), m.matrix.GetLength(1)];
            for (int i = 0; i < m.matrix.GetLength(0); i++)
            {
                for (int j = 0; j < m.matrix.GetLength(1); j++)
                {
                    resultMatrix[i, j] = m.matrix[i, j] * number;
                }
            }
            return new Matrix(resultMatrix);
        }

        public static Matrix operator *(Matrix m1, Matrix m2)
        {

            if (m1.matrix.GetLength(1) == m2.matrix.GetLength(0))
            {
                double[,] resultMatrix = new double[m1.matrix.GetLength(0), m2.matrix.GetLength(1)];
                for (int i = 0; i < m1.matrix.GetLength(0); i++)
                {
                    for (int j = 0; j < m2.matrix.GetLength(1); j++)
                    {
                        for (int index = 0; index < m2.matrix.GetLength(0); index++)
                        {
                            resultMatrix[i, j] += m1.matrix[i, index] * m2.matrix[index, j];
                        }
                    }
                }
                return new Matrix(resultMatrix);
            }
            throw new Exception("С матрицами не так, они не согласованные: " + m1.matrix.GetLength(1) + " " + m2.matrix.GetLength(0));
        }

        public Matrix Inversion()
        {
            double determinate = CalculateDeterminant();
            if (determinate != 0)
            {
                double[,] resultMatrix = new double[matrix.GetLength(0), matrix.GetLength(1)];
                for (int i = 0; i < resultMatrix.GetLength(0); i++)
                {
                    for (int j = 0; j < resultMatrix.GetLength(1); j++)
                    {
                        resultMatrix[i, j] = GetMinor(matrix, j, i).CalculateDeterminant() * Math.Pow(-1, i + j) / determinate;
                    }
                }
                return new Matrix(resultMatrix);
            }
            throw new Exception(" det = 0");

        }

        public double CalculateDeterminant()
        {
            if (matrix.GetLength(0) == matrix.GetLength(1))
            {
                if (matrix.GetLength(0) == 2)
                {
                    return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
                }
                if(matrix.GetLength(0) == 1)
                {
                    return matrix[0, 0];
                }
                else
                {
                    double determinant = 0;
                    for (int i = 0; i < matrix.GetLength(0); i++)
                    {
                        if (i % 2 == 0)
                            determinant += matrix[0, i] * GetMinor(matrix, 0, i).CalculateDeterminant();
                        else
                            determinant -= matrix[0, i] * GetMinor(matrix, 0, i).CalculateDeterminant();
                    }
                    return determinant;
                }
            }
            throw new Exception("Матрица не квадратная");
        }

        public static Matrix GetMinor(double[,] matrix, int row, int column)
        {
            double[,] minor = new double[matrix.GetLength(0) - 1, matrix.GetLength(0) - 1];
            int minorRow = 0;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                int minorColumn = 0;
                for (int j = 0; j < matrix.GetLength(0); j++)
                {
                    if (minorColumn == matrix.GetLength(1) - 1 || minorRow == matrix.GetLength(1) - 1)
                        break;

                    minor[minorRow, minorColumn] = matrix[i, j];
                    if (j != column)
                    {
                        minorColumn++;
                    }
                }
                if (i != row)
                {
                    minorRow++;
                }
            }
            return new Matrix(minor);
        }

    }
}
