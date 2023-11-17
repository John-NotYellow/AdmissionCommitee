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
    public partial class AddIDocument : Form
    {
        public string userLogin = string.Empty;
        public AddIDocument()
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

        private void AddIDocument_Load(object sender, EventArgs e)
        {
            List<string> country = SQLClass.Select("SELECT citizenship FROM entrant WHERE login = '" + userLogin + "';");
            var citizenship = country[0];

            if (citizenship == "РФ")
                comboBox1.Items.AddRange(new string[] { "Паспорт", "Загранпаспорт", "Военный билет", "Временное удостоверение (форма №2П)" });
            else
                comboBox1.Items.AddRange(new string[] { "Паспорт иностранного гражданина", "Вид на жительство РФ", "Служебный паспорт" });
            comboBox1.SelectedIndex = 0;
                       
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
                MessageBox.Show("Вы не ввели все необходимые данные!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                string query = $"INSERT INTO identityDocument(documentType, series, number, issuedBy, dateOfIssue, entrantId) VALUES( '{documentType}', '{series}', '{number}', '{issuedBy}', '{issueDate}', '{entrantId}');";

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
