﻿using Util.RSA.WienerAttackTest.Entities.Abstract;

namespace Util.RSA.WienerAttackTest.Services.Abstract;

public interface IWienerAttackTestService
{
    Task<IWienerAttackComplexTestResult> PerformComplexTestAsync();
}