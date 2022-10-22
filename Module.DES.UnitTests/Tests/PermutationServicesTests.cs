﻿using Module.DES.Services;
using Module.DES.Services.Abstract;
using Module.DES.UnitTests.Helpers;
using NUnit.Framework;

namespace Module.DES.UnitTests.Tests;

[TestFixture]
public class PermutationServicesTests
{
    [Test]
    [TestCaseSource(nameof(GetTestCases32))]
    public void Permute32_Test(
        IUInt32BitPermutationService permutationService,
        IReadOnlyList<byte> permutationTable)
    {
        for (var i = 0; i < permutationTable.Count; i++)
        {
            var value = 1u << i;
            var expected = PermutationHelper.PermuteWithTable(value, permutationTable);
            var actual = permutationService.Permute(value);
            Assert.AreEqual(expected, actual);
        }
    }

    [Test]
    [TestCaseSource(nameof(GetTestCases64))]
    public void Permute64_Test(
        IUInt64BitPermutationService permutationService,
        IReadOnlyList<byte> permutationTable)
    {
        for (var i = 0; i < permutationTable.Count; i++)
        {
            var value = 1ul << i;
            var expected = PermutationHelper.PermuteWithTable(value, permutationTable);
            var actual = permutationService.Permute(value);
            Assert.AreEqual(expected, actual);
        }
    }

    private static IEnumerable<object[]> GetTestCases32()
    {
        var feistelFunctionPermutationTable = new byte[]
        {
            15, 6, 19, 20, 28, 11, 27, 16,
            0, 14, 22, 25, 4, 17, 30, 9,
            1, 7, 23, 13, 31, 26, 2, 8,
            18, 12, 29, 5, 21, 10, 3, 24,
        };
        yield return new object[]
        {
            new FeistelFunctionPermutationService(new BitPermutationService()),
            feistelFunctionPermutationTable
        };
    }

    private static IEnumerable<object[]> GetTestCases64()
    {
        var bitPermutationService = new BitPermutationService();

        var initialPermutationTable = new byte[]
        {
            57, 49, 41, 33, 25, 17, 9, 1, 59, 51, 43, 35, 27, 19, 11, 3,
            61, 53, 45, 37, 29, 21, 13, 5, 63, 55, 47, 39, 31, 23, 15, 7,
            56, 48, 40, 32, 24, 16, 8, 0, 58, 50, 42, 34, 26, 18, 10, 2,
            60, 52, 44, 36, 28, 20, 12, 4, 62, 54, 46, 38, 30, 22, 14, 6
        };
        yield return new object[]
        {
            new DesInitialPermutationService(bitPermutationService),
            initialPermutationTable
        };

        var finalPermutationTable = new byte[]
        {
            39, 7, 47, 15, 55, 23, 63, 31, 38, 6, 46, 14, 54, 22, 62, 30,
            37, 5, 45, 13, 53, 21, 61, 29, 36, 4, 44, 12, 52, 20, 60, 28,
            35, 3, 43, 11, 51, 19, 59, 27, 34, 2, 42, 10, 50, 18, 58, 26,
            33, 1, 41, 9, 49, 17, 57, 25, 32, 0, 40, 8, 48, 16, 56, 24,
        };
        yield return new object[]
        {
            new DesFinalPermutationService(bitPermutationService),
            finalPermutationTable
        };
    }
}