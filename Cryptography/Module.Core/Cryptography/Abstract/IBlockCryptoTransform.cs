﻿namespace Module.Core.Cryptography.Abstract;

public interface IBlockCryptoTransform
{
    int InputBlockSize { get; }
    int OutputBlockSize { get; }

    /// <exception cref="ArgumentException">Invalid size of input or output.</exception>
    void Transform(ReadOnlySpan<byte> input, Span<byte> output);
}