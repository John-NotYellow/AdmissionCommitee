using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace AdmissionCommitee
{
    public partial class SpecializationViewForm : Form
    {
        public string getdegree = string.Empty;
        public string getfaculty = string.Empty;
        public SpecializationViewForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void CreateColumns()
        {
            dataGridView1.Columns.Add("faculty", "Факультет");
            dataGridView1.Columns.Add("department", "Кафедра");
            dataGridView1.Columns.Add("speciaization", "Специальность");
            dataGridView1.Columns.Add("sCode", "Код специальности");
            dataGridView1.Columns.Add("degree", "Программа");
            dataGridView1.Columns.Add("sForm", "Форма обучения");
        }

        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetString(0), record.GetString(1), record.GetString(2), record.GetValue(3), record.GetString(4), record.GetString(5));
        }

        private void LoadData(DataGridView dgw)
        {
            string query = "SELECT `facultyName`, `departmentName`, `specializationName`, `specializationCode`.`id`, `degree`, `form` " +
                "FROM `specializationOffer` " +
                "INNER JOIN `department` ON `specializationOffer`.`departmentId`=`department`.`id` " +
                "INNER JOIN `specializationCode` ON `specializationOffer`.`sCodeId`=`specializationCode`.`id` " +
                "INNER JOIN `specialization` ON `specializationCode`.`specializationId`=`specialization`.`id` " +
                "INNER JOIN `studyForm` ON `specializationOffer`.`studyFormId`=`studyForm`.`id` " +
                "INNER JOIN `faculty` ON `department`.`facultyId`=`faculty`.`id` " +
                "INNER JOIN `degree` ON `specializationCode`.`degreeId`=`degree`.`id` " +
                "WHERE `degree` = '" + getdegree + "' " +
                "AND facultyName = '" + getfaculty + "';";
            
            MySqlCommand command = new MySqlCommand(query, SQLClass.conn);
            DbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);                                               
            }

            reader.Close();
        }

        private void SpecializationViewForm_Load(object sender, EventArgs e)
        {
            CreateColumns();
            LoadData(dataGridView1);

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Visible = false;
                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                    if (dataGridView1.Rows[i].Cells[j].Value != null)
                        if (dataGridView1.Rows[i].Cells[j].Value.ToString().IndexOf(textBox1.Text, StringComparison.OrdinalIgnoreCase) != -1)
                        {
                            dataGridView1.Rows[i].Visible = true;
                            break;
                        }
            }

            textBox1.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
