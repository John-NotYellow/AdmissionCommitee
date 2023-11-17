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
    public partial class AddSpecialization : Form
    {
        int selectedRow;
        public AddSpecialization()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void CreateColumns()
        {
            dataGridView1.Columns.Add("sCode", "Код Специальности");
            dataGridView1.Columns.Add("sName", "Специальность");
            var valueCol = new DataGridViewComboBoxColumn { DisplayStyleForCurrentCellOnly = true };
            valueCol.DataPropertyName = "degree";
            valueCol.HeaderText = "Программа";
            List<string> degrees = SQLClass.Select("SELECT `degree` FROM `degree` ORDER BY `id`");
            valueCol.DataSource = degrees;
            dataGridView1.Columns.Add(valueCol);
        }

        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetValue(0), record.GetString(1), record.GetString(2));
        }

        private void RefreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string query = "SELECT `specializationCode`.`id`, `specializationName`, `degree` FROM `specializationCode`" +
                "INNER JOIN `specialization` ON `specializationCode`.`specializationId`=`specialization`.`id`" +
                "INNER JOIN `degree` ON `specializationCode`.`degreeId`=`degree`.`id`" +
                "ORDER BY `specializationCode`.`id`;";

            MySqlCommand command = new MySqlCommand(query, SQLClass.conn);
            DbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }

            reader.Close();
        }

        private void AddSpecialization_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            List<string> degrees = SQLClass.Select("SELECT degree FROM degree ORDER BY id;");
            comboBox1.Items.AddRange(degrees.ToArray());
            comboBox1.SelectedIndex = -1;

            List<string> faculties = SQLClass.Select(
                "SELECT facultyName FROM faculty ORDER BY id;");
            comboBox2.Items.AddRange(faculties.ToArray());
            comboBox2.SelectedIndex = -1;
            this.comboBox2.SelectedIndexChanged += new EventHandler(comboBox2_SelectedIndexChanged);

            List<string> studyForms = SQLClass.Select("SELECT form FROM studyForm ORDER BY id;");
            comboBox4.Items.AddRange(studyForms.ToArray());
            comboBox4.SelectedIndex = -1;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<string> departments = SQLClass.Select(
                "SELECT DISTINCT departmentName FROM department " +
                "INNER JOIN faculty ON department.facultyId = faculty.id " +
                "WHERE facultyName = '" + comboBox2.SelectedItem + "' " +
                "ORDER BY department.id;");
            comboBox3.Items.Clear();
            comboBox3.Items.AddRange(departments.ToArray());
            comboBox3.SelectedIndex = 0;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];

                textBox3.Text = row.Cells[0].Value.ToString() + " / " + row.Cells[1].Value.ToString() + " / " + row.Cells[2].Value.ToString();
            }
        }

        private Boolean checkspecialization()
        {
            int index = dataGridView1.CurrentCell.RowIndex;

            var specId = dataGridView1.Rows[index].Cells[0].Value;

            MySqlDataAdapter adapter = new MySqlDataAdapter();
            DataTable dt = new DataTable();
            string query = $"SELECT * FROM specializationOffer WHERE sCodeId = '{specId}'";

            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

            adapter.SelectCommand = cmd;
            adapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                MessageBox.Show("Специальность приобщена к кафедре!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            else
            {
                return false;
            }
        }

 
        private void button1_Click(object sender, EventArgs e)
        {
            var code = textBox1.Text;

            MySqlDataAdapter adapter = new MySqlDataAdapter();
            DataTable dt = new DataTable();
            string query = $"SELECT * FROM specializationCode WHERE id = '{code}'";

            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

            adapter.SelectCommand = cmd;
            adapter.Fill(dt);

            if (dt.Rows.Count > 0)
                MessageBox.Show("Специальность c таким кодом уже существует!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if ((textBox1.Text.Equals("")) || (textBox2.Text.Equals("")) || (comboBox1.Text.Equals("")))
                MessageBox.Show("Вы не ввели все необходимые данные!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                var id = textBox1.Text;
                var specialization = textBox2.Text;
                var degree = comboBox1.SelectedItem;

                List<string> specquery = SQLClass.Select("SELECT `auto_increment` FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'specialization';");
                var specid = specquery[0];

                List<string> degreequery = SQLClass.Select("SELECT id FROM `degree` WHERE `degree` = '" + degree + "';");
                var degreeid = degreequery[0];

                var addQuery1 = $"INSERT INTO `specialization` (`specializationName`) values ('{specialization}');";

                var command1 = new MySqlCommand(addQuery1, SQLClass.conn);
                command1.ExecuteNonQuery();

                var addQuery2 = $"INSERT INTO `specializationCode` (`id`, `specializationId`, `degreeId`) values ('{id}', '{specid}', '{degreeid}');";

                var command2 = new MySqlCommand(addQuery2, SQLClass.conn);
                command2.ExecuteNonQuery();
            }

            textBox1.Clear();
            textBox2.Clear();
            comboBox1.SelectedItem = null;
            RefreshDataGrid(dataGridView1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            String[] oldData = textBox3.Text.Split(new String[] {" / "}, StringSplitOptions.RemoveEmptyEntries);
            string oldCode = oldData[0];
            string oldSpec = oldData[1];

            List<string> query = SQLClass.Select("SELECT `id` FROM `specialization` WHERE `specializationName` = '" + oldSpec + "';");
            var oldSpecId = query[0];

            var index = dataGridView1.CurrentCell.RowIndex;

            var id = dataGridView1.Rows[index].Cells[0].Value.ToString();
            var specialization = dataGridView1.Rows[index].Cells[1].Value.ToString();
            var degree = dataGridView1.Rows[index].Cells[2].Value.ToString();

            List<string> query2 = SQLClass.Select("SELECT `id` FROM `degree` WHERE `degree` = '" + degree + "';");
            var degreeId = query2[0];

            MySqlDataAdapter adapter = new MySqlDataAdapter();
            DataTable dt = new DataTable();
            string checkquery = $"SELECT * FROM specializationCode WHERE id = '{id}'";

            MySqlCommand cmd = new MySqlCommand(checkquery, SQLClass.conn);

            adapter.SelectCommand = cmd;
            adapter.Fill(dt);

            if (dt.Rows.Count > 0 && oldCode != id)
                MessageBox.Show("Специальность c таким кодом уже существует!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                var changeQuery = $"UPDATE `specializationCode`, `specialization` SET `specializationCode`.`id` = '{id}', `specializationCode`.`degreeId` = '{degreeId}', `specialization`.`specializationName` = '{specialization}' WHERE `specializationCode`.`id` = '{oldCode}' AND `specialization`.`id` = '{oldSpecId}';";

                var command = new MySqlCommand(changeQuery, SQLClass.conn);
                command.ExecuteNonQuery();

                RefreshDataGrid(dataGridView1);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            var index = dataGridView1.CurrentCell.RowIndex;

            if (checkspecialization())
            {
                return;
            }

            var specialization = dataGridView1.Rows[index].Cells[1].Value;

            List<string> query = SQLClass.Select("SELECT `id` FROM `specialization` WHERE `specializationName` = '" + specialization + "';");
            var id = query[0];

            var delQuery = $"DELETE FROM `specialization` WHERE `id` = {id}";

            var command = new MySqlCommand(delQuery, SQLClass.conn);
            command.ExecuteNonQuery();

            RefreshDataGrid(dataGridView1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var index = dataGridView1.CurrentCell.RowIndex;

            var code = dataGridView1.Rows[index].Cells[0].Value.ToString();

            if (code != string.Empty && comboBox3.SelectedItem != null && comboBox4.SelectedItem != null)
            {
                List<string> depquery = SQLClass.Select("SELECT `id` FROM `department` WHERE `departmentName` = '" + comboBox3.SelectedItem + "';");
                var depId = depquery[0];

                List<string> sfid = SQLClass.Select("SELECT id FROM studyForm WHERE form = '" + comboBox4.SelectedItem + "';");
                var studyForm = sfid[0];

                MySqlDataAdapter adapter = new MySqlDataAdapter();
                DataTable dt = new DataTable();
                string soquery = $"SELECT * FROM specializationOffer WHERE sCodeId = '{code}' AND departmentId = '{depId}' AND studyFormId = '{studyForm}';";

                MySqlCommand cmd = new MySqlCommand(soquery, SQLClass.conn);

                adapter.SelectCommand = cmd;
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                    MessageBox.Show("Специальность уже связана с указаной кафедрой в рамках указаной формы обучения!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    var addQuery = $"INSERT INTO `specializationOffer` (`departmentId`, `sCodeId`, `studyFormId`) values ('{depId}', '{code}', '{studyForm}');";

                    var command = new MySqlCommand(addQuery, SQLClass.conn);
                    command.ExecuteNonQuery();
                }
            }
            else
                MessageBox.Show("Вы не ввели все необходимые данные!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);

            comboBox2.ResetText();
            comboBox3.SelectedItem = null;
            comboBox4.SelectedItem = null;

        }

        private void button5_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Visible = false;
                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                    if (dataGridView1.Rows[i].Cells[j].Value != null)
                        if (dataGridView1.Rows[i].Cells[j].Value.ToString().IndexOf(textBox4.Text, StringComparison.OrdinalIgnoreCase) != -1)
                        {
                            dataGridView1.Rows[i].Visible = true;
                            break;
                        }
            }

            textBox4.Clear();
        }
        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Xbutton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
