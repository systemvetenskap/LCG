<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Licensiering.aspx.cs" Inherits="LCGBanking.Licensiering" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section>
        
        <section id="test"> 
            <h1>Tester</h1>


        <asp:Button ID="ButtonStart" runat="server" Text="Starta licenseringstest" Height="23px" OnClick="ButtonStart_Click" />
        <br /><br />

            

        <asp:Label ID="LabelQuestion" runat="server" Text="Label"></asp:Label>



        <br /><br />
        <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label> 
        
        <asp:Label ID="Label2" runat="server" Text="Label"></asp:Label>
        

        <asp:Label ID="Label3" runat="server" Text="Label"></asp:Label>
       
            <br /><br />
            <asp:Button ID="ButtonPrevious" runat="server" Text="Föregående fråga" OnClick="ButtonPrevious_Click" />
            <asp:Button ID="ButtonNext" runat="server" Text="Nästa fråga" OnClick="ButtonNext_Click" />
        </section> 
    </section>
  
</asp:Content>

