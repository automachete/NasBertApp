using Microsoft.WindowsAPICodePack.Dialogs;
using System.Diagnostics;

namespace NasBertApp.Models
{
    internal class SaveAction
    {
        public static CommonOpenFileDialog GetCommonOpenFolderDialog()
        {
            var folderDialog = new CommonOpenFileDialog()
            {
                Title = "Choice Folder.",
                RestoreDirectory = true,
                IsFolderPicker = true,
            };
            return folderDialog;
        }

        public static CommonOpenFileDialog GetCommonOpenFileDialog()
        {
            var fileDialog = new CommonOpenFileDialog()
            {
                Title = "Choice File.",
                RestoreDirectory = true,
                IsFolderPicker = false,
            };
            return fileDialog;
        }

        public static string GetSavePath(CommonFileDialog Dialog, string oldPath)
        {
            if (Dialog.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return oldPath;
            }
            return Dialog.FileName;
        }

        public static void OpenFolder(string folderPath)
        {
            Process.Start("explorer.exe", folderPath);
        }
    }
}
