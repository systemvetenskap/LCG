<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Licensiering.aspx.cs" Inherits="LCGBanking.Licensiering" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section>
        <asp:Label ID="Welcome" runat="server"></asp:Label>
        <br />
        <asp:Image ID="ImageUser" runat="server" Height="95px" Width="95px" ImageUrl="~/BILDER/userpicdummy.jpg" CssClass="ImageUser" ImageAlign="Right" AlternateText="UserPicDummy" BorderColor="#3366CC" BorderStyle="Double" />
    <section id="main" runat="server">
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
            <asp:Button ID="ButtonSparaProv" runat="server" Text="Skicka in svar / Spara prov" CssClass="roundCorner" Height="35px" TabIndex="2" Width="200px" Visible="False" OnClick="ButtonSparaProv_Click" OnClientClick = "SkickaIn()"/>
            <script type = "text/javascript">
                function SkickaIn() {
                    var confirm_value = document.createElement("INPUT");
                    confirm_value.type = "hidden";
                    confirm_value.name = "confirm_value";
                    if (confirm("Vill du spara/skicka in dina svar?")) {
                        confirm_value.value = "Ja";
                    } else {
                        confirm_value.value = "Nej";
                    }
                    document.forms[0].appendChild(confirm_value);
                }
            </script>
        </div>
        </section>
        <section id="IndividuellaResultat" class="admin" runat="server">
            <script src="http://code.jquery.com/jquery-1.10.2.js"></script>
            <asp:Label ID="LabelIndResNamn" runat="server" Font-Bold="True" Font-Size="Larger"></asp:Label><br />
            <asp:Label ID="LabelIndResDatum" runat="server" Text=""></asp:Label>
            <br /><br />
            <asp:GridView ID="GridViewIndResOversikt" CssClass="GVIndRes" runat="server" OnDataBound="GridViewIndResOversikt_DataBound"></asp:GridView>
            <br /><br />
            <asp:CheckBox ID="CheckBoxSvarText" AutoPostBack="true" Text=" Visa hela svar" runat="server" OnCheckedChanged="CheckBoxSvarText_CheckedChanged"/>
            <br /><br />
            <asp:Label ID="LabelIndResKategori1" runat="server" Font-Bold="True"></asp:Label>
            <asp:GridView ID="GridViewIndividResultat1" CssClass="GVIndRes" runat="server" OnDataBound="GridViewIndividResultat_DataBound"></asp:GridView>
            <br /><br />
            <asp:Label ID="LabelIndResKategori3" runat="server" Font-Bold="True"></asp:Label>
            <asp:GridView ID="GridViewIndividResultat3" CssClass="GVIndRes" runat="server" OnDataBound="GridViewIndividResultat_DataBound"></asp:GridView>
            <br /><br />
            <asp:Label ID="LabelIndResKategori2" runat="server" Font-Bold="True"></asp:Label>
            <asp:GridView ID="GridViewIndividResultat2" CssClass="GVIndRes" runat="server" OnDataBound="GridViewIndividResultat_DataBound"></asp:GridView>
            <script>
                if (document.getElementById("ContentPlaceHolder1_CheckBoxSvarText").checked) {
                        $(".GVIndRes_fraga").css("width", "25%");
                    }
                    else{
                        $(".GVIndRes_fraga").css("width", "80%");
                }

                var date = new Date();
                var datum = date.toLocaleDateString();
                $("#ContentPlaceHolder1_LabelIndResDatum").text(datum);
            </script>
        </section>
    </section>  
</asp:Content>

