<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="AsAssignment.Registration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Account Registration</title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
            margin-left: 15px;
            margin-top: 14px;
        }
        .auto-style2 {
            width: 166px;
        }
        .auto-style3 {
            width: 157px;
        }
        .auto-style4 {
            width: 373px;
        }
    </style>

    <script type="text/javascript">
        function validate() {

            var str = document.getElementById('<%=password_tb.ClientID %>').value;
            if (str.length < 12) {
                document.getElementById("Label1").innerHTML = "Password must be at least 12 characters long";
                document.getElementById("Label1").style.color = "Red";
                return ("too_short");
            }
            else if (str.search(/[0-9]/) == -1) {
                document.getElementById("Label1").innerHTML = "Password must have at least 1 number";
                document.getElementById("Label1").style.color = "Red";
                return ("no_number");
            }
            else if (str.search(/[A-Z]/) == -1) {
                document.getElementById("Label1").innerHTML = "Password must have at least a capital letter";
                document.getElementById("Label1").style.color = "Red";
                return ("no_capital");
            }
            else if (str.search(/[a-z]/) == -1) {
                document.getElementById("Label1").innerHTML = "Password must have at least a small letter";
                document.getElementById("Label1").style.color = "Red";
                return ("no_small");
            }
            else if (str.search(/[@#$%&]/) == -1) {
                document.getElementById("Label1").innerHTML = "Password must have at least a special character";
                document.getElementById("Label1").style.color = "Red";
                return ("no_special");
            }
            document.getElementById("Label1").innerHTML = "STRONG password";
            document.getElementById("Label1").style.color = "Green";

        }
        function validateFName() {
            var fname = document.getElementById('<%=firstname_tb.ClientID %>').value;
            
            if (fname.search(/[0-9]/) >= 1) {
                document.getElementById("Label5").innerHTML = "Name has numbers in it";
                document.getElementById("Label5").style.color = "Red";
                return ("has_number");
            }
            else if (fname.search(/[@#$%&]/) >= 1) {
                document.getElementById("Label5").innerHTML = "Name has special characters in it";
                document.getElementById("Label5").style.color = "Red";
                return ("has_special");
            }
            document.getElementById("Label5").innerHTML = "Vaild Name";
            document.getElementById("Label5").style.color = "Green";

        }
        function validateLName() {
            var lname = document.getElementById('<%=lastname_tb.ClientID %>').value;
            
            if (lname.search(/[0-9]/) >= 1) {
                document.getElementById("Label6").innerHTML = "Name has numbers in it";
                document.getElementById("Label6").style.color = "Red";
                return ("has_number");
            }
            else if (lname.search(/[@#$%&]/) >= 1) {
                document.getElementById("Label6").innerHTML = "Name has special characters in it";
                document.getElementById("Label6").style.color = "Red";
                return ("has_special");
            }
            document.getElementById("Label6").innerHTML = "Vaild Name";
            document.getElementById("Label6").style.color = "Green";

            }
        function validateEmail() {
            var str = document.getElementById('<%=email_tb.ClientID %>').value;
            if (str.search(/^[-a-z0-9~!$%^&*_=+}{\'?]+(\.[-a-z0-9~!$%^&*_=+}{\'?]+)*@([a-z0-9_][-a-z0-9_]*(\.[-a-z0-9_]+)*\.(aero|arpa|biz|com|coop|edu|gov|info|int|mil|museum|name|net|org|pro|travel|mobi|[a-z][a-z])|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,5})?$/)) {
                document.getElementById("Label2").innerHTML = "Email is invalid";
                document.getElementById("Label2").style.color = "Red";
                return ("invalid_email");
            }
            document.getElementById("Label2").innerHTML = "Valid Email";
            document.getElementById("Label2").style.color = "Green";

        }</script>

    <script src="https://www.google.com/recaptcha/api.js?render=6LdjbmgeAAAAAAjvm5IYf0DhaT2L9vBBqgkqE6j5"></script>


    

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <fieldset>
                <legend>Account Registration</legend>
                <table class="auto-style1">
                    <tr>
                        <td class="auto-style2">&nbsp;</td>
                        <td class="auto-style4"><asp:Label ID="Label3" runat="server" Text=""></asp:Label></td>
                        <td class="auto-style3">
                            &nbsp;</td>
                    </tr>
                    <tr>
                        <td class="auto-style2">First Name:</td>
                        <td class="auto-style4"><asp:TextBox ID="firstname_tb" runat="server" onkeyup="javascript:validateFName()" Width="480px"></asp:TextBox></td>
                        <td class="auto-style3">
                            <asp:RequiredFieldValidator ControlToValidate="firstname_tb" ID="RequiredFieldValidator1" runat="server" ForeColor="#CC0000" ErrorMessage="First Name is Required" ></asp:RequiredFieldValidator><br />
                            <asp:Label ID="Label5" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style2">Last Name: </td>
                        <td class="auto-style4"><asp:TextBox ID="lastname_tb" runat="server" Width="480px" onkeyup="javascript:validateLName()"></asp:TextBox></td>
                        <td class="auto-style3">
                            <asp:RequiredFieldValidator ControlToValidate="lastname_tb" ID="RequiredFieldValidator2" runat="server" ForeColor="#CC0000" ErrorMessage="Last Name is Required"></asp:RequiredFieldValidator> <br />
                            <asp:Label ID="Label6" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style2">Email:  </td>
                        <td class="auto-style4"><asp:TextBox ID="email_tb" runat="server" Width="480px" onkeyup="javascript:validateEmail()" TextMode="Email"></asp:TextBox></td>
                        <td class="auto-style3">
                            <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                            <asp:RequiredFieldValidator ControlToValidate="email_tb" ID="RequiredFieldValidator3" runat="server" ForeColor="#CC0000" ErrorMessage="Email is Required"></asp:RequiredFieldValidator><br />
                            <asp:RegularExpressionValidator ControlToValidate="email_tb" ID="RegularExpressionValidator3" runat="server" ForeColor="Red" ErrorMessage="Email is invalid!" ValidationExpression="^[-a-z0-9~!$%^&amp;*_=+}{\'?]+(\.[-a-z0-9~!$%^&amp;*_=+}{\'?]+)*@([a-z0-9_][-a-z0-9_]*(\.[-a-z0-9_]+)*\.(aero|arpa|biz|com|coop|edu|gov|info|int|mil|museum|name|net|org|pro|travel|mobi|[a-z][a-z])|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,5})?$"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style2">Password: </td>
                        <td class="auto-style4"><asp:TextBox ID="password_tb" runat="server" Width="480px" onkeyup="javascript:validate()" TextMode="Password" ></asp:TextBox></td>
                        <td class="auto-style3">
                            <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style2">Date of Birth: </td>
                        <td class="auto-style4"><asp:TextBox ID="dob_tb" runat="server" Width="480px" TextMode ="Date"></asp:TextBox></td>
                        <td class="auto-style3">
                            <asp:RequiredFieldValidator ControlToValidate="dob_tb" ID="RequiredFieldValidator4" runat="server" ForeColor="#CC0000" ErrorMessage="Date of Birth is Required"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style2">Credit Card Number:</td>
                        <td class="auto-style4"><asp:TextBox ID="credit_card_tb" runat="server" Width="480px"></asp:TextBox></td>
                        <td class="auto-style3">
                            <asp:Label ID="Label4" runat="server" Text=""></asp:Label>
                            <asp:RequiredFieldValidator ControlToValidate="credit_card_tb" ID="RequiredFieldValidator5" runat="server" ForeColor="#CC0000" ErrorMessage="Credit Card Number is Required"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style2">Photo:</td>
                        <td class="auto-style4"> 
                            
                            <asp:FileUpload ID="FileUpload1" runat="server" />
                            
                            <asp:Label ID="lbl_image" runat="server" Text=""></asp:Label>
                        </td>
                        <td class="auto-style3">
                            &nbsp;</td>
                    </tr>
                    <tr>
                        <td class="auto-style2"></td>
                        <td class="auto-style4"><br /><asp:Button ID="register_account_button" runat="server" Text="Register" Width="480px" OnClick="register_button_Click1"  /></td>
                        <td class="auto-style3"></td>
                    </tr>
                </table>
            </fieldset>
            <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>
        </div>
            

    </form>
    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('6LdjbmgeAAAAAAjvm5IYf0DhaT2L9vBBqgkqE6j5', { action: 'Login' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            });
        });
    </script>
   
    
   
</body>
</html>
