using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace AsAssignment.Image
{
    /// <summary>
    /// Summary description for ShowImage
    /// </summary>
    public class ShowImage : IHttpHandler
    {

        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;


        public void ProcessRequest(HttpContext context)
        {
            string empno;
            if (context.Request.QueryString["id"] != null)
                empno = context.Request.QueryString["id"];
            else
                throw new ArgumentException("No parameter specified");
            context.Response.ContentType = "image/jpeg";
            Stream strm = ShowEmpImage(empno);
            byte[] buffer = new byte[4096];
            int byteSeq = strm.Read(buffer, 0, 4096);
            while (byteSeq > 0)
            {
                context.Response.OutputStream.Write(buffer, 0, byteSeq);
                byteSeq = strm.Read(buffer, 0, 4096);
            }
        }

        public Stream ShowEmpImage(string userid)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select * FROM UserAccount WHERE EmailAddress=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            connection.Open();
            object img = command.ExecuteScalar();
            try
            {
                return new MemoryStream((byte[])img);
            }

            catch
            { return null; }

            finally
            { connection.Close(); }
        }
        

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}