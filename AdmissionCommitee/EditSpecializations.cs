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
    public partial class EditSpecializations : Form
    {
        int selectedRow;
        public EditSpecializations()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void CreateColumns()
        {
            dataGridView1.Columns.Add("id", "id");            
            dataGridView1.Columns.Add("department", "Кафедра");
            dataGridView1.Columns.Add("sCode", "Код Специальности");
            dataGridView1.Columns.Add("sName", "Специальность");
            dataGridView1.Columns.Add("degree", "Программа");
            dataGridView1.Columns.Add("studyForm", "Форма обучения");
        }

        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetValue(2), record.GetString(3), record.GetString(4), record.GetString(5), RowState.ModifiedNew);
        }

        private void RefreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string query = "SELECT `specializationOffer`.`id`, `departmentName`, `sCodeId`, `specializationName`, `degree`, `form` FROM `specializationOffer`" +
                "INNER JOIN `department` ON `specializationOffer`.`departmentId`=`department`.`id`" +
                "INNER JOIN `specializationCode` ON `specializationOffer`.`sCodeId`=`specializationCode`.`id`" +
                "INNER JOIN `specialization` ON `specializationCode`.`specializationId`=`specialization`.`id`" +
                "INNER JOIN `degree` ON `specializationCode`.`degreeId`=`degree`.`id`" +
                "INNER JOIN `studyForm` ON `specializationOffer`.`studyFormId`=`studyForm`.`id`" +
                "ORDER BY `specializationOffer`.`id`;";

            MySqlCommand command = new MySqlCommand(query, SQLClass.conn);
            DbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }

            reader.Close();
        }

        private void EditSpecializations_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];

                textBox1.Text = row.Cells[3].Value.ToString();
            }
        }

        private Boolean checkadmission()
        {
            int index = dataGridView1.CurrentCell.RowIndex;

            var sOId = dataGridView1.Rows[index].Cells[0].Value;

            MySqlDataAdapter adapter = new MySqlDataAdapter();
            DataTable dt = new DataTable();
            string query = $"SELECT * FROM admission WHERE specializationOfferId = '{sOId}'";

            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

            adapter.SelectCommand = cmd;
            adapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                MessageBox.Show("По данной специальности есть активные заявления!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (checkadmission())
            {
                return;
            }

            var selectedRowIndex = dataGridView1.CurrentCell.RowIndex;

            var id = dataGridView1.Rows[selectedRowIndex].Cells[0].Value;
            var delQuery = $"DELETE FROM `specializationOffer` WHERE `id` = {id}";

            var command = new MySqlCommand(delQuery, SQLClass.conn);
            command.ExecuteNonQuery();

            textBox1.Clear();
            textBox2.Clear();
            RefreshDataGrid(dataGridView1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AddSpecialization rf = new AddSpecialization();
            this.Hide();
            rf.ShowDialog();
            this.Show();

            textBox1.Clear();
            textBox2.Clear();
            RefreshDataGrid(dataGridView1);
        }

        private void button3_Click(object sender, EventArgs e)
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

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Xbutton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
