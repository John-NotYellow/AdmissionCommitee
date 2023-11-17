using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace AdmissionCommitee
{
    public partial class EditIDocument : Form
    {
        public string userLogin = string.Empty;
        public EditIDocument()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void EditIDocument_Load(object sender, EventArgs e)
        {
            List<string> country = SQLClass.Select("SELECT citizenship FROM entrant WHERE login = '" + userLogin + "';");
            var citizenship = country[0];

            if (citizenship == "РФ")
                comboBox1.Items.AddRange(new string[] { "Паспорт", "Загранпаспорт", "Военный билет", "Временное удостоверение (форма №2П)" });
            else
                comboBox1.Items.AddRange(new string[] { "Паспорт иностранного гражданина", "Вид на жительство РФ", "Служебный паспорт" });

            List<string> id = SQLClass.Select("SELECT id FROM entrant WHERE login = '" + userLogin + "';");
            var entrantId = id[0];

            List<string> document = SQLClass.Select("SELECT documentType, series, number, issuedBy FROM identityDocument WHERE entrantId = '" + entrantId + "';");
            comboBox1.SelectedItem = document[0];
            textBox1.Text = document[1];
            textBox2.Text = document[2];
            textBox3.Text = document[3];

            List<string> issueDate = SQLClass.Select("SELECT DATE_FORMAT(dateOfIssue, '%d.%m.%Y') FROM identityDocument WHERE entrantId = '" + entrantId + "';");
            DateTime dateOfIssue = DateTime.Parse(issueDate[0]);
            dateTimePicker1.Value = dateOfIssue;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<string> id = SQLClass.Select("SELECT id FROM entrant WHERE login = '" + userLogin + "';");

            var documentType = comboBox1.SelectedItem;
            var series = textBox1.Text;
            var number = textBox2.Text;
            var issuedBy = textBox3.Text;
            var issueDate = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            var entrantId = id[0];

            if ((textBox1.Text.Equals("")) || (textBox2.Text.Equals("")) || (textBox3.Text.Equals("")))
                MessageBox.Show("Остались пустые поля!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                string query = $"UPDATE identityDocument SET documentType = '{documentType}', series = '{series}', number = '{number}', issuedBy = '{issuedBy}', dateOfIssue = '{issueDate}' WHERE entrantId = '{entrantId}';";

                MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

                if (cmd.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("Данные внесены!", "Success");

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
                else
                {
                    MessageBox.Show("Ошибка, данные не внесены!");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
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
    }
}
