using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator2
{
    class X
    {
        public double coef = 1;
        public int power = 1;

        public X(string operand)
        {
            if (operand.Contains("x"))
            {
                int indexOfcoef = operand.IndexOf("*");
                if (indexOfcoef > 0)
                {
                    coef = Convert.ToDouble(operand.Substring(0, indexOfcoef));
                }
                else
                {
                    int indexOfX = operand.IndexOf("x");
                    if (indexOfX > 0)
                    {
                        string coefS = operand.Substring(0, indexOfX);
                        if (coefS == "+")
                            coefS = "1";
                        else if (coefS == "-")
                            coefS = "-1";
                        coef = Convert.ToDouble(coefS);
                    }
                }
                int indexOfPower = operand.IndexOf("^");
                if (indexOfPower > 0)
                {
                    power = Convert.ToInt32(operand.Substring(indexOfPower + 1, operand.Length - indexOfPower - 1));
                }
            }
            else
            {
                coef = Convert.ToDouble(operand);
                power = 0;
            }

        }

        public X(double coef, int power)
        {
            this.coef = coef;
            this.power = power;
        }

        public override string ToString()
        {
            return coef + "*x^" + power;
        }

        public static X operator /(X left, X right)
        {
            double coef = Math.Round(left.coef / right.coef, 3);
            //Console.WriteLine(coef);
            int power = left.power - right.power > 0 ? left.power - right.power : 0;
            X result = new X(coef, power);
            return result;
        }
    }


    class Expression
    {
        string[] operations =
        {
            "+",
            "-"
        };

        List<X> expression;

        public Expression(string exp)
        {

            expression = new List<X>();
            string operand = "";
            for (int i = 0; i < exp.Length; i++)
            {
                string key = Convert.ToString(exp[i]);
                if (operations.Contains(key)
                    && (i - 1 >= 0 && Convert.ToString(exp[i - 1]) != "*")
                    && (i - 1 >= 0 && Convert.ToString(exp[i - 1]) != "^")
                    && !operations.Contains(Convert.ToString(exp[i - 1])))
                {
                    if (operand != "")
                    {
                        
                        expression.Add(new X(operand));
                        operand = "";
                    }
                }
                operand += exp[i];
            }
            expression.Add(new X(operand));
        }

        public Expression()
        {
            expression = new List<X>();
        }

        public override string ToString()
        {

            string result = "";
            bool first = true;
            foreach (var item in expression)
            {
                if (!first && item.coef >= 0)
                    result += "+" + item.ToString();
                else if (item.coef < 0)
                {
                    result += item.ToString();
                }
                else
                    result += item.ToString();
                first = false;
            }
            return result;
        }

        public static Expression operator +(Expression left, Expression right)
        {
            for (int i = 0; i < left.expression.Count; i++)
            {
                for (int j = 0; j < right.expression.Count; j++)
                {
                    if (left.expression[i].power == right.expression[j].power)
                    {
                        left.expression[i].coef += right.expression[j].coef;
                        right.expression.RemoveAt(j);
                    }
                }
            }

            foreach (var item in right.expression)
            {
                left.expression.Add(item);
            }
            left.Minimize();
            return left;
        }
        public static Expression operator -(Expression left, Expression right)
        {
            for (int i = 0; i < left.expression.Count; i++)
            {
                for (int j = 0; j < right.expression.Count; j++)
                {
                    if (left.expression[i].power == right.expression[j].power)
                    {
                        left.expression[i].coef -= right.expression[j].coef;
                        right.expression.RemoveAt(j);
                    }
                }
            }

            foreach (var item in right.expression)
            {
                item.coef = -1 * item.coef;
                left.expression.Add(item);
            }
            left.Minimize();
            return left;
        }
        public static Expression operator *(Expression left, Expression right)
        {
            Expression ex = new Expression();
            for (int i = 0; i < left.expression.Count; i++)
            {
                for (int j = 0; j < right.expression.Count; j++)
                {
                    X x = new X(
                        left.expression[i].coef * right.expression[j].coef,
                        left.expression[i].power + right.expression[j].power
                        );

                    ex.expression.Add(x);
                }
            }
            left.Minimize();
            return ex;
        }
        public static Expression operator /(Expression left, Expression right)
        {
            Expression newExp = new Expression();
            X maxLeft = left.FindMaxX();
            X maxRight = right.FindMaxX();
            if (maxRight.power == 0 && maxRight.coef == 0)
            {
                throw new Exception("Деление на ноль");
            }
            while (maxLeft.power >= maxRight.power)
            {
                X diff = maxLeft / maxRight;
                Expression expForMult = new Expression();
                expForMult.expression.Add(diff);
                newExp.expression.Add(diff);
                Expression ex = right * expForMult;
                left -= ex;
                maxLeft = left.FindMaxX();
            }
            if (newExp.expression.Count == 0)
                newExp.expression.Add(new X(0, 0));
            return newExp;
        }

        public void ToNormalView()
        {
            int max = FindMaxPower();
            List<X> newExp = new List<X>();
            for (int i = max; i >= 0; i--)
            {
                bool isFind = false;
                for (int j = 0; j < expression.Count; j++)
                {
                    if (expression[j].power == i)
                    {
                        newExp.Add(expression[j]);
                        isFind = true;
                    }
                }
                if (!isFind)
                {
                    X x = new X(0, i);
                    newExp.Add(x);
                }
            }

            expression = newExp;
        }

        public void Minimize()
        {
            for (int i = 0; i < expression.Count; i++)
            {
                for (int j = 0; j < expression.Count; j++)
                {
                    if (i != j && expression[i].power == expression[j].power)
                    {
                        expression[i].coef += expression[j].coef;
                        expression.RemoveAt(j);
                        i = 0;
                        continue;
                    }
                }
            }
        }

        int FindMaxPower()
        {
            int max = 0;
            for (int i = 0; i < expression.Count; i++)
            {
                if (expression[i].power > max)
                {
                    max = expression[i].power;
                }
            }
            return max;
        }
        X FindMaxX()
        {
            X max = new X(0, 0);
            for (int i = 0; i < expression.Count; i++)
            {
                if (expression[i].power > max.power && expression[i].coef != 0)
                {
                    max = expression[i];
                }
            }

            return max;
        }
    }
}
