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
    public partial class AddCommiteeDecision : Form
    {
        public string userLogin = string.Empty;

        int selectedRow;
        public AddCommiteeDecision()
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
            dataGridView1.Columns.Add("recordingDate", "Дата подачи заявления");
            dataGridView1.Columns.Add("degree", "Программа");
            dataGridView1.Columns.Add("department", "Кафедра");
            dataGridView1.Columns.Add("specialization", "Специальность");
            dataGridView1.Columns.Add("studyform", "Форма обучения");
            dataGridView1.Columns.Add("decision", "Решение комисии");
            dataGridView1.Columns.Add("decisionDate", "Дата решения");
        }

        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            var fullName = record.GetString(1) + " " + record.GetString(2) + " " + record.GetString(3);
            dgw.Rows.Add(record.GetInt32(0), fullName, record.GetString(4), record.GetString(5), record.GetString(6), record.GetString(7), record.GetString(8), record.GetString(9), record.GetString(10));
        }

        private void RefreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string query = "SELECT admission.id, surname, firstname, patronymic, DATE_FORMAT(recordingDate, '%d.%m.%Y'), degree, departmentName, specializationName, form, comissionDecision, DATE_FORMAT(decisionDate, '%d.%m.%Y') FROM admission " +
                "INNER JOIN entrant ON admission.entrantId = entrant.id " +
                "INNER JOIN specializationOffer ON admission.specializationOfferId = specializationOffer.id " +
                "INNER JOIN specializationCode ON specializationOffer.sCodeId = specializationCode.id " +
                "INNER JOIN degree ON specializationCode.degreeId = degree.id " +
                "INNER JOIN department ON specializationOffer.departmentId = department.id " +
                "INNER JOIN specialization ON specializationCode.specializationId = specialization.id " +
                "INNER JOIN studyForm ON specializationOffer.studyFormId = studyForm.id " +
                "ORDER BY admission.id;";

            MySqlCommand command = new MySqlCommand(query, SQLClass.conn);
            DbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }

            reader.Close();

        }

        private void AddCommiteeDecision_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.Columns[0].Visible = false;

        }

        public string getdegree = string.Empty;
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];

                textBox1.Clear();

                List<string> getid = SQLClass.Select("SELECT entrantId FROM admission WHERE admission.id = '" + row.Cells[0].Value + "';");

                int entrantId = int.Parse(getid[0]);

                textBox1.Text += "Удостоверение личности:\r\n";
                List<string> iDoc = SQLClass.Select("SELECT documentType, series, number, issuedBy, DATE_FORMAT(dateOfIssue, '%d.%m.%Y') FROM identityDocument WHERE entrantId = '" + entrantId + "';");
                textBox1.Text += iDoc[0] + "\r\n";
                textBox1.Text += iDoc[1] + " " + iDoc[2] + "\r\n";
                textBox1.Text += "Выдан: " + iDoc[3] + ", " + iDoc[4] + "\r\n";

                textBox1.Text += "\r\nДокумент об образовании:\r\n";
                List<string> eDoc = SQLClass.Select("SELECT * FROM educationDocument WHERE entrantId = '" + entrantId + "';");
                textBox1.Text += eDoc[1] + "\r\n";
                textBox1.Text += eDoc[2] + " " + eDoc[3] + "\r\n";

                textBox1.Text += "\r\nВступительные испытания:\r\n";
                List<string> disciplines = SQLClass.Select(
                    "SELECT discipline FROM exams " +
                    "INNER JOIN passedExams ON exams.id = passedExams.examsId " +
                    "INNER JOIN entrant ON passedExams.entrantId = entrant.id " +
                    "WHERE entrant.id = '" + entrantId + "';");

                for (int i = 0; i <= disciplines.Count - 1; i++)
                {
                    if (i < disciplines.Count - 1) { textBox1.Text += disciplines[i] + ", "; }
                    if (i == disciplines.Count - 1) { textBox1.Text += disciplines.Last() + ".\r\n"; }
                }
                
                List<string> scores = SQLClass.Select("SELECT scores FROM passedExams " +
                    "INNER JOIN entrant ON passedExams.entrantId = entrant.id " +
                    "WHERE entrant.id = '" + entrantId + "';");

                int[] ints = scores.Select(int.Parse).ToArray();
                int sum = ints.Sum();

                textBox1.Text += "\r\nСумма балов: " + sum + "\r\n";

                List<string> benefit = SQLClass.Select(
                "SELECT privilege FROM benefits INNER JOIN beneficiaries ON benefits.id = beneficiaries.benefitsId WHERE entrantId = '" + entrantId + "';");

                foreach (string item in benefit)
                    textBox1.Text += "\r\nПривилегии:\r\n" + item + "\r\n";

                List<string> departments = SQLClass.Select(
                "SELECT DISTINCT departmentName FROM department " +
                "INNER JOIN specializationOffer ON department.id = specializationOffer.departmentId " +
                "INNER JOIN specializationCode ON specializationOffer.sCodeId = specializationCode.id " +
                "INNER JOIN degree ON specializationCode.degreeId =degree.id " +
                "WHERE degree = '" + row.Cells[3].Value + "' " +
                "ORDER BY department.id;");
                comboBox2.Items.Clear();
                comboBox2.Items.AddRange(departments.ToArray());
                comboBox2.SelectedItem = row.Cells[4].Value.ToString();
                this.comboBox2.SelectedIndexChanged += new EventHandler(comboBox2_SelectedIndexChanged);

                List<string> specializations = SQLClass.Select(
                "SELECT DISTINCT specializationName FROM specialization " +
                "INNER JOIN specializationCode ON specialization.id = specializationCode.specializationId " +
                "INNER JOIN degree ON specializationCode.degreeId =degree.id " +
                "INNER JOIN specializationOffer ON specializationCode.id = specializationOffer.sCodeId " +
                "INNER JOIN department ON specializationOffer.departmentId = department.id " +
                "WHERE degree = '" + row.Cells[3].Value + "' " +
                "AND departmentName = '" + row.Cells[4].Value + "' " +
                "ORDER BY specialization.id;");
                comboBox3.Items.Clear();
                comboBox3.Items.AddRange(specializations.ToArray());
                comboBox3.SelectedItem = row.Cells[5].Value.ToString();
                this.comboBox3.SelectedIndexChanged += new EventHandler(comboBox3_SelectedIndexChanged);

                List<string> studyForms = SQLClass.Select(
                "SELECT DISTINCT form FROM studyForm " +
                "INNER JOIN specializationOffer ON studyForm.id = specializationOffer.studyFormId " +
                "INNER JOIN department ON specializationOffer.departmentId = department.id " +
                "INNER JOIN specializationCode ON specializationOffer.sCodeId = specializationCode.id " +
                "INNER JOIN degree ON specializationCode.degreeId = degree.id " +
                "INNER JOIN specialization ON specializationCode.specializationId = specialization.id " +
                "WHERE degree = '" + row.Cells[3].Value + "' " +
                "AND departmentName = '" + row.Cells[4].Value + "' " +
                "AND specializationName = '" + row.Cells[5].Value + "' " +
                "ORDER BY studyForm.id;");
                comboBox4.Items.Clear();
                comboBox4.Items.AddRange(studyForms.ToArray()); 
                comboBox4.SelectedItem = row.Cells[6].Value.ToString();

                getdegree = row.Cells[3].Value.ToString();
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i <= comboBox2.SelectedIndex; i++)
            {
                List<string> specializations = SQLClass.Select(
                "SELECT DISTINCT specializationName FROM specialization " +
                "INNER JOIN specializationCode ON specialization.id = specializationCode.specializationId " +
                "INNER JOIN degree ON specializationCode.degreeId =degree.id " +
                "INNER JOIN specializationOffer ON specializationCode.id = specializationOffer.sCodeId " +
                "INNER JOIN department ON specializationOffer.departmentId = department.id " +
                "WHERE degree = '" + getdegree + "' " +
                "AND departmentName = '" + comboBox2.SelectedItem + "' " +
                "ORDER BY specialization.id;");
                comboBox3.Items.Clear();
                comboBox3.ResetText();
                comboBox3.Items.AddRange(specializations.ToArray());

                List<string> studyForms = SQLClass.Select(
                "SELECT DISTINCT form FROM studyForm " +
                "INNER JOIN specializationOffer ON studyForm.id = specializationOffer.studyFormId " +
                "INNER JOIN department ON specializationOffer.departmentId = department.id " +
                "INNER JOIN specializationCode ON specializationOffer.sCodeId = specializationCode.id " +
                "INNER JOIN degree ON specializationCode.degreeId = degree.id " +
                "INNER JOIN specialization ON specializationCode.specializationId = specialization.id " +
                "WHERE degree = '" + getdegree + "' " +
                "AND departmentName = '" + comboBox2.SelectedItem + "' " +
                "AND specializationName = '" + comboBox3.SelectedItem + "' " +
                "ORDER BY studyForm.id;");
                comboBox4.Items.Clear();
                comboBox4.ResetText();
                comboBox4.Items.AddRange(studyForms.ToArray());
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i <= comboBox2.SelectedIndex; i++)
            {
                List<string> studyForms = SQLClass.Select(
                "SELECT DISTINCT form FROM studyForm " +
                "INNER JOIN specializationOffer ON studyForm.id = specializationOffer.studyFormId " +
                "INNER JOIN department ON specializationOffer.departmentId = department.id " +
                "INNER JOIN specializationCode ON specializationOffer.sCodeId = specializationCode.id " +
                "INNER JOIN degree ON specializationCode.degreeId = degree.id " +
                "INNER JOIN specialization ON specializationCode.specializationId = specialization.id " +
                "WHERE degree = '" + getdegree + "' " +
                "AND departmentName = '" + comboBox2.SelectedItem + "' " +
                "AND specializationName = '" + comboBox3.SelectedItem + "' " +
                "ORDER BY studyForm.id;");
                comboBox4.Items.Clear();
                comboBox4.ResetText();
                comboBox4.Items.AddRange(studyForms.ToArray());
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if(comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Решение не выбрано!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                var selectedRowIndex = dataGridView1.CurrentCell.RowIndex;
                var id = dataGridView1.Rows[selectedRowIndex].Cells[0].Value;
                var decision = comboBox1.SelectedItem.ToString();
                var date = DateTime.Today.ToString("yyyy-MM-dd");

                string query = $"UPDATE admission SET comissionDecision = '{decision}', decisionDate = '{date}' WHERE admission.id = '{id}';";

                MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

                if (cmd.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("Данные внесены!", "Success");
                }
                else
                {
                    MessageBox.Show("Ошибка, данные не внесены!");
                }
            }

            RefreshDataGrid(dataGridView1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int index = dataGridView1.CurrentCell.RowIndex;

            if ((comboBox2.Text.Equals("")) || (comboBox3.Text.Equals("")) || (comboBox4.Text.Equals("")))
                MessageBox.Show("Вы не ввели все необходимые данные!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                var id = dataGridView1.Rows[index].Cells[0].Value.ToString();
                var degree = dataGridView1.Rows[index].Cells[3].Value.ToString();
                var department = comboBox2.SelectedItem.ToString();
                var specialization = comboBox3.SelectedItem.ToString();
                var studyForm = comboBox4.SelectedItem.ToString();

                List<string> query = SQLClass.Select(
                        "SELECT `specializationOffer`.`id` FROM `specializationOffer` " +
                        "INNER JOIN `department` ON `specializationOffer`.`departmentId` = `department`.`id` " +
                        "INNER JOIN `specializationCode` ON `specializationOffer`.`sCodeId` = `specializationCode`.`id` " +
                        "INNER JOIN `specialization` ON `specializationCode`.`specializationId` = `specialization`.`id` " +
                        "INNER JOIN `degree` ON `specializationCode`.`degreeId` = `degree`.`id` " +
                        "INNER JOIN `studyForm` ON `specializationOffer`.`studyFormId` = `studyForm`.`id` " +
                        "WHERE `degree` = '" + degree + "' " +
                        "AND `departmentName` = '" + department + "' " +
                        "AND `specializationName` = '" + specialization + "' " +
                        "AND `form` = '" + studyForm + "';");
                var specOfferId = query[0];

                var changeQuery = $"UPDATE `admission` SET `specializationOfferId` = '{specOfferId}' WHERE id = '{id}'";

                var command = new MySqlCommand(changeQuery, SQLClass.conn);
                command.ExecuteNonQuery();
            }

            RefreshDataGrid(dataGridView1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("Вы уверены, что необходимо удалить заявление?", "Message", MessageBoxButtons.YesNo);
            
            if (confirm == DialogResult.No)
            {
                return;
            }
            else
            {
                int index = dataGridView1.CurrentCell.RowIndex;

                var id = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);
                var delQuery = $"DELETE FROM `admission` WHERE id = {id}";

                var command = new MySqlCommand(delQuery, SQLClass.conn);
                command.ExecuteNonQuery();

                RefreshDataGrid(dataGridView1);
            }
            
        }

        private void button4_Click(object sender, EventArgs e)
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

        private void button5_Click(object sender, EventArgs e)
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
