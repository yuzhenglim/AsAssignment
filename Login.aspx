<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="AsAssignment.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="https://www.google.com/recaptcha/api.js?render=6LdjbmgeAAAAAAjvm5IYf0DhaT2L9vBBqgkqE6j5"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <fieldset>
            <p> 
                <asp:Label ID="lbl_lockout" runat="server" Text=""></asp:Label>
                </p>
                <p> Username: <asp:TextBox ID="username_tb" runat="server"></asp:TextBox> 
                </p>
            <p> Password: <asp:TextBox ID="password_tb" runat="server" Height="25px" Width="168px" TextMode="Password"></asp:TextBox>                 <asp:Label ID="lbl_pwd" runat="server" Text=""></asp:Label>
                </p>
            <p>    <asp:Button ID="register_button" runat="server" Text="Register" Width="100px" OnClick="register_button_Click"  /> &nbsp;&nbsp;&nbsp; <asp:Button ID="login_button" runat="server" Text="Login" Width="100px" OnClick="login_button_Click" /></p>
                
            </fieldset>
        </div>
        <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>

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
