using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ProjectTask
{
    public partial class LoginForm : Form
    {
        public string LoginName { get; set; } = string.Empty;
        public LoginForm()
        {
            InitializeComponent();

            if (Debugger.IsAttached)
            {
                txtUsername.Text = "Admin";
                txtPassword.Text = "password";
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            //Declare and Instantiate an object
            Authentication auth = new Authentication(txtUsername.Text, txtPassword.Text);
            
            bool isValid = auth.Authenticate();
            if (isValid)
            {
                LoginName = txtUsername.Text;
                Close();
                //MDIParentForm mdi = new MDIParentForm();
                //mdi.Show();
                //this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid Username or Password!", "Invalid!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
