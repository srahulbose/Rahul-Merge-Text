using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Linq;
using System.Windows.Forms;

namespace Rahul_Merge_Text
{
    public partial class Form1 : Form
    {
        private bool cancelOperation;

        public Form1()
        {
            InitializeComponent();
            cancelOperation = false; // Initialize cancel operation flag

            // Initialize the footer label
            InitializeFooter();
            InitializeMenu();
        }

        private void InitializeFooter()
        {
            footerLabel.Click += (s, e) =>
            {
                // Handle the footer click to direct to the website
                Process.Start(new ProcessStartInfo("https://srahulbose.github.io/") { UseShellExecute = true });
            };
        }

        private void MergeButton_Click(object sender, EventArgs e)
        {
            // Open File Dialog to select multiple files
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select Files to Merge";
                openFileDialog.Filter = "All files (*.*)|*.*"; // Allow all files to be selected
                openFileDialog.Multiselect = true; // Allow selection of multiple files

                // Show the dialog and check if selection was made
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Log("User selected " + openFileDialog.FileNames.Length + " files through OpenFileDialog.");
                    // Get the selected files
                    MergeFiles(openFileDialog.FileNames);
                }
            }
        }

        private void MergeFiles(string[] files)
        {
            if (files.Length == 0)
            {
                MessageBox.Show("No files to merge.");
                return;
            }

            // Get the base directory name for the output file
            string baseDirectory = Path.GetDirectoryName(files.First());
            string directoryName = new DirectoryInfo(baseDirectory).Name;
            string outputFileName = Path.Combine(
                baseDirectory,
                $"{directoryName}_{DateTime.Now:yyyyMMdd_HHmmss}_export.txt"
            );

            StringBuilder mergedContent = new StringBuilder();
            int totalFiles = files.Length;

            progressBar.Maximum = totalFiles;
            progressBar.Value = 0;

            foreach (var file in files)
            {
                if (cancelOperation)
                {
                    MessageBox.Show("Operation canceled.");
                    return;
                }

