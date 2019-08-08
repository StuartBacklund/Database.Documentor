using System;
using System.ComponentModel;
using System.IO;

namespace Database.Documentor.Settings
{
    /// <summary>Properties for the Property Grid Control</summary>
    public class DbDocSettings
    {
        private string outputFolder;
        private string dataBaseDisplayName;
        private string DataDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

        /// <summary>Project output folder</summary>
        /// <value>String containing output folder path.</value>
        [Browsable(true)]
        [Category("Output Settings")]
        [Description("Project Destination Folder.")]
        public string OutputFolder
        {
            get
            {
                return outputFolder;
            }
            set
            {
                outputFolder = value;
            }
        }

        /// <summary>Database name to display</summary>
        /// <value>String used to represent the name of the database in output files.</value>
        [Browsable(true)]
        [Category("Output Settings")]
        [Description("Database Name to display in output files.")]
        public string DatabaseName
        {
            get
            {
                return dataBaseDisplayName;
            }
            set
            {
                dataBaseDisplayName = value;
            }
        }

        private string cssStyleSheet;
        private string tableImage;
        private string viewImage;
        private string procedureImage;
        private string columnImage;
        private string databaseImage;
        private string indexImage;
        private string pKColumnImage;
        private string parameterImage;

        /// <summary>Style Sheet</summary>
        /// <value>String containing the style sheet path.</value>
        [Browsable(true)]
        [Category("Customize HTML Appearance")]
        [Description("The style sheet to be used by the output HTML and CHM files.")]
        public string CssStyleSheet
        {
            get
            {
                return cssStyleSheet;
            }
            set
            {
                cssStyleSheet = value;
            }
        }

        /// <summary>Table Image</summary>
        /// <value>String containing the path to the Table image file.</value>
        [Browsable(true)]
        [Category("Customize HTML Appearance")]
        [Description("Image used to represent a Table.  Use 16x16 Gif for best results.")]
        public string TableImage
        {
            get
            {
                return tableImage;
            }
            set
            {
                tableImage = value;
            }
        }

        /// <summary>View Image</summary>
        /// <value>String containing the path to the View image file.</value>
        [Browsable(true)]
        [Category("Customize HTML Appearance")]
        [Description("Image used to represent a View.  Use 16x16 Gif for best results.")]
        public string ViewImage
        {
            get
            {
                return viewImage;
            }
            set
            {
                viewImage = value;
            }
        }

        /// <summary>Stored Procedure Image</summary>
        /// <value>String containing the path to the Stored Procedure image file.</value>
        [Browsable(true)]
        [Category("Customize HTML Appearance")]
        [Description("Image used to represent a Stored Procedure.  Use 16x16 Gif for best results.")]
        public string ProcedureImage
        {
            get
            {
                return procedureImage;
            }
            set
            {
                procedureImage = value;
            }
        }

        /// <summary>Column Image</summary>
        /// <value>String containing the path to the Column image file.</value>
        [Browsable(true)]
        [Category("Customize HTML Appearance")]
        [Description("Image used to represent a Column.  Use 16x16 Gif for best results.")]
        public string ColumnImage
        {
            get
            {
                return columnImage;
            }
            set
            {
                columnImage = value;
            }
        }

        /// <summary>Database Image</summary>
        /// <value>String containing the path to the Database image file.</value>
        [Browsable(true)]
        [Category("Customize HTML Appearance")]
        [Description("Image used to represent a Database.  Use 16x16 Gif for best results.")]
        public string DatabaseImage
        {
            get
            {
                return databaseImage;
            }
            set
            {
                databaseImage = value;
            }
        }

        /// <summary>Index Image</summary>
        /// <value>String containing the path to the Index image file.</value>
        [Browsable(true)]
        [Category("Customize HTML Appearance")]
        [Description("Image used to represent an Index.  Use 16x16 Gif for best results.")]
        public string IndexImage
        {
            get
            {
                return indexImage;
            }
            set
            {
                indexImage = value;
            }
        }

        /// <summary>Primary Key Column Image</summary>
        /// <value>String containing the path to the Primary Key Column image file.</value>
        [Browsable(true)]
        [Category("Customize HTML Appearance")]
        [Description("Image used to represent a Primary Key Column.  Use 16x16 Gif for best results.")]
        public string PKColumnImage
        {
            get
            {
                return pKColumnImage;
            }
            set
            {
                pKColumnImage = value;
            }
        }

        /// <summary>Parameter Image</summary>
        /// <value>String containing the path to the Parameter image file.</value>
        [Browsable(true)]
        [Category("Customize HTML Appearance")]
        [Description("Image used to represent a Parameter.  Use 16x16 Gif for best results.")]
        public string ParameterImage
        {
            get
            {
                return parameterImage;
            }
            set
            {
                parameterImage = value;
            }
        }

        private string treeViewStyleSheet;
        private string treeViewJavascript;
        private string treeNodeDotImage;
        private string treeNodeOpenImage;
        private string treeNodeClosedImage;

