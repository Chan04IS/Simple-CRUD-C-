using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exam.Pages.Patients
{
    public class IndexModel : PageModel
    {
        public List<PatientInfo> ListPatients { get; set; } = new List<PatientInfo>();


        public void OnGet()
        {
            try
            {
                string connectionString = "Data Source=DESKTOP-8RIB6C7;Initial Catalog=Patient;Integrated Security=True;Trust Server Certificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM dbo.Patients";


                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PatientInfo patient = new PatientInfo();
                                patient.Id = reader.GetInt32(0);
                                patient.FirstName = reader.GetString(1);
                                patient.MiddleName = reader.GetString(2);
                                patient.LastName = reader.GetString(3);
                                patient.SuffixName = reader.IsDBNull(4) ? "" : reader.GetString(4);
                                patient.BirthDate = reader.IsDBNull(5) ? null : reader.GetDateTime(5); 
                                patient.Gender = reader.GetString(6);
                                patient.InitialDiagnosis = reader.GetString(7);

                                ListPatients.Add(patient);
                            }
                        }
                    }
                 }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exceptiion: " + ex.ToString());
            }
        }
    }

    public class PatientInfo
    {
        internal object id;

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string SuffixName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public string InitialDiagnosis { get; set; }
    }
}
