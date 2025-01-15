// using Microsoft.Data.SqlClient;
//
// namespace EmployeeTimeTrackingBackend
// {
//     public class Database
//     {
//         //string for the database
//         private string connectionString = "Server=MP25W9JV-NB\\MSSQLSERVER01;Database=employee-time-tracking;Integrated Security=True;TrustServerCertificate=True;";
//         
//
//
//         //getting the db connected
//         public SqlConnection GetConnection()
//         {
//             try
//             {
//                 var connection = new SqlConnection(connectionString);
//                 connection.Open();
//                 Console.WriteLine("Database connection established.");
//                 return connection;
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine("Database connection failed: " + ex.Message);
//                 throw;
//             }
//         }
//     }
// }