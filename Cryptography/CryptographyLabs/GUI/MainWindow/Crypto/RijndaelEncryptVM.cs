﻿using System.Windows;
using CryptographyLabs.Crypto;
using CryptographyLabs.Helpers;
using Microsoft.WindowsAPICodePack.Dialogs;
using Module.Core.Enums;
using Module.Core.Factories.Abstract;
using Module.Rijndael.Entities;
using Module.Rijndael.Entities.Abstract;

namespace CryptographyLabs.GUI
{
    class RijndaelEncryptVM : RijndaelVM
    {
        private readonly MainWindowVM _owner;
        private readonly ICryptoTransformFactory<IRijndaelParameters> _rijndaelCryptoTransformFactory;

        public RijndaelEncryptVM(
            MainWindowVM owner,
            ICryptoTransformFactory<IRijndaelParameters> rijndaelCryptoTransformFactory)
        {
            _owner = owner;
            _rijndaelCryptoTransformFactory = rijndaelCryptoTransformFactory;
        }

        protected override void ChangeFilePath()
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.Filters.Add(new CommonFileDialogFilter("Any file", "*"));
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    FilePath = dialog.FileName;
            }
        }

        protected override void Go()
        {
            if (!TryGetKeyBytes(out var keyBytes))
            {
                return;
            }

            var targetFilePath = FilePath + ".rjn399";

            if (Mode == Rijndael_.Mode.ECB)
            {
                StartEcbTransform(targetFilePath, keyBytes);
            }
            else
            {
                if (!TryGetInitialVector(out var initialVector))
                {
                    return;
                }

                StartTransform(targetFilePath, keyBytes, initialVector);
            }
        }

        private bool TryGetKeyBytes(out byte[] keyBytes)
        {
            if (!StringEx.TryParse(Key, out keyBytes))
            {
                MessageBox.Show("Wrong key format.");
                return false;
            }

            if (keyBytes.Length != Rijndael_.GetBytesCount(KeySize))
            {
                MessageBox.Show("Wrong bytes count in key.");
                return false;
            }

            return true;
        }

        private bool TryGetInitialVector(out byte[] initialVector)
        {
            if (!StringEx.TryParse(IV, out initialVector))
            {
                MessageBox.Show("Wrong IV format.");
                return false;
            }

            if (initialVector.Length != Rijndael_.GetBytesCount(BlockSize))
            {
                MessageBox.Show($"Wrong IV bytes count. Must be {Rijndael_.GetBytesCount(BlockSize)}.");
                return false;
            }

            return true;
        }

        private void StartEcbTransform(string targetFilePath, byte[] keyBytes)
        {
            var transformVM = CreateTransformVM(targetFilePath);

            var cryptoTransform = _rijndaelCryptoTransformFactory.CreateEcb(
                TransformDirection.Encrypt,
                new RijndaelParameters(keyBytes, LegacyCodeHelper.Fix(BlockSize)),
                Multithread
            );

            transformVM.Start(cryptoTransform);

            _owner.ProgressViewModels.Add(transformVM);
        }

        private void StartTransform(string targetFilePath, byte[] keyBytes, byte[] initialVector)
        {
            var transformVM = CreateTransformVM(targetFilePath);

            var cryptoTransform = _rijndaelCryptoTransformFactory.Create(
                TransformDirection.Encrypt,
                new RijndaelParameters(keyBytes, LegacyCodeHelper.Fix(BlockSize)),
                LegacyCodeHelper.Fix(Mode),
                initialVector
            );

            transformVM.Start(cryptoTransform);

            _owner.ProgressViewModels.Add(transformVM);
        }

        private TransformVM CreateTransformVM(string targetFilePath)
        {
            return new TransformVM(IsDeleteAfter, CryptoDirection.Encrypt)
            {
                CryptoName = "Rijndael",
                SourceFilePath = FilePath,
                DestFilePath = targetFilePath
            };
        }
    }
}