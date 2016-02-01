using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace Aplikacja_magazynowa
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /* Inicjalizacja programu */
        private void Form1_Load(object sender, EventArgs e)
        {
            //pobierz tabele towarów z bazy danych, ustaw nagłówki kolumn
            this.dataGridView_towary.DataSource = Database.getContent();
            this.dataGridView_towary.Columns["kod_towaru"].HeaderText = "Kod towaru";
            this.dataGridView_towary.Columns["ilosc"].HeaderText = "Ilość";
            this.dataGridView_towary.Columns["typ_towaru"].HeaderText = "Typ towaru";
            this.dataGridView_towary.Columns["nazwa"].HeaderText = "Nazwa";
            this.dataGridView_towary.Columns["data_gwarancji"].HeaderText = "Data gwarancji";

            //pobierz tabele zamówień z bazy danych, ustaw nagłówki kolumn
            this.dataGridView_zamowienia.DataSource = Database.getOrders();
            this.dataGridView_zamowienia.Columns["id"].HeaderText = "ID";
            this.dataGridView_zamowienia.Columns["data_utworzenia"].HeaderText = "Data utworzenia";
            this.dataGridView_zamowienia.Columns["data_realizacji"].HeaderText = "Data realizacji";
            this.dataGridView_zamowienia.Columns["osoba"].HeaderText = "Osoba";
            this.dataGridView_zamowienia.Columns["adres"].HeaderText = "Adres";
            this.dataGridView_zamowienia.Columns["sposob_dostarczenia"].HeaderText = "Sposób dostarczenia";

            //formularz - pole wyboru typu towaru z listy rozwijanej - pobierz dostępne wartości z bazy
            this.cb_typ_towaru.DataSource = Database.getTypes();
            this.cb_typ_towaru.ValueMember = "id";
            this.cb_typ_towaru.DisplayMember = "nazwa";
        }

        /* Event - tabela towarów - zmiana zaznaczonego wiersza */
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            //jeśli nie zaznaczony lub brak wyników
            if (this.dataGridView_towary.SelectedRows.Count <= 0 || this.dataGridView_towary.Rows.Count <= 1)
            {
                return;
            }

            var row = this.dataGridView_towary.SelectedRows[0]; //aktualnie wybrany wiersz
            var rowNum = row.Index; //index wiersza
            this.dataGridView_towary.CurrentCell = this.dataGridView_towary[0, rowNum];

            //wyczyść bindy pól formularza
            this.tb_nazwa.DataBindings.Clear();
            this.tb_kod_towaru.DataBindings.Clear();
            this.tb_ilosc.DataBindings.Clear();
            this.dateTimePicker_data_gwarancji.DataBindings.Clear();
            this.cb_typ_towaru.DataBindings.Clear();

            if (row.Index < this.dataGridView_towary.RowCount - 1) //jeśli zaznaczony inny niż ostatni
            {
                //ustaw stan przycisków
                this.btn_add_new.Enabled = false;
                this.btn_save.Enabled = true;

                //kod_towaru
                var _kod = row.Cells["kod_towaru"].Value;
                if (_kod is DBNull)
                    return;
                int kod = Convert.ToInt32(_kod);

                //! czy dla danego towaru istnieje zamówienie
                bool jestZamowienie = Database.hasOrder(kod);

                if (!jestZamowienie) //jeśli istnieje zamówienie, nie można usunąć wpisu
                {
                    this.btn_remove.Enabled = true;
                    this.lbl_cannot_remove.Visible = false;
                }
                else
                {
                    this.lbl_cannot_remove.Visible = true;
                    this.btn_remove.Enabled = false;
                }

                //przypisz komórki wiersza do pól formularza
                this.tb_nazwa.DataBindings.Add("Text", dataGridView_towary.DataSource, "nazwa", false);
                this.tb_kod_towaru.DataBindings.Add("Text", dataGridView_towary.DataSource, "kod_towaru", false);
                this.tb_ilosc.DataBindings.Add("Text", dataGridView_towary.DataSource, "ilosc", false);
                this.dateTimePicker_data_gwarancji.DataBindings.Add("Text", dataGridView_towary.DataSource, "data_gwarancji");
                this.cb_typ_towaru.DataBindings.Add("SelectedValue", dataGridView_towary.DataSource, "typ_towaru");
            }
            else
            {
                //stan przycisków
                this.btn_add_new.Enabled = true;
                this.btn_save.Enabled = false;
                this.btn_remove.Enabled = false;
            }
        }


        /* Zapis zmian w tabeli towary (przycisk ZAPISZ) */
        private void saveChanges_towary()
        {
            //SqlDataAdapter generuje update na podstawie poprzedniego zapytania
            try
            {
                DataTable changes = ((DataTable)dataGridView_towary.DataSource).GetChanges(); //zmiany w tabeli towary

                if (changes != null)
                {
                    MySqlCommandBuilder mcb = new MySqlCommandBuilder(Database.mySqlDataAdapter_content);
                    Database.mySqlDataAdapter_content.UpdateCommand = mcb.GetUpdateCommand();
                    Database.mySqlDataAdapter_content.Update(changes);
                    ((DataTable)dataGridView_towary.DataSource).AcceptChanges();
                }
            }
            catch (Exception ex)
            {
                Alert.Warning(ex.Message);
            }
        }

        /* Event - tabela towarów - przyciski ZAPISZ, USUŃ, DODAJ */
        private void btn_save_Click(object sender, EventArgs e)
        {
            var rowNum = this.dataGridView_towary.SelectedRows[0].Index; //index wiersza

            if (rowNum < this.dataGridView_towary.RowCount - 1) //jeśli zaznaczony inny niż ostatni
            {
                this.dataGridView_towary.CurrentCell = this.dataGridView_towary[0, rowNum + 1]; //przejdź do następnego

                if (((Button)sender).Name == "btn_remove") //jeśli przycisk USUŃ
                {
                    var dt = (DataTable)this.dataGridView_towary.DataSource; //pobierz DataSource tabeli

                    this.dataGridView_towary.Rows.RemoveAt(rowNum); //usuń wiersz z DS

                    this.dataGridView_towary.CurrentCell = this.dataGridView_towary[0, 0];
                }
            }
            else
            {
                if (((Button)sender).Name == "btn_add_new") // jeśli przycisk DODAJ
                {
                    var dt = (DataTable)this.dataGridView_towary.DataSource; //pobierz DataSource tabeli

                    //dane z pól formularza
                    var kod = this.tb_kod_towaru.Text;
                    var nazwa = this.tb_nazwa.Text;
                    var ilosc = this.tb_ilosc.Text;
                    var dataGw = this.dateTimePicker_data_gwarancji.Value.Date;
                    var typ = this.cb_typ_towaru.SelectedValue;

                    if (!string.IsNullOrWhiteSpace(kod) && !string.IsNullOrWhiteSpace(nazwa) && !string.IsNullOrWhiteSpace(ilosc)) //czy wszystkie pola nie puste
                    {
                        dt.Rows.Add(kod, nazwa, dataGw, typ, ilosc); //dodaj wpis
                    }
                    else
                    {
                        Alert.Warning("Wypełnij wszystkie pola");
                    }
                }
                else
                {
                    this.dataGridView_towary.CurrentCell = this.dataGridView_towary[0, 0]; //przejdź do pierwszego wiersza
                }
            }

            //zapisz zmiany w tabeli
            this.saveChanges_towary();
        }

        /* Event - tabela towarów - pola tekstowe FILTRY */
        private void tb_filter_kod_TextChanged(object sender, EventArgs e)
        {
            this.dataGridView_towary.CurrentCell = this.dataGridView_towary[0, 0]; //wybierz pierwszy wiersz

            var dt = (DataTable)this.dataGridView_towary.DataSource; //pobierz DataSource tabeli

            //wyczyść bindy tabeli towary i pól formularza
            this.dataGridView_towary.DataBindings.Clear();
            this.tb_nazwa.DataBindings.Clear();
            this.tb_kod_towaru.DataBindings.Clear();
            this.tb_ilosc.DataBindings.Clear();
            this.dateTimePicker_data_gwarancji.DataBindings.Clear();
            this.cb_typ_towaru.DataBindings.Clear();

            //pola filtrów
            var filtr_kod = this.tb_filter_kod.Text;
            var filtr_nazwa = this.tb_filter_nazwa.Text;

            //ustaw .RowFilter w zależności od zawartości pól tekstowych
            dt.DefaultView.RowStateFilter = DataViewRowState.OriginalRows;
            if (!string.IsNullOrWhiteSpace(filtr_kod) && !string.IsNullOrWhiteSpace(filtr_nazwa)) //jeśli kod towaru i nazwa podane
            {
                dt.DefaultView.RowFilter = string.Format("kod_towaru = '{0}' AND nazwa LIKE '%{1}%'", Convert.ToInt32(filtr_kod), filtr_nazwa);
            }
            else if (!string.IsNullOrWhiteSpace(filtr_kod) && string.IsNullOrWhiteSpace(filtr_nazwa)) //jeśli podany kod towaru
            {
                dt.DefaultView.RowFilter = string.Format("kod_towaru = '{0}'", Convert.ToInt32(filtr_kod));
            }
            else if (string.IsNullOrWhiteSpace(filtr_kod) && !string.IsNullOrWhiteSpace(filtr_nazwa)) //jeśli podana nazwa towaru
            {
                dt.DefaultView.RowFilter = string.Format("nazwa LIKE '%{0}%'", filtr_nazwa);
            }
            else //puste pola
            {
                dt.DefaultView.RowFilter = "";
            }
        }

        /* Event - pola tekstowe w których zezwalaj jedynie na liczby */
        private void tb_ilosc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back)
            {
                e.Handled = false;
                return;
            }
            int isNumber = 0;
            e.Handled = !int.TryParse(e.KeyChar.ToString(), out isNumber);
        }

        /* Event - tabela zamówień - zmiana zaznaczonego wiersza */
        private void dataGridView_zamowienia_SelectionChanged(object sender, EventArgs e)
        {
            if (this.dataGridView_zamowienia.SelectedRows.Count <= 0) //jeśli brak zaznaczonych wierszy
            {
                return;
            }

            var row = this.dataGridView_zamowienia.SelectedRows[0]; //wybrany wiersz
            var rowNum = row.Index; //index wiersza
            this.dataGridView_zamowienia.CurrentCell = this.dataGridView_zamowienia[0, rowNum];

            //"id"
            var _kod = row.Cells["id"].Value;
            int kod = Convert.ToInt32(_kod);

            DataTable dt = Database.getOrderContent(kod); //pobierz z bazy towary dla danego zamówienia

            this.listView1.Clear(); //wyczyść listę towarów

            //wyświetl listę towarów w list view
            foreach (DataColumn column in dt.Columns)
            {
                this.listView1.Columns.Add(column.ColumnName, 100);
            }

            foreach (DataRow r in dt.Rows)
            {
                ListViewItem item = new ListViewItem(r[0].ToString());
                for (int i = 1; i < dt.Columns.Count; i++)
                {
                    item.SubItems.Add(r[i].ToString());
                }
                this.listView1.Items.Add(item);
            }

            //rozmiar kolumn
            this.listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            this.listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            //nagłówki kolumn
            this.listView1.Columns[0].Text = "Kod towaru";
            this.listView1.Columns[1].Text = "Nazwa";
            this.listView1.Columns[2].Text = "Ilość";
        }

        /* Event - tabela zamówień - przycisk ZREALIZUJ ZAMÓWIENIE */
        private void btn_order_Click(object sender, EventArgs e)
        {
            var row = this.dataGridView_zamowienia.SelectedRows[0]; //wybrany wiersz
            var rowNum = row.Index; //index wiersza

            //"id"
            var _id = row.Cells["id"].Value;
            int id = Convert.ToInt32(_id);

            //czy pole data_realizacji nie jest puste - zamówienie jest już zrealizowane
            string data_realizacji = row.Cells["data_realizacji"].Value.ToString();
            if (!string.IsNullOrWhiteSpace(data_realizacji))
            {
                Alert.Info("Zamówienie jest już zrealizowane!");
                return;
            }

            //pobierz towary z listy, sprawdź czy jest wystarczająca ilość
            foreach (ListViewItem i in this.listView1.Items)
            {
                var kod = Convert.ToInt32(i.SubItems[0].Text);
                var ilosc = Convert.ToInt32(i.SubItems[2].Text);

                var dostepna_ilosc = Database.getContentAmount(kod); //pobierz ilość danego towaru z bazy

                if (dostepna_ilosc < ilosc) //jeśli za mało towaru
                {
                    Alert.Warning(string.Format("Nie można zrealizować zamówienia! Nie wystarczająca ilość towaru (kod_towaru: {0})", kod));

                    return;
                }
            }

            //wykonaj zamówienie - odjęcie z magazynu danej ilości towarów
            foreach (ListViewItem i in this.listView1.Items)
            {
                var kod = Convert.ToInt32(i.SubItems[0].Text);
                var ilosc = Convert.ToInt32(i.SubItems[2].Text);

                Database.updateContentAmount(kod, ilosc);
            }

            row.Cells["data_realizacji"].Value = DateTime.Today.Date; //zapisz datę realizacji

            Database.updateOrders(id, DateTime.Today.Date); //zapis zmian w tabeli zamówienia

            this.dataGridView_zamowienia.CurrentCell = this.dataGridView_zamowienia[0, 0]; //przejdź do pierwszego wiersza
        }

        /* Event - przycisk ODŚWIEŻ DANE */
        private void button1_Click_1(object sender, EventArgs e)
        {
            //ponownie wczytaj dane do tabeli towarów np. po zrealizowaniu zamówienia dane uległy zmianie
            this.Form1_Load(this, new EventArgs());
        }
    }
}