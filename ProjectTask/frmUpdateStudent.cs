using ProjectTask.Properties;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ProjectTask
{
    public partial class frmUpdateStudent : Form
    {
        StudentController sc;
        List<ProgramInfo> programList = new List<ProgramInfo>();
        public frmUpdateStudent()
        {
            InitializeComponent();
            StudentRepository repository = new StudentRepository(Settings.Default.connString);
            sc = new StudentController(repository);
            this.StartPosition = FormStartPosition.CenterScreen;
        }
        public void LoadData(string studentId, string firstName, string middleName, string lastName,
            string v, string address, string contactNumber, string yearLevel,
            string programName, string firstName1, string middleName1, string lastName1,
            string address1, string contactNumber1, string relationship, bool birthCertificate,
            bool tOR, bool goodMoral, decimal scholarshipDiscount, decimal amountPaid, string method)
        {
            ShowData(studentId, firstName, middleName, lastName, v, address, contactNumber, yearLevel, programName, firstName1,
                middleName1, lastName1, address1, contactNumber1, relationship, birthCertificate, tOR, goodMoral, scholarshipDiscount,
                amountPaid, method);
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                Student currentStudent = sc.GetStudentById(lblStudNumber.Text);
                if (currentStudent == null)
                {
                    MessageBox.Show("Student not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Guardian guardian = new Guardian(currentStudent.Guardian.Id, txtGuardianFirstName.Text, txtGuardianMiddleName.Text,
                                                    txtGuardianLastName.Text, txtRelationship.Text,
                                                    txtGuardianAddress.Text, txtGuardianContact.Text);

                Requirements requirements = new Requirements();
                requirements.Id = currentStudent.Requirements.Id;
                requirements.BirthCertificate = chkBirthCertificate.Checked;
                requirements.TOR = chkTranscript.Checked;
                requirements.GoodMoral = chkGoodMoral.Checked;


                decimal selectedValue = cmbProgram.SelectedValue != null ? (decimal)cmbProgram.SelectedValue : 0;
                ProgramInfo programInfo = new ProgramInfo(currentStudent.Program.Id, cmbProgram.Text, selectedValue);

                string modeOfPayment = string.Empty;
                if (rbCash.Checked)
                    modeOfPayment = rbCash.Text;
                else if (rbInstallment.Checked)
                    modeOfPayment = rbInstallment.Text;
                else
                    modeOfPayment = string.Empty;

                decimal amountToPay = !string.IsNullOrWhiteSpace(txtAmountPay.Text) ? decimal.Parse(txtAmountPay.Text) : 0;
                Payment payment = new Payment(currentStudent.Payment.Id, amountToPay, modeOfPayment, GetDiscount());




                Student student = new Student(lblStudNumber.Text, txtFirstName.Text, txtLastName.Text,
                                            txtMiddleName.Text, dtpDOB.Value, programInfo, guardian, requirements,
                                            payment, txtContact.Text, txtAddress.Text, cmbYearLevel.Text);

                DialogResult dr = MessageBox.Show("Confirm Changes?", "Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                if(dr == DialogResult.Yes)
                {
                    sc.UpdateStudentRegistration(student);
                }
                else
                {
                    MessageBox.Show("Student not updated!");
                    return;
                }

                
                MessageBox.Show("Student information updated successfully!", "Update Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshView();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Updating Student", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private void ShowDiscountAmount()
        {
            decimal discount = GetDiscount();
            decimal tuitionFee = (decimal)cmbProgram.SelectedValue;
            decimal discountedTuition = tuitionFee * discount;
            lblDiscountedAmount.Text = $"₱{tuitionFee - discountedTuition:N2}";
        }

        private decimal GetDiscount()
        {
            decimal discount = 0;
            if (rb25.Checked)
            {
                discount = .25m;
            }
            else if (rb50.Checked)
            {
                discount = .50m;
            }
            else if (rbFullScholar.Checked)
            {
                discount = 1m;
            }

            return discount;
        }
        private void ShowData(string studentId, string firstName, string middleName, string lastName,
            string v, string address, string contactNumber, string yearLevel,
            string programName, string firstName1, string middleName1, string lastName1,
            string address1, string contactNumber1, string relationship, bool birthCertificate,
            bool tOR, bool goodMoral, decimal scholarshipDiscount, decimal amountPaid, string method)
        {
            programList = sc.GetAllPrograms();

            cmbProgram.DisplayMember = "ProgramName";
            cmbProgram.ValueMember = "TuitionFee";
            cmbProgram.DataSource = null;
            cmbProgram.DataSource = programList;

            lblStudNumber.Text = studentId;
            txtFirstName.Text = firstName;
            txtMiddleName.Text = middleName;
            txtLastName.Text = lastName;

            dtpDOB.Value = DateTime.Parse(v);
            txtAddress.Text = address;
            txtContact.Text = contactNumber;
            cmbYearLevel.Text = yearLevel;
            cmbProgram.Text = programName;

            txtGuardianFirstName.Text = firstName1;
            txtGuardianMiddleName.Text = middleName1;
            txtGuardianLastName.Text = lastName1;
            txtGuardianAddress.Text = address1;

            txtGuardianContact.Text = contactNumber1;
            txtRelationship.Text = relationship;
            chkBirthCertificate.Checked = birthCertificate;
            chkTranscript.Checked = tOR;
            chkGoodMoral.Checked = goodMoral;

            if (cmbProgram.SelectedIndex != -1)
            {
                decimal tuition = (decimal)cmbProgram.SelectedValue;
                lblTuitionFee.Text = $"₱{tuition:N2}";
                EnableDiscountRB();
                ShowDiscountAmount();
            }

            if (scholarshipDiscount == 0)
            {
                rbCash.Checked = true;
            }
            else if (scholarshipDiscount == .25m)
            {
                rb25.Checked = true;
            }
            else if (scholarshipDiscount == .50m)
            {
                rb50.Checked = true;
            }
            else if (scholarshipDiscount == 1)
            {
                rbFullScholar.Checked = true;
            }
            txtAmountPay.Text = amountPaid.ToString("N2");
            if (method == "Cash")
                rbCash.Checked = true;
            else if (method == "Installment")
                rbInstallment.Checked = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            frmStudentView sv = new frmStudentView();
            sv.MdiParent = this.MdiParent;
            sv.Show();
            this.Close();
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

        private void cmbProgram_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProgram.SelectedIndex != -1)
            {
                decimal tuition = (decimal)cmbProgram.SelectedValue;
                lblTuitionFee.Text = $"₱{tuition:N2}";
                ShowDiscountAmount();
                EnableDiscountRB();
            }
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
    }
}
