<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Admin.aspx.cs" Inherits="LCGBanking.Admin" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <asp:Label ID="Welcome" runat="server"></asp:Label>
    <br /><br />
    <section class="admin">
        <!-- har tagit bort id="main" från följande sektion för att undvika problem med vänster marginal -->
        <section class="admin">
            <asp:GridView ID="GridViewDeltagarLista" runat="server">
            </asp:GridView>
        </section>
        <section>
            <script src="http://code.jquery.com/jquery-1.10.2.js"></script>
            <asp:ListBox ID="ListBoxGVIndRes" runat="server" Height="202px" Width="215px" AutoPostBack="true" OnSelectedIndexChanged="ListBoxGVIndRes_SelectedIndexChanged"></asp:ListBox>
            <asp:TextBox ID="TextBoxGVIndRes" runat="server"></asp:TextBox>
            <br /><br />
            <asp:CheckBox ID="CheckBoxSvarText" AutoPostBack="true" Text=" Visa hela svar" runat="server" OnCheckedChanged="CheckBoxSvarText_CheckedChanged" />
            <br /><br />
            <asp:Label ID="LabelIndResKategori1" runat="server" Text=""></asp:Label>
            <asp:GridView ID="GridViewIndividResultat1" runat="server" OnDataBound="GridViewIndividResultat_DataBound"></asp:GridView>
            <br /><br />
            <asp:Label ID="LabelIndResKategori2" runat="server" Text=""></asp:Label>
            <asp:GridView ID="GridViewIndividResultat2" runat="server" OnDataBound="GridViewIndividResultat_DataBound"></asp:GridView>
            <br /><br />
            <asp:Label ID="LabelIndResKategori3" runat="server" Text=""></asp:Label>
            <asp:GridView ID="GridViewIndividResultat3" runat="server" OnDataBound="GridViewIndividResultat_DataBound"></asp:GridView>
            <script>
                document.getElementById("ContentPlaceHolder1_TextBoxGVIndRes").addEventListener("input", ListBoxFilter);
                function ListBoxFilter() {
                    var input = $("#ContentPlaceHolder1_TextBoxGVIndRes").val();
                    var regex = new RegExp(input, "i");
                    var antalPoster = $("#ContentPlaceHolder1_ListBoxGVIndRes").children().length;
                    for (i = 0; i < antalPoster; i++) {
                        var namn = $("#ContentPlaceHolder1_ListBoxGVIndRes").children()[i].innerHTML;
                        if (!namn.match(regex)) {
                            $("#ContentPlaceHolder1_ListBoxGVIndRes option:eq(" + i + ")").hide();
                        }
                        else {
                            $("#ContentPlaceHolder1_ListBoxGVIndRes option:eq(" + i + ")").show();
                        }
                    }
                }
            </script>
        </section>
    </section>
</asp:Content>
