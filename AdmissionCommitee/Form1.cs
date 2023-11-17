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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var login = textBox1.Text;
            var pass = textBox2.Text;

            MySqlDataAdapter adapter = new MySqlDataAdapter();
            DataTable dt = new DataTable();

            string query = $"SELECT id, login, password, is_admin FROM entrant WHERE login = '{login}' and password = '{pass}'";

            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

            adapter.SelectCommand = cmd;
            adapter.Fill(dt);

            if (dt.Rows.Count == 1)
            {
                var user = new CheckAdmin(dt.Rows[0].ItemArray[1].ToString(), Convert.ToBoolean(dt.Rows[0].ItemArray[3]));

                textBox1.Clear();
                textBox2.Clear();

                EnterProfile rf = (EnterProfile)Application.OpenForms["EnterProfile"];
                if (rf == null)
                {
                    EnterProfile form = new EnterProfile(user);
                    this.Hide();
                    form.Show();
                    this.Activate();
                }
                else
                {
                    rf.Activate();
                    this.Hide();
                }
                    
            }
            else
                MessageBox.Show("Неверное имя пользователя или пароль!", "Access denied", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RegistrationForm rf = new RegistrationForm();
            this.Hide();
            rf.ShowDialog();
            this.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SpecializationViewForm rf = new SpecializationViewForm();
            rf.getdegree = comboBox1.SelectedItem.ToString();
            rf.getfaculty = comboBox2.SelectedItem.ToString();
            this.Hide();
            rf.ShowDialog();
            this.Show();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox2.PasswordChar = '*';
            textBox1.MaxLength = 45;
            textBox2.MaxLength = 45;
            
            List<string> degrees = SQLClass.Select(
                "SELECT degree FROM degree ORDER BY id;");
            comboBox1.Items.AddRange(degrees.ToArray());
            comboBox1.SelectedIndex = 0;
            this.comboBox1.SelectedIndexChanged += new EventHandler(comboBox1_SelectedIndexChanged);

            List<string> faculties = SQLClass.Select(
                "SELECT DISTINCT facultyName FROM faculty " +
                "INNER JOIN department ON faculty.id = department.facultyId " +
                "INNER JOIN specializationOffer ON department.id = specializationOffer.departmentId " +
                "INNER JOIN specializationCode ON specializationOffer.sCodeId = specializationCode.id " +
                "INNER JOIN degree ON specializationCode.degreeId =degree.id " +
                "WHERE degree = '" + comboBox1.SelectedItem + "' " +
                "ORDER BY faculty.id;");
            comboBox2.Items.AddRange(faculties.ToArray());
            comboBox2.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i <= comboBox1.SelectedIndex; i ++)
            {
                List<string> faculties = SQLClass.Select(
                    "SELECT DISTINCT facultyName FROM faculty " +
                    "INNER JOIN department ON faculty.id = department.facultyId " +
                    "INNER JOIN specializationOffer ON department.id = specializationOffer.departmentId " +
                    "INNER JOIN specializationCode ON specializationOffer.sCodeId = specializationCode.id " +
                    "INNER JOIN degree ON specializationCode.degreeId =degree.id " +
                    "WHERE degree = '" + comboBox1.SelectedItem + "' " +
                    "ORDER BY faculty.id;");
                comboBox2.Items.Clear();
                comboBox2.Items.AddRange(faculties.ToArray());
                comboBox2.SelectedIndex = 0;
            }
        }

    }
}
