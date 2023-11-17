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
    public partial class AddExams : Form
    {
        int selectedRow;
        public string entrantId = string.Empty;
        public AddExams()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void CreateColumns()
        {
            dataGridView1.Columns.Add("id", "id");
            dataGridView1.Columns.Add("entrant", "Абитуриент");
            dataGridView1.Columns.Add("citizenship", "Гражданство");
            dataGridView1.Columns.Add("birthdate", "Дата рождения");
        }

        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            var fullName = record.GetString(1) + " " + record.GetString(2) + " " + record.GetString(3);
            dgw.Rows.Add(record.GetInt32(0), fullName, record.GetString(4), record.GetValue(5));
        }

        private void RefreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string query = "SELECT `entrant`.`id`, `surname`, `firstname`, `patronymic`, `citizenship`, DATE_FORMAT(`birthdate`, '%d.%m.%Y') FROM `admission` " +
                "INNER JOIN `entrant` ON `admission`.`entrantId` = `entrant`.`id` GROUP BY `entrant`.`id`;";

            MySqlCommand command = new MySqlCommand(query, SQLClass.conn);
            DbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }

            reader.Close();
        }

        private void AddExams_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns["entrant"].ReadOnly = true;
            dataGridView1.Columns["citizenship"].ReadOnly = true;
            dataGridView1.Columns["birthdate"].ReadOnly = true;

            List<string> disciplines = SQLClass.Select(
                "SELECT discipline FROM exams ORDER BY id;");
            comboBox1.Items.AddRange(disciplines.ToArray());
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];

                var id = row.Cells[0].Value.ToString();
                textBox1.Text = row.Cells[1].Value.ToString();
                entrantId = id;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if ((textBox1.Text.Equals("выберите абитуриента в таблице")) || (comboBox1.Text.Equals("")) || (textBox2.Text.Equals("")))
                MessageBox.Show("Вы не ввели все необходимые данные!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                List<string> idexam = SQLClass.Select("SELECT id FROM exams WHERE discipline = '" + comboBox1.SelectedItem + "';");
                var examsId = idexam[0];

                int scores;
             
                if(int.TryParse(textBox2.Text, out scores))
                {
                    if(Convert.ToInt32(textBox2.Text) <= 100)
                    {
                        var addQuery = $"INSERT INTO passedExams (entrantId, examsId, scores) values ('{entrantId}', '{examsId}', '{scores}');";

                        var command = new MySqlCommand(addQuery, SQLClass.conn);
                        command.ExecuteNonQuery();

                        MessageBox.Show("Данные внесены!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Число баллов не может превышать 100!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    } 
                }
                else
                {
                    MessageBox.Show("Баллы должны быть в формате целых чисел!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            textBox1.Clear();
            textBox2.Clear();
            comboBox1.SelectedIndex = -1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Xbutton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
