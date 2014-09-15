﻿using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Windows.Forms;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace GoProTimelapse
{
    public sealed class WpfFileService
        : IFileService
    {
        public string SaveFile(string extension = null)
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "Files|*." + (extension ?? "*");
            dialog.AddExtension = extension != null;

            if (!dialog.ShowDialog().GetValueOrDefault(false))
                return null;

            return dialog.FileName;
        }

        public string SelectFile(string extension = null)
        {
            var dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = "Files|*." + (extension ?? "*");
            dialog.AddExtension = extension != null;

            if (!dialog.ShowDialog().GetValueOrDefault(false))
                return null;

            return dialog.FileName;
        }

        public IEnumerable<string> SelectFiles(string extension = null)
        {
            var dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Filter = "Files|*." + (extension ?? "*");
            dialog.AddExtension = extension != null;

            if (!dialog.ShowDialog().GetValueOrDefault(false))
                return null;

            return dialog.FileNames;
        }

        public string SelectDirectory(string description = null)
        {
            var dialog = new FolderBrowserDialog();
            dialog.ShowNewFolderButton = false;
            dialog.Description = description;

            if (dialog.ShowDialog() == DialogResult.OK)
                return dialog.SelectedPath;

            return null;
        }
    }
}