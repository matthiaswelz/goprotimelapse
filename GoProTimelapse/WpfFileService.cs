﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace journeyofcode.GoProTimelapse
{
    public sealed class WpfFileService
        : IFileService
    {
        public string SaveFile(string extension = null)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Files|*." + (extension ?? "*"),
                AddExtension = extension != null
            };

            if (!dialog.ShowDialog().GetValueOrDefault(false))
                return null;

            return dialog.FileName;
        }

        public IEnumerable<string> SelectFiles(string extension = null)
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Files|*." + (extension ?? "*"),
                AddExtension = extension != null
            };

            if (!dialog.ShowDialog().GetValueOrDefault(false))
                return null;

            return dialog.FileNames;
        }

        public string SelectDirectory(string description = null)
        {
            var dialog = new FolderBrowserDialog
            {
                ShowNewFolderButton = false, 
                Description = description
            };

            if (dialog.ShowDialog() == DialogResult.OK)
                return dialog.SelectedPath;

            return null;
        }
    }
}