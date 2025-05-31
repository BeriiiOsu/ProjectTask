using System;
using System.Collections;
using System.Linq;
using System.Windows.Forms;
using ProjectTask.Properties;

namespace ProjectTask
{
    public partial class frmStudentView : Form
    {

        StudentController studentController;
        ArrayList studentList = new ArrayList();
        private DataGridViewRow row;
        private string studentId;
        Student selectedStudent;
        public frmStudentView()
        {
            InitializeComponent();
            StudentRepository repo = new StudentRepository(Settings.Default.connString);
            studentController = new StudentController(repo);
            this.StartPosition = FormStartPosition.CenterScreen;
        }
        private void SetupDataGrid()
        {
            studentList = studentController.GetAllStudents();

            dgvStudent.DataSource = studentList;

            dgvStudent.Columns["Program"].Visible = false;
            dgvStudent.Columns["Guardian"].Visible = false;
            dgvStudent.Columns["Requirements"].Visible = false;
            dgvStudent.Columns["Payment"].Visible = false;

            dgvStudent.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            if (dgvStudent.Rows.Count > 0 && dgvStudent.Columns.Count > 0)
            {
                DataGridViewCellEventArgs args = new DataGridViewCellEventArgs(0, 0);
                dgridStudentList_CellClick(dgvStudent, args);
            }
        }
        private void dgridStudentList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) 
            {
                row = dgvStudent.Rows[e.RowIndex];

                studentId = row.Cells["StudentId"].Value?.ToString();
                selectedStudent = studentController.GetStudentById(studentId);


                lblStudNumberStudent.Text = selectedStudent.StudentId.ToString();
                txtFullnameStudent.Text = $"{selectedStudent.LastName}, {selectedStudent.FirstName} {selectedStudent.MiddleName}";
                txtDOBStudent.Text = selectedStudent.DateOfBirth.ToShortDateString();
                txtAddressStudent.Text = selectedStudent.Address;
                txtContactNumStudent.Text = selectedStudent.ContactNumber;
                txtYearLevelStudent.Text = selectedStudent.YearLevel;

                Guardian guardian = selectedStudent.Guardian;
                txtGuardianNameStudent.Text = $"{guardian.LastName}, {guardian.FirstName} {guardian.MiddleName}";
                txtGuardianAddressStudent.Text = guardian.Address;
                txtGuardianContactStudent.Text = guardian.ContactNumber;
                txtGuardianRelationshipStudent.Text = guardian.Relationship;

                ProgramInfo program = selectedStudent.Program;
                txtProgramStudent.Text = program.ProgramName;
                txtTuitionStudent.Text = $"₱ {program.TuitionFee:N2}";

                Requirements requirements = selectedStudent.Requirements;
                lblBCStudent.Text = requirements.BirthCertificate ? "✔ Birth Certificate" : "✘ Birth Ceritificate";
                lblTORStudent.Text = requirements.TOR ? "✔ Transcript of Records" : "✘ Transcript of Records";
                lblGoodMoralStudent.Text = requirements.TOR ? "✔ Good Moral" : "✘ Good Moral";

                Payment payment = selectedStudent.Payment;
                txtDiscountStudent.Text = $"{payment.ScholarshipDiscount * 100} %";

                txtDiscountedStudent.Text = $"₱ {program.TuitionFee * payment.ScholarshipDiscount:N2}";
                txtAmountPayStudent.Text = $"₱ {payment.AmountPaid:N2}";
                txtMethodStudent.Text = payment.Method;
            }
        }
        private void btnUpdate_Click(object sender, System.EventArgs e)
        {
            //var mdiParent = this.MdiParent as MDIParentForm;
            //if (mdiParent == null) return;

            //var existingForm = mdiParent.MdiChildren.FirstOrDefault(f => f is frmEnrollment) as frmEnrollment;
            //if (existingForm != null)
            //{
            //    existingForm.Close();
            //}
            frmUpdateStudent updateStudent = new frmUpdateStudent();
            updateStudent.MdiParent = this.MdiParent;
            updateStudent.Show();
            this.Hide();
            updateStudent.LoadData(selectedStudent.StudentId, selectedStudent.FirstName, selectedStudent.MiddleName, selectedStudent.LastName,
                selectedStudent.DateOfBirth.ToShortDateString(), selectedStudent.Address, selectedStudent.ContactNumber, selectedStudent.YearLevel,
                selectedStudent.Program.ProgramName, 
                
                selectedStudent.Guardian.FirstName, selectedStudent.Guardian.MiddleName, selectedStudent.Guardian.LastName,
                selectedStudent.Guardian.Address, selectedStudent.Guardian.ContactNumber, selectedStudent.Guardian.Relationship,
                selectedStudent.Requirements.BirthCertificate, selectedStudent.Requirements.TOR, selectedStudent.Requirements.GoodMoral,
                selectedStudent.Payment.ScholarshipDiscount, selectedStudent.Payment.AmountPaid, selectedStudent.Payment.Method

                );
        }

        private void frmStudentView_Load(object sender, System.EventArgs e)
        {
            SetupDataGrid();
        }

        private void FrmStudentView_Activated(object sender, System.EventArgs e)
        {
            SetupDataGrid();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(selectedStudent.StudentId))
            {
                studentController.DeleteStudent(selectedStudent.StudentId);
                SetupDataGrid();
            }
            else
            {
                MessageBox.Show("Please select a student to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        
    }
}
