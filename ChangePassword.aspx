<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="AsAssignment.ChangePassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }
        .auto-style2 {
            width: 498px;
        }
        .auto-style3 {
            width: 319px;
        }
    </style>
</head>
<body>
    <fieldset>
        <legend>Change Password</legend>
        <form id="form1" runat="server">
            <table class="auto-style1">
                <tr>
                    <td class="auto-style3">&nbsp;</td>
                    <td class="auto-style2">
                        &nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style3">Old Password</td>
                    <td class="auto-style2">
                        <asp:TextBox ID="old_pwd_tb" runat="server" Width="400px" TextMode="Password"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style3">New Password</td>
                    <td class="auto-style2">
                        <asp:TextBox ID="new_pwd_tb" runat="server" Width="400px" TextMode="Password"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style3">Confirm New Password:</td>
                    <td class="auto-style2">
                        <asp:TextBox ID="cfm_pwd_tb" runat="server" Width="400px" TextMode="Password"></asp:TextBox>
                    </td>
                    <td>
                        
                        <asp:CompareValidator ID="CompareValidator1" ControlToValidate="cfm_pwd_tb" ControlToCompare="new_pwd_tb" Operator="Equal" runat="server" ErrorMessage="Is not the same as New Password field!"></asp:CompareValidator>
                        
                    </td>
                </tr>
                <tr>
                    <td class="auto-style3">&nbsp;</td>
                    <td class="auto-style2">&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style3">&nbsp;</td>
                    <td class="auto-style2">
                        <asp:Button ID="chnpwd_button" runat="server" Text="Change Password" OnClick="chnpwd_button_Click" />
                    </td>
                    <td>&nbsp;</td>
                </tr>
            </table>
    
        </form>
    </fieldset>
</body>
</html>
