using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Windows;

namespace Excel2DB.Models
{
    /// <summary>
    /// Handles database connection and cleaning
    /// </summary>
    class DataConnecter
    {
        /// <summary>
        /// esenbles a valid conection string to be passed to context.dataContext
        /// </summary>
        /// <returns>The connection string</returns>
        public static string EstablishValidConnection()
        {
            if (Properties.Settings.Default.Connection != "")
            {
                return Properties.Settings.Default.Connection;
            }

            //trying computer name
            string CompName = System.Environment.MachineName;
            string conectstr = @"Data Source=" + CompName + @"\SQLEXPRESS;Initial Catalog=ModelDB;Integrated Security=True;Persist Security Info=True";
            string serversection = @"Data Source=" + CompName + @"\SQLEXPRESS;Integrated Security=True;Persist Security Info=True";
            Context.DataContext connection = new Context.DataContext(serversection);
            try
            {
                connection.Connection.Open();
            }
            catch (Exception e)
            {
                //not normal name
                bool accept = false;
                string prompt = "Eer SQL server Name: ";
                //error not default.  need to repeativle prompt
                while (!accept)
                {
                    string server = Interaction.InputBox(prompt);
                    conectstr = @"Data Source=" + server + @";Initial Catalog=ModelDB;Integrated Security=True;Persist Security Info=True";
                    connection = new Context.DataContext(conectstr);
                    try
                    {
                        connection.Connection.Open();
                        accept = true;
                    }
                    catch (Exception ex)
                    {
                        prompt = "Connectionn failed.  Enter SQL server Name: ";
                    }
                }
            }

            //save database info
            Properties.Settings.Default.Connection = conectstr;
            Properties.Settings.Default.Save();
            return conectstr;
        }

        /// <summary>
        /// wipe out all data from database
        /// </summary>
        public static void ClearData()
        {
            Context.DataContext dbContext = new Context.DataContext(EstablishValidConnection());

            //delte data and reset ids
            dbContext.ExecuteCommand("TRUNCATE TABLE Relateds");
            dbContext.ExecuteCommand("DBCC CHECKIDENT ('[ModelDb].[dbo].[Relateds]', RESEED, 0)");

            dbContext.ExecuteCommand("DELETE FROM BaselineSecurityMappings");
            dbContext.ExecuteCommand("DBCC CHECKIDENT ('[ModelDb].[dbo].[BaselineSecurityMappings]', RESEED, 0)");

            dbContext.ExecuteCommand("TRUNCATE TABLE MapTypesCapabilitiesControls");
            dbContext.ExecuteCommand("DBCC CHECKIDENT ('[ModelDb].[dbo].[MapTypesCapabilitiesControls]', RESEED, 0)");

            dbContext.ExecuteCommand("DELETE FROM TICMappings");
            dbContext.ExecuteCommand("DBCC CHECKIDENT ('[ModelDb].[dbo].[TICMappings]', RESEED, 0)");

            dbContext.ExecuteCommand("DELETE FROM Specs");
            dbContext.ExecuteCommand("DBCC CHECKIDENT ('[ModelDb].[dbo].[Specs]', RESEED, 0)");

            dbContext.ExecuteCommand("DELETE FROM Controls");
            dbContext.ExecuteCommand("DBCC CHECKIDENT ('[ModelDb].[dbo].[Controls]', RESEED, 0)");

            dbContext.ExecuteCommand("DELETE FROM Capabilities");
            dbContext.ExecuteCommand("DBCC CHECKIDENT ('[ModelDb].[dbo].[Capabilities]', RESEED, 0)");
        }

        /// <summary>
        /// clear capablities data 
        /// </summary>
        public static void wipeCape()
        {
            Context.DataContext dbContext = new Context.DataContext(EstablishValidConnection());

            dbContext.ExecuteCommand("DELETE FROM MapTypesCapabilitiesControls");
            dbContext.ExecuteCommand("DBCC CHECKIDENT ('[ModelDb].[dbo].[MapTypesCapabilitiesControls]', RESEED, 0)");

            dbContext.ExecuteCommand("DELETE FROM TICMappings");
            dbContext.ExecuteCommand("DBCC CHECKIDENT ('[ModelDb].[dbo].[TICMappings]', RESEED, 0)");

            dbContext.ExecuteCommand("DELETE FROM Capabilities");
            dbContext.ExecuteCommand("DBCC CHECKIDENT ('[ModelDb].[dbo].[Capabilities]', RESEED, 0)");
        }

        public static void wipeBaselines()
        {
            Context.DataContext dbContext = new Context.DataContext(EstablishValidConnection());

            dbContext.ExecuteCommand("DELETE FROM BaselineSecurityMappings");
            dbContext.ExecuteCommand("DBCC CHECKIDENT ('[ModelDb].[dbo].[BaselineSecurityMappings]', RESEED, 0)");
        }

        private static void InitData()
        {
            Context.DataContext dbContext = new Context.DataContext(EstablishValidConnection());
            string initScript = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InitData.sql");
            string[] script = File.ReadAllLines(initScript);
            foreach (string command in script)
            {
                try
                {
                    dbContext.ExecuteCommand(command);
                }
                catch (Exception ex)
                {
                    string s = ex.Message;
                }
            }
        }

        /// <summary>
        /// setup a new database with all tables
        /// </summary>
        public static void FirstUse()
        {
            Context.DataContext dbContext = new Context.DataContext(EstablishValidConnection());
            if (!dbContext.DatabaseExists())
                dbContext.CreateDatabase();
            else
            {
                ClearData();
            }
            InitData();
        }
    }
}