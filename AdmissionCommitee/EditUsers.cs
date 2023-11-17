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
    public partial class EditUsers : Form
    {
        public string userLogin = string.Empty;

        public EditUsers()
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
            dataGridView1.Columns.Add("surname", "Фамилия");
            dataGridView1.Columns.Add("firstname", "Имя");
            dataGridView1.Columns.Add("patronymic", "Отчество");
            var valueCol = new DataGridViewComboBoxColumn { DisplayStyleForCurrentCellOnly = true };
            valueCol.DataPropertyName = "citizenship";
            valueCol.HeaderText = "Гражданство";
            var countries = new List<string>() { "РФ", "Республика Беларусь", "Республика Казахстан", "Республика Грузия", "Республика Армения", "Монгольская Народная Республика",
                "КНР", "Азербайджанская Республика", "Республика Узбекистан", "Республика Таджикистан", "Кыргызская Республика", "Республика Туркменистан", "Республика Сербская",
                "Республика Болгария", "Республика Молдова", "Приднестровская Молдавская Республика"};
            valueCol.DataSource = countries;
            dataGridView1.Columns.Add(valueCol);
            dataGridView1.Columns.Add("birthdate", "Дата рождения");
            dataGridView1.Columns.Add("login", "Логин");
            dataGridView1.Columns.Add("password", "Пароль");
            var checkColumn = new DataGridViewCheckBoxColumn();
            checkColumn.HeaderText = "IsAdmin";
            dataGridView1.Columns.Add(checkColumn);
        }

        private void ReadSingleRow(DataGridView dgw, IDataReader record)
        {
            dgw.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetString(2), record.GetString(3), record.GetString(4), record.GetValue(5), record.GetString(6), record.GetString(7), record.GetBoolean(8));
        }

        private void RefreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string query = "SELECT `id`, `surname`, `firstname`, `patronymic`, `citizenship`, DATE_FORMAT(`birthdate`, '%d.%m.%Y'), `login`, `password`, `is_admin` FROM `entrant` ORDER BY `id`;";

            MySqlCommand command = new MySqlCommand(query, SQLClass.conn);
            DbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }

            reader.Close();
        }

        private void EditUsers_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);

            comboBox1.SelectedIndex = -1;
            textBox5.PasswordChar = '*';
            textBox4.MaxLength = 45;
            textBox5.MaxLength = 45;

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            this.dataGridView1.CellFormatting += new DataGridViewCellFormattingEventHandler(this.dataGridView1_CellFormatting);
            dataGridView1.Columns["id"].ReadOnly = true;
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (this.dataGridView1.Columns[e.ColumnIndex].Index == 7 && e.Value != null)
            {
                dataGridView1.Rows[e.RowIndex].Tag = e.Value;
                e.Value = new String('*', e.Value.ToString().Length);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            var surname = textBox1.Text;
            var firstname = textBox2.Text;
            var patronymic = textBox3.Text;
            var citizenship = comboBox1.SelectedItem;
            var birthdate = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            var login = textBox4.Text;
            var password = textBox5.Text;

            if (checkuser())
            {
                return;
            }

            var addQuery = $"INSERT INTO entrant(surname, firstname, patronymic, citizenship, birthdate, login, password, is_admin) VALUES('{surname}','{firstname}','{patronymic}','{citizenship}','{birthdate}','{login}','{password}', 0);";

            MySqlCommand cmd = new MySqlCommand(addQuery, SQLClass.conn);

            if (cmd.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Аккаунт успешно создан!", "Success");

                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();
                textBox5.Clear();
                comboBox1.SelectedIndex = -1;
            }
            else
            {
                MessageBox.Show("Аккаунт не создан!");
            }

            RefreshDataGrid(dataGridView1);
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

        private void button2_Click(object sender, EventArgs e)
        {
            var selectedRowIndex = dataGridView1.CurrentCell.RowIndex;

            var id = Convert.ToInt32(dataGridView1.Rows[selectedRowIndex].Cells[0].Value);

            var delQuery = $"DELETE FROM entrant WHERE id = {id};";

            var command = new MySqlCommand(delQuery, SQLClass.conn);
            command.ExecuteNonQuery();

            RefreshDataGrid(dataGridView1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var selectedRowIndex = dataGridView1.CurrentCell.RowIndex;

            var username = dataGridView1.Rows[selectedRowIndex].Cells[6].Value;

            MySqlDataAdapter adapter = new MySqlDataAdapter();
            DataTable dt = new DataTable();
            string query = $"SELECT id, login FROM entrant WHERE login = '{username}'";

            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

            adapter.SelectCommand = cmd;
            adapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                MessageBox.Show("Пользователь уже существует!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                for (int index = 0; index < dataGridView1.Rows.Count; index++)
                {
                    var id = dataGridView1.Rows[index].Cells[0].Value.ToString();
                    var surname = dataGridView1.Rows[index].Cells[1].Value.ToString();
                    var firstname = dataGridView1.Rows[index].Cells[2].Value.ToString();
                    var patronymic = dataGridView1.Rows[index].Cells[3].Value;
                    var citizenship = dataGridView1.Rows[index].Cells[4].Value.ToString();
                    var birthdate = Convert.ToDateTime(dataGridView1.Rows[index].Cells[5].Value).ToString("yyyy-MM-dd");
                    var login = dataGridView1.Rows[index].Cells[6].Value;
                    var password = dataGridView1.Rows[index].Cells[7].Value.ToString();
                    var isadmin = Convert.ToInt32(dataGridView1.Rows[index].Cells[8].Value);

                    var changeQuery = $"UPDATE `entrant` SET `surname`='{surname}',`firstname`='{firstname}',`patronymic`='{patronymic}',`citizenship`='{citizenship}',`birthdate`='{birthdate}',`login`='{login}', `password`='{password}', `is_admin`='{isadmin}' WHERE `id`={id};";

                    var command = new MySqlCommand(changeQuery, SQLClass.conn);
                    command.ExecuteNonQuery();
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

        DateTimePicker dateTimePicker = new DateTimePicker();
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 5)
            {
                var index = dataGridView1.CurrentCell.RowIndex;

                dataGridView1.Controls.Add(dateTimePicker);
                dateTimePicker.Format = DateTimePickerFormat.Short;
                Rectangle rectangle = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                dateTimePicker.Size = new Size(rectangle.Width, rectangle.Height);
                dateTimePicker.Location = new Point(rectangle.X, rectangle.Y);
                dateTimePicker.CloseUp += new EventHandler(dateTimePicker_CloseUp);
                dateTimePicker.TextChanged += new EventHandler(dateTimePicker_OnTextChange);
                dateTimePicker.Visible = true;
                dateTimePicker.Value = Convert.ToDateTime(dataGridView1.Rows[index].Cells[5].Value);
            }
            else
            {
                dateTimePicker.Hide();
            }
        }

        private void dateTimePicker_OnTextChange(object sender, EventArgs e)
        {
            dataGridView1.CurrentCell.Value = dateTimePicker.Text.ToString();
        }
        void dateTimePicker_CloseUp(object sender, EventArgs e)
        {
            dateTimePicker.Visible = false;
        }

        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            dateTimePicker.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Visible = false;
                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                    if (dataGridView1.Rows[i].Cells[j].Value != null)
                        if (dataGridView1.Rows[i].Cells[j].Value.ToString().IndexOf(textBox6.Text, StringComparison.OrdinalIgnoreCase) != -1)
                        {
                            dataGridView1.Rows[i].Visible = true;
                            break;
                        }
            }

            textBox6.Clear();
        }

        private void Xbutton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
 
    }
}
