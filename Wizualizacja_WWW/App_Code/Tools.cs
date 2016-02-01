using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Wizualizacja_WWW.App_Code
{
    /* Globalne metody  */
    public static class Tools
    {
        /* Wyonanie kodu JS na aktualnej stronie */
        internal static void callJavaScriptCode(string code)
        {
            var p = (Page)HttpContext.Current.Handler;
            ScriptManager.RegisterStartupScript(p, typeof(Page), "js_method_" + p.GetHashCode(), code, true);
        }
    }
}
