<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="LCGBanking.Index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="bannerpic">
        <figure class="slider">
                <figure>
                    <asp:Image ID="banner1" runat="server" Height="300px" ImageUrl="~/BILDER/jebanklogg.jpg" />
                    <figcaption>Vårt signum - Värdepapper</figcaption>
                </figure>
            </figure>
    </div>
    <section id="main">
        <h2>JE Banken - Din licens Vår tyngd</h2>
        <p id="fraga">Alla personer som är anställda vid JE-banken och som arbetar med värdepapper måste licensieras. 
            <br />Detta är viktigt för att upprätthålla branschens goda rykte. För att erhålla en licens måste man genomföra ett licensieringstest.
        </p>
        <br />
        <br />
        <h1>Vi investerar i dina krediter!!!</h1>
    </section>
</asp:Content>

