using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AsAssignment
{
    public partial class Login : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void register_button_Click(object sender, EventArgs e)
        {
            Response.Redirect("Registration.aspx");
        }

        protected void login_button_Click(object sender, EventArgs e)
        {

            if (ValidateGoogleCaptcha())
            {
                string pwd = HttpUtility.HtmlEncode(password_tb.Text).ToString().Trim();
                string userid = HttpUtility.HtmlEncode(username_tb.Text).ToString().Trim();
                SHA512Managed hashing = new SHA512Managed();
                string dbHash = getDBHash(userid);
                string dbSalt = getDBSalt(userid);
                int countLock = getCountLock(userid);
                try
                {
                    if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0 && countLock < 3)
                    {
                        string pwdWithSalt = pwd + dbSalt;
                        byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                        string userHash = Convert.ToBase64String(hashWithSalt);
                        if (userHash.Equals(dbHash))
                        {
                            Session["Username"] = username_tb.Text.Trim();

                            string guid = Guid.NewGuid().ToString();
                            Session["AuthToken"] = guid;

                            Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                            Response.Redirect("UserProfile.aspx", false);
                        }

                        else
                        {
                            addCountLock(userid);
                            lbl_pwd.Text = "Userid or password is not valid. Please try again.";
                            lbl_pwd.ForeColor = Color.Red;
                        }

                    }
                    else if (countLock == 3)
                    {
                        lbl_lockout.Text = "Account is currently locked due to too many login attempts";
                        lbl_lockout.ForeColor = Color.Red;
                    }
                    else
                    {

                        lbl_pwd.Text = "Userid or password is not valid. Please try again.";
                        lbl_pwd.ForeColor = Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally { }
            }

            else
            {

                lbl_lockout.Text = "Unable to process your login";
                lbl_lockout.ForeColor = Color.Red;
            }
                
        }

        public class GoogleObj
        {
            public string success { get; set; }
            public List<string> ErrorMsg { get; set; }
        }

        public bool ValidateGoogleCaptcha()
        {
            bool result = false;
            string captchaResponse = Request.Form["g-recaptcha-response"];

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
           (" https://www.google.com/recaptcha/api/siteverify?secret=6LdjbmgeAAAAAGDpnRlfesvlPvRmdcnUDkkXAgp8 &response=" + captchaResponse);


            try
            {
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        GoogleObj jsonObject = js.Deserialize<GoogleObj>(jsonResponse);
                        result = Convert.ToBoolean(jsonObject.success);//

                    }
                }
                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        protected string getDBHash(string userid)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordHash FROM UserAccount WHERE EmailAddress=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PasswordHash"] != null)
                        {
                            if (reader["PasswordHash"] != DBNull.Value)
                            {
                                h = reader["PasswordHash"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return h;
        }

        protected string getDBSalt(string userid)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordSalt FROM UserAccount WHERE EmailAddress=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PasswordSalt"] != null)
                        {
                            if (reader["PasswordSalt"] != DBNull.Value)
                            {
                                s = reader["PasswordSalt"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }

        protected int getCountLock(string userid)
        {
            int c = 0;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select AccountLock FROM UserAccount WHERE EmailAddress=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["AccountLock"] != null)
                        {
                            if (reader["AccountLock"] != DBNull.Value)
                            {
                                c = int.Parse(reader["AccountLock"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return c;
        }

        protected void addCountLock(string userid)
        {

            string count = getCountLock(userid).ToString() ;


            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE UserAccount SET AccountLock = @AccountLock + 1 WHERE EmailAddress=@USERID"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            
                            cmd.Parameters.AddWithValue("@AccountLock", count);
                            cmd.Parameters.AddWithValue("@USERID", userid);
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                            
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

    }
}