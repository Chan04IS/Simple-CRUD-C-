using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Exam.Pages.Patients
{
    public class CreateModel : PageModel
    {
        public PatientInfo patientInfo = new PatientInfo();
        public string errorMessage = "";
        public string successMessage = "";
        public void OnGet()
        {
        }

        public void OnPost()
        {
            // Assign values only if they exist in the form
            patientInfo.FirstName = Request.Form.ContainsKey("FirstName") ? Request.Form["FirstName"] : "";
            patientInfo.MiddleName = Request.Form.ContainsKey("MiddleName") ? Request.Form["MiddleName"] : "";
            patientInfo.LastName = Request.Form.ContainsKey("LastName") ? Request.Form["LastName"] : "";
            patientInfo.SuffixName = Request.Form.ContainsKey("SuffixName") ? Request.Form["SuffixName"] : "";
            patientInfo.Gender = Request.Form.ContainsKey("Gender") ? Request.Form["Gender"] : "";
            patientInfo.InitialDiagnosis = Request.Form.ContainsKey("InitialDiagnosis") ? Request.Form["InitialDiagnosis"] : "";

            // Handle BirthDate properly
            if (Request.Form.ContainsKey("BirthDate") && DateTime.TryParse(Request.Form["BirthDate"], out DateTime birthDate))
            {
                patientInfo.BirthDate = birthDate;
            }
            else
            {
                patientInfo.BirthDate = null;
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(patientInfo.LastName) ||
                patientInfo.BirthDate == null ||
                string.IsNullOrWhiteSpace(patientInfo.InitialDiagnosis))
            {
                errorMessage = "Last Name, Birth Date, and Initial Diagnosis are required.";
                return;
            }

            try
            {
                string connectionString = "Data Source=DESKTOP-8RIB6C7;Initial Catalog=Patient;Integrated Security=True;Trust Server Certificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO dbo.Patients (FirstName, MiddleName, LastName, SuffixName, BirthDate, Gender, InitialDiagnosis) " +
                                 "VALUES (@FirstName, @MiddleName, @LastName, @SuffixName, @BirthDate, @Gender, @InitialDiagnosis)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", patientInfo.FirstName);
                        command.Parameters.AddWithValue("@MiddleName", patientInfo.MiddleName);
                        command.Parameters.AddWithValue("@LastName", patientInfo.LastName);
                        command.Parameters.AddWithValue("@SuffixName", patientInfo.SuffixName);
                        command.Parameters.AddWithValue("@BirthDate", patientInfo.BirthDate);
                        command.Parameters.AddWithValue("@Gender", patientInfo.Gender);
                        command.Parameters.AddWithValue("@InitialDiagnosis", patientInfo.InitialDiagnosis);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Error: " + ex.Message;
                return;
            }

            // Success message if everything is fine
            successMessage = "New Patient Added Correctly";

            // Clear fields after successful submission
            patientInfo = new PatientInfo();
        }
    }
}
