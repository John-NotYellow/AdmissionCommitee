using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdmissionCommitee
{
    public partial class EnterProfile : Form
    {
        private readonly CheckAdmin _user;
        public EnterProfile(CheckAdmin user)
        {
            _user = user;
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;

            List<string> userData = SQLClass.Select("SELECT surname, firstname, patronymic, citizenship FROM entrant WHERE login = '" + user.Login + "';");

            foreach (string item in userData)
            {
                textBox1.Text += item + "\r\n";
            }

            List<string> userBirthday = SQLClass.Select("SELECT DATE_FORMAT(birthdate, '%d.%m.%Y') FROM entrant WHERE login = '" + user.Login + "';");
            textBox1.Text += userBirthday[0];

            List<string> IDocument = SQLClass.Select(
                "SELECT documentType, series, number, issuedBy, DATE_FORMAT(dateOfIssue, '%d.%m.%Y') FROM identityDocument INNER JOIN entrant ON identityDocument.entrantId = entrant.id WHERE login = '" + user.Login + "';");

            for (int i = 0; i < IDocument.Count; i += 5)
            {
                string doctype = IDocument[i];
                string series = IDocument[i+1];
                string number = IDocument[i+2];
                string issued = IDocument[i+3];
                string date = IDocument[i+4];
                textBox2.Text += "документ: " + doctype + "\r\n" + "серия: " + series + " №: " + number + "\r\n" + "выдан: " + issued + "\r\n" + "дата выдачи: " + date;
            }

            List<string> EDocument = SQLClass.Select(
                "SELECT documentType, series, number FROM educationDocument INNER JOIN entrant ON educationDocument.entrantId = entrant.id WHERE login = '" + user.Login + "';");

            for (int i = 0; i < EDocument.Count; i += 3)
            {
                string doctype = EDocument[i];
                string series = EDocument[i + 1];
                string number = EDocument[i + 2];
                textBox3.Text += "документ: " + doctype + "\r\n" + "серия: " + series + "\r\n" + "№: " + number;
            }

            List<string> admissionId = SQLClass.Select(
                "SELECT admission.id FROM admission INNER JOIN entrant ON admission.entrantId = entrant.id WHERE login = '" + user.Login + "';");

            foreach (string item in admissionId)
            {
                List<string> recordingDate = SQLClass.Select(
                    "SELECT DATE_FORMAT(recordingDate, '%d.%m.%Y') FROM admission WHERE id = '" + item + "';");

                string date = recordingDate[0];
                textBox4.Text += "Дата записи: " + date + "\r\n";

                List<string> admissionDetales = SQLClass.Select(
                "SELECT degree, facultyName, departmentName, specializationCode.id, specializationName, form FROM specializationOffer " +
                "INNER JOIN department ON specializationOffer.departmentId = department.id " +
                "INNER JOIN faculty ON department.facultyId = faculty.id " +
                "INNER JOIN specializationCode ON specializationOffer.sCodeId = specializationCode.id " +
                "INNER JOIN specialization ON specializationCode.specializationId = specialization.id " +
                "INNER JOIN degree ON specializationCode.degreeId = degree.id " +
                "INNER JOIN studyForm ON specializationOffer.studyFormId = studyForm.id " +
                "INNER JOIN admission ON specializationOffer.id = admission.specializationOfferId " +
                "WHERE admission.id = '" + item + "';");

                for (int i = 0; i <= admissionDetales.Count - 1; i++)
                {
                    if (i < admissionDetales.Count - 1) textBox4.Text += admissionDetales[i] + ", ";
                    else textBox4.Text += admissionDetales.Last() + ".\r\n";
                }
            }       

            List<string> benefits = SQLClass.Select(
                "SELECT privilege FROM beneficiaries " +
                "INNER JOIN benefits ON beneficiaries.benefitsId = benefits.id " +
                "INNER JOIN entrant ON beneficiaries.entrantId = entrant.id " +
                "WHERE login = '" + user.Login + "';");

            foreach (string item in benefits)
            {
                textBox6.Text += item + "\r\n";
            }

            List<string> exams = SQLClass.Select(
                "SELECT discipline, scores FROM passedExams " +
                "INNER JOIN exams ON passedExams.examsId = exams.id " +
                "INNER JOIN entrant ON passedExams.entrantId = entrant.id " +
                "WHERE login = '" + user.Login + "';");

            for (int i = 0; i < exams.Count; i += 2)
            {
                textBox5.Text += exams[i] + ": " + exams[i+1] +"\r\n";
            }

            foreach (string item in admissionId)
            {
                List<string> decision = SQLClass.Select(
                "SELECT comissionDecision FROM admission WHERE admission.id = '" + item + "';");

                string status = decision[0];

                if (status != string.Empty)
                {
                    List<string> decisionText = SQLClass.Select(
                    "SELECT sCodeId, comissionDecision, DATE_FORMAT(decisionDate, '%d.%m.%Y') FROM admission " +
                    "INNER JOIN specializationoffer ON admission.specializationOfferId = specializationoffer.id " +
                    "WHERE admission.id = '" + item + "';");

                    for (int y = 0; y < decisionText.Count; y += 3)
                    {
                        string sCode = decisionText[y];
                        string result = decisionText[y + 1];
                        string date = decisionText[y + 2];
                        textBox7.Text += "Код специальности: " + sCode + " - " + result + ", Дата решения: " + date + "\r\n";
                    }
                }

            }

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

        private void IsAdmin()
        {
            button7.Visible = _user.IsAdmin;
            button8.Visible = _user.IsAdmin;
            button9.Visible = _user.IsAdmin;
            button10.Visible = _user.IsAdmin;
            button11.Visible = _user.IsAdmin;
        }

        private void EnterProfile_Load(object sender, EventArgs e)
        {
            IsAdmin();
            List<string> iDocument = SQLClass.Select(
                "SELECT identityDocument.id FROM identityDocument INNER JOIN entrant ON identityDocument.entrantId = entrant.id WHERE login = '" + _user.Login + "';");
            if (iDocument.Count == 0)
            {
                button3.Enabled = false;
                button6.Enabled = false;
            }
            else
                button2.Enabled = false;

            List<string> eDocument = SQLClass.Select(
                "SELECT educationDocument.id FROM educationDocument INNER JOIN entrant ON educationDocument.entrantId = entrant.id WHERE login = '" + _user.Login + "';");
            if (eDocument.Count == 0)
            {
                button5.Enabled = false;
                button6.Enabled = false;
            }
            else
                button4.Enabled = false;

            List<string> aRequest = SQLClass.Select(
                "SELECT admission.id FROM admission INNER JOIN entrant ON admission.entrantId = entrant.id WHERE login = '" + _user.Login + "';");
            if (aRequest.Count >= 5)
                button6.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EditProfile rf = new EditProfile();
            List<string> citizenship = SQLClass.Select("SELECT citizenship FROM entrant WHERE login = '" + _user.Login + "';");
            rf.comboBox1.SelectedItem = citizenship[0];
            List<string> names = SQLClass.Select("SELECT surname, firstname, patronymic FROM entrant WHERE login = '" + _user.Login + "';");
            rf.textBox1.Text = names[0];
            rf.textBox2.Text = names[1];
            rf.textBox3.Text = names[2];
            List<string> birthday = SQLClass.Select("SELECT DATE_FORMAT(birthdate, '%d.%m.%Y') FROM entrant WHERE login = '" + _user.Login + "';");
            DateTime userBirthday = DateTime.Parse(birthday[0]);
            rf.dateTimePicker1.Value = userBirthday;
            rf.userLogin = _user.Login;
            this.Close();            
            rf.Show();
            rf.Activate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AddIDocument rf = new AddIDocument();
            rf.userLogin = _user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            EditIDocument rf = new EditIDocument();
            rf.userLogin = _user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AddEDocument rf = new AddEDocument();
            rf.userLogin = _user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            EditEDocument rf = new EditEDocument();
            rf.userLogin = _user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            AdmissionRequest rf = new AdmissionRequest();
            rf.userLogin = _user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Exams rf = new Exams();
            rf.userLogin = _user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Benefits rf = new Benefits();
            rf.userLogin = _user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }
        
        private void button9_Click(object sender, EventArgs e)
        {
            AddCommiteeDecision rf = new AddCommiteeDecision();
            rf.userLogin = _user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            EditDepartments rf = new EditDepartments();
            rf.userLogin = _user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            EditUsers rf = new EditUsers();
            rf.userLogin = _user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Form1 form = (Form1)Application.OpenForms["Form1"];
            if (form != null)
            {
                form.Show();
                this.Close();
            }
            else
                this.Close();
        }

        private void Xbutton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
