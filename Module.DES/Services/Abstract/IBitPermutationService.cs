﻿namespace Module.DES.Services.Abstract;

public interface IBitPermutationService
{
    /// <exception cref="ArgumentException">Invalid masks count.</exception>
    ulong Permute(ulong value, IReadOnlyList<ulong> masks);
}