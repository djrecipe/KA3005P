using System;
using System.IO;

namespace KA3005P.UI
{
    partial class Settings
    {
        public static string FilePath
        {
            get
            {
                string my_documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string path = Path.Combine(my_documents, "KA3005P", "UI.settings");
                return path;
            }
        }

        private void InitializeRows()
        {
            GeneralRow general_row = this.General.NewGeneralRow();
            general_row.VoltageFilePath = null;
            this.General.AddGeneralRow(general_row);
            return;
        }
        public void Load()
        {
            if (File.Exists(Settings.FilePath))
                this.ReadXml(Settings.FilePath);
            return;
        }
        public void Save()
        {
            string directory = Path.GetDirectoryName(Settings.FilePath);
            Directory.CreateDirectory(directory);
            this.WriteXml(Settings.FilePath);
            return;
        }
    }
}
