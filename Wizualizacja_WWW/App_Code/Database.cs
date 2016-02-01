using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace Wizualizacja_WWW.App_Code
{
    /*===========================*/
    /* Operacje na bazie danych  */
    /*===========================*/

    public class Database
    {
        /*Server=127.0.0.1;Database=system_magazynowy;Uid=magazyn;Pwd=magazyn; */

        //connection string połączenia z bazą danych (konfiguracja połączenia - Web.config)
        private static string connectionString = String.Format("Server={0};Database={1};Uid={2};Pwd={3};", ConfigurationManager.AppSettings["mysql_server"].ToString(), ConfigurationManager.AppSettings["mysql_database"].ToString(), ConfigurationManager.AppSettings["mysql_username"].ToString(), ConfigurationManager.AppSettings["mysql_password"].ToString());


        //////////////////////////////////////////////////////////////////


        /* Pobierz wszystkie towary + na podstawie filtrów */
        internal static DataTable loadData(string filtr_kod = null, string filtr_nazwa = null)
        {
            MySqlConnection conn;
            DataTable dt = new DataTable(); //output danych

            //parametry -> na lowercase
            filtr_kod = filtr_kod != null ? filtr_kod.ToLower() : null;
            filtr_nazwa = filtr_nazwa != null ? filtr_nazwa.ToLower() : null;

            try
            {
                //połączenie bazą
                conn = new MySqlConnection(Database.connectionString);
                conn.Open();

                //stored procedure
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                cmd.CommandText = "select_towary";
                cmd.CommandType = CommandType.StoredProcedure;

                //parametry zapytania w zależności od filtrów
                if (string.IsNullOrWhiteSpace(filtr_kod) && string.IsNullOrWhiteSpace(filtr_nazwa)) //domyślnie, wyświetl wszystkie
                {
                    cmd.Parameters.AddWithValue("@_kod_towaru", -1);
                    cmd.Parameters["@_kod_towaru"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("@_nazwa_towaru", "");
                    cmd.Parameters["@_nazwa_towaru"].Direction = ParameterDirection.Input;
                }
                else if (!string.IsNullOrWhiteSpace(filtr_kod) && string.IsNullOrWhiteSpace(filtr_nazwa)) //kod_towaru
                {
                    cmd.Parameters.AddWithValue("@_kod_towaru", filtr_kod);
                    cmd.Parameters["@_kod_towaru"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("@_nazwa_towaru", "");
                    cmd.Parameters["@_nazwa_towaru"].Direction = ParameterDirection.Input;
                }
                else if (string.IsNullOrWhiteSpace(filtr_kod) && !string.IsNullOrWhiteSpace(filtr_nazwa)) //nazwa
                {
                    cmd.Parameters.AddWithValue("@_kod_towaru", -1);
                    cmd.Parameters["@_kod_towaru"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("@_nazwa_towaru", filtr_nazwa);
                    cmd.Parameters["@_nazwa_towaru"].Direction = ParameterDirection.Input;
                }
                else if (!string.IsNullOrWhiteSpace(filtr_kod) && !string.IsNullOrWhiteSpace(filtr_nazwa)) //kod_towaru i nazwa
                {
                    cmd.Parameters.AddWithValue("@_kod_towaru", filtr_kod);
                    cmd.Parameters["@_kod_towaru"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("@_nazwa_towaru", filtr_nazwa);
                    cmd.Parameters["@_nazwa_towaru"].Direction = ParameterDirection.Input;
                }

                MySqlDataAdapter sda = new MySqlDataAdapter();
                sda.SelectCommand = cmd;

                sda.Fill(dt);

                conn.Close();
            }
            catch (Exception ex)
            {
                Tools.callJavaScriptCode("alertError(\"" + ex.Message + "\")");
            }

            return dt;
        }

        /* Dodaj nowe zamówienie */
        internal static void addNewOrder(string osoba, string adres, int dostawa, Dictionary<int, int> towar)
        {
            MySqlConnection conn;

            try
            {
                //połączenie z bazą
                conn = new MySqlConnection(Database.connectionString);
                conn.Open();

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                // 1. dodaj nowe zamówienie do tabeli 'zamówienia'

                cmd.CommandText = "dodaj_zamowienie";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@_osoba", osoba);
                cmd.Parameters["@_osoba"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("@_adres", adres);
                cmd.Parameters["@_adres"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("@_dostawa", dostawa);
                cmd.Parameters["@_dostawa"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("@_id", MySqlDbType.Int32);
                cmd.Parameters["@_id"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                int orderID = Convert.ToInt32(cmd.Parameters["@_id"].Value); //pobierz id dodanego zamówienia


                // 2. przypisz towary do zamówienia (w tabeli 'zamówienia_towary')
 
                foreach (var i in towar) //dla każdego elementu z kolekcji towar
                {
                    cmd = new MySqlCommand();
                    cmd.Connection = conn;

                    cmd.CommandText = "dodaj_zamowienie_towary";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@_id_zamowienie", orderID);
                    cmd.Parameters["@_id_zamowienie"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("@_id_towar", i.Key);
                    cmd.Parameters["@_id_towar"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("@_ilosc", i.Value);
                    cmd.Parameters["@_ilosc"].Direction = ParameterDirection.Input;

                    cmd.ExecuteNonQuery();
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                Tools.callJavaScriptCode("alertError(\"" + ex.Message + "\")");

                throw ex;
            }
        }

        /* Pobierz sposoby dostarczenia */
        internal static DataTable getDeliveryMethods()
        {
            MySqlConnection conn;
            DataTable dt = new DataTable();

            try
            {
                conn = new MySqlConnection(Database.connectionString);
                conn.Open();

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                cmd.CommandText = "select_sposoby_dostarczenia_all";
                cmd.CommandType = CommandType.StoredProcedure;

                MySqlDataAdapter sda = new MySqlDataAdapter();

                sda.SelectCommand = cmd;
                sda.Fill(dt);

                conn.Close();
            }
            catch (Exception ex)
            {
                Tools.callJavaScriptCode("alertError(\"" + ex.Message + "\")");
            }

            return dt;
        }


        ////////////////////////////////////////


        /* konwersja DataTable na format JSON */
        internal static string DataTableToJson(DataTable table)
        {
            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(table);
            return jsonString;
        }
    }
}