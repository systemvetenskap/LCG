﻿<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Admin.aspx.cs" Inherits="LCGBanking.Admin" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="admin">
        <!-- har tagit bort id="main" från följande sektion för att undvika problem med vänster marginal -->
        <section class="admin">
            <asp:GridView ID="GridViewDeltagarLista" runat="server">
            </asp:GridView>
        </section>
    </section>
</asp:Content>
