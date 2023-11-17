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
    public partial class Benefits : Form
    {
        public string userLogin = string.Empty;

        public Benefits()
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
            valueCol.DataPropertyName = "benefit";
            valueCol.HeaderText = "Привилегия";
            List<string> benefits = SQLClass.Select("SELECT `privilege` FROM `benefits` ORDER BY `id`");
            valueCol.DataSource = benefits;
            dataGridView1.Columns.Add(valueCol);
            dataGridView1.Columns.Add("IsNew", String.Empty);
        }

        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            var fullName = record.GetString(1) + " " + record.GetString(2) + " " + record.GetString(3);
            dgw.Rows.Add(record.GetInt32(0), fullName, record.GetString(4));
        }

        private void RefreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string query = "SELECT `beneficiaries`.`id`, `surname`, `firstname`, `patronymic`, `privilege` FROM `beneficiaries` " +
                "INNER JOIN `entrant` ON `beneficiaries`.`entrantId`=`entrant`.`id` " +
                "INNER JOIN `benefits` ON `beneficiaries`.`benefitsId`=`benefits`.`id` " +
                "ORDER BY `entrant`.`id`;";

            MySqlCommand command = new MySqlCommand(query, SQLClass.conn);
            DbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }

            reader.Close();
        }

        private void Benefits_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns["entrant"].ReadOnly = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddBeneficiary rf = new AddBeneficiary();
            this.Hide();
            rf.ShowDialog();
            this.Show();

            RefreshDataGrid(dataGridView1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var selectedRowIndex = dataGridView1.CurrentCell.RowIndex;
            var id = dataGridView1.Rows[selectedRowIndex].Cells[0].Value.ToString();

            var delQuery = $"DELETE FROM `beneficiaries` WHERE id = {id}";
            var command = new MySqlCommand(delQuery, SQLClass.conn);
            command.ExecuteNonQuery();

            RefreshDataGrid(dataGridView1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                var id = dataGridView1.Rows[index].Cells[0].Value.ToString();
                var privilege = dataGridView1.Rows[index].Cells[2].Value.ToString();

                if (id != String.Empty)
                {
                    List<string> query = SQLClass.Select("SELECT `id` FROM `benefits` WHERE `privilege` = '" + privilege + "';");
                    var benefitsId = query[0];

                    var changeQuery = $"UPDATE `beneficiaries` SET `benefitsId` = '{benefitsId}' WHERE id = '{id}';";

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

        private void Xbutton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
