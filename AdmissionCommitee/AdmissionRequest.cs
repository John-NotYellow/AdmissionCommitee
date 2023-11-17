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
    public partial class AdmissionRequest : Form
    {
        public string userLogin = string.Empty;
        public AdmissionRequest()
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

        private void AdmissionRequest_Load(object sender, EventArgs e)
        {
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
            this.comboBox2.SelectedIndexChanged += new EventHandler(comboBox2_SelectedIndexChanged);

            List<string> departments = SQLClass.Select(
                "SELECT DISTINCT departmentName FROM department " +
                "INNER JOIN faculty ON department.facultyId = faculty.id " +
                "INNER JOIN specializationOffer ON department.id = specializationOffer.departmentId " +
                "INNER JOIN specializationCode ON specializationOffer.sCodeId = specializationCode.id " +
                "INNER JOIN degree ON specializationCode.degreeId =degree.id " +
                "WHERE degree = '" + comboBox1.SelectedItem + "' " +
                "AND facultyName = '" + comboBox2.SelectedItem + "' " +
                "ORDER BY department.id;");
            comboBox3.Items.AddRange(departments.ToArray());
            comboBox3.SelectedIndex = 0;
            this.comboBox3.SelectedIndexChanged += new EventHandler(comboBox3_SelectedIndexChanged);

            List<string> specializations = SQLClass.Select(
                "SELECT DISTINCT specializationName FROM specialization " +
                "INNER JOIN specializationCode ON specialization.id = specializationCode.specializationId " +
                "INNER JOIN degree ON specializationCode.degreeId =degree.id " +
                "INNER JOIN specializationOffer ON specializationCode.id = specializationOffer.sCodeId " +
                "INNER JOIN department ON specializationOffer.departmentId = department.id " +
                "INNER JOIN faculty ON department.facultyId = faculty.id " +
                "WHERE degree = '" + comboBox1.SelectedItem + "' " +
                "AND facultyName = '" + comboBox2.SelectedItem + "' " +
                "AND departmentName = '" +comboBox3.SelectedItem + "' " +
                "ORDER BY specializationId;");
            comboBox4.Items.AddRange(specializations.ToArray());
            comboBox4.SelectedIndex = 0;
            this.comboBox4.SelectedIndexChanged += new EventHandler(comboBox4_SelectedIndexChanged);

            List<string> studyForms = SQLClass.Select(
                "SELECT DISTINCT form FROM studyForm " +
                "INNER JOIN specializationOffer ON studyForm.id = specializationOffer.studyFormId " +
                "INNER JOIN department ON specializationOffer.departmentId = department.id " +
                "INNER JOIN faculty ON department.facultyId = faculty.id " +
                "INNER JOIN specializationCode ON specializationOffer.sCodeId = specializationCode.id " +
                "INNER JOIN degree ON specializationCode.degreeId = degree.id " +
                "INNER JOIN specialization ON specializationCode.specializationId = specialization.id " +
                "WHERE degree = '" + comboBox1.SelectedItem + "' " +
                "AND facultyName = '" + comboBox2.SelectedItem + "' " +
                "AND departmentName = '" + comboBox3.SelectedItem + "' " +
                "AND specializationName = '" + comboBox4.SelectedItem + "' " +
                "ORDER BY studyForm.id;");
            comboBox5.Items.AddRange(studyForms.ToArray());
            comboBox5.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i <= comboBox1.SelectedIndex; i++)
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

                List<string> departments = SQLClass.Select(
                    "SELECT DISTINCT departmentName FROM department " +
                    "INNER JOIN faculty ON department.facultyId = faculty.id " +
                    "INNER JOIN specializationOffer ON department.id = specializationOffer.departmentId " +
                    "INNER JOIN specializationCode ON specializationOffer.sCodeId = specializationCode.id " +
                    "INNER JOIN degree ON specializationCode.degreeId =degree.id " +
                    "WHERE degree = '" + comboBox1.SelectedItem + "' " +
                    "AND facultyName = '" + comboBox2.SelectedItem + "' " +
                    "ORDER BY department.id;");
                comboBox3.Items.Clear();
                comboBox3.Items.AddRange(departments.ToArray());
                comboBox3.SelectedIndex = 0;

                List<string> specializations = SQLClass.Select(
                    "SELECT DISTINCT specializationName FROM specialization " +
                    "INNER JOIN specializationCode ON specialization.id = specializationCode.specializationId " +
                    "INNER JOIN degree ON specializationCode.degreeId =degree.id " +
                    "INNER JOIN specializationOffer ON specializationCode.id = specializationOffer.sCodeId " +
                    "INNER JOIN department ON specializationOffer.departmentId = department.id " +
                    "INNER JOIN faculty ON department.facultyId = faculty.id " +
                    "WHERE degree = '" + comboBox1.SelectedItem + "' " +
                    "AND facultyName = '" + comboBox2.SelectedItem + "' " +
                    "AND departmentName = '" + comboBox3.SelectedItem + "' " +
                    "ORDER BY specializationId;");
                comboBox4.Items.Clear();
                comboBox4.Items.AddRange(specializations.ToArray());
                comboBox4.SelectedIndex = 0;

                List<string> studyForms = SQLClass.Select(
                    "SELECT DISTINCT form FROM studyForm " +
                    "INNER JOIN specializationOffer ON studyForm.id = specializationOffer.studyFormId " +
                    "INNER JOIN department ON specializationOffer.departmentId = department.id " +
                    "INNER JOIN faculty ON department.facultyId = faculty.id " +
                    "INNER JOIN specializationCode ON specializationOffer.sCodeId = specializationCode.id " +
                    "INNER JOIN degree ON specializationCode.degreeId = degree.id " +
                    "INNER JOIN specialization ON specializationCode.specializationId = specialization.id " +
                    "WHERE degree = '" + comboBox1.SelectedItem + "' " +
                    "AND facultyName = '" + comboBox2.SelectedItem + "' " +
                    "AND departmentName = '" + comboBox3.SelectedItem + "' " +
                    "AND specializationName = '" + comboBox4.SelectedItem + "' " +
                    "ORDER BY studyForm.id;");
                comboBox5.Items.Clear();
                comboBox5.Items.AddRange(studyForms.ToArray());
                comboBox5.SelectedIndex = 0;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i <= comboBox2.SelectedIndex; i++)
            {
                List<string> departments = SQLClass.Select(
                    "SELECT DISTINCT departmentName FROM department " +
                    "INNER JOIN faculty ON department.facultyId = faculty.id " +
                    "INNER JOIN specializationOffer ON department.id = specializationOffer.departmentId " +
                    "INNER JOIN specializationCode ON specializationOffer.sCodeId = specializationCode.id " +
                    "INNER JOIN degree ON specializationCode.degreeId =degree.id " +
                    "WHERE degree = '" + comboBox1.SelectedItem + "' " +
                    "AND facultyName = '" + comboBox2.SelectedItem + "' " +
                    "ORDER BY department.id;");
                comboBox3.Items.Clear();
                comboBox3.Items.AddRange(departments.ToArray());
                comboBox3.SelectedIndex = 0;

                List<string> specializations = SQLClass.Select(
                    "SELECT DISTINCT specializationName FROM specialization " +
                    "INNER JOIN specializationCode ON specialization.id = specializationCode.specializationId " +
                    "INNER JOIN degree ON specializationCode.degreeId =degree.id " +
                    "INNER JOIN specializationOffer ON specializationCode.id = specializationOffer.sCodeId " +
                    "INNER JOIN department ON specializationOffer.departmentId = department.id " +
                    "INNER JOIN faculty ON department.facultyId = faculty.id " +
                    "WHERE degree = '" + comboBox1.SelectedItem + "' " +
                    "AND facultyName = '" + comboBox2.SelectedItem + "' " +
                    "AND departmentName = '" + comboBox3.SelectedItem + "' " +
                    "ORDER BY specializationId;");
                comboBox4.Items.Clear();
                comboBox4.Items.AddRange(specializations.ToArray());
                comboBox4.SelectedIndex = 0;

                List<string> studyForms = SQLClass.Select(
                    "SELECT DISTINCT form FROM studyForm " +
                    "INNER JOIN specializationOffer ON studyForm.id = specializationOffer.studyFormId " +
                    "INNER JOIN department ON specializationOffer.departmentId = department.id " +
                    "INNER JOIN faculty ON department.facultyId = faculty.id " +
                    "INNER JOIN specializationCode ON specializationOffer.sCodeId = specializationCode.id " +
                    "INNER JOIN degree ON specializationCode.degreeId = degree.id " +
                    "INNER JOIN specialization ON specializationCode.specializationId = specialization.id " +
                    "WHERE degree = '" + comboBox1.SelectedItem + "' " +
                    "AND facultyName = '" + comboBox2.SelectedItem + "' " +
                    "AND departmentName = '" + comboBox3.SelectedItem + "' " +
                    "AND specializationName = '" + comboBox4.SelectedItem + "' " +
                    "ORDER BY studyForm.id;");
                comboBox5.Items.Clear();
                comboBox5.Items.AddRange(studyForms.ToArray());
                comboBox5.SelectedIndex = 0;
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i <= comboBox3.SelectedIndex; i++)
            {
                List<string> specializations = SQLClass.Select(
                    "SELECT DISTINCT specializationName FROM specialization " +
                    "INNER JOIN specializationCode ON specialization.id = specializationCode.specializationId " +
                    "INNER JOIN degree ON specializationCode.degreeId =degree.id " +
                    "INNER JOIN specializationOffer ON specializationCode.id = specializationOffer.sCodeId " +
                    "INNER JOIN department ON specializationOffer.departmentId = department.id " +
                    "INNER JOIN faculty ON department.facultyId = faculty.id " +
                    "WHERE degree = '" + comboBox1.SelectedItem + "' " +
                    "AND facultyName = '" + comboBox2.SelectedItem + "' " +
                    "AND departmentName = '" + comboBox3.SelectedItem + "' " +
                    "ORDER BY specializationId;");
                comboBox4.Items.Clear();
                comboBox4.Items.AddRange(specializations.ToArray());
                comboBox4.SelectedIndex = 0;

                List<string> studyForms = SQLClass.Select(
                    "SELECT DISTINCT form FROM studyForm " +
                    "INNER JOIN specializationOffer ON studyForm.id = specializationOffer.studyFormId " +
                    "INNER JOIN department ON specializationOffer.departmentId = department.id " +
                    "INNER JOIN faculty ON department.facultyId = faculty.id " +
                    "INNER JOIN specializationCode ON specializationOffer.sCodeId = specializationCode.id " +
                    "INNER JOIN degree ON specializationCode.degreeId = degree.id " +
                    "INNER JOIN specialization ON specializationCode.specializationId = specialization.id " +
                    "WHERE degree = '" + comboBox1.SelectedItem + "' " +
                    "AND facultyName = '" + comboBox2.SelectedItem + "' " +
                    "AND departmentName = '" + comboBox3.SelectedItem + "' " +
                    "AND specializationName = '" + comboBox4.SelectedItem + "' " +
                    "ORDER BY studyForm.id;");
                comboBox5.Items.Clear();
                comboBox5.Items.AddRange(studyForms.ToArray());
                comboBox5.SelectedIndex = 0;
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i <= comboBox4.SelectedIndex; i++)
            {
                List<string> studyForms = SQLClass.Select(
                    "SELECT DISTINCT form FROM studyForm " +
                    "INNER JOIN specializationOffer ON studyForm.id = specializationOffer.studyFormId " +
                    "INNER JOIN department ON specializationOffer.departmentId = department.id " +
                    "INNER JOIN faculty ON department.facultyId = faculty.id " +
                    "INNER JOIN specializationCode ON specializationOffer.sCodeId = specializationCode.id " +
                    "INNER JOIN degree ON specializationCode.degreeId = degree.id " +
                    "INNER JOIN specialization ON specializationCode.specializationId = specialization.id " +
                    "WHERE degree = '" + comboBox1.SelectedItem + "' " +
                    "AND facultyName = '" + comboBox2.SelectedItem + "' " +
                    "AND departmentName = '" + comboBox3.SelectedItem + "' " +
                    "AND specializationName = '" + comboBox4.SelectedItem + "' " +
                    "ORDER BY studyForm.id;");
                comboBox5.Items.Clear();
                comboBox5.Items.AddRange(studyForms.ToArray());
                comboBox5.SelectedIndex = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<string> id = SQLClass.Select("SELECT id FROM entrant WHERE login = '" + userLogin + "';");

            List<string> sOfferId = SQLClass.Select(
                "SELECT specializationOffer.id FROM specializationOffer " +
                "INNER JOIN department ON specializationOffer.departmentId = department.id " +
                "INNER JOIN faculty ON department.facultyId = faculty.id " +
                "INNER JOIN specializationCode ON specializationOffer.sCodeId = specializationCode.id " +
                "INNER JOIN specialization ON specializationCode.specializationId = specialization.id " +
                "INNER JOIN degree ON specializationCode.degreeId = degree.id " +
                "INNER JOIN studyForm ON specializationOffer.studyFormId = studyForm.id " +
                "WHERE degree = '" + comboBox1.SelectedItem + "' " +
                "AND facultyName = '" + comboBox2.SelectedItem + "' " +
                "AND departmentName = '" + comboBox3.SelectedItem + "' " +
                "AND specializationName = '" + comboBox4.SelectedItem + "' " +
                "AND form = '" + comboBox5.SelectedItem + "';");

            
            var recordingDate = DateTime.Today.ToString("yyyy-MM-dd");
            var entrantId = id[0];
            var offerId = sOfferId[0];

            string query = $"INSERT INTO admission(recordingDate, entrantId, specializationOfferId) VALUES( '{recordingDate}', '{entrantId}', '{offerId}');";

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
