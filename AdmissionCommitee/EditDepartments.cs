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
    public partial class EditDepartments : Form
    {
        public string userLogin = string.Empty;

        public EditDepartments()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        private void CreateColumns()
        {
            dataGridView1.Columns.Add("id", "id");
            dataGridView1.Columns.Add("faculty", "Факультет");
            dataGridView1.Columns.Add("department", "Кафедра");
        }

        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetValue(0), record.GetString(1), record.GetString(2));
        }

        private void RefreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string query = "SELECT `department`.`id`, `facultyName`, `departmentName` FROM `department`" +
                "INNER JOIN `faculty` ON `department`.`facultyId`=`faculty`.`id`" +
                "ORDER BY `department`.`id`;";

            MySqlCommand command = new MySqlCommand(query, SQLClass.conn);
            DbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }

            reader.Close();
        }

        private void EditDepartments_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns["id"].ReadOnly = true;
            dataGridView1.Columns["faculty"].ReadOnly = true;

            List<string> faculties = SQLClass.Select(
                "SELECT facultyName FROM faculty ORDER BY id;");
            comboBox1.Items.AddRange(faculties.ToArray());
            comboBox1.SelectedIndex = -1;
        }

        private Boolean checkdepartment()
        {
            int index = dataGridView1.CurrentCell.RowIndex;

            var depId = dataGridView1.Rows[index].Cells[0].Value;

            MySqlDataAdapter adapter = new MySqlDataAdapter();
            DataTable dt = new DataTable();
            string query = $"SELECT * FROM specializationOffer WHERE departmentId = '{depId}'";

            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

            adapter.SelectCommand = cmd;
            adapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                MessageBox.Show("При кафедре числятся специальности!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<string> id = SQLClass.Select("SELECT id FROM faculty WHERE facultyName = '" + comboBox1.SelectedItem + "' ");

            var facultyId = id[0];
            var department = textBox1.Text;

            MySqlDataAdapter adapter = new MySqlDataAdapter();
            DataTable dt = new DataTable();
            string query = $"SELECT * FROM department WHERE facultyId = '{facultyId}'";

            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

            adapter.SelectCommand = cmd;
            adapter.Fill(dt);

            var depId = facultyId.ToString() + "." + (dt.Rows.Count + 1).ToString();

            MySqlDataAdapter depAdapter = new MySqlDataAdapter();
            DataTable checkdt = new DataTable();
            string checkquery = $"SELECT * FROM department WHERE facultyId = '{facultyId}' AND departmentName = '{department}';";

            MySqlCommand checkcmd = new MySqlCommand(checkquery, SQLClass.conn);

            depAdapter.SelectCommand = checkcmd;
            depAdapter.Fill(checkdt);

            if ((textBox1.Text.Equals("")) || (comboBox1.Text.Equals("")))
                MessageBox.Show("Вы не ввели все необходимые данные!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (checkdt.Rows.Count > 0)
                MessageBox.Show("Кафедра с таким названием уже есть на факультете!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                string addquery = $"INSERT INTO department(id, departmentName, facultyId) VALUES('{depId}', '{department}', '{facultyId}');";

                MySqlCommand addcmd = new MySqlCommand(addquery, SQLClass.conn);

                if (addcmd.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("Данные внесены!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }

            textBox1.Clear();
            comboBox1.ResetText();
            RefreshDataGrid(dataGridView1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (checkdepartment())
            {
                return;
            }

            var selectedRowIndex = dataGridView1.CurrentCell.RowIndex;

            var id = dataGridView1.Rows[selectedRowIndex].Cells[0].Value.ToString();

            var delQuery = $"DELETE FROM `department` WHERE `id` = '{id}';";

            var command = new MySqlCommand(delQuery, SQLClass.conn);
            command.ExecuteNonQuery();

            textBox2.Clear();
            RefreshDataGrid(dataGridView1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var selectedRowIndex = dataGridView1.CurrentCell.RowIndex;

            List<string> fid = SQLClass.Select("SELECT id FROM faculty WHERE facultyName = '" + dataGridView1.Rows[selectedRowIndex].Cells[1].Value.ToString() + "' ");
                        
            var facultyId = fid[0];
            var newdepartment = dataGridView1.Rows[selectedRowIndex].Cells[2].Value.ToString();

            MySqlDataAdapter depAdapter = new MySqlDataAdapter();
            DataTable checkdt = new DataTable();
            string checkquery = $"SELECT * FROM department WHERE facultyId = '{facultyId}' AND departmentName = '{newdepartment}';";

            MySqlCommand checkcmd = new MySqlCommand(checkquery, SQLClass.conn);

            depAdapter.SelectCommand = checkcmd;
            depAdapter.Fill(checkdt);

            if (checkdt.Rows.Count > 0)
                MessageBox.Show("Кафедра с таким названием уже есть на факультете!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                for (int index = 0; index < dataGridView1.Rows.Count; index++)
                {
                    var id = dataGridView1.Rows[index].Cells[0].Value.ToString();
                    var department = dataGridView1.Rows[index].Cells[2].Value.ToString();

                    var changeQuery = $"UPDATE `department` SET `departmentName` = '{department}' WHERE id = '{id}';";

                    var command = new MySqlCommand(changeQuery, SQLClass.conn);
                    command.ExecuteNonQuery();
                }
            }
            
            textBox2.Clear();
            RefreshDataGrid(dataGridView1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            EditSpecializations rf = new EditSpecializations();
            this.Hide();
            rf.ShowDialog();
            this.Show();

            textBox2.Clear();
            RefreshDataGrid(dataGridView1);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Visible = false;
                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                    if (dataGridView1.Rows[i].Cells[j].Value != null)
                        if (dataGridView1.Rows[i].Cells[j].Value.ToString().IndexOf(textBox2.Text, StringComparison.OrdinalIgnoreCase) != -1)
                        {
                            dataGridView1.Rows[i].Visible = true;
                            break;
                        }
            }

            textBox2.Clear();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            DataTable dt = new DataTable();

            string query2 = $"SELECT id, login, password, is_admin FROM entrant WHERE login = '{userLogin}'";

            MySqlCommand cmd2 = new MySqlCommand(query2, SQLClass.conn);

            adapter.SelectCommand = cmd2;
            adapter.Fill(dt);

            var user = new CheckAdmin(dt.Rows[0].ItemArray[1].ToString(), Convert.ToBoolean(dt.Rows[0].ItemArray[3]));

            EnterProfile rf = new EnterProfile(user);
            this.Close();
            rf.Show();
            rf.Activate();

        }

        private void Xbutton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
