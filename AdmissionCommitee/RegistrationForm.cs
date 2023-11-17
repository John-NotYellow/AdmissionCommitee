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
    public partial class RegistrationForm : Form
    {
        public RegistrationForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void RegistrationForm_Load(object sender, EventArgs e)
        {
            textBox5.PasswordChar = '*';
            textBox4.MaxLength = 45;
            textBox5.MaxLength = 45;
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var surname = textBox1.Text;
            var firstname = textBox2.Text;
            var patronymic = textBox3.Text;
            var citizenship = comboBox1.SelectedItem;
            var birthdate = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            var login = textBox4.Text;
            var pass = textBox5.Text;
            
            if(checkuser())
            {
                return;
            }

            string query = $"INSERT INTO entrant(surname, firstname, patronymic, citizenship, birthdate, login, password, is_admin) VALUES('{surname}','{firstname}','{patronymic}','{citizenship}','{birthdate}','{login}','{pass}', 0)";

            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

            if(cmd.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Аккаунт успешно создан!", "Success");
                this.Close();
            }
            else
            {
                MessageBox.Show("Аккаунт не создан!");
            }
        }

        private Boolean checkuser()
        {
            var login = textBox4.Text;

            MySqlDataAdapter adapter = new MySqlDataAdapter();
            DataTable dt = new DataTable();
            string query = $"SELECT id, login FROM entrant WHERE login = '{login}'";

            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

            adapter.SelectCommand = cmd;
            adapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                MessageBox.Show("Пользователь уже существует!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            else if (dt.Rows.Count == 0 && (textBox1.Text.Equals("")) || (textBox2.Text.Equals("")) || (textBox4.Text.Equals("")) || (textBox5.Text.Equals("")))
            {
                MessageBox.Show("Вы не ввели все необходимые данные!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
