<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="_2_1_galleriet.Default" ViewStateMode="Disabled" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>2-1-galleriet</title>

    <link href='http://fonts.googleapis.com/css?family=Source+Sans+Pro' rel='stylesheet' type='text/css' />

    <%: Styles.Render("~/Content/styles") %>
    <%: Scripts.Render("~/Content/javascript") %>  

</head>
<body>
    <h1>Galleriet</h1>

    <asp:Panel ID="InfoPanel" runat="server" Visible="False">
        <asp:Literal ID="InfoPanelLiteral" runat="server"></asp:Literal>
    </asp:Panel>

    <asp:Panel ID="ImageContainer" runat="server" Visible="False"></asp:Panel>

    <div runat="server" id="ThumbnailContainer">
        <div class="left-fader"></div>
        <div class="right-fader"></div>
        <div class="content">
            <asp:Repeater ID="ThumbnailRepeater" runat="server">
                <ItemTemplate>
                    <img src="<%# Container.DataItem.ToString()  %>" />
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>

    <form id="form1" runat="server">

        <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="error-message" HeaderText="Följande fel inträffade:" />

        <asp:Panel ID="Panel1" runat="server" GroupingText="Ladda upp bild">
            <asp:FileUpload ID="FileUpload" runat="server" /> <asp:Button ID="UploadButton" runat="server" Text="Ladda upp" OnClick="UploadButton_Click" />
        </asp:Panel>
        
        <asp:RequiredFieldValidator ID="FileUploadRequired" runat="server" ErrorMessage="Var god välj en bild som ska laddas upp." ControlToValidate="FileUpload"></asp:RequiredFieldValidator>    
        
    </form>
</body>
</html>
