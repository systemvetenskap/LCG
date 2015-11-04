<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="LCGBanking.Index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="bannerpic">
        <figure class="slider">
                <figure>
                    <asp:Image ID="banner1" runat="server" Height="220px" ImageUrl="~/BILDER/jebank00.jpg" />
                    <figcaption>Vårt signum - Värdepapper</figcaption>
                </figure>
            </figure>
    </div>
    <section id="main">
            <p id="fraga">Alla personer som är anställda vid JE-banken och som arbetar med värdepapper måste licensieras. 
                <br />Detta är viktigt för att upprätthålla branschens goda rykte. För att erhålla en licens måste man genomföra ett licensieringstest.
            </p>
        </section>
</asp:Content>

