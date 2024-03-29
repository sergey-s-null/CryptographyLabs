﻿using System.Windows.Input;
using CryptographyLabs.GUI.AbstractViewModels;

namespace CryptographyLabs.GUI.DesignTimeViewModel;

public class PrimesGenerationDTVM : IPrimesGenerationVM
{
    public IPrimesGenerationParametersVM Parameters { get; } = new PrimesGenerationParametersDTVM();
    public IPrimesGenerationResultsVM Results { get; } = new PrimesGenerationResultsDTVM();

    public bool IsInProgress => true;
    public string GenerationTextAnimation => "-----";
    public ICommand Generate { get; } = new RelayCommand(_ => { });
}