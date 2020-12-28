using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Core.Math;
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEngine;

namespace UnitTests.Harem.Math
{
    public class MathTests
    {
        private const string JSON_TESTS_PATH = @"Editor\UnitTests\Math\MathTest.Json";
        [Serializable]
        public class Number
        {
            public int value;
            public int rank;
        }

        [Serializable]
        public class MathOp
        {
            public string a;
            public string b;
            public string op;
            public Number result;
        }

        [Serializable]
        public class LogicOp
        {
            public Number a;
            public Number b;
            public string condition;
            public bool logicResult;
        }

        [Serializable]
        public class Test
        {
            public List<LogicOp> logicTests;
            public List<MathOp> mathTests;
        }
        
        
        [Test]
        [TestCase("10", "15", "25")]
        [TestCase("10", "-15", "-5")]
        [TestCase("200000000", "300000000", "500000000")]
        [TestCase("200000000", "-250000000", "-50000000")]
        [TestCase("20000e5", "-30000e5", "-10000e5")]
        [TestCase("1e9", "-14786e4", "85214e4")]
        [TestCase("1.29", "2.35", "3.64")]
        public void BigNumberAdditionTest(string firstValue, string secondValue, string result)
        {
            BigNumber firstNumber = new BigNumber(firstValue);
            BigNumber secondNumber = new BigNumber(secondValue);

            BigNumber expectedResult = new BigNumber(result);

            // Checking if this operation even works
            Assert.IsTrue(firstNumber + secondNumber == expectedResult,
                $"Addition isn't working correctly: {firstValue} + {secondValue} != {result}\n" +
                $"firstNumber + secondNumber = {firstNumber} + {secondNumber} = {firstNumber + secondNumber}\n" +
                $"expectedResult = {expectedResult}");
            // Checking commutativity
            Assert.IsTrue(firstNumber + secondNumber == secondNumber + firstNumber,
                $"Addition isn't commutative: {firstValue} + {secondValue} != {secondValue} + {firstValue}\n" +
                $"firstNumber + secondNumber = {firstNumber + secondNumber}\n" +
                $"secondNumber + firstNumber = {secondNumber + firstNumber}");
            // Checking associativity
            Assert.IsTrue(
                (firstNumber + secondNumber) + expectedResult == firstNumber + (secondNumber + expectedResult),
                $"Addition isn't assotiative: ({firstValue} + {secondValue}) + {result} != {firstValue} + ({secondValue} + {result})");
        }

        [Test]
        [TestCase("0", "0")]
        [TestCase("1", "1")]
        [TestCase("10", "10")]
        [TestCase("156321", "156.32K")]
        [TestCase("1500", "1.5K")]
        [TestCase("1567", "1.57K")]
        [TestCase("15567", "15.57K")]
        [TestCase("155670", "155.67K")]
        [TestCase("1556700", "1.56M")]
        [TestCase("4e9", "4B")]
        [TestCase("1400000", "1.4M")]
        [TestCase("14000000", "14M")]
        [TestCase("140000000", "140M")]
        [TestCase("1400000000", "1.4B")]
        public void BigNumberToStringTest(string numValue, string expectedValue)
        {
            BigNumber number = new BigNumber(numValue);
            Assert.IsTrue(number.ToString() == expectedValue, $"To String fail: {expectedValue} != {number}");
        }

        [Test]
        [TestCase("1200000", "1100200", "99.8K")]
        public void BigNumberMinusToStringTest(string firstValue, string secondValue, string result)
        {
            BigNumber firstNumber = new BigNumber(firstValue);
            BigNumber secondNumber = new BigNumber(secondValue);
            Assert.IsTrue((firstNumber - secondNumber).ToString() == result,
                $"To String fail: {firstValue} - {secondValue} != {(firstNumber - secondNumber).ToString()}");
        }

        [Test]
        [TestCase("10", "2", "20")]
        [TestCase("31", "45", "1395")]
        [TestCase("25000", "25000", "625000000")]
        [TestCase("250000", "250000", "62500000000")]
        public void BigNumberMultiplicationTest(string firstValue, string secondValue, string result)
        {
            BigNumber firstNumber = new BigNumber(firstValue);
            BigNumber secondNumber = new BigNumber(secondValue);

            BigNumber expectedResult = new BigNumber(result);

            // Checking if this operation even works
            Assert.IsTrue(firstNumber * secondNumber == expectedResult,
                $"Multiplication isn't working correctly: {firstValue} * {secondValue} != {result}");
            // Checking commutativity
            Assert.IsTrue(firstNumber * secondNumber == secondNumber * firstNumber,
                $"Multiplication isn't commutative: {firstValue} * {secondValue} != {secondValue} * {firstValue}");
            // Checking associativity
            Assert.IsTrue(
                (firstNumber * secondNumber) * expectedResult == firstNumber * (secondNumber * expectedResult),
                $"Multiplication isn't assotiative: ({firstValue} * {secondValue}) * {result} != {firstValue} * ({secondValue} * {result})");
        }

