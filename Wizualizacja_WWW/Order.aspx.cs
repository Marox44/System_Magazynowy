using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Wizualizacja_WWW.App_Code;


namespace Wizualizacja_WWW
{
    public partial class Orders : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                loadDataFromDB(null, null);
            }
        }

        private void loadDataFromDB(string kod = null, string nazwa = null)
        {
            var dt = Database.loadData(kod, nazwa);
            GridView_order.DataSource = dt;
            GridView_order.DataBind();
            // var span_amount = this.entries_amount;
            // span_amount.InnerHtml = dt.Rows.Count.ToString();
        }


        [WebMethod]
        public static string loadDataAsJSON()
        {
            return Database.DataTableToJson(Database.loadData(null, null));
        }

        [WebMethod]
        public static string getDeliveryMethodsAsJSON()
        {
            return Database.DataTableToJson(Database.getDeliveryMethods());
        }

        [WebMethod]
        public static string addOrder(Dictionary<string, object> data)
        {
            string osoba = data["osoba"] as string;
            string adres = data["adres"] as string;
            int dostawa = Convert.ToInt32(data["dostawa"]);
            Dictionary<string, object> _towar = data["towar"] as Dictionary<string, object>;

            Dictionary<int, int> towar = new Dictionary<int, int>();

            foreach (var pair in _towar)
            {
                int key = Convert.ToInt32(pair.Key);
                int value = Convert.ToInt32(pair.Value);

                if (value > 0)
                {
                    towar.Add(key, value);
                }
            }

            try
            {
                Database.addNewOrder(osoba, adres, dostawa, towar);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "0";
        }

    }
}

