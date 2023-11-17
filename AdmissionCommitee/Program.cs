using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdmissionCommitee
{
    enum RowState
    {
        Existed,
        New,
        Modified,
        ModifiedNew,
        Deleted
    }
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            SQLClass.conn = new MySql.Data.MySqlClient.MySqlConnection();
            SQLClass.conn.ConnectionString = AdmissionCommitee.Properties.Settings.Default.MySQLConn;
            SQLClass.conn.Open();
            Application.Run(new Form1());
            SQLClass.conn.Close();
        }
    }
}
