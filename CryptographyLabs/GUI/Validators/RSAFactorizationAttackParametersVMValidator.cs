﻿using CryptographyLabs.GUI.AbstractViewModels;
using FluentValidation;

namespace CryptographyLabs.GUI.Validators;

public class RSAFactorizationAttackParametersVMValidator : AbstractValidator<IRSAFactorizationAttackParametersVM>
{
    public RSAFactorizationAttackParametersVMValidator()
    {
        RuleFor(x => x.PublicExponent)
            .NotNull()
            .GreaterThan(2)
            .OverridePropertyName(nameof(IRSAFactorizationAttackParametersVM.PublicExponentStr));
        RuleFor(x => x.Modulus)
            .NotNull()
            .GreaterThan(2)
            .OverridePropertyName(nameof(IRSAFactorizationAttackParametersVM.ModulusStr));
    }
}