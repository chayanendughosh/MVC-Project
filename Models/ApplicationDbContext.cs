using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;

namespace WebApplication1.Models
{
    // Simple minimal data access without EF to avoid adding packages.
    public class ApplicationDbContext : IDisposable
    {
        private readonly string _connString;
        public ApplicationDbContext()
        {
            _connString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            // quick connectivity check to fail fast with a clearer message
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    conn.Close();
                }
            }
            catch (SqlException ex)
            {
                Debug.WriteLine("Database connectivity check failed: " + ex.Message);

                var message = "Unable to connect to the database. Verify the connection string and LocalDB/SQL instance.";
                // 4060: Cannot open database requested by the login
                if (ex.Number == 4060)
                {
                    message = "Cannot open database 'UserRegistrationDb'. It likely does not exist in the configured SQL Server instance. Run 'CreateDatabase.sql' against (localdb)\\MSSQLLocalDB (or update the connection string to the instance where you created the DB).";
                }
                // 18456: Login failed for user
                else if (ex.Number == 18456)
                {
                    message = "Login failed for the configured connection. If you are using Integrated Security, ensure SQL Server/LocalDB is running and that your Windows user has access to the database. If the DB was created under another account or instance, recreate it using 'CreateDatabase.sql' on this machine/instance.";
                }

                throw new InvalidOperationException(message, ex);
            }
        }

        public List<State> GetStates(string term = null)
        {
            var list = new List<State>();
            using (var conn = new SqlConnection(_connString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                if (string.IsNullOrEmpty(term))
                {
                    cmd.CommandText = "SELECT Id, Name FROM [dbo].[States] ORDER BY Name";
                }
                else
                {
                    cmd.CommandText = "SELECT Id, Name FROM [dbo].[States] WHERE Name LIKE @term ORDER BY Name";
                    cmd.Parameters.AddWithValue("@term", "%" + term + "%");
                }

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read()) list.Add(new State { Id = r.GetInt32(0), Name = r.GetString(1) });
                }
            }
            return list;
        }

        public List<User> GetUsers()
        {
            var list = new List<User>();
            using (var conn = new SqlConnection(_connString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = @"SELECT u.Id, u.Name, u.Email, u.Mobile, u.Address, u.Gender, u.StateId, u.Hobbies, u.CreatedAt, s.Id, s.Name
FROM [dbo].[Users] u
LEFT JOIN [dbo].[States] s ON u.StateId = s.Id
ORDER BY u.CreatedAt DESC";
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        var u = new User
                        {
                            Id = r.GetInt32(0),
                            Name = r.GetString(1),
                            Email = r.GetString(2),
                            Mobile = r.GetString(3),
                            Address = r.GetString(4),
                            Gender = r.GetString(5),
                            StateId = r.GetInt32(6),
                            Hobbies = r.GetString(7),
                            CreatedAt = r.GetDateTime(8)
                        };
                        if (!r.IsDBNull(9)) u.State = new State { Id = r.GetInt32(9), Name = r.GetString(10) };
                        list.Add(u);
                    }
                }
            }
            return list;
        }

        public int InsertUser(User u)
        {
            using (var conn = new SqlConnection(_connString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = @"INSERT INTO [dbo].[Users] (Name, Email, Mobile, Address, Gender, StateId, Hobbies, CreatedAt)
VALUES (@Name, @Email, @Mobile, @Address, @Gender, @StateId, @Hobbies, GETDATE()); SELECT SCOPE_IDENTITY();";
                cmd.Parameters.AddWithValue("@Name", u.Name);
                cmd.Parameters.AddWithValue("@Email", u.Email);
                cmd.Parameters.AddWithValue("@Mobile", u.Mobile);
                cmd.Parameters.AddWithValue("@Address", u.Address);
                cmd.Parameters.AddWithValue("@Gender", u.Gender);
                cmd.Parameters.AddWithValue("@StateId", u.StateId);
                cmd.Parameters.AddWithValue("@Hobbies", u.Hobbies);
                var obj = cmd.ExecuteScalar();
                return System.Convert.ToInt32(obj);
            }
        }

        public void UpdateUser(User u)
        {
            using (var conn = new SqlConnection(_connString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = @"UPDATE [dbo].[Users] SET Name=@Name, Email=@Email, Mobile=@Mobile, Address=@Address, Gender=@Gender, StateId=@StateId, Hobbies=@Hobbies WHERE Id=@Id";
                cmd.Parameters.AddWithValue("@Name", u.Name);
                cmd.Parameters.AddWithValue("@Email", u.Email);
                cmd.Parameters.AddWithValue("@Mobile", u.Mobile);
                cmd.Parameters.AddWithValue("@Address", u.Address);
                cmd.Parameters.AddWithValue("@Gender", u.Gender);
                cmd.Parameters.AddWithValue("@StateId", u.StateId);
                cmd.Parameters.AddWithValue("@Hobbies", u.Hobbies);
                cmd.Parameters.AddWithValue("@Id", u.Id);
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteUser(int id)
        {
            using (var conn = new SqlConnection(_connString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = "DELETE FROM [dbo].[Users] WHERE Id=@Id";
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public User GetUser(int id)
        {
            using (var conn = new SqlConnection(_connString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = @"SELECT u.Id, u.Name, u.Email, u.Mobile, u.Address, u.Gender, u.StateId, u.Hobbies, u.CreatedAt, s.Id, s.Name
FROM [dbo].[Users] u
LEFT JOIN [dbo].[States] s ON u.StateId = s.Id
WHERE u.Id=@Id";
                cmd.Parameters.AddWithValue("@Id", id);
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        var u = new User
                        {
                            Id = r.GetInt32(0),
                            Name = r.GetString(1),
                            Email = r.GetString(2),
                            Mobile = r.GetString(3),
                            Address = r.GetString(4),
                            Gender = r.GetString(5),
                            StateId = r.GetInt32(6),
                            Hobbies = r.GetString(7),
                            CreatedAt = r.GetDateTime(8)
                        };
                        if (!r.IsDBNull(9)) u.State = new State { Id = r.GetInt32(9), Name = r.GetString(10) };
                        return u;
                    }
                }
            }
            return null;
        }

        public void Dispose()
        {
            // no unmanaged resources
        }
    }
}
