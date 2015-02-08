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

    <%-- Informational messages panel --%>
    <asp:Panel ID="InfoPanel" runat="server" Visible="False">
        <asp:Literal ID="InfoPanelLiteral" runat="server"></asp:Literal>
    </asp:Panel>

    <%-- Container for active image --%>
    <asp:Panel ID="ImageContainer" runat="server" Visible="False"></asp:Panel>

    <%-- Container for thumbnails --%>
    <asp:Panel ID="ThumbnailContainer" runat="server" Visible="False">
        <div class="left-fader"></div>
        <div class="right-fader"></div>
        <div class="content">

            <%-- Repeater for thumbnail images --%>
            <asp:Repeater ID="ThumbnailRepeater" runat="server">
                <ItemTemplate>
                    <asp:HyperLink ID="ThumbnailLink" runat="server"
                        NavigateUrl='<%# DataBinder.Eval(Container.DataItem, "imagePath")  %>'
                        CssClass='<%# DataBinder.Eval(Container.DataItem, "cssClass")  %>'
                    >
                        <asp:Image ID="ThumbnailImage" runat="server" ImageUrl='<%# DataBinder.Eval(Container.DataItem, "thumbNailPath")  %>' />
                    </asp:HyperLink>
                </ItemTemplate>
            </asp:Repeater>

        </div>
    </asp:Panel>

    <form id="UploadForm" runat="server">

        <%-- Display all Errors in Validationsummary --%>
        <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="error-message" HeaderText="Följande fel inträffade:" />

        <%-- Upload image panel --%>
        <asp:Panel ID="Panel1" runat="server" GroupingText="Ladda upp bild">
            <asp:FileUpload ID="FileUpload" runat="server" /> <asp:Button ID="UploadButton" runat="server" Text="Ladda upp" OnClick="UploadButton_Click" />
        </asp:Panel>
        
        <%-- Upload image validators --%>
        <asp:RequiredFieldValidator ID="FileUploadRequired" runat="server" ErrorMessage="Var god välj en bild som ska laddas upp." ControlToValidate="FileUpload" />
        <asp:RegularExpressionValidator ID="FileUploadRegularExpressionValidator" runat="server" ErrorMessage="Bilden måste vara en av typerna: .gif, .png eller .jpg" ControlToValidate="FileUpload" ValidationExpression="(.*?)\.(jpg|JPG|png|PNG|gif|GIF)$" Display="None" />

    </form>

    <script type="text/javascript" src="/Scripts/jquery.scrollTo.js" />
</body>
</html>
