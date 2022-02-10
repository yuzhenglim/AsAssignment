using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AsAssignment
{
    public partial class UserProfile : System.Web.UI.Page
    {

        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        string emailID = null;
        byte[] Key;
        byte[] IV;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Username"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("/ErrorPages/UnAuthorizedAdvanced.html", false);
                }
                else
                {
                    emailID = (string)Session["Username"];
                    displayUserProfile(emailID);
                }

            }

            if (Session["AuthToken"] == null)
            {
                Session.Clear();
                Session.Abandon();
                Session.RemoveAll();

                Response.Redirect("/ErrorPages/UnAuthorized.html", false);

                if (Request.Cookies["ASP.NET_SessionId"] != null)
                {
                    Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                    Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-10);
                }
                if (Request.Cookies["AuthToken"] != null)
                {
                    Response.Cookies["AuthToken"].Value = string.Empty;
                    Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-10);
                }
            }
        }



        protected void btn_logOut_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            Response.Redirect("Login.aspx", false);

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-10);
            }
            if (Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies["AuthToken"].Value = string.Empty;
                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-10);
            }
        }

        protected void displayUserProfile(string userid)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select * FROM UserAccount WHERE EmailAddress=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);

            
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["EmailAddress"] != DBNull.Value)
                        {
                            lbl_email.Text = reader["EmailAddress"].ToString();
                        }
                        if (reader["DateOfBirth"] != DBNull.Value)
                        {
                            lbl_dob.Text = reader["DateOfBirth"].ToString();
                        }
                        if (reader["FirstName"] != DBNull.Value && reader["LastName"] != DBNull.Value)
                        {
                            string fname = reader["FirstName"].ToString();
                            string lname = reader["LastName"].ToString();
                            lbl_name.Text = fname + " " + lname;
                        }
                        if (reader["CCinfo"] != DBNull.Value)
                        {
                            string result = System.Text.Encoding.UTF8.GetString((Byte[])reader["CCinfo"]);

                            lbl_ccInfo.Text = result;
                        }
                        if (reader["Photo"] != DBNull.Value)
                        {
                            Image1.ImageUrl = "~/ShowImage.ashx?userid=" + reader["EmailAddress"];
                        }
                        if (reader["IV"] != DBNull.Value)
                        {
                            IV = Convert.FromBase64String(reader["IV"].ToString());
                        }
                        if (reader["Key"] != DBNull.Value)
                        {
                            Key = Convert.FromBase64String(reader["Key"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Label3.Text = "Input Invalid. Please try again";
                lbl_name.Text = "";
                lbl_email.Text = "";
                lbl_dob.Text = "";
                lbl_ccInfo.Text = "";
                Console.WriteLine(ex.ToString());
                throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
        }

        protected string decryptData(byte[] cipherText)
        {
            string plainText = null;

            try
            {
                AesManaged cipher = new AesManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform decryptTransform = cipher.CreateDecryptor();
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptTransform, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plainText = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }
            catch (Exception ex) { throw new Exception(ex.ToString()); }
            finally { }
            return plainText;
        }

        protected void chg_pwd_button_Click(object sender, EventArgs e)
        {
            Session["Username"] = emailID;
            Response.Redirect("ChangePassword.aspx", false);

        }
    }
}