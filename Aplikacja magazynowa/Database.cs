using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;

namespace Aplikacja_magazynowa
{
    /*===========================*/
   /* Operacje na bazie danych  */
  /*===========================*/
    public static class Database
    {
        //przykładowy connection string:

        /* Server=127.0.0.1;Database=system_magazynowy;Uid=magazyn;Pwd=magazyn; */

        //connection string połączenia z bazą danych (konfiguracja połączenia - App.config)
        private static string connectionString = String.Format("Server={0};Database={1};Uid={2};Pwd={3};", ConfigurationManager.AppSettings["mysql_server"].ToString(), ConfigurationManager.AppSettings["mysql_database"].ToString(), ConfigurationManager.AppSettings["mysql_username"].ToString(), ConfigurationManager.AppSettings["mysql_password"].ToString());

        //SqlDataAdapter zadeklarowane globalnie, potrzebne do ponownego wykorzystania
        internal static MySqlDataAdapter mySqlDataAdapter_content = new MySqlDataAdapter();
        internal static MySqlDataAdapter mySqlDataAdapter_orders = new MySqlDataAdapter();


        //////////////////////////////////////////////////////////////////


        /* Pobierz wszystkie towary */
        internal static DataTable getContent()
        {
            MySqlConnection conn; //połączenie z MySql
            DataTable dt = new DataTable(); //output na dane

            try
            {
                //połącz
                conn = new MySqlConnection(Database.connectionString);
                conn.Open();

                //użyj stored procedure
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                cmd.CommandText = "select_towary_all";
                cmd.CommandType = CommandType.StoredProcedure;

                //SqlDataAdapter zwraca dane do DataTable
                Database.mySqlDataAdapter_content.SelectCommand = cmd;
                Database.mySqlDataAdapter_content.Fill(dt);

                conn.Close(); //zamknij połączenie
            }
            catch (Exception ex)
            {
                Alert.Error(ex.Message);
                Alert.Error("Błąd operacji bazy danych");
            }

            return dt;
        }

        /* Pobierz typy towarów */
        internal static DataTable getTypes()
        {
            MySqlConnection conn;
            DataTable dt = new DataTable();

            try
            {
                conn = new MySqlConnection(Database.connectionString);
                conn.Open();

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                cmd.CommandText = "select_typy_towaru_all";
                cmd.CommandType = CommandType.StoredProcedure;

                var mySqlDataAdapter = new MySqlDataAdapter();

                mySqlDataAdapter.SelectCommand = cmd;
                mySqlDataAdapter.Fill(dt);

                conn.Close();
            }
            catch (Exception ex)
            {
                Alert.Error(ex.Message);
                Alert.Error("Błąd operacji bazy danych");
            }

            return dt;
        }

        /* Sprawdź czy na towar są zamówienia */
        internal static bool hasOrder(int kod_towaru)
        {
            MySqlConnection conn;
            DataTable dt = new DataTable();

            try
            {
                conn = new MySqlConnection(Database.connectionString);
                conn.Open();

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                cmd.CommandText = "zamowienia_towaru";
                cmd.CommandType = CommandType.StoredProcedure;

                //stored procedure - parametry
                cmd.Parameters.AddWithValue("@_kod_towaru", kod_towaru);
                cmd.Parameters["@_kod_towaru"].Direction = ParameterDirection.Input;

                var mySqlDataAdapter = new MySqlDataAdapter();

                mySqlDataAdapter.SelectCommand = cmd;
                mySqlDataAdapter.Fill(dt);

                conn.Close();
            }
            catch (Exception ex)
            {
                Alert.Error(ex.Message);
                Alert.Error("Błąd operacji bazy danych");
            }

            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /* Pobierz wszystkie zamówienia */
        internal static DataTable getOrders()
        {
            MySqlConnection conn;
            DataTable dt = new DataTable();

            try
            {
                conn = new MySqlConnection(Database.connectionString);
                conn.Open();

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                cmd.CommandText = "select_zamowienia_all";
                cmd.CommandType = CommandType.StoredProcedure;

                Database.mySqlDataAdapter_orders.SelectCommand = cmd;
                Database.mySqlDataAdapter_orders.Fill(dt);

                conn.Close();
            }
            catch (Exception ex)
            {
                Alert.Error(ex.Message);
                Alert.Error("Błąd operacji bazy danych");
            }

            return dt;
        }

        /* Pobierz towary dla danego zamówienia */
        internal static DataTable getOrderContent(int id_zamowienia)
        {
            MySqlConnection conn;
            DataTable dt = new DataTable();

            try
            {
                conn = new MySqlConnection(Database.connectionString);
                conn.Open();

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                cmd.CommandText = "towary_zamowienia";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@_id_zamowienia", id_zamowienia);
                cmd.Parameters["@_id_zamowienia"].Direction = ParameterDirection.Input;

                var mySqlDataAdapter = new MySqlDataAdapter();

                mySqlDataAdapter.SelectCommand = cmd;
                mySqlDataAdapter.Fill(dt);

                conn.Close();
            }
            catch (Exception ex)
            {
                Alert.Error(ex.Message);
                Alert.Error("Błąd operacji bazy danych");
            }

            return dt;
        }

        /* Aktualizuj zamówienie */
        internal static void updateOrders(int id_zamowienia, DateTime data_realizacji)
        {
            MySqlConnection conn;

            try
            {
                conn = new MySqlConnection(Database.connectionString);
                conn.Open();

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                cmd.CommandText = "update_zamowienia";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@_id", id_zamowienia);
                cmd.Parameters["@_id"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("@_data", data_realizacji);
                cmd.Parameters["@_data"].Direction = ParameterDirection.Input;

                cmd.ExecuteNonQuery();

                conn.Close();
            }
            catch (Exception ex)
            {
                Alert.Error(ex.Message);
                Alert.Error("Błąd operacji bazy danych");
            }
        }

        /* Zmień ilość danego towaru */
        internal static void updateContentAmount(int kod_towaru, int ilosc)
        {
            MySqlConnection conn;

            try
            {
                conn = new MySqlConnection(Database.connectionString);
                conn.Open();

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                cmd.CommandText = "towary_ilosc";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@_id_towar", kod_towaru);
                cmd.Parameters["@_id_towar"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("@_ilosc", ilosc);
                cmd.Parameters["@_ilosc"].Direction = ParameterDirection.Input;

                cmd.ExecuteNonQuery();

                conn.Close();
            }
            catch (Exception ex)
            {
                Alert.Error(ex.Message);
                Alert.Error("Błąd operacji bazy danych");
            }
        }

        /* Pobierz ilość danego towaru */
        internal static int getContentAmount(int kod_towaru)
        {
            MySqlConnection conn;
            DataTable dt = new DataTable();

            try
            {
                conn = new MySqlConnection(Database.connectionString);
                conn.Open();

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                cmd.CommandText = "select_towary_ilosc";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@_kod_towaru", kod_towaru);
                cmd.Parameters["@_kod_towaru"].Direction = ParameterDirection.Input;

                var da = new MySqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(dt);

                conn.Close();
            }
            catch (Exception ex)
            {
                Alert.Error(ex.Message);
                Alert.Error("Błąd operacji bazy danych");
            }

            if (dt.Rows.Count > 0)
            {
                return Convert.ToInt32(dt.Rows[0]["ilosc"]);
            }
            else
            {
                return 0;
            }
        }
    }
}