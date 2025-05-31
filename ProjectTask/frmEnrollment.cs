using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ProjectTask.Properties;

namespace ProjectTask
{
    public partial class frmEnrollment : Form
    {
        StudentController studentController;
        List<ProgramInfo> programList = new List<ProgramInfo>();
        public frmEnrollment()
        {
            InitializeComponent();
            StudentRepository repo = new StudentRepository(Settings.Default.connString);
            studentController = new StudentController(repo);
            dtpDOB.MaxDate = DateTime.Now;
            this.StartPosition = FormStartPosition.CenterScreen;
        }
        private void frmEnrollment_Load(object sender, EventArgs e)
        {
            programList = studentController.GetAllPrograms();

            cmbProgram.DisplayMember = "ProgramName";
            cmbProgram.ValueMember = "TuitionFee";
            cmbProgram.DataSource = null;
            cmbProgram.DataSource = programList;

            ClearAllInputs(this);
        }
        private void cmbProgram_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProgram.SelectedIndex != -1) 
            {
                decimal tuition = (decimal)cmbProgram.SelectedValue;
                lblTuitionFee.Text = $"₱{tuition:N2}";
            }
            EnableDiscountRB();
        }

        private void EnableDiscountRB()
        {
            rb25.Enabled = cmbProgram.SelectedIndex != -1;
            rb50.Enabled = cmbProgram.SelectedIndex != -1;
            rbFullScholar.Enabled = cmbProgram.SelectedIndex != -1;

            if (cmbProgram.SelectedIndex == -1)
            {
                lblDiscountedAmount.Text = lblTuitionFee.Text = "₱0.00";
            }
            else
            {
                ShowDiscountAmount();
            }
        }
        private void DisableDiscountRB()
        {
            rb25.Enabled = false;
            rb50.Enabled = false;
            rbFullScholar.Enabled = false;
        }

        private void rb25_CheckedChanged(object sender, EventArgs e)
        {
            ShowDiscountAmount();
        }

        private void rb50_CheckedChanged(object sender, EventArgs e)
        {
            ShowDiscountAmount();
        }

        private void rbFullScholar_CheckedChanged(object sender, EventArgs e)
        {
            ShowDiscountAmount();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearAllInputs(this);
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                    Guardian guardian = new Guardian(0, txtGuardianFirstName.Text, txtGuardianMiddleName.Text,
                                                    txtGuardianLastName.Text, txtRelationship.Text,
                                                    txtGuardianAddress.Text, txtGuardianContact.Text);

                    Requirements requirements = new Requirements();
                    requirements.BirthCertificate = chkBirthCertificate.Checked;
                    requirements.TOR = chkTranscript.Checked;
                    requirements.GoodMoral = chkGoodMoral.Checked;


                    decimal selectedValue = cmbProgram.SelectedValue != null ? (decimal)cmbProgram.SelectedValue : 0;
                    ProgramInfo programInfo = new ProgramInfo(0, cmbProgram.Text, selectedValue);

                    string modeOfPayment = string.Empty;
                if (rbCash.Checked)
                    modeOfPayment = rbCash.Text;
                else if (rbInstallment.Checked)
                    modeOfPayment = rbInstallment.Text;
                else
                    modeOfPayment = string.Empty;

                    decimal amountToPay = !string.IsNullOrWhiteSpace(txtAmountPay.Text) ? decimal.Parse(txtAmountPay.Text) : 0;
                    Payment payment = new Payment(0, amountToPay, modeOfPayment, GetDiscount());


                    Student student = new Student(lblStudNumber.Text, txtFirstName.Text, txtLastName.Text,
                                                txtMiddleName.Text, dtpDOB.Value, programInfo, guardian, requirements,
                                                payment, txtContact.Text, txtAddress.Text, cmbYearLevel.Text);

                    studentController.SaveStudentRegistration(student);
                    MessageBox.Show(student.DisplayInfo(), "Student Registered", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshView();
                ClearAllInputs(this);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Something went wrong...(Registering Student)", MessageBoxButtons.OK, MessageBoxIcon.Error); }

        }
        private void RefreshView()
        {
            if (this.MdiParent != null)
            {
                foreach (Form form in this.MdiParent.MdiChildren)
                {
                    if (form is frmStudentView studentView)
                    {
                        studentView.Show();
                        studentView.Activate();
                        break;
                    }
                }
            }
        }

        private decimal GetDiscount()
        {
            decimal discount = 0;
            if (rb25.Checked)
            {
                discount = .25m;
            }else if (rb50.Checked)
            {
                discount = .50m;
            }else if (rbFullScholar.Checked)
            {
                discount = 1m;
            }

            return discount;
        }

        private void ClearAllInputs(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is TextBox tb)
                    tb.Clear();
                else if (ctrl is ComboBox cb)
                    cb.SelectedIndex = -1;
                else if (ctrl is CheckBox chk)
                    chk.Checked = false;
                else if (ctrl is RadioButton rb)
                    rb.Checked = false;

                else if (ctrl is DateTimePicker dtp)
                {
                    int currentYear = DateTime.Now.Year;
                    dtp.MaxDate = DateTime.Now.AddYears(-15);
                    dtp.MinDate = DateTime.Now.AddYears(-100);

                    dtp.Value = dtp.MaxDate;
                }

                if(ctrl.HasChildren)
                    ClearAllInputs(ctrl);
            }
            DisableDiscountRB();
            lblDiscountedAmount.Text = lblTuitionFee.Text = "₱0.00";
            lblStudNumber.Text = studentController.GenerateNewStudentId();
        }

        private void ShowDiscountAmount()
        {
            decimal discount = GetDiscount();

            decimal tuitionFee = cmbProgram.SelectedValue != null ? (decimal)cmbProgram.SelectedValue : 0;

            decimal discountedTuition = tuitionFee * discount;
            lblDiscountedAmount.Text = $"₱{tuitionFee-discountedTuition:N2}";
        }

        
    }
}
