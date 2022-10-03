﻿using System;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Autofac;
using CryptographyLabs.GUI.AbstractViewModels;
using Module.RSA.Entities;
using Module.RSA.Entities.Abstract;
using Module.RSA.Services.Abstract;
using PropertyChanged;

namespace CryptographyLabs.GUI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class PrimesGenerationVM : IPrimesGenerationVM
{
    public IPrimesGenerationParametersVM Parameters { get; }
    public IPrimesGenerationResultsVM Results { get; }

    public bool IsInProgress { get; private set; }
    public ICommand Generate => _generate ??= new AsyncRelayCommand(_ => GenerateAsync());
    private ICommand? _generate;

    private readonly ILifetimeScope _lifetimeScope;

    public PrimesGenerationVM(
        ILifetimeScope lifetimeScope,
        IPrimesGenerationParametersVM parametersVM,
        IPrimesGenerationResultsVM resultsVM)
    {
        _lifetimeScope = lifetimeScope;
        Parameters = parametersVM;
        Results = resultsVM;
    }

    private async Task GenerateAsync()
    {
        if (IsInProgress)
        {
            return;
        }

        IsInProgress = true;

        try
        {
            await GenerateAsync_Internal();
        }
        finally
        {
            IsInProgress = false;
        }
    }

    private async Task GenerateAsync_Internal()
    {
        var parameters = new PrimesPairGeneratorCombinedParameters(
            new Random(Parameters.Seed),
            Parameters.ByteCount,
            Parameters.ByteCount * 8 - 1,
            100,
            Parameters.Probability
        );

        var registeredParametersLifetimeScope = _lifetimeScope.BeginLifetimeScope(x =>
        {
            x
                .Register(_ => parameters)
                .As<IRandomProvider>()
                .As<IPrimesPairGeneratorParameters>()
                .As<IPrimalityTesterParameters>()
                .SingleInstance();
        });

        var generator = registeredParametersLifetimeScope.Resolve<IPrimesPairGenerator>();
        var (p, q) = await generator.GenerateAsync();

        Results.P = p;
        Results.Q = q;

        SaveToFileIfConfigured(p, q);
    }

    private void SaveToFileIfConfigured(BigInteger p, BigInteger q)
    {
        if (!Parameters.IsSaveToFile)
        {
            return;
        }

        if (!Directory.Exists(Parameters.SaveDirectory))
        {
            MessageBox.Show(
                $"Directory \"{Parameters.SaveDirectory}\" does not exists.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
            return;
        }

        var timeStr = DateTime.Now.ToString("yyyy.MM.dd HH-mm-ss.fff");
        var pFilePath = Path.Combine(Parameters.SaveDirectory, $"{timeStr} p.txt");
        var qFilePath = Path.Combine(Parameters.SaveDirectory, $"{timeStr} q.txt");

        try
        {
            File.WriteAllText(pFilePath, p.ToString());
            File.WriteAllText(qFilePath, q.ToString());
        }
        catch (Exception e)
        {
            MessageBox.Show(
                $"Error save results to files: {e.Message}\n\n" +
                $"{e.StackTrace}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
    }
}