using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Tools.Database.Extensions;

namespace TestTools
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string CONNECTION_STRING = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DbSlide;Integrated Security=True;";

            using(DbConnection dbConnection = new SqlConnection(CONNECTION_STRING))
            {
                IEnumerable<Student> students = dbConnection.ExecuteReader("SELECT student_id, last_name, first_name, section_id, year_result from student where section_id like @Filter and year_result > @MinYearResult", (reader) => ToStudent(reader), parameters:new { Filter = "10%", MinYearResult = 5 });
                foreach (Student item in students)
                {
                    Console.WriteLine($"{item.StudentId:D2} : {item.LastName} {item.FirstName} ({item.SectionId}) -> {item.YearResult}");
                }
            }
        }

        private static Student ToStudent(IDataRecord reader)
        {
            return new Student()
            {
                StudentId = (int)reader["student_id"],
                LastName = (string)reader["Last_Name"],
                FirstName = (string)reader["First_Name"],
                SectionId = (int)reader["Section_Id"],
                YearResult = (int)reader["Year_Result"],
            };
        }
    }
}