using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Web.Services;
using Wizualizacja_WWW.App_Code;

namespace Wizualizacja_WWW
{
    [System.Runtime.InteropServices.GuidAttribute("C3857BC4-2D12-4DF2-B56E-DF20C42210B6")]
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                loadDataFromDB(null, null);
            }

        }


        protected void btn_filter_nazwa_Click(object sender, EventArgs e)
        {
            GridViewRow row = (GridViewRow)((WebControl)sender).NamingContainer;
            TextBox tbNazwa = (TextBox)row.FindControl("input_filter_nazwa");
            TextBox tbKod = (TextBox)row.FindControl("input_filter_kod");

            loadDataFromDB(tbKod.Text, tbNazwa.Text);
        }

        private void loadDataFromDB(string kod = null, string nazwa = null)
        {
            var dt = Database.loadData(kod, nazwa);
            GridView1.DataSource = dt;
            GridView1.DataBind();
            var span_amount = this.entries_amount;
            span_amount.InnerHtml = dt.Rows.Count.ToString();
        }


        protected void btn_filter_reset_Click(object sender, EventArgs e)
        {
            loadDataFromDB(null, null);
        }

    }
}

