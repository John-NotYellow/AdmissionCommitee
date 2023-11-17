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
    public partial class AddBeneficiary : Form
    {
        int selectedRow;
        public string entrantId = string.Empty;
        public AddBeneficiary()
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

        private void AddBeneficiary_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.Columns[0].Visible = false;

            List<string> privileges = SQLClass.Select(
                "SELECT privilege FROM benefits ORDER BY id;");
            comboBox1.Items.AddRange(privileges.ToArray());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<string> id = SQLClass.Select("SELECT id FROM benefits WHERE privilege = '" + comboBox1.SelectedItem + "';");
            
            var benefitsId = id[0];

            if ((textBox1.Text.Equals("выберите абитуриента в таблице")) || (comboBox1.Text.Equals("")))
                MessageBox.Show("Вы не ввели все необходимые данные!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                string query = $"INSERT INTO beneficiaries(entrantId, benefitsId) VALUES( '{entrantId}', '{benefitsId}');";

                MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

                if (cmd.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("Данные внесены!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.Close();
            }
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
