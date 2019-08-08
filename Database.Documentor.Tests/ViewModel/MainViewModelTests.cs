using Database.Documentor.Providers;
using Database.Documentor.Settings;
using Database.Documentor.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Database.Documentor.ViewModel.Tests
{
    [TestClass()]
    public class MainViewModelTests
    {
        private string databaseName = @"Northwind";

        [TestMethod()]
        public void MainViewModelTest()
        {
            string directoryName = @"C:\Users\Stuart\source\repos\Database.Documentor\Database.Documentor.Tests\bin\Debug\Output";
            string defaultTopic = @"DB_Test";
            DbDocSettings dbDocSettings = new DbDocSettings()
            {
                DatabaseName = databaseName,
            };
            dbDocSettings.LoadDefaults();

            HtmlFunctions htmlFunctions = new HtmlFunctions(directoryName, dbDocSettings);

            SqlServerSchemaProvider sp = new SqlServerSchemaProvider()
            {
                ConnStringBuilder = new SqlServerConnectionStringBuilder()
                {
                    Database = databaseName,
                    IntegratedSecurity = true,
                    Server = ".",
                }
            };
            SchemaProviderFactory spf = new SchemaProviderFactory();

            BuildPageOutput buildPageOutput = new BuildPageOutput(htmlFunctions, dbDocSettings, spf, sp);
            ApplicationFunctions applicationFunctions = new ApplicationFunctions(buildPageOutput, dbDocSettings, spf, sp, htmlFunctions);
            applicationFunctions.Build();
            Assert.IsNotNull(applicationFunctions);
        }
    }
}