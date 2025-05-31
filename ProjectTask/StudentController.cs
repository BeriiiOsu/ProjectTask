using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ProjectTask
{
    public class StudentController
    {
        IStudentRepository studentRepository;
        public StudentController(IStudentRepository repo)
        {
            studentRepository = repo;
        }
        public void SaveStudentRegistration(Student student)
        {
            try
            {
                isValidated(student);


                studentRepository.SaveStudentRegistration(student);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void isValidated(Student student)
        {
            if (string.IsNullOrWhiteSpace(student.FirstName) ||
               string.IsNullOrWhiteSpace(student.LastName))
                throw new Exception("Please fill the student's name");

            if (string.IsNullOrWhiteSpace(student.Address))
                throw new Exception("Please fill the student's address");

            if (string.IsNullOrWhiteSpace(student.ContactNumber))
                throw new Exception("Please fill the student's contact number");

            //guardians info if null or empty
            if (string.IsNullOrWhiteSpace(student.Guardian.FirstName) ||
                string.IsNullOrWhiteSpace(student.Guardian.MiddleName) ||
                string.IsNullOrWhiteSpace(student.Guardian.LastName) ||
                string.IsNullOrWhiteSpace(student.Guardian.Relationship) ||
                string.IsNullOrWhiteSpace(student.Guardian.ContactNumber))
                throw new Exception("Please fill the guardian's info");

            //students info if null or empty

            if (!student.Requirements.BirthCertificate && !student.Requirements.GoodMoral && !student.Requirements.TOR)
                throw new Exception("Please select one of the requirements");

            if (string.IsNullOrWhiteSpace(student.YearLevel))
                throw new Exception("Please select the student's year level");

            if (string.IsNullOrWhiteSpace(student.Program.ProgramName))
                throw new Exception("Please select the student's program");

            if (student.Payment.AmountPaid == 0)
                throw new Exception("Please enter a valid amount");

            if (string.IsNullOrEmpty(student.Payment.Method))
                throw new Exception("Please select one of the mode of payment");
        }

        public void UpdateStudentRegistration(Student student)
        {
            try
            {
                isValidated(student);
                studentRepository.UpdateStudentRegistration(student);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void DeleteStudent(string id)
        {
            try
            {
                //Dito ilalagay ang mga Validation
                //if (id === string.Empty)
                //{
                //    throw new Exception("Id is Required");
                //}

                //kapag nakapasa sa validation, saka tawang ang method sa studentRepository
                studentRepository.DeleteStudent(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public Student GetStudentById(string id)
        {
            try
            {
                //Dito ilalagay ang mga Validation
                //if (id === string.Empty)
                //{
                //    throw new Exception("Id is Required");
                //}

                //kapag nakapasa sa validation, saka tawang ang method sa studentRepository
                return studentRepository.GetStudentById(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public ArrayList GetAllStudents()
        {
            try
            {
                return studentRepository.GetAllStudents();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No Student Registered!");
                throw new Exception(ex.Message);
            }
        }

        public List<ProgramInfo> GetAllPrograms()
        {
            try
            {
                return studentRepository.GetAllPrograms();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string GenerateNewStudentId()
        {
            try
            {
                return studentRepository.GenerateNewStudentId();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}