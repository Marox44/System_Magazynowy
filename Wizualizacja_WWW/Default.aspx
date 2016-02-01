<%@ Page Title="Podgląd magazynu" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Wizualizacja_WWW._Default" %>

<%@ MasterType VirtualPath="~/Site.Master" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <h3>Podgląd magazynu</h3>
    <hr />

    <asp:GridView runat="server" ID="GridView1" CssClass="table table-striped" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" GridLines="None" BorderStyle="None" ShowFooter="false" AllowSorting="true" ClientIDMode="Static">
        <Columns>
            <asp:TemplateField>
                <HeaderTemplate>
                    <div class="row filter_label">
                        <div class="col-xs-12">
                            <div class="btns_filter">
                                <span class="pull-left">Filtry: </span>
                                <asp:Button Text="zastosuj" CssClass="btn btn-primary btn-xs" ID="btn_filter_nazwa" runat="server" OnClick="btn_filter_nazwa_Click" Font-Bold="false" />
                                <asp:Button Text="reset" CssClass="btn btn-warning btn-xs" runat="server" OnClick="btn_filter_reset_Click" ID="btn_filter_reset" />
                            </div>
                        </div>
                    </div>
                </HeaderTemplate>
            </asp:TemplateField>

            <asp:TemplateField SortExpression="kod_towaru">
                <HeaderTemplate>
                    <asp:Label runat="server" Text="Kod towaru"></asp:Label>
                    <div class="row_input">
                        <span class="glyphicon glyphicon-filter"></span>
                        <asp:TextBox runat="server" placeholder="filtr" ID="input_filter_kod" CssClass="form-control input-sm input_filter" EnableViewState="false" ClientIDMode="Static" autocomplete="off"></asp:TextBox>
                    </div>
                </HeaderTemplate>
                <ItemTemplate>
                    <span id="entry_id"><%#Eval("kod_towaru") %></span>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField SortExpression="nazwa">
                <HeaderTemplate>
                    <asp:Label runat="server" Text="Nazwa"></asp:Label>
                    <div class="row_input">
                        <span class="glyphicon glyphicon-filter"></span>
                        <asp:TextBox runat="server" placeholder="filtr" ID="input_filter_nazwa" CssClass="form-control input-sm input_filter" ClientIDMode="Static" autocomplete="off"></asp:TextBox>
                    </div>

                </HeaderTemplate>
                <ItemTemplate>
                    <%#Eval("nazwa") %>
                </ItemTemplate>
            </asp:TemplateField>


            <asp:BoundField DataField="data_gwarancji" HeaderText="Data gwarancji" />
            <asp:BoundField DataField="typ_towaru" HeaderText="Typ towaru" />
            <asp:BoundField DataField="ilosc" HeaderText="Ilość" />

        </Columns>
    </asp:GridView>

    <div>
        Znalezione rekordy: <span id="entries_amount" class="text_bold" runat="server">nn</span>
    </div>



</asp:Content>
