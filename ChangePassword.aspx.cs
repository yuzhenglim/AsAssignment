using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AsAssignment
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        static string finalHash;
        static string salt_pwd;
        string OldHashTwo = null;
        string OldHashOne = null;
        string OldSaltOne = null;
        string userid;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Username"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("/ErrorPages/UnAuthorizedAdvanced.aspx", false);
                }
                else
                {
                    userid = (string)Session["Username"];
                }
                
            }

            if (Session["AuthToken"] == null)
            {
                Session.Clear();
                Session.Abandon();
                Session.RemoveAll();

                Response.Redirect("/ErrorPages/UnAuthorized.aspx", false);

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

        protected void chnpwd_button_Click(object sender, EventArgs e)
        {

            
            string pwd = HttpUtility.HtmlEncode(new_pwd_tb.Text).ToString().Trim();
            string oldpwd = HttpUtility.HtmlEncode(old_pwd_tb.Text).ToString().Trim();

            SHA512Managed hashing = new SHA512Managed();
            string dbHash = getDBHash(userid);
            string dbSalt = getDBSalt(userid);
            try
            {
                if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                {
                    //Checks if the current password is as entered
                    string pwdWithSalt = oldpwd + dbSalt;
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                    string userHash = Convert.ToBase64String(hashWithSalt);
                    if (userHash.Equals(dbHash))
                    {
                        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                        byte[] pwdSaltByte = new byte[8];
                        rng.GetBytes(pwdSaltByte);
                        salt_pwd = Convert.ToBase64String(pwdSaltByte);
                        SHA512Managed hash = new SHA512Managed();
                        string pwdAndSalt = pwd + salt_pwd;
                        byte[] plainHash = hash.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                        byte[] hashAndSalt = hash.ComputeHash(Encoding.UTF8.GetBytes(pwdAndSalt));
                        finalHash = Convert.ToBase64String(hashAndSalt);

                        //checks if current password has any password history
                        try
                        {
                            
                            SqlConnection connection = new SqlConnection(MYDBConnectionString);
                            string sql = "select COUNT(*) FROM UserAccount WHERE EmailAddress=@USERID AND (PasswordHash=@NewHash OR OldPasswordOneHash=@NewHash OR OldPasswordTwoHash=@NewHash)";
                            SqlCommand command = new SqlCommand(sql, connection);
                            command.Parameters.AddWithValue("@NewHash", finalHash);
                            command.Parameters.AddWithValue("@USERID", userid);
                            connection.Open();

                            if (command.ExecuteScalar() == null || (int)command.ExecuteScalar() == 0)
                            {
                                passwordChange(userid, dbHash, dbSalt);
                                Response.Redirect("UserProfile.aspx");
                            }
                            else
                            {
                                Label2.Text = "Password has been used before!";
                                Label2.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.ToString());
                        }


                    }
                    else
                    {
                        Label1.Text = "Wrong Password";
                        Label1.ForeColor = Color.Red;
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally { }
        }

        protected void SearchForOldHash(string userid)
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

                        if (reader["OldPasswordTwoHash"] != DBNull.Value)
                        {
                            OldHashOne = reader["OldPasswordTwoHash"].ToString();
                        }
                        if (reader["OldPasswordOneSalt"] != DBNull.Value)
                        {
                            OldSaltOne = reader["OldPasswordOneSalt"].ToString();
                        }

                        if (reader["OldPasswordOneHash"] != DBNull.Value)
                        {
                            OldHashTwo = reader["OldPasswordTwoHash"].ToString();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
        }


        protected void passwordChange(string userid, string dbHash, string dbSalt)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE UserAccount SET PasswordHash = @NewPasswordHash, PasswordSalt = @NewPasswordSalt, OldPasswordOneHash = @PasswordHash, OldPasswordOneSalt = @PasswordSalt, OldPasswordTwoHash = @PasswordTwoHash, OldPasswordTwoSalt = @PasswordTwoSalt WHERE EmailAddress=@USERID"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@USERID", userid);
                            cmd.Parameters.AddWithValue("@NewPasswordHash", finalHash);
                            cmd.Parameters.AddWithValue("@NewPasswordSalt", salt_pwd);
                            cmd.Parameters.AddWithValue("@PasswordHash", dbHash);
                            cmd.Parameters.AddWithValue("@PasswordSalt", dbSalt);
                            if (OldHashOne != null)
                            {
                                cmd.Parameters.AddWithValue("@PasswordTwoHash", OldHashOne);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@PasswordTwoHash", DBNull.Value);
                            }
                            if (OldHashTwo != null)
                            {
                                cmd.Parameters.AddWithValue("@PasswordTwoSalt", OldHashTwo);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@PasswordTwoSalt", DBNull.Value);
                            }
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
    }
}