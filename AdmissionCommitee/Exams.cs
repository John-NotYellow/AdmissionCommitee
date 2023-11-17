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
    public partial class Exams : Form
    {
        public string userLogin = string.Empty;

        public Exams()
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
            dataGridView1.Columns.Add("entrant", "Абитуриент");
            var valueCol = new DataGridViewComboBoxColumn { DisplayStyleForCurrentCellOnly = true };
            valueCol.DataPropertyName = "exam";
            valueCol.HeaderText = "Экзамен";
            List<string> exams = SQLClass.Select("SELECT `discipline` FROM `exams` ORDER BY `id`");
            valueCol.DataSource = exams;
            dataGridView1.Columns.Add(valueCol);
            dataGridView1.Columns.Add("scores", "Баллы");
        }

        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            var fullName = record.GetString(1) + " " + record.GetString(2) + " " + record.GetString(3);
            dgw.Rows.Add(record.GetInt32(0), fullName, record.GetString(4), record.GetInt32(5));
        }

        private void RefreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string query = "SELECT `passedExams`.`id`, `surname`, `firstname`, `patronymic`, `discipline`, `scores` FROM `passedExams` " +
                "INNER JOIN `entrant` ON `passedExams`.`entrantId`=`entrant`.`id` " +
                "INNER JOIN `exams` ON `passedExams`.`examsId`=`exams`.`id` " +
                "ORDER BY `entrant`.`id`";

            MySqlCommand command = new MySqlCommand(query, SQLClass.conn);
            DbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }

            reader.Close();
        }

        private void Exams_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns["entrant"].ReadOnly = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddExams rf = new AddExams();
            this.Hide();
            rf.ShowDialog();
            this.Show();
            RefreshDataGrid(dataGridView1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var selectedRowIndex = dataGridView1.CurrentCell.RowIndex;

            var id = Convert.ToInt32(dataGridView1.Rows[selectedRowIndex].Cells[0].Value);

            var delQuery = $"DELETE FROM `passedExams` WHERE id = {id}";

            var command = new MySqlCommand(delQuery, SQLClass.conn);
            command.ExecuteNonQuery();

            RefreshDataGrid(dataGridView1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                var id = dataGridView1.Rows[index].Cells[0].Value.ToString();
                var discipline = dataGridView1.Rows[index].Cells[2].Value.ToString();
                int scores;

                if (dataGridView1.Rows[index].Cells[0].Value.ToString() != string.Empty)
                {
                    if (int.TryParse(dataGridView1.Rows[index].Cells[3].Value.ToString(), out scores))
                    {
                        if (scores <= 100)
                        {
                            List<string> query = SQLClass.Select("SELECT `id` FROM `exams` WHERE `discipline` = '" + discipline + "';");
                            var examsId = query[0];

                            var changeQuery = $"UPDATE `passedExams` SET `examsId` = '{examsId}', `scores` = '{scores}' WHERE id = '{id}'";

                            var command = new MySqlCommand(changeQuery, SQLClass.conn);
                            command.ExecuteNonQuery();
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
            }

            RefreshDataGrid(dataGridView1);
        }

        private void button4_Click(object sender, EventArgs e)
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