        [Test]
        [TestCase("10", 2, "20")]
        [TestCase("31", 45, "1395")]
        public void BigNumberMultiplicationIntTest(string firstValue, int secondValue, string result)
        {
            BigNumber firstNumber = new BigNumber(firstValue);

            BigNumber expectedResult = new BigNumber(result);

            // Checking if this operation even works
            Assert.IsTrue(firstNumber * secondValue == expectedResult,
                $"Multiplication isn't working correctly: {firstValue} * (int) {secondValue} != {result}");
            // Checking commutativity
            Assert.IsTrue(firstNumber * secondValue == secondValue * firstNumber,
                $"Multiplication isn't commutative: {firstValue} * (int) {secondValue} != (int) {secondValue} * {firstValue}");
            // Checking associativity
            Assert.IsTrue((firstNumber * secondValue) * expectedResult == firstNumber * (secondValue * expectedResult),
                $"Multiplication isn't assotiative: ({firstValue} * (int) {secondValue}) * {result} != {firstValue} * ((int) {secondValue} * {result})");
        }

        [Test]
        [TestCase("10", 2, "20")]
        [TestCase("10", 0.5f, "5")]
        [TestCase("31", 45, "1395")]
        [TestCase("20000e15", 0.2f, "40000e14")]
        public void BigNumberMultiplicationFloatTest(string firstValue, float secondValue, string result)
        {
            BigNumber firstNumber = new BigNumber(firstValue);

            BigNumber expectedResult = new BigNumber(result);

            // Checking if this operation even works
            Assert.IsTrue(firstNumber * secondValue == expectedResult,
                $"Multiplication isn't working correctly: {firstValue} * (float) {secondValue} != {result}" +
                $"firstNumber + secondNumber = {firstNumber} * (float) {secondValue} = {firstNumber * secondValue}\n" +
                $"expectedResult = {expectedResult}");
            // Checking commutativity
            Assert.IsTrue(firstNumber * secondValue == secondValue * firstNumber,
                $"Multiplication isn't commutative: {firstValue} * (float) {secondValue} != (float) {secondValue} * {firstValue}");
        }

        [Test]
        [TestCase("10", "2", 5)]
        [TestCase("0", "29", 0)]
        [TestCase("300", "3", 100)]
        [TestCase("25000", "25000", 1)]
        [TestCase("250e10", "1e10", 250)]
        public void BigNumberFloatDivisionTest(string firstValue, string secondValue, float result)
        {
            BigNumber firstNumber = new BigNumber(firstValue);
            BigNumber secondNumber = new BigNumber(secondValue);

            // Checking if this operation even works
            Assert.IsTrue(firstNumber % secondNumber == result,
                $"Division isn't working correctly: {firstValue} / {secondValue} ({firstNumber % secondNumber}) != {result}");
        }

        [Test]
        [TestCase("10", "2", "5")]
        [TestCase("0", "29", "0")]
        [TestCase("300", "3", "100")]
        [TestCase("25000", "25000", "1")]
        [TestCase("250e10", "1e10", "250")]
        [TestCase("22e252", "2151e12", "10227e234")]
        [TestCase("1", "0.2", "5")]
        public void BigNumberDivisionTest(string firstValue, string secondValue, string result)
        {
            BigNumber firstNumber = new BigNumber(firstValue);
            BigNumber secondNumber = new BigNumber(secondValue);
            BigNumber resultNumber = new BigNumber(result);

            // Checking if this operation even works
            Assert.IsTrue(firstNumber / secondNumber == resultNumber,
                $"Division isn't working correctly: {firstValue} / {secondValue} ({firstNumber / secondNumber}) != {resultNumber.ToString()}");
        }

        [Test]
        [TestCase("2151e12", "22e252", false)]
        [TestCase("250e10", "250e10", false)]
        [TestCase("251e10", "250e10", true)]
        [TestCase("12.07", "12.06", true)]
        [TestCase("2.25", "2.3", false)]
        public void BigNumberGreaterComparisonTest(string firstValue, string secondValue, bool result)
        {
            BigNumber firstNumber = new BigNumber(firstValue);
            BigNumber secondNumber = new BigNumber(secondValue);

            Assert.IsTrue((firstNumber > secondNumber) == result);
        }

        [Test]
        [TestCase("2151e12", "22e252", "-22000e249")]
        [TestCase("124421", "983381", "-858960")]
        [TestCase("983000", "124000", "859000")]
        [TestCase("1", "0.36", "0.64")]
        public void BigNumberSubtractTest(string firstValue, string secondValue, string result)
        {
            BigNumber firstNumber = new BigNumber(firstValue);
            BigNumber secondNumber = new BigNumber(secondValue);
            BigNumber expectedResult = new BigNumber(result);

            Assert.IsTrue((firstNumber - secondNumber) == expectedResult, $"{(firstNumber - secondNumber)}");
        }

