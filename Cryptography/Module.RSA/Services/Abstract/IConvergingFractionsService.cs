﻿using System.Numerics;
using Module.RSA.Entities;

namespace Module.RSA.Services.Abstract;

public interface IConvergingFractionsService
{
    /// <summary>
    /// По непрерывной дроби вычисляет подходящие дроби.
    /// </summary>
    /// <exception cref="ArgumentException">Непрерывная дробь содержит отрицательный элемент.</exception>
    IEnumerable<ConvergingFraction> EnumerateConvergingFractions(IEnumerable<BigInteger> continuedFraction);
}