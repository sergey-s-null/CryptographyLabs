﻿using Autofac;
using Module.RSA.Exceptions;
using Module.RSA.Services;
using Module.RSA.Services.Abstract;
using Module.RSA.UnitTests.Modules;
using NUnit.Framework;

namespace Module.RSA.UnitTests.Tests;

[TestFixture(typeof(FactorizationAttackService))]
[TestFixture(typeof(WienerAttackService))]
public class RSAAttackServiceCommonTests<T> where T : IRSAAttackService
{
    private readonly IContainer _container;

    private IRSAAttackService? _rsaAttackService;

    public RSAAttackServiceCommonTests()
    {
        _container = BuildContainer();
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _rsaAttackService = _container.Resolve<T>();
    }

    [Test]
    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(-2)]
    [TestCase(-928734)]
    public void Attack_InvalidPublicExponentTest(int publicExponent)
    {
        Assert.ThrowsAsync<ArgumentException>(
            () => _rsaAttackService!.AttackAsync(publicExponent, 2118100319)
        );
    }

    [Test]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(-1)]
    [TestCase(-2)]
    [TestCase(-3295634)]
    public void Attack_InvalidModulusTest(int modulus)
    {
        Assert.ThrowsAsync<ArgumentException>(
            () => _rsaAttackService!.AttackAsync(65537, modulus)
        );
    }

    [Test]
    [TestCase(4937)]
    [TestCase(933931)]
    [TestCase(66476491)]
    public void Attack_PrimeTest(int modulus)
    {
        Assert.ThrowsAsync<CryptographyAttackException>(
            async () => await _rsaAttackService!.AttackAsync(65537, modulus)
        );
    }

    private static IContainer BuildContainer()
    {
        var builder = new ContainerBuilder();

        builder.RegisterModule<FactorizationAttackModule>();
        builder.RegisterModule<WienerAttackModule>();

        return builder.Build();
    }
}