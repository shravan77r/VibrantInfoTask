using System.Data;
using System.Data.SqlClient;
using System;
using VibrantInfoTask.Models;
using System.Runtime.CompilerServices;

namespace VibrantInfoTask.Data
{
    public class DbContext
    {
        private static string constr = "Server=DELL\\SQLEXPRESS;Database=DBVibrantInfoTask;Trusted_Connection=True;MultipleActiveResultSets=true";
        private SqlConnection cn = new SqlConnection(constr);

        public DataTable Getdata(string qry)
        {

            DataTable dt = new DataTable();
            try
            {
                cn.Open();
                SqlDataAdapter ad = new SqlDataAdapter(qry, cn);
                ad.Fill(dt);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                cn.Close();
            }
            return dt;
        }
        public int INSERT_UPDATE_DELETE(User obj)
        {

            int i = 0;
            try
            {

                SqlCommand com = new SqlCommand("INSERT_UPDATE_DELETE", cn);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", obj.Id);
                com.Parameters.AddWithValue("@FirstName", obj.FirstName);
                com.Parameters.AddWithValue("@LastName", obj.LastName);
                com.Parameters.AddWithValue("@DateOfBirth", obj.DateOfBirth);
                com.Parameters.AddWithValue("@Email", obj.Email);
                com.Parameters.AddWithValue("@Password", obj.Password);
                com.Parameters.AddWithValue("@PhoneNumber", obj.PhoneNumber);
                com.Parameters.AddWithValue("@Gender", obj.Gender);
                com.Parameters.AddWithValue("@BloodGroup", obj.BloodGroup);
                com.Parameters.AddWithValue("@ProfilePhoto", obj.ProfilePhoto);
                com.Parameters.AddWithValue("@Address", obj.Address);
                com.Parameters.AddWithValue("@OperationType", obj.OperationType);

                SqlParameter insertedId = new SqlParameter("@InsertedId", SqlDbType.Int);
                insertedId.Direction = ParameterDirection.Output;
                com.Parameters.Add(insertedId);

                cn.Open();
                com.ExecuteNonQuery();
                i = (int)insertedId.Value;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                cn.Close();
            }
            return i;
        }
        public Tuple<DataTable,int> GetList(UserList request)
        {
            DataTable dataTable = new DataTable();
            int TotalRecords = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(constr))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("List_Users", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@searchKeyword", SqlDbType.NVarChar, 1000)).Value = request.Keyword;
                        command.Parameters.Add(new SqlParameter("@pageSize", SqlDbType.Int)).Value = request.PageSize;
                        command.Parameters.Add(new SqlParameter("@pageNumber", SqlDbType.Int)).Value = request.PageIndex;
                        command.Parameters.Add(new SqlParameter("@SortCol", SqlDbType.NVarChar, 50)).Value = request.SortCol;
                        command.Parameters.Add(new SqlParameter("@SortDir", SqlDbType.NVarChar, 4)).Value = request.SortDir;

                        SqlParameter _TotalRecords = new SqlParameter("@TotalRecords", SqlDbType.Int);
                        _TotalRecords.Direction = ParameterDirection.Output;
                        command.Parameters.Add(_TotalRecords);

                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        
                        adapter.Fill(dataTable);
                        TotalRecords = (int)_TotalRecords.Value;

                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return Tuple.Create(dataTable, TotalRecords);
        }
    }
}
