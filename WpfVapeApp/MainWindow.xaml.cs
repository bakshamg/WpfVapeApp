using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.Xml;
using System.Windows.Controls;
using Microsoft.Win32;

namespace WpfVapeApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Variables

        private string _sourcePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Vape.csv";
        private List<VapeModel> _vapeModelList = new List<VapeModel>();
        public string _exportPath { get; set; }

        #endregion

        #region Public Properties

        public List<VapeModel> VapeModelList
        {
            get
            {
                return _vapeModelList;
            }
            set
            {
                _vapeModelList = value;
            }
        }

        #endregion

        #region Public Methods

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                CreateCSVFileWithData();
                LoadFromCSV();
                VapeGrid.ItemsSource = VapeModelList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Export failed with error: " + ex.ToString(), "Error");
            }
        }

        public void Export(object sender, RoutedEventArgs e)
        {
            try
            {
                Save();
                CreateXMLFile();

                if (String.IsNullOrEmpty(_exportPath))
                {
                    return;
                }

                UpdateRowToProcessed();
                VapeGrid.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Export failed with error: " + ex.ToString(), "Error");
            }
        }

        public void Save(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var VapeModel in VapeModelList)
                {
                    bool deviceExists = false;
                    using (StreamReader reader = new StreamReader(_sourcePath))
                    {
                        String line;
                        while ((line = reader.ReadLine()) != null)
                        {

                            if (line.Contains(","))
                            {
                                String[] row = line.Split(',');
                                if ((VapeModel.DeviceName == row[0]) & (VapeModel.Model == row[1]))
                                {
                                    deviceExists = true;
                                }
                            }
                        }
                    }
                    if (!deviceExists)
                    {
                        StringBuilder _vapeRows = new StringBuilder();
                        _vapeRows.AppendLine(VapeModel.DeviceName + "," + VapeModel.Model + "," + VapeModel.TankCapacity + "," + VapeModel.Processed);
                        File.AppendAllText(_sourcePath, _vapeRows.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("save failed with error: " + ex.ToString(), "Error");
            }

            VapeGrid.Items.Refresh();
        }

        #endregion

        #region Private Methods

        private void CreateXMLFile()
        {
            //Guid id = Guid.NewGuid();
            //string _exportPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + id + @".xml";
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "XML-File | *.xml";
                if (saveFileDialog.ShowDialog() == true)
                {

                    XmlTextWriter writer = new XmlTextWriter(saveFileDialog.FileName, System.Text.Encoding.UTF8);
                    writer.WriteStartDocument(true);
                    writer.Formatting = Formatting.Indented;
                    writer.Indentation = 2;
                    writer.WriteStartElement("Table");

                    foreach (var VapeModel in VapeModelList.Where(s => s.Processed == false))
                    {
                        CreateNode(VapeModel.DeviceName.ToString(), VapeModel.Model.ToString(), VapeModel.TankCapacity.ToString(), writer);
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Close();
                    _exportPath = saveFileDialog.FileName;
                }
                else
                    _exportPath = saveFileDialog.FileName;
                //MessageBox.Show("XML file with unprocessed rows created here: " + _exportPath, "XML File Created");
            }
            catch (Exception ex)
            {
                MessageBox.Show("CreateXMLFile failed with error: " + ex.ToString(), "Error");
            }
        }

        private void CreateNode(string DeviceName, string Model, string TankCapacity, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("Row");
                writer.WriteStartElement("DeviceName");
                writer.WriteString(DeviceName);
                writer.WriteEndElement();
                writer.WriteStartElement("Model");
                writer.WriteString(Model);
                writer.WriteEndElement();
                writer.WriteStartElement("TankCapacity");
                writer.WriteString(TankCapacity);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            catch (Exception ex)
            {
                MessageBox.Show("UpdateRowToProcessed failed with error: " + ex.ToString(), "Error");
            }
        }

        //using a stored procedure on a database would be easier but for this purpose I have a list of all the objects I just recreate the CSV file.
        //Meeting objective
        private void UpdateRowToProcessed()
        {
            try
            {
                if (File.Exists(_sourcePath))
                {
                    File.Delete(_sourcePath);
                }

                StringBuilder _vapeRows = new StringBuilder();
                _vapeRows.AppendLine("DeviceName,Model,TankCapacity,Processed");

                foreach (var VapeModel in VapeModelList)
                {
                    VapeModel.Processed = true;
                    _vapeRows.AppendLine(VapeModel.DeviceName + "," + VapeModel.Model + "," + VapeModel.TankCapacity + "," + true);
                }
                File.AppendAllText(_sourcePath, _vapeRows.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("UpdateRowToProcessed failed with error: " + ex.ToString(), "Error");
            }

        }

        private void Save()
        {
            try
            {
                foreach (var VapeModel in VapeModelList)
                {
                    bool deviceExists = false;
                    using (StreamReader reader = new StreamReader(_sourcePath))
                    {
                        String line;
                        while ((line = reader.ReadLine()) != null)
                        {

                            if (line.Contains(","))
                            {
                                String[] row = line.Split(',');
                                if ((VapeModel.DeviceName == row[0]) & (VapeModel.Model == row[1]))
                                {
                                    deviceExists = true;
                                }
                            }
                        }
                    }
                    if (!deviceExists)
                    {
                        StringBuilder _vapeRows = new StringBuilder();
                        _vapeRows.AppendLine(VapeModel.DeviceName + "," + VapeModel.Model + "," + VapeModel.TankCapacity + "," + VapeModel.Processed);
                        File.AppendAllText(_sourcePath, _vapeRows.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("save failed with error: " + ex.ToString(), "Error");
            }

            VapeGrid.Items.Refresh();
        }

        public void CreateCSVFileWithData()
        {
            try
            {
                if (File.Exists(_sourcePath))
                {
                    return;
                }

                StringBuilder _vapeRows = new StringBuilder();
                _vapeRows.AppendLine("DeviceName,Model,TankCapacity,Processed");
                _vapeRows.AppendLine("Kangertech,EMOW,5,false");
                _vapeRows.AppendLine("Eleaf,IJUSTS,5,false");
                File.AppendAllText(_sourcePath, _vapeRows.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("createCSVFileWithData failed with error: " + ex.ToString(), "Error");
            }
        }

        private void LoadFromCSV()
        {
            VapeModelList = File.ReadAllLines(_sourcePath)
                   .Skip(1)
                   .Select(v => VapeModel.LoadData(v))
                   .ToList();
        }
        #endregion
    }
}
