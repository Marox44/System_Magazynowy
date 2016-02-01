<%@ Page Title="Dodawanie zamówienia" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Order.aspx.cs" Inherits="Wizualizacja_WWW.Orders" %>

<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="OrdersContent" ContentPlaceHolderID="MainContent" runat="server">

    <script>
        $(function () {
            loadData();
            loadDataToOrderForm();
        });
    </script>


    <h3>Złóż zamówienie</h3>
    <hr />

    <div class="row">
        <div class="col-sm-1"></div>
        <div class="col-sm-10">
            <asp:GridView runat="server" ID="GridView_order" CssClass="table table-striped table-condensed" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" GridLines="None" BorderStyle="None" ShowFooter="false" AllowSorting="true" ClientIDMode="Static">
                <Columns>
                    <asp:TemplateField SortExpression="kod_towaru">
                        <HeaderTemplate>
                            <asp:Label runat="server" Text="Kod towaru"></asp:Label>
                            <div class="row_input">
                                <asp:TextBox runat="server" placeholder="filtr" ID="input_filter_kod_order" CssClass="form-control input-sm input_filter" oninput="filter_orders();" autocomplete="off" ClientIDMode="Static"></asp:TextBox>
                            </div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <span class="entry_id"><%#Eval("kod_towaru") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField SortExpression="nazwa">
                        <HeaderTemplate>
                            <asp:Label runat="server" Text="Nazwa"></asp:Label>
                            <div class="row_input">
                                <asp:TextBox runat="server" placeholder="filtr" ID="input_filter_nazwa_order" CssClass="form-control input-sm input_filter" oninput="filter_orders();" autocomplete="off" ClientIDMode="Static"></asp:TextBox>
                            </div>

                        </HeaderTemplate>
                        <ItemTemplate>
                            <span class="entry_name"><%#Eval("nazwa") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="ilosc" HeaderText="Dostępna ilość" />

                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label runat="server" Text="Zamów"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <div>
                                <button type="button" class="btn btn-danger btn-xs" onclick="btn_order_minus(<%#Eval("kod_towaru") %>, this);" data-entry-id="" disabled><i class="glyphicon glyphicon-minus"></i></button>
                                <span class="order_amount">0</span>
                                <button type="button" class="btn btn-success btn-xs" onclick="btn_order_plus(<%#Eval("kod_towaru") %>, this);" data-entry-id=""><i class="glyphicon glyphicon-plus"></i></button>
                        </ItemTemplate>
                    </asp:TemplateField>

                </Columns>
            </asp:GridView>
        </div>
    </div>

    <hr />

    <div class="form-horizontal">
        <div class="form-group">
            <label for="inputEmail3" class="col-sm-4 control-label">Imię i nazwisko</label>
            <div class="col-sm-6">
                <input type="text" class="form-control" id="input_osoba" placeholder="np.  Jan Kowalski" required>
            </div>
        </div>

        <div class="form-group">

            <label for="inputEmail3" class="col-sm-4 control-label">Adres</label>
            <div class="col-sm-6">
                <input type="text" class="form-control" id="input_adres" placeholder="np.  ul. Piłsudskiego 44, 02-122 Warszawa" required>
            </div>
        </div>

        <div class="form-group">
            <label for="inputEmail3" class="col-sm-4 control-label">Sposób dostarczenia</label>
            <div class="col-sm-2">
                <select class="form-control" id="select_delivery">
                </select>
            </div>
        </div>

        <div class="row">
            <div class="col-sm-4"></div>
            <div class="col-sm-4">
                <button type="button" class="btn btn-primary" onclick="addOrder();">Wyślij</button>
            </div>
        </div>
    </div>


</asp:Content>

