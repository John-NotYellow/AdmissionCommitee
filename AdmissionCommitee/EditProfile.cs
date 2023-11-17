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
    public partial class EditProfile : Form
    {
        public string userLogin = string.Empty;
        public EditProfile()
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

        private void EditProfile_Load(object sender, EventArgs e)
        {
            textBox5.PasswordChar = '*';
            textBox4.MaxLength = 45;
            textBox5.MaxLength = 45;
            textBox4.Text = userLogin;
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

            if (checkuser())
            {
                return;
            }

            string query = $"UPDATE `entrant` SET `surname`='{surname}',`firstname`='{firstname}',`patronymic`='{patronymic}',`citizenship`='{citizenship}',`birthdate`='{birthdate}',`login`='{login}', `password`='{pass}' WHERE login = '{userLogin}';";

            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

            if (cmd.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Данные обновлены!", "Success");
                this.Close();

            }
            else
            {
                MessageBox.Show("Ошибка!");
            }

            MySqlDataAdapter adapter = new MySqlDataAdapter();
            DataTable dt = new DataTable();

            string query2 = $"SELECT id, login, password, is_admin FROM entrant WHERE login = '{login}'";

            MySqlCommand cmd2 = new MySqlCommand(query2, SQLClass.conn);

            adapter.SelectCommand = cmd2;
            adapter.Fill(dt);

            var user = new CheckAdmin(dt.Rows[0].ItemArray[1].ToString(), Convert.ToBoolean(dt.Rows[0].ItemArray[3]));

            EnterProfile rf = new EnterProfile(user);
            this.Close();
            rf.Show();
            rf.Activate();

        }

        private Boolean checkuser()
        {
            var login = textBox4.Text;

            MySqlDataAdapter adapter = new MySqlDataAdapter();
            DataTable dt = new DataTable();
            string query = $"SELECT id, login FROM entrant WHERE login = '{login}' AND login != '{userLogin}'";

            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

            adapter.SelectCommand = cmd;
            adapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                MessageBox.Show("Имя пользователя уже занято!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
