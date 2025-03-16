using Exam.Pages.Patients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Exam.Pages
{
    public class EditModel : PageModel
    {
        public PatientInfo patientInfo = new PatientInfo();
        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
            string id = Request.Query["id"];

            if (string.IsNullOrEmpty(id))
            {
                errorMessage = "Invalid ID.";
                return;
            }

            try
            {
                string connectionString = "Data Source=DESKTOP-8RIB6C7;Initial Catalog=Patient;Integrated Security=True;Trust Server Certificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Patients WHERE Id = @Id";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                patientInfo.id = reader.GetInt32(0);
                                patientInfo.FirstName = reader.GetString(1);
                                patientInfo.MiddleName = reader.GetString(2);
                                patientInfo.LastName = reader.GetString(3);
                                patientInfo.SuffixName = reader.IsDBNull(4) ? "" : reader.GetString(4);
                                patientInfo.BirthDate = reader.GetDateTime(5);
                                patientInfo.Gender = reader.GetString(6);
                                patientInfo.InitialDiagnosis = reader.GetString(7);
                            }
                            else
                            {
                                errorMessage = "Patient not found.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Error: " + ex.Message;
            }
        }

        public void OnPost()
        {
            try
            {
                patientInfo.id = Convert.ToInt32(Request.Form["id"]);
                patientInfo.FirstName = Request.Form["firstname"];
                patientInfo.MiddleName = Request.Form["middlename"];
                patientInfo.LastName = Request.Form["lastname"];
                patientInfo.SuffixName = Request.Form["suffixname"];
                patientInfo.BirthDate = DateTime.TryParse(Request.Form["birthdate"], out DateTime birthDate)
                    ? birthDate : throw new Exception("Invalid birth date format.");
                patientInfo.Gender = Request.Form["gender"];
                patientInfo.InitialDiagnosis = Request.Form["initialdiagnosis"];

                if (string.IsNullOrEmpty(patientInfo.FirstName) ||
                    string.IsNullOrEmpty(patientInfo.MiddleName) ||
                    string.IsNullOrEmpty(patientInfo.LastName))
                {
                    errorMessage = "All fields must be filled.";
                    return;
                }

                string connectionString = "Data Source=DESKTOP-8RIB6C7;Initial Catalog=Patient;Integrated Security=True;Trust Server Certificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE Patients SET FirstName=@FirstName, MiddleName=@MiddleName, " +
                                 "LastName=@LastName, SuffixName=@SuffixName, BirthDate=@BirthDate, " +
                                 "Gender=@Gender, InitialDiagnosis=@InitialDiagnosis WHERE Id=@Id";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", patientInfo.id);
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

                successMessage = "Patient updated successfully.";
            }
            catch (Exception ex)
            {
                errorMessage = "Error: " + ex.Message;
            }
        }
    }
}
