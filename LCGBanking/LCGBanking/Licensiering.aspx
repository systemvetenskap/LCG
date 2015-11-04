<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Licensiering.aspx.cs" Inherits="LCGBanking.Licensiering" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section>
        <asp:Label ID="Welcome" runat="server"></asp:Label>
        <br />
        <asp:Image ID="ImageUser" runat="server" Height="95px" Width="95px" ImageUrl="~/BILDER/userpicdummy.jpg" CssClass="ImageUser" ImageAlign="Right" AlternateText="UserPicDummy" BorderColor="#3366CC" BorderStyle="Double" />
    <section id="main">
        <nav id="QuestionNavbar">
            <h4><asp:Label ID="LabelKategori" runat="server"></asp:Label></h4>
            <br />
            <asp:Button ID="ButtonStart" runat="server" Text="" Height="35px" OnClick="ButtonStart_Click" Width="220px" CssClass="roundCorner" />
            <asp:Repeater ID="RepeaterQuestNav" runat="server">
                <ItemTemplate>
                    <asp:LinkButton ID="LinkButtonQuestNav" CssClass='<%# Eval("value") %>' Text='<%# Eval("key") %>' runat="server" OnClick="LinkButtonQuestNav_Click"></asp:LinkButton>
                </ItemTemplate>
            </asp:Repeater>
        </nav>
               
        <div id="fraga">
            <asp:Label ID="LabelQuestion" runat="server"></asp:Label>
            <p class="p-mindretext"><asp:Label ID="LabelInfo" runat="server"></asp:Label></p>
        </div>
        <div id="svar">
            <asp:Panel ID="PanelSvar" runat="server"></asp:Panel>                
        </div>
        <div id="knappar">
            <asp:Button ID="ButtonPrevious" runat="server" Text="&larr;  Föregående fråga" OnClick="ButtonPrevious_Click" CssClass="roundCorner" Height="35px" Width="175px" Visible="False" />
            <asp:Button ID="ButtonNext" runat="server" Text="Nästa fråga  &rarr;" OnClick="ButtonNext_Click" CssClass="roundCorner" Height="35px" TabIndex="1" Width="175px" Visible="False" />
            <asp:Button ID="ButtonSparaProv" runat="server" OnClick="ButtonSparaProv_Click" Text="Skicka in svar / Spara prov" CssClass="roundCorner" Height="35px" TabIndex="2" Width="200px" Visible="False" />
            <asp:Label ID="Msg" runat="server" CssClass="Msg" Visible="False"></asp:Label>
        </div>
        </section>
    </section>  
</asp:Content>

