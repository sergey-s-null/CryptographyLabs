﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Autofac.Features.Indexed;
using CryptographyLabs.GUI.AbstractViewModels;
using Module.RSA.Enums;
using Module.RSA.Exceptions;
using Module.RSA.Services.Abstract;
using PropertyChanged;

namespace CryptographyLabs.GUI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class RSAAttackVM : IRSAAttackVM
{
    public IReadOnlyCollection<IRSAAttackTypeVM> AttackTypes { get; }
    public IRSAAttackTypeVM SelectedAttackType { get; set; }

    public IRSAAttackParametersVM Parameters { get; }
    public IRSAAttackResultsVM Results { get; }

    public bool IsInProgress { get; private set; }

    public ICommand Attack => _attack ??= new AsyncRelayCommand(_ => Attack_Internal());
    public ICommand Cancel => _cancel ??= new RelayCommand(_ => Cancel_Internal());

    private ICommand? _attack;
    private ICommand? _cancel;

    private readonly IIndex<RSAAttackType, IRSAAttackService> _rsaAttackServices;

    private CancellationTokenSource? _tokenSource;

    public RSAAttackVM(
        IRSAAttackParametersVM parameters,
        IRSAAttackResultsVM results,
        IIndex<RSAAttackType, IRSAAttackService> rsaAttackServices)
    {
        Parameters = parameters;
        Results = results;
        _rsaAttackServices = rsaAttackServices;

        AttackTypes = new[]
        {
            new RSAAttackTypeVM(RSAAttackType.Factorization, "Factorization attack"),
            new RSAAttackTypeVM(RSAAttackType.Wiener, "Wiener attack")
        };
        SelectedAttackType = AttackTypes.First();
    }

    private async Task Attack_Internal()
    {
        if (IsInProgress)
        {
            return;
        }

        if (Parameters.HasErrors)
        {
            MessageBox.Show("Parameters has invalid state.");
            return;
        }

        IsInProgress = true;
        Results.PrivateExponent = 0;

        _tokenSource = new CancellationTokenSource();
        try
        {
            var attackService = _rsaAttackServices[SelectedAttackType.Type];
            var privateExponent = await attackService.AttackAsync(
                Parameters.PublicExponent!.Value,
                Parameters.Modulus!.Value,
                _tokenSource.Token
            );

            Results.PrivateExponent = privateExponent;
        }
        catch (OperationCanceledException)
        {
        }
        catch (CryptographyAttackException e)
        {
            MessageBox.Show($"Attack error:\n\n{e}");
        }
        finally
        {
            _tokenSource.Dispose();
            _tokenSource = null;
            IsInProgress = false;
        }
    }

    private void Cancel_Internal()
    {
        _tokenSource?.Cancel();
    }
}