        /// <summary>MSDN TreeView Style Sheet</summary>
        /// <value>String containing the Tree View style sheet path.</value>
        [Browsable(true)]
        [Category("Customize Tree View")]
        [Description("The style sheet to be used by the output TreeView Index HTML file.")]
        public string TreeViewStyleSheet
        {
            get
            {
                return treeViewStyleSheet;
            }
            set
            {
                treeViewStyleSheet = value;
            }
        }

        /// <summary>MSDN TreeView Javascript File</summary>
        /// <value>String containing the Tree View javascript path.</value>
        [Browsable(true)]
        [Category("Customize Tree View")]
        [Description("The Javascript to be used by the output TreeView Index HTML file.")]
        public string TreeViewJavascript
        {
            get
            {
                return treeViewJavascript;
            }
            set
            {
                treeViewJavascript = value;
            }
        }

        /// <summary>Table Image</summary>
        /// <value>String containing the path to the Tree Node Dot Image file.</value>
        [Browsable(true)]
        [Category("Customize Tree View")]
        [Description("Image used to represent am item in a tree node.  Use 9x9 Gif for best results.")]
        public string TreeNodeDotImage
        {
            get
            {
                return treeNodeDotImage;
            }
            set
            {
                treeNodeDotImage = value;
            }
        }

        /// <summary>Table Image</summary>
        /// <value>String containing the path to the TreeNode Opened Image file.</value>
        [Browsable(true)]
        [Category("Customize Tree View")]
        [Description("Image used to represent am opened item in a tree.  Use 9x9 Gif for best results.")]
        public string TreeNodeOpenImage
        {
            get
            {
                return treeNodeOpenImage;
            }
            set
            {
                treeNodeOpenImage = value;
            }
        }

        /// <summary>Table Image</summary>
        /// <value>String containing the path to the TreeNode Closed Image file.</value>
        [Browsable(true)]
        [Category("Customize Tree View")]
        [Description("Image used to represent am closed item in a tree.  Use 9x9 Gif for best results.")]
        public string TreeNodeClosedImage
        {
            get
            {
                return treeNodeClosedImage;
            }
            set
            {
                treeNodeClosedImage = value;
            }
        }

        public DbDocSettings()
        {
        }

        public void LoadDefaults()
        {
            OutputFolder = Path.Combine(DataDirectory, "Output");

            if (File.Exists(Path.Combine(DataDirectory, "Resources", "msdn.css")))
            {
                CssStyleSheet = Path.Combine(DataDirectory, "Resources", "msdn.css");
            }

            if (File.Exists(Path.Combine(DataDirectory, "Resources", "tree.css")))
            {
                TreeViewStyleSheet = Path.Combine(DataDirectory, "Resources", "tree.css");
            }

            if (File.Exists(Path.Combine(DataDirectory, "Resources", "tree.js")))
            {
                TreeViewJavascript = Path.Combine(DataDirectory, "Resources", "tree.js");
            }

            if (File.Exists(Path.Combine(DataDirectory, "Images", "treenodedot.gif")))
                TreeNodeDotImage = Path.Combine(DataDirectory, "Images", "treenodedot.gif");

            if (File.Exists(Path.Combine(DataDirectory, "Images", "treenodeminus.gif")))
                TreeNodeOpenImage = Path.Combine(DataDirectory, "Images", "treenodeminus.gif");

            if (File.Exists(Path.Combine(DataDirectory, "Images", "treenodeplus.gif")))
                TreeNodeClosedImage = Path.Combine(DataDirectory, "Images", "treenodeplus.gif");

            // Defaults for HTML Style sheet and images

            if (File.Exists(Path.Combine(DataDirectory, "Images", "Column.gif")))
                ColumnImage = Path.Combine(DataDirectory, "Images", "Column.gif");

            if (File.Exists(Path.Combine(DataDirectory, "Images", "diskdrive.png")))
                DatabaseImage = Path.Combine(DataDirectory, "Images", "diskdrive.png");

            if (File.Exists(Path.Combine(DataDirectory, "Images", "Index.gif")))
                IndexImage = Path.Combine(DataDirectory, "Images", "Index.gif");

            if (File.Exists(Path.Combine(DataDirectory, "Images", "Parameter.gif")))
                ParameterImage = Path.Combine(DataDirectory, "Images", "Parameter.gif");

            if (File.Exists(Path.Combine(DataDirectory, "Images", "PKColumn.gif")))
                PKColumnImage = Path.Combine(DataDirectory, "Images", "PKColumn.gif");

            if (File.Exists(Path.Combine(DataDirectory, "Images", "Procedure.gif")))
                ProcedureImage = Path.Combine(DataDirectory, "Images", "Procedure.gif");

            if (File.Exists(Path.Combine(DataDirectory, "Images", "Table.gif")))
                TableImage = Path.Combine(DataDirectory, "Images", "Table.gif");

            if (File.Exists(Path.Combine(DataDirectory, "Images", "View.gif")))
                ViewImage = Path.Combine(DataDirectory, "Images", "View.gif");
        }

