<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserProfile.aspx.cs" Inherits="AsAssignment.UserProfile" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Home Page</title>
    <script type="text/javascript">
        
    </script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <fieldset>
                <legend>User Profile</legend>
                <br />
                Photo:<asp:Image ImageUrl="" ID="Image1" runat="server" />

                <br />
                <br />
                <asp:Label ID="Label1" runat="server" Text="Name: "></asp:Label>
                <asp:Label ID ="lbl_name" runat ="server"></asp:Label>
                <br />
                <br />
                <asp:Label ID="Label2" runat="server" Text="Email Address: "></asp:Label>
                <asp:Label ID ="lbl_email" runat ="server"></asp:Label>
                <br />
                <br />
                <asp:Label ID="Label4" runat="server" Text="Date of Birth: "></asp:Label>
                <asp:Label ID ="lbl_dob" runat ="server"></asp:Label>
                <br />
                <br />
                <asp:Label ID="Label6" runat="server" Text="Credit Card Info: "></asp:Label>
                <asp:Label ID ="lbl_ccInfo" runat ="server"></asp:Label>
                <br />
                <br />
                <asp:Button ID="chg_pwd_button" runat="server" Text="Change Password" OnClick="chg_pwd_button_Click" />
                <br />
                
                <br />
                <asp:Button ID="btn_logOut" runat="server" Text="Log Out" ForeColor="Red" OnClick="btn_logOut_Click" />
                
            </fieldset>
        </div>
    </form>
</body>
</html>