        [Test]
        [TestCase("2151e12", "22e252", true)]
        [TestCase("250e10", "250e10", false)]
        [TestCase("983000", "124000000", true)]
        [TestCase("124e6", "983e6", true)]
        [TestCase("250e10", "251e10", true)]
        public void BigNumberLessComparisonTest(string firstValue, string secondValue, bool result)
        {
            BigNumber firstNumber = new BigNumber(firstValue);
            BigNumber secondNumber = new BigNumber(secondValue);

            Assert.IsTrue((firstNumber < secondNumber) == result);
        }


        [TestCaseSource(typeof(JsonMathOperationTests))]
        public void JsonMathOperationTest(MathOp mathTest)
        {
            BigNumber a = new BigNumber(mathTest.a);
            BigNumber b = new BigNumber(mathTest.b);
            BigNumber res = BigNumber.One;
            switch (mathTest.op)
            {
                case "+":
                    res = a + b;
                    break;
                case "*":
                    res = a * b;
                    break;
                case "-":
                    res = a - b;
                    break;
                case "/":
                    res = a / b;

                    break;
                case "^":
                    int pow = Convert.ToInt32(mathTest.b);
                    res = BigNumber.Pow(a, pow);
                    break;
                default:
                    Assert.Fail($"Unknown operation {mathTest.op}");
                    break;
            }

            Assert.IsTrue(res == new BigNumber(mathTest.result.value,
                mathTest.result.rank),
               $"Fail!\n" +
               $" [rank: {a.Rank}, value: {a.Value}] {mathTest.op} [rank: {b.Rank}, value: {b.Value}] != [rank: {mathTest.result.rank} , value: {mathTest.result.value}]\n" +
               $"Got [rank: {res.Rank}  value: {res.Value}");
        }

        [TestCaseSource(typeof(JsonLogicOperationTests))]
        public void JsonLogicOperationTest(LogicOp logicTest)
        {
            BigNumber a = new BigNumber(logicTest.a.value, logicTest.a.rank);
            BigNumber b = new BigNumber(logicTest.b.value, logicTest.b.rank);
            bool res = false;
            switch (logicTest.condition)
            {
                case ">":
                    res = a > b;
                    break;
                case ">=":
                    res = a >= b;
                    break;
                case "==":
                    res = a == b;
                    break;
                case "!=":
                    res = a != b;
                    break;
                case "<":
                    res = a < b;
                    break;
                case "<=":
                    res = a <= b;
                    break;
                default:
                    Assert.Fail($"Unknown condition {logicTest.condition}");
                    break;
            }

            Assert.IsTrue(res == logicTest.logicResult,
                $"Fail!\n" +
                $"[rank: {a.Rank}, value: {a.Value}] {logicTest.condition} [rank: {b.Rank}, value: {b.Value}] != {logicTest.logicResult}\n" +
                $"Got {res}");
        }

        class JsonMathOperationTests : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                string path = Path.Combine(Application.dataPath, JSON_TESTS_PATH);
                
                if (!File.Exists(path))
                {
                    yield break;
                }

                using (StreamReader sr = new StreamReader(path))
                {
                    var test = JsonConvert.DeserializeObject<Test>(sr.ReadToEnd());
                    foreach (MathOp mathTest in test.mathTests)
                    {
                        TestCaseData data = new TestCaseData(mathTest)
                            .SetName($"Math Operation Test: {mathTest.a} {mathTest.op} {mathTest.b} = {new BigNumber(mathTest.result.value, mathTest.result.rank)}[value: {mathTest.result.value}, rank: {mathTest.result.rank}]")
                            .SetCategory("Math tests from Json");
                        yield return data;
                    }

                }
            }
        }
        
        class JsonLogicOperationTests : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                string path = Path.Combine(Application.dataPath, JSON_TESTS_PATH);
                
                if (!File.Exists(path))
                {
                    yield break;
                }

                using (StreamReader sr = new StreamReader(path))
                {
                    var test = JsonConvert.DeserializeObject<Test>(sr.ReadToEnd());
                    foreach (LogicOp logicTest in test.logicTests)
                    {
                        BigNumber a = new BigNumber(logicTest.a.value, logicTest.a.rank);
                        BigNumber b = new BigNumber(logicTest.b.value, logicTest.b.rank);
                        TestCaseData data = new TestCaseData(logicTest)
                            .SetName($"Logic Operation Test: {a}[value: {a.Value}, rank: {a.Rank}] {logicTest.condition} {b}[value: {b.Value}, rank: {b.Rank}] = {logicTest.logicResult}")
                            .SetCategory("Logic tests from Json");
                        yield return data;
                    }

                }
            }
        }
    }
}