using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectTask
{
    public class StudentRepository : IStudentRepository
    {
        private readonly string _connectionString;
        public StudentRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public ArrayList GetAllStudents()
        {
            ArrayList students = new ArrayList();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql = @"SELECT stud.StudentId, stud.FirstName, stud.LastName, stud.MiddleName, stud.Address,
                stud.ContactNumber, stud.DateOfBirth,stud.YearLevel, stud.ProgramId, prog.ProgramName, prog.TuitionFee,
                guard.GuardianId, guard.FirstName, guard.LastName, guard.MiddleName, guard.Address, guard.ContactNumber,
                guard.Relationship, req.RequirementsId, req.BirthCertificate, req.TOR, req.GoodMoral, pay.PaymentId, 
                pay.AmountPaid, pay.Method, pay.ScholarshipDiscount
                FROM dbo.Student stud
                INNER JOIN dbo.Guardian guard ON stud.GuardianId = guard.GuardianId
                INNER JOIN dbo.ProgramInfo prog ON stud.ProgramId = prog.ProgramId
                INNER JOIN dbo.Requirements req ON stud.RequirementsId = req.RequirementsId
                INNER JOIN dbo.Payment pay ON stud.PaymentId = pay.PaymentId";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {     
                            Guardian guardian = new Guardian(reader.GetInt32(11),
                                    reader.GetString(12),
                                    reader.GetString(14),
                                    reader.GetString(13),
                                    reader.GetString(17),
                                    reader.GetString(15),
                                    reader.GetString(16));

                            Requirements requirements = new Requirements();
                            requirements.Id = reader.GetInt32(18);
                            requirements.BirthCertificate = reader.GetBoolean(19);
                            requirements.TOR = reader.GetBoolean(20);
                            requirements.GoodMoral = reader.GetBoolean(21);

                            ProgramInfo programInfo = new ProgramInfo(reader.GetInt32(8), reader.GetString(9),
                                                                      reader.GetDecimal(10));

                            Payment payment = new Payment(reader.GetInt32(22), reader.GetDecimal(23), reader.GetString(24), Convert.ToDecimal(reader.GetValue(25)));

                            Student student = new Student(reader.GetString(0), reader.GetString(1), reader.GetString(2),
                                                          reader.GetString(3), reader.GetDateTime(6), programInfo, guardian,
                                                          requirements, payment, reader.GetString(5), reader.GetString(4),
                                                          reader.GetString(7));

                            students.Add(student);
                        }
                    }
                }
            }
            return students;
        }

        public Student GetStudentById(string id)
        {
            Student student = null;

            string sql = @"SELECT stud.StudentId, stud.FirstName, stud.LastName, stud.MiddleName, stud.Address,
                stud.ContactNumber, stud.DateOfBirth,stud.YearLevel, stud.ProgramId, prog.ProgramName, prog.TuitionFee,
                guard.GuardianId, guard.FirstName, guard.LastName, guard.MiddleName, guard.Address, guard.ContactNumber,
                guard.Relationship, req.RequirementsId, req.BirthCertificate, req.TOR, req.GoodMoral, pay.PaymentId, 
                pay.AmountPaid, pay.Method, pay.ScholarshipDiscount
                FROM dbo.Student stud
                INNER JOIN dbo.Guardian guard ON stud.GuardianId = guard.GuardianId
                INNER JOIN dbo.ProgramInfo prog ON stud.ProgramId = prog.ProgramId
                INNER JOIN dbo.Requirements req ON stud.RequirementsId = req.RequirementsId
                INNER JOIN dbo.Payment pay ON stud.PaymentId = pay.PaymentId
            WHERE stud.StudentId = @StudentId";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentId", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Guardian guardian = new Guardian(reader.GetInt32(11),
                                                             reader.GetString(12),
                                                             reader.GetString(14),
                                                             reader.GetString(13),
                                                             reader.GetString(17),
                                                             reader.GetString(15),
                                                             reader.GetString(16));
                            Requirements requirements = new Requirements();
                            requirements.Id = reader.GetInt32(18);
                            requirements.BirthCertificate = reader.GetBoolean(19);
                            requirements.TOR = reader.GetBoolean(20);
                            requirements.GoodMoral = reader.GetBoolean(21);

                            ProgramInfo programInfo = new ProgramInfo(reader.GetInt32(8), reader.GetString(9),
                                                                      reader.GetDecimal(10));

                            Payment payment = new Payment(reader.GetInt32(22), reader.GetDecimal(23), reader.GetString(24), Convert.ToDecimal(reader.GetValue(25)));

                            student = new Student(reader.GetString(0), reader.GetString(1), reader.GetString(2),
                                                         reader.GetString(3), reader.GetDateTime(6), programInfo, guardian,
                                                         requirements, payment, reader.GetString(5), reader.GetString(4),
                                                         reader.GetString(7));
                        }
                    }
                }
            }
            return student;
        }
        public void SaveStudentRegistration(Student student)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    int guardianId = InsertGuardian(conn, transaction, student.Guardian);
                    int requirementsId = InsertRequirements(conn, transaction, student.Requirements);
                    int paymentId = InsertPayment(conn, transaction, student.Payment);
                    InsertStudent(conn, transaction, student, guardianId, requirementsId, paymentId);
                    InsertStudentRegistration(conn, transaction, student.StudentId, requirementsId);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    throw new Exception("Error saving student registration: " + ex.Message);
                }
            }
        }

        public void UpdateStudentRegistration(Student student)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    UpdateGuardian(conn, transaction, student.Guardian);
                    UpdateRequirements(conn, transaction, student.Requirements);
                    UpdatePayment(conn, transaction, student.Payment);
                    UpdateStudent(conn, transaction, student);

