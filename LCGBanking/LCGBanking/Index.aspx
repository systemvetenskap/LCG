<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="LCGBanking.Index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
            <div id="bannerpic">
            <img src="BILDER/bankjelogg.jpg" style="height: 200px; width: 100%" />
            </div>

        <section id="main">
        
        <p>Logga in på JE Banken</p>
            <br />
            <asp:TextBox ID="TextBoxAnvnamn" runat="server"></asp:TextBox>
            <br /><br />
    </section>

</asp:Content>

