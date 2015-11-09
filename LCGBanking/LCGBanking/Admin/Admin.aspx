<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Admin.aspx.cs" Inherits="LCGBanking.Admin" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <asp:Label ID="Welcome" runat="server"></asp:Label>
    <br /><br />
    <section class="admin">
            <asp:GridView ID="GridViewDeltagarLista" runat="server">
            </asp:GridView>
        <article id="IndividuellaResultat">
            <script src="http://code.jquery.com/jquery-1.10.2.js"></script>
            <h2>Utvärdera och licensiera provtagare</h2>
            <div id="IndResPersonval">
                <div id="Personval1">
                    <asp:Label ID="LabelListBoxGVIR" runat="server" Text="Välj person:"></asp:Label><br />
                    <asp:ListBox ID="ListBoxGVIndRes" runat="server" Height="191px" Width="215px" AutoPostBack="true" OnSelectedIndexChanged="ListBoxGVIndRes_SelectedIndexChanged"></asp:ListBox>
                    <br />
                    <asp:Label ID="LabelTextBoxGVIR" runat="server" Text="Sök namn:"></asp:Label><br />
                    <asp:TextBox ID="TextBoxGVIndRes" runat="server" Width="209px"></asp:TextBox>
                </div>
                <div id="Personval2">
            <asp:Label ID="LabelIndResNamn" runat="server" Font-Bold="True" Font-Size="Larger"></asp:Label><br />
            <asp:Label ID="LabelIndResDatum" runat="server" Text=""></asp:Label>
            <br /><br />
            <asp:GridView ID="GridViewIndResOversikt" CssClass="GVIndRes" runat="server" OnDataBound="GridViewIndResOversikt_DataBound"></asp:GridView>
                    <asp:Button ID="ButtonIndResGeLicens" runat="server" Text="Ge licens" OnClick="ButtonIndResGeLicens_Click" Enabled="False" Width="102px" />
                </div>
            </div>
            <br />
            <asp:CheckBox ID="CheckBoxSvarText" AutoPostBack="true" Text=" Visa hela svar" runat="server" OnCheckedChanged="CheckBoxSvarText_CheckedChanged" />
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

                if (document.getElementById("ContentPlaceHolder1_CheckBoxSvarText").checked) {
                        $(".GVIndRes_fraga").css("width", "25%");
                    }
                    else{
                        $(".GVIndRes_fraga").css("width", "80%");
                    }
            </script>
        </article>
    </section>
</asp:Content>