//                  UpdateStudentRegistration(student);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Error updating student registration: " + ex.Message);
                }
            }

        }

        public void DeleteStudent(string id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        string deleteStudentReg = "DELETE FROM dbo.StudentRegistration WHERE StudentId = @Id";
                        using (SqlCommand cmd = new SqlCommand(deleteStudentReg, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Id", id);

                            DialogResult dr = MessageBox.Show("This process is irreversible!", "Caution", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                            if (dr == DialogResult.Yes)
                            {
                                cmd.ExecuteNonQuery();

                                string deleteStudent = "DELETE FROM dbo.Student WHERE StudentId = @Id";
                                using (SqlCommand cmd2 = new SqlCommand(deleteStudent, conn, transaction))
                                {
                                    cmd2.Parameters.AddWithValue("@Id", id);
                                    int rowsAffected = cmd2.ExecuteNonQuery();

                                    if (rowsAffected == 0)
                                    {
                                        transaction.Rollback();
                                        MessageBox.Show("No student found with that ID.");
                                        return;
                                    }
                                    else
                                    {
                                        transaction.Commit();
                                        MessageBox.Show("Student deleted successfully.");
                                        return;
                                    }
                                }
                            }

                            else
                            {
                                transaction.Rollback();
                                MessageBox.Show("Student not deleted!");
                                return;
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Error deleting student registration: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting student registration: " + ex.Message);
            }
        }

        public List<ProgramInfo> GetAllPrograms()
        {
            List<ProgramInfo> programList = new List<ProgramInfo>();

            string sql = "SELECT ProgramId, ProgramName, TuitionFee FROM ProgramInfo";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ProgramInfo program = new ProgramInfo
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("ProgramId")),
                            ProgramName = reader.GetString(reader.GetOrdinal("ProgramName")),
                            TuitionFee = reader.GetDecimal(reader.GetOrdinal("TuitionFee"))
                        };
                        programList.Add(program);
                    }
                }
            }
            return programList;
        }

        public string GenerateNewStudentId()
        {
            string prefix = "084";
            string year = DateTime.Now.Year.ToString();
            int nextSequence = 1;

            string sql = @"
                SELECT TOP 1 StudentId
                FROM dbo.Student
                WHERE StudentId LIKE @YearFilter
                ORDER BY StudentId DESC";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@YearFilter", $"%-{year}-%");

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        string latestId = result.ToString();
                        string lastFour = latestId.Substring(latestId.Length - 4);
                        if (int.TryParse(lastFour, out int numericPart))
                        {
                            nextSequence = numericPart + 1; 
                        }
                    }
                }
            }

            return $"{prefix}-{year}-{nextSequence.ToString("D4")}";
        }

        private int InsertGuardian(SqlConnection conn, SqlTransaction tran, Guardian guardian)
        {
            string query = @"INSERT INTO Guardian (FirstName, LastName, MiddleName, Address, ContactNumber, Relationship)
                         OUTPUT INSERTED.GuardianId
                         VALUES (@FirstName, @LastName, @MiddleName, @Address, @ContactNumber, @Relationship)";
            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                SetupGuardianParameters(cmd, guardian);
                return (int)cmd.ExecuteScalar();
            }
        }

        private int InsertRequirements(SqlConnection conn, SqlTransaction tran, Requirements req)
        {
            string query = @"INSERT INTO Requirements (BirthCertificate, TOR, GoodMoral)
                         OUTPUT INSERTED.RequirementsId
                         VALUES (@BirthCertificate, @TOR, @GoodMoral)";

            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                SetupRequirementsParameters(cmd, req);
                return (int)cmd.ExecuteScalar();
            }
        }

        private int InsertPayment(SqlConnection conn, SqlTransaction tran, Payment pay)
        {
            string query = @"INSERT INTO Payment (AmountPaid,Method,ScholarshipDiscount)
                         OUTPUT INSERTED.PaymentId
                         VALUES (@AmountPaid,@Method,@ScholarshipDiscount)";

            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                SetupPaymentParameters(cmd, pay);
                return (int)cmd.ExecuteScalar();
            }
        }

        private void InsertStudent(SqlConnection conn, SqlTransaction tran, Student student, int guardianId, int requirementsId, int paymentId)
        {
            string query = @"INSERT INTO Student (StudentId, FirstName, LastName, MiddleName, Address, ContactNumber, DateOfBirth, ProgramId, 
                                GuardianId, RequirementsId, PaymentId, YearLevel)
                         VALUES (@StudentId, @FirstName, @LastName, @MiddleName, @Address, @ContactNumber, @DOB, @ProgramId, @GuardianId, @RequirementsId, 
                                @PaymentId, @YearLevel)";

            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                SetupStudentParameters(conn, tran, cmd, student, guardianId, requirementsId, paymentId);
                cmd.ExecuteNonQuery();
            }
        }

        private void InsertStudentRegistration(SqlConnection conn, SqlTransaction tran, string studentId, int requirementsId)
        {
            string query = @"INSERT INTO StudentRegistration (StudentId, RequirementsId, IsValid)
                         VALUES (@StudentId, @RequirementsId, @IsValid)";

            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                cmd.Parameters.AddWithValue("@StudentId", studentId);
                cmd.Parameters.AddWithValue("@RequirementsId", requirementsId);
                cmd.Parameters.AddWithValue("@IsValid", true);

                cmd.ExecuteNonQuery();
            }
        }

        private int GetProgramId(SqlConnection conn, SqlTransaction tran, string programName)
        {
            string query = "SELECT ProgramId FROM ProgramInfo WHERE ProgramName = @Name";
            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                cmd.Parameters.AddWithValue("@Name", programName);
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : throw new Exception("Program not found.");
            }
        }

        private void UpdateGuardian(SqlConnection conn, SqlTransaction transaction, Guardian guardian)
        {
            string sql = @"UPDATE Guardian
                   SET FirstName = @FirstName,
                       LastName = @LastName,
                       MiddleName = @MiddleName,
                       ContactNumber = @ContactNumber,
                       Address = @Address,
                       Relationship = @Relationship
                   WHERE GuardianId = @GuardianId";

            using (SqlCommand cmd = new SqlCommand(sql, conn, transaction))
            {
                SetupGuardianParameters(cmd, guardian);
                cmd.Parameters.AddWithValue("@GuardianId", guardian.Id);
                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateRequirements(SqlConnection conn, SqlTransaction transaction, Requirements requirements)
        {
            string sql = @"UPDATE Requirements
                   SET BirthCertificate = @BirthCertificate,
                       TOR = @TOR,
                       GoodMoral = @GoodMoral
                   WHERE RequirementsId = @RequirementsId";

            using (SqlCommand cmd = new SqlCommand(sql, conn, transaction))
            {
                SetupRequirementsParameters(cmd, requirements);
                cmd.Parameters.AddWithValue("@RequirementsId", requirements.Id);
                cmd.ExecuteNonQuery();
            }
        }

        private void UpdatePayment(SqlConnection conn, SqlTransaction transaction, Payment payment)
        {
            string sql = @"UPDATE Payment
                   SET AmountPaid = @AmountPaid,
                       Method = @Method,
                       ScholarshipDiscount = @ScholarshipDiscount
                   WHERE PaymentId = @PaymentId";

            //PaymentDate = @PaymentDate,

            using (SqlCommand cmd = new SqlCommand(sql, conn, transaction))
            {
                SetupPaymentParameters(cmd, payment);
                cmd.Parameters.AddWithValue("@PaymentId", payment.Id);

                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateStudent(SqlConnection conn, SqlTransaction transaction, Student student)
        {
            string sql = @"UPDATE Student
                   SET FirstName = @FirstName,
                       LastName = @LastName,
                       MiddleName = @MiddleName,
                       Address = @Address,
                       ContactNumber = @ContactNumber,
                       DateOfBirth = @DOB,
                       ProgramId = @ProgramId,
                       GuardianId = @GuardianId,
                       RequirementsId = @RequirementsId,
                       PaymentId = @PaymentId,
                       YearLevel = @YearLevel
                   WHERE StudentId = @StudentId";

            using (SqlCommand cmd = new SqlCommand(sql, conn, transaction))
            {
                SetupStudentParameters(conn, transaction, cmd, student, student.Guardian.Id, student.Requirements.Id, student.Payment.Id);
                //cmd.Parameters.AddWithValue("@StudentId", student.StudentId);
                cmd.ExecuteNonQuery();
            }
        }

        private void SetupGuardianParameters(SqlCommand cmd, Guardian guardian)
        {
            cmd.Parameters.AddWithValue("@FirstName", guardian.FirstName);
            cmd.Parameters.AddWithValue("@LastName", guardian.LastName);
            cmd.Parameters.AddWithValue("@MiddleName", guardian.MiddleName);
            cmd.Parameters.AddWithValue("@Address", guardian.Address);
            cmd.Parameters.AddWithValue("@ContactNumber", guardian.ContactNumber);
            cmd.Parameters.AddWithValue("@Relationship", guardian.Relationship);
        }

        private void SetupRequirementsParameters(SqlCommand cmd, Requirements req)
        {
            cmd.Parameters.AddWithValue("@BirthCertificate", req.BirthCertificate);
            cmd.Parameters.AddWithValue("@TOR", req.TOR);
            cmd.Parameters.AddWithValue("@GoodMoral", req.GoodMoral);
        }

        private void SetupPaymentParameters(SqlCommand cmd, Payment pay)
        {
            cmd.Parameters.AddWithValue("@AmountPaid", pay.AmountPaid);
            cmd.Parameters.AddWithValue("@Method", pay.Method);
            cmd.Parameters.AddWithValue("@ScholarshipDiscount", pay.ScholarshipDiscount);
        }

        private void SetupStudentParameters(SqlConnection conn, SqlTransaction tran, SqlCommand cmd, Student student, int guardianId,
            int requirementsId, int paymentId)
        {
            //
            cmd.Parameters.AddWithValue("@StudentId", student.StudentId);
            cmd.Parameters.AddWithValue("@FirstName", student.FirstName);
            cmd.Parameters.AddWithValue("@LastName", student.LastName);
            cmd.Parameters.AddWithValue("@MiddleName", student.MiddleName ?? "");
            cmd.Parameters.AddWithValue("@Address", student.Address);
            cmd.Parameters.AddWithValue("@ContactNumber", student.ContactNumber);
            cmd.Parameters.AddWithValue("@DOB", student.DateOfBirth);
            cmd.Parameters.AddWithValue("@ProgramId", GetProgramId(conn, tran, student.Program.ProgramName));
            cmd.Parameters.AddWithValue("@GuardianId", guardianId);
            cmd.Parameters.AddWithValue("@RequirementsId", requirementsId);
            cmd.Parameters.AddWithValue("@PaymentId", paymentId);
            cmd.Parameters.AddWithValue("@YearLevel", student.YearLevel);
        }

        
    }
}
