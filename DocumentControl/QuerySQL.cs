using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;

namespace DocumentControl
{
    public class QuerySQL
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["TCTV1ConnectionString"].ConnectionString);

        //ส่วน Select ข้อมูล
        public string SelectAt(int index, string sql)
        {
            if (conn.State == ConnectionState.Open)
                conn.Close();
            string ans = "";
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader result = cmd.ExecuteReader();
            if (result.HasRows)
            {
                result.Read();
                ans = result.GetValue(index).ToString();
                result.Close();
            }
            else
            {
                ans = "0";
            }
            conn.Close();
            return ans;
        }
        public Boolean CheckRow(string sql)
        {
            if (conn.State == ConnectionState.Open)
                conn.Close();
            Boolean ans = false;
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader result = cmd.ExecuteReader();
            if (result.HasRows)
            {
                ans = true;
            }
            conn.Close();
            return ans;
        }
        public DataTable SelectTable(string sql)
        {
            if (conn.State == ConnectionState.Open)
                conn.Close();
            DataTable ans = new DataTable();
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader result = cmd.ExecuteReader();
            ans.Load(result);
            conn.Close();
            return ans;
        }

        //ส่วน Insert ข้อมูล
        public Boolean Excute(string sql)
        {
            if (conn.State == ConnectionState.Open)
                conn.Close();
            Boolean ans = false;
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            int result = cmd.ExecuteNonQuery();
            if (result == 1)
            {
                ans = true;
            }
            conn.Close();
            return ans;
        }
    }
}