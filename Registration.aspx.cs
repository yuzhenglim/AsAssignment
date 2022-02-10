using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;

namespace AsAssignment
{
    public partial class Registration : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        
        static string finalHash;
        static string salt_pwd;
        byte[] Key;
        byte[] IV;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private int checkPwd(string pwd)
        {
            int pwdScore = 3;


            if (Regex.IsMatch(pwd, @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{12,}$"))
            {
                pwdScore = 1;
            }
            else if (Regex.IsMatch(pwd, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{12,}$"))
            {
                pwdScore = 2;
            }
            else
            {
                pwdScore = 3;
            }

            return pwdScore;
        }

        private int checkCredit(string credit)
        {
            int creditScore = 5;


            if (Regex.IsMatch(credit, @"^4[0-9]{12}(?:[0-9]{3})?$"))
            {
                creditScore = 1;
            }
            else if (Regex.IsMatch(credit, @"^5[1-5][0-9]{14}$"))
            {
                creditScore = 2;
            }
            else if (Regex.IsMatch(credit, @"^3[47][0-9]{13}$"))
            {
                creditScore = 3;
            }
            else if (Regex.IsMatch(credit, @"^3(?:0[0-5]|[68][0-9])[0-9]{11}$"))
            {
                creditScore = 4;
            }
            else
            {
                creditScore = 5;
            }

            return creditScore;
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



        protected void createAcc()
        {
           
            string fname = HttpUtility.HtmlEncode(firstname_tb.Text);
            string lname = HttpUtility.HtmlEncode(lastname_tb.Text);
            string email = HttpUtility.HtmlEncode(email_tb.Text);
            string dob = HttpUtility.HtmlEncode(dob_tb.Text);
            string ccinfo = HttpUtility.HtmlEncode(credit_card_tb.Text);

            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select COUNT(*) FROM [UserAccount] WHERE ([EmailAddress]=@USERID)";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", email_tb.Text.ToString().Trim());
            connection.Open();
            
            if (command.ExecuteScalar() == null || (int)command.ExecuteScalar() == 0)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("INSERT INTO UserAccount VALUES(@FirstName, @LastName, @EmailAddress, @PasswordHash, @PasswordSalt, @DateOfBirth, @CCinfo, @Photo, @IV, @Key, @AccountLock, @OldPasswordOneHash, @OldPasswordTwoHash, @DateTimePassword)"))
                        {
                            using (SqlDataAdapter sda = new SqlDataAdapter())
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.AddWithValue("@FirstName", fname.ToString().Trim());
                                cmd.Parameters.AddWithValue("@LastName", lname.ToString().Trim());
                                cmd.Parameters.AddWithValue("@EmailAddress", email.ToString().Trim());
                                cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                                cmd.Parameters.AddWithValue("@PasswordSalt", salt_pwd);
                                cmd.Parameters.AddWithValue("@DateOfBirth", dob.ToString().Trim());
                                cmd.Parameters.AddWithValue("@CCinfo", encryptData(ccinfo.ToString().Trim()));
                                if (FileUpload1.HasFile)
                                {
                                    FileUpload img = FileUpload1;
                                    HttpPostedFile File = FileUpload1.PostedFile;
                                    byte[] imgByte = new byte[File.ContentLength];
                                    File.InputStream.Read(imgByte, 0, File.ContentLength);

                                    cmd.Parameters.AddWithValue("@Photo", imgByte);
                                }
                                else
                                {
                                    SqlParameter imageParameter = new SqlParameter("@Photo", SqlDbType.Image);
                                    imageParameter.Value = DBNull.Value;
                                    cmd.Parameters.Add(imageParameter);
                                }
                                cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                                cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
                                cmd.Parameters.AddWithValue("@AccountLock", 0);
                                cmd.Parameters.AddWithValue("@OldPasswordOneHash", DBNull.Value);
                                cmd.Parameters.AddWithValue("@OldPasswordTwoHash", DBNull.Value);
                                cmd.Parameters.AddWithValue("@DateTimePassword", DateTime.Now);
                                cmd.Connection = con;
                                con.Open();
                                cmd.ExecuteNonQuery();
                                con.Close();
                                Console.WriteLine("Registration Success");
                                Response.Redirect("Login.aspx", false);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    Label3.Text = "Input Invalid. Please try again";
                    firstname_tb.Text = "";
                    lastname_tb.Text = "";
                    email_tb.Text = "";
                    dob_tb.Text = "";
                    credit_card_tb.Text = "";
                    Console.WriteLine(ex.ToString());

                }
            }
            else
            {
                Label2.Text = "Email Address in already in use";
                Label2.ForeColor = Color.Red;
            }

            
        }

        protected byte[] encryptData(string data)
        {
            byte[] cipherText;
            try
            {
                AesManaged cipher = new AesManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0, plainText.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return cipherText;
        }

        protected void register_button_Click1(object sender, EventArgs e)
        {
            int pwdScores = checkPwd(password_tb.Text);
            int creditScores = checkCredit(credit_card_tb.Text);
            if (ValidateGoogleCaptcha())
            {

                if (creditScores == 1 || creditScores == 2 || creditScores == 3 || creditScores == 4)
                {
                    if (pwdScores == 1 || pwdScores == 2)
                    {
                        string pwd = HttpUtility.HtmlEncode(password_tb.Text).ToString().Trim();
                        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                        byte[] pwdSaltByte = new byte[8];
                        rng.GetBytes(pwdSaltByte);
                        salt_pwd = Convert.ToBase64String(pwdSaltByte);
                        SHA512Managed hashing = new SHA512Managed();
                        string pwdWithSalt = pwd + salt_pwd;
                        byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                        byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                        finalHash = Convert.ToBase64String(hashWithSalt);


                        AesManaged cipher = new AesManaged();
                        cipher.GenerateKey();
                        Key = cipher.Key;
                        IV = cipher.IV;
                        createAcc();

                    }
                    else
                    {
                        Label1.Text = "Password is invalid, please try again";
                        Label1.ForeColor = Color.Red;

                    }
                }
                else
                {
                    Label4.Text = "Invalid Credit Card Number. Please Check input";
                    Label4.ForeColor = Color.Red;
                }

                
            }
            else
            {
                Label3.Text = "Unable to process your registration";
                Label3.ForeColor = Color.Red;
            }
                
            

        }
    }
}