        public bool LoadProject(string filename)
        {
            if (File.Exists(filename))
            {
                XmlSettings x = new XmlSettings(filename);

                this.DatabaseName = x.GetStringSetting("Output", "DatabaseName", "");
                this.OutputFolder = x.GetStringSetting("Output", "OutputFolder", "");

                string[,] AppearanceSettings = x.GetAllSetting("Appearance");
                var loopTo = AppearanceSettings.GetUpperBound(0);
                for (int ct = 0; ct <= loopTo; ct++)
                {
                    if (File.Exists(AppearanceSettings[ct, 1]))
                    {
                        switch (AppearanceSettings[ct, 0])
                        {
                            case "StyleSheet":
                                {
                                    this.CssStyleSheet = AppearanceSettings[ct, 1];
                                    break;
                                }

                            case "TableImage":
                                {
                                    this.TableImage = AppearanceSettings[ct, 1];
                                    break;
                                }

                            case "ViewImage":
                                {
                                    this.ViewImage = AppearanceSettings[ct, 1];
                                    break;
                                }

                            case "ProcedureImage":
                                {
                                    this.ProcedureImage = AppearanceSettings[ct, 1];
                                    break;
                                }

                            case "ColumnImage":
                                {
                                    this.ColumnImage = AppearanceSettings[ct, 1];
                                    break;
                                }

                            case "DatabaseImage":
                                {
                                    this.DatabaseImage = AppearanceSettings[ct, 1];
                                    break;
                                }

                            case "IndexImage":
                                {
                                    this.IndexImage = AppearanceSettings[ct, 1];
                                    break;
                                }

                            case "PKColumnImage":
                                {
                                    this.PKColumnImage = AppearanceSettings[ct, 1];
                                    break;
                                }

                            case "ParameterImage":
                                {
                                    this.ParameterImage = AppearanceSettings[ct, 1];
                                    break;
                                }
                        }
                    }
                }

                string[,] TreeViewSettings = x.GetAllSetting("TreeViewAppearance");
                var loopTo1 = TreeViewSettings.GetUpperBound(0);
                for (int ct = 0; ct <= loopTo1; ct++)
                {
                    if (File.Exists(TreeViewSettings[ct, 1]))
                    {
                        switch (TreeViewSettings[ct, 0])
                        {
                            case "StyleSheet":
                                {
                                    this.TreeViewStyleSheet = TreeViewSettings[ct, 1];
                                    break;
                                }

                            case "Javascript":
                                {
                                    this.TreeViewJavascript = TreeViewSettings[ct, 1];
                                    break;
                                }

                            case "DotImage":
                                {
                                    this.TreeNodeDotImage = TreeViewSettings[ct, 1];
                                    break;
                                }

                            case "OpenImage":
                                {
                                    this.TreeNodeOpenImage = TreeViewSettings[ct, 1];
                                    break;
                                }

                            case "CloseImage":
                                {
                                    this.TreeNodeClosedImage = TreeViewSettings[ct, 1];
                                    break;
                                }
                        }
                    }
                }

                x = null/* TODO Change to default(_) if this is not a reference type */;
                return true;
            }
            else
                return false;
        }

        public bool SaveProject(string filename)
        {
            if (File.Exists(filename))
            {
                XmlSettings x = new XmlSettings(filename);

                x.SaveStringSetting("Output", "DatabaseName", this.DatabaseName);
                x.SaveStringSetting("Output", "OutputFolder", this.OutputFolder);

                // Save Appearance Settings
                x.SaveStringSetting("Appearance", "StyleSheet", this.CssStyleSheet);
                x.SaveStringSetting("Appearance", "TableImage", this.TableImage);
                x.SaveStringSetting("Appearance", "ViewImage", this.ViewImage);
                x.SaveStringSetting("Appearance", "ProcedureImage", this.ProcedureImage);
                x.SaveStringSetting("Appearance", "ColumnImage", this.ColumnImage);
                x.SaveStringSetting("Appearance", "DatabaseImage", this.DatabaseImage);
                x.SaveStringSetting("Appearance", "IndexImage", this.IndexImage);
                x.SaveStringSetting("Appearance", "PKColumnImage", this.PKColumnImage);
                x.SaveStringSetting("Appearance", "ParameterImage", this.ParameterImage);

                // Save Tree View Settings
                x.SaveStringSetting("TreeViewAppearance", "StyleSheet", this.TreeViewStyleSheet);
                x.SaveStringSetting("TreeViewAppearance", "Javascript", this.TreeViewJavascript);
                x.SaveStringSetting("TreeViewAppearance", "DotImage", this.TreeNodeDotImage);
                x.SaveStringSetting("TreeViewAppearance", "OpenImage", this.TreeNodeOpenImage);
                x.SaveStringSetting("TreeViewAppearance", "CloseImage", this.TreeNodeClosedImage);

                x = null/* TODO Change to default(_) if this is not a reference type */;
                return true;
            }
            else
                return false;
        }
    }
}