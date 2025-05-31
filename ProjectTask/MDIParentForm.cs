using System;
using System.Windows.Forms;

namespace ProjectTask
{
    public partial class MDIParentForm : Form
    {
        public MDIParentForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
        }

        private void ShowLogin()
        {
            using (LoginForm lf = new LoginForm())
            {
                lf.ShowDialog();
                if (!lf.LoginName.Equals(string.Empty))
                {
                    MessageBox.Show($"\nWelcome, {lf.LoginName}\n\nTimeIN: {DateTime.Now:yyyy-MM-dd hh:mm:ss tt}\n\n", 
                        "System Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Environment.Exit(0);
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void enrolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmEnrollment enrollment = new frmEnrollment();
            enrollment.MdiParent = this;
            enrollment.Show();
        }

        private void studentListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmStudentView enrollForm = new frmStudentView();
            enrollForm.MdiParent = this;
            enrollForm.Show();
        }

        private void MDIParentForm_Shown(object sender, EventArgs e)
        {
            ShowLogin();
        }
    }
}
