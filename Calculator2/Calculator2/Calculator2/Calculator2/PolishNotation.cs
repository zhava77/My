using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;

namespace Calculator2
{
    class PolishNotation
    {
        string[] trigonometry =
        {
            "sin(",
            "cos(",
            "tan(",
            "inv("
        };
        string[] boolOp =
        {
            "!",
            @"/\",
            @"\/"
        };
        Dictionary<string, int> priorities = new Dictionary<string, int>
        {
            { "!", 3},
            { @"/\", 1 },
            { @"\/", 2 },
            { "cos(", -1 },
            { "tan(", -1 },
            { "sin(", -1 },
            { "inv(",-1 },
            { "(", -1 },
            { "+", 1 },
            { "-", 1 },
            { "/", 2 },
            { "*", 2 },
        };
        Dictionary<string, int> prioritiesExpression = new Dictionary<string, int>
        {
            { "(", -1 },
            { "+", 1 },
            { "-", 1 },
            { "/", 2 },
            { "*", 2 },
        };
        List<string> polishString;
        string inputString;

        public PolishNotation(string input)
        {
            PreCheck(input);
            input = DeleteSpace(input);
            inputString = input;
            ReplaceConstants();
            //Костыль для унарного минуса
            if (inputString[0] == '-')
            {
                inputString = inputString.Remove(0, 1);
                inputString = inputString.Insert(0, "-1*");
            }
            if (inputString[0] == '+')
            {
                inputString = inputString.Remove(0, 1);
                inputString = inputString.Insert(0, "1*");
            }

            for (int i = 1; i < inputString.Length; i++)
            {
                if (priorities.Keys.Contains(Convert.ToString(inputString[i - 1])))
                {
                    if (inputString[i] == '-' && inputString[i - 1] != '^')
                    {
                        inputString = inputString.Remove(i, 1);
                        inputString = inputString.Insert(i, "-1*");
                        i++;
                    }
                    else if (inputString[i] == '+')
                    {
                        inputString = inputString.Remove(i, 1);
                        inputString = inputString.Insert(i, "1*");
                        i++;
                    }
                }
            }
        }

        public List<string> GeneratePolishString()
        {
            if (inputString.Contains('K'))
            {
                return GeneratePolishStringForExpression();
            }

            Stack<string> operationStack = new Stack<string>();
            List<string> polishString = new List<string>();
            string operand = "";
            string[] operations = priorities.Keys.ToArray();

            bool lastIsOp = true; // Для унарных операций
            for (int i = 0; i < inputString.Length; i++)
            {
                string key = Convert.ToString(inputString[i]);
                if (key == "[")
                {
                    int closeIndex = inputString.IndexOf("]", i);
                    string matrix = inputString.Substring(i + 1, closeIndex - i - 1);
                    polishString.Add(matrix);
                    i = closeIndex;
                    continue;
                }
                //bool
                if (key == @"\" || key == "/")
                {
                    if (i + 1 < inputString.Length && boolOp.Contains(inputString.Substring(i, 2)))
                    {
                        key = inputString.Substring(i, 2);
                        i += 1;
                        if (operand != "")
                            polishString.Add(operand);
                        operand = "";
                        operationStack.Push(key);
                        continue;
                    }
                }
                //тригонометрия and inv
                else if (key == "s" || key == "c" || key == "t" || key == "i")
                {
                    if (i + 4 < inputString.Length && trigonometry.Contains(inputString.Substring(i, 4)))
                    {
                        operationStack.Push(inputString.Substring(i, 4));
                        i += 3;
                    }
                    else
                    {
                        operand += key;
                        lastIsOp = false;
                    }
                    continue;
                }
                //usual
                if (operations.Contains(key) && !(lastIsOp && key == "-"))
                {
                    if (operationStack.Count == 0)
                    {
                        operationStack.Push(key);
                    }
                    else if (priorities[operationStack.Peek()] >= priorities[key] && priorities[key] != -1)
                    {
                        //Нужно добавить число перед тем как считать операции
                        if (operand != "")
                        {
                            polishString.Add(operand);
                            operand = "";
                        }

                        while (operationStack.Count != 0
                            && operationStack.Peek() != "("
                            && !trigonometry.Contains(operationStack.Peek()))
                        {
                            polishString.Add(operationStack.Pop());
                        }

                        operationStack.Push(key);
                    }

                    else if (priorities[operationStack.Peek()] <= priorities[key] || priorities[key] == -1)
                    {
                        operationStack.Push(key);
                    }
                    lastIsOp = true;
                    //Добавляем если key - операция
                    if (operand != "")
                    {
                        polishString.Add(operand);
                        operand = "";
                    }
                }
                else if (key == ")")
                {
                    if (operand != "")
                    {
                        polishString.Add(operand);
                        operand = "";
                    }

                    while (operationStack.Peek() != "(" && !trigonometry.Contains(operationStack.Peek()))
                    {
                        polishString.Add(operationStack.Pop());
                    }
                    if (trigonometry.Contains(operationStack.Peek()))
                        polishString.Add(operationStack.Pop());
                    else
                        operationStack.Pop();
                }
                //Если у нас что то в опернаде - добавляем в главную строку
                else
                {
                    operand += key;
                    lastIsOp = false;
                }
            }

            if (operand != "")
            {
                polishString.Add(operand);
                operand = "";
            }
            while (operationStack.Count != 0)
            {
                polishString.Add(operationStack.Pop());
            }
            this.polishString = polishString;
            return polishString;
        }

        public List<string> GeneratePolishStringForExpression()
        {
            Stack<string> operationStack = new Stack<string>();
            List<string> polishString = new List<string>();
            string operand = "";
            for (int i = 0; i < inputString.Length; i++)
            {
                string key = Convert.ToString(inputString[i]);
                if (
                    key == "*"
                    || key == "/"
                    || (key == "-" && (i >= 1
                    && inputString[i - 1] != '^'
                    && !priorities.Keys.Contains(Convert.ToString(inputString[i - 1]))))
                    || key == "+"
                    || key == "("
                )
                {
                    if (operationStack.Count == 0)
                    {
                        operationStack.Push(key);
                    }
                    else if (priorities[operationStack.Peek()] >= priorities[key] && priorities[key] != -1)
                    {
                        //Нужно добавить число перед тем как считать операции
                        if (operand != "")
                        {
                            polishString.Add(operand);
                            operand = "";
                        }

                        while (operationStack.Count != 0
                            && operationStack.Peek() != "("
                            && !trigonometry.Contains(operationStack.Peek()))
                        {
                            polishString.Add(operationStack.Pop());
                        }

                        operationStack.Push(key);
                    }

                    else if (priorities[operationStack.Peek()] <= priorities[key] || priorities[key] == -1)
                    {
                        operationStack.Push(key);
                    }
                    //Добавляем если key - операция
                    if (operand != "")
                    {
                        polishString.Add(operand);
                        operand = "";
                    }
                }
                else if (key == ")")
                {
                    if (operand != "")
                    {
                        polishString.Add(operand);
                        operand = "";
                    }

                    while (operationStack.Peek() != "(")
                    {
                        polishString.Add(operationStack.Pop());
                    }
                    if (trigonometry.Contains(operationStack.Peek()))
                        polishString.Add(operationStack.Pop());
                    else
                        operationStack.Pop();
                }
                //Если у нас что то в опернаде - добавляем в главную строку
                else
                {
                    operand += key;
                }
            }

            if (operand != "")
            {
                polishString.Add(operand);
                operand = "";
            }
            while (operationStack.Count != 0)
            {
                polishString.Add(operationStack.Pop());
            }
            this.polishString = polishString;
            return polishString;

        }

        public string CalculatePolishString()
        {

            ReplaceVar();
            Stack<string> operandStack = new Stack<string>();

            if (priorities.Keys.Contains(Convert.ToString(inputString[inputString.Length - 1])))
            {
                throw new Exception("Последнее действие - операция");
            }

            for (int i = 0; i < polishString.Count; i++)
            {
                string key = polishString[i];
                if (!priorities.Keys.Contains(key))
                {
                    operandStack.Push(key);
                }
                else
                {
                    if (key == "sin(" || key == "cos(" || key == "tan(")
                    {
                        string newOperand = CalculateTrigonometry(operandStack.Pop(), key);
                        operandStack.Push(newOperand);
                    }
                    else if (key == @"\/" || key == @"/\")
                    {
                        string newOperand = Calculation(operandStack.Pop(), operandStack.Pop(), key);
                        operandStack.Push(newOperand);
                    }
                    else if (key == "!")
                    {
                        string newOperand = CalculationNot(operandStack.Pop());
                        operandStack.Push(newOperand);
                    }
                    else if (key == "inv(")
                    {
                        string newOperand = CalculateInv(operandStack.Pop());
                        operandStack.Push(newOperand);
                    }
                    else
                    {
                        string newOperand = Calculation(operandStack.Pop(), operandStack.Pop(), key);
                        operandStack.Push(newOperand);
                    }
                }
            }
            if (operandStack.Count == 1)
            {
                string result = operandStack.Pop();
                if (result.Contains("x"))
                {
                    Expression res = new Expression(result);
                    res.ToNormalView();
                    return res.ToString();
                }
                else
                {
                    return result;
                }
            }
            else
            {
                throw new Exception("Ошибка");
            }

        }

        string CalculateInv(string operand)
        {
            Matrix m = new Matrix(operand);
            Console.WriteLine(m.Inversion().ToString());
            return m.Inversion().ToString();
        }

        string CalculateTrigonometry(string operand, string operation)
        {
            if (operation == "sin(")
                return Convert.ToString(Math.Sin(Convert.ToDouble(operand)));
            else if (operation == "cos(")
                return Convert.ToString(Math.Cos(Convert.ToDouble(operand)));
            else if (operation == "tan(")
            {
                if (Convert.ToDouble(operand) == Math.PI / 2)
                {
                    throw new Exception("Введите корректные данные");
                }
                return Convert.ToString(Math.Tan(Convert.ToDouble(operand)));
            }

            else
                throw new Exception("Введите выражение корректно");
        }

        string CalculationNot(string operand)
        {
            if (Convert.ToInt16(operand) > 1 || Convert.ToInt16(operand) < 0)
            {
                Console.WriteLine("Что то с бул не так");
                throw new Exception("");
            }
            else
            {
                
                return Convert.ToString(Convert.ToInt16(!Convert.ToBoolean(Convert.ToInt16(operand))));
            }
        }

        string Calculation(string rightOperand, string leftOperand, string operation)
        {
            leftOperand = leftOperand.Replace('.', ',');
            rightOperand = rightOperand.Replace('.', ',');
            if (leftOperand.Contains("x") || rightOperand.Contains("x"))
            {
                Expression left = new Expression(leftOperand);
                Expression right = new Expression(rightOperand);
                return Calculate(left, right, operation).ToString();
            }
            else if (boolOp.Contains(operation))
            {
                if (Convert.ToInt16(rightOperand) > 1
                    || Convert.ToInt16(rightOperand) < 0
                    || Convert.ToInt16(leftOperand) > 1
                    || Convert.ToInt16(leftOperand) < 0)
                {
                    throw new Exception("Что то с бул не так");
                }
                bool right = Convert.ToBoolean(Convert.ToInt16(rightOperand));
                bool left = Convert.ToBoolean(Convert.ToInt16(leftOperand));
                return Convert.ToString(Calculate(left, right, operation));

            }
            else if (leftOperand.Contains(" ") && rightOperand.Contains(" "))
            {
                Matrix right = new Matrix(rightOperand);
                Matrix left = new Matrix(leftOperand);
                return Calculate(left, right, operation).ToString();
            }
            else if (leftOperand.Contains(" ") && operation == "*")
            {
                Matrix m = new Matrix(leftOperand);
                return (m * Convert.ToDouble(rightOperand)).ToString();
            }
            else if (rightOperand.Contains(" ") && operation == "*")
            {
                Matrix m = new Matrix(rightOperand);
                return (m * Convert.ToDouble(leftOperand)).ToString();
            }
            else if (leftOperand.Contains(",") || rightOperand.Contains(","))
            {
                double right = Convert.ToDouble(rightOperand);
                double left = Convert.ToDouble(leftOperand);
                return Convert.ToString(Math.Round(Calculate(left, right, operation), 3));
            }
            else
            {
                long right = Convert.ToInt64(rightOperand);
                long left = Convert.ToInt64(leftOperand);
                return Convert.ToString(Calculate(left, right, operation));
            }
        }

        Expression Calculate(Expression left, Expression right, string operation)
        {
            if (operation == "+")
            {
                return left + right;
            }
            else if (operation == "-")
            {
                return left - right;
            }
            else if (operation == "*")
            {
                return left * right;
            }
            else if (operation == "/")
            {
                return left / right;
            }
            throw new Exception("Выражение имело неверный формат" + left.ToString() + " " + right.ToString());
        }

        Matrix Calculate(Matrix left, Matrix right, string operation)
        {
            if (operation == "+")
            {
                return left + right;
            }
            else if (operation == "-")
            {
                return left - right;
            }
            else if (operation == "*")
            {
                return left * right;
            }
            throw new Exception("Выражение имело неверный формат");
        }

        short Calculate(bool left, bool right, string operation)
        {
            if (operation == @"/\")
                return Convert.ToInt16(left && right);
            else if (operation == @"\/")
                return Convert.ToInt16(left || right);
            return 0;
        }

        double Calculate(double left, double right, string operation)
        {
            if (operation == "+")
            {
                return left + right;
            }
            else if (operation == "-")
            {
                return left - right;
            }
            else if (operation == "*")
            {
                return left * right;
            }
            else if (operation == "/")
            {
                return left / right;
            }
            throw new Exception("Выражение имело неверный формат");
        }

        long Calculate(long left, long right, string operation)
        {
            if (operation == "+")
            {
                return left + right;
            }
            else if (operation == "-")
            {
                return left - right;
            }
            else if (operation == "*")
            {
                return left * right;
            }
            else if (operation == "/")
            {
                return left / right;
            }
            throw new Exception("Выражение имело неверный формат");
        }

        void ReplaceConstants()
        {
            inputString = inputString.Replace("e", Convert.ToString(Math.E));
            inputString = inputString.Replace("pi", Convert.ToString(Math.PI));
        }

        void ReplaceVar()
        {
            List<string> varCount = new List<string>();
            Regex r = new Regex(@"[0-9]{1}");
            foreach (var item in polishString)
            {
                Match m = r.Match(item);
                //Для выпажений
                if (item.Contains("x"))
                {
                    for (int i = 0; i < item.Length; i++)
                    {
                        m = r.Match(Convert.ToString(item[i]));
                        if (item[i] != 'x' && item[i] != '*' && item[i] != '^' && !m.Success)
                        {
                            varCount.Add(Convert.ToString(item[i]));
                        }
                    }
                }
                //Для всего
                else if (item.Length == 1 && !varCount.Contains(item) && !m.Success && !priorities.Keys.Contains(item))
                {
                    varCount.Add(item);
                }
            }

            if (varCount.Count > 0)
            {
                Console.WriteLine("Введите значения для переменных");
                foreach (var item in varCount)
                {
                    Console.Write(item + " = ");
                    string value = Console.ReadLine();
                        value = value.Replace("e", Convert.ToString(Math.E));
                        value = value.Replace("pi", Convert.ToString(Math.PI));
                    ReplaceValueInList(ref polishString, item, value);

                }
            }
        }

        void ReplaceValueInList(ref List<string> list, string find, string replace)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == find)
                {
                    list[i] = replace;
                }
                else if (list[i].Contains("x") && list[i].Contains(find))
                {
                    list[i] = list[i].Replace(find, replace);
                }
            }
        }

        void PreCheck(string input)
        {
            bool contains = false;
            foreach (var item in priorities.Keys)
            {
                contains = contains || input.Contains(item);
            }
            if (!contains)
            {
                throw new Exception("Введите выражение с какими то операциями!");
            }
            int count = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '(')
                    count++;
                else if (input[i] == ')')
                    count--;
            }
            if (count != 0)
                throw new Exception("Неверное кол во скобок");

        }

        string DeleteSpace(string input)
        {
            bool pass = false;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '[')
                    pass = true;
                else if (input[i] == ']')
                    pass = false;
                else if (!pass && input[i] == ' ')
                {
                    input = input.Remove(i, 1);
                    i--;
                }
            }
            return input;
        }
    }
}
