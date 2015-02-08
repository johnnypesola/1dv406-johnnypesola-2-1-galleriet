using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace _2_1_galleriet
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/Content/javascript").Include(
                "~/Scripts/main.js",
                "~/Scripts/jquery-2.1.3.min.js"
            ));

            bundles.Add(new StyleBundle("~/Content/styles").Include(
                "~/Content/css/style.css"
            ));

            BundleTable.EnableOptimizations = true;
        }
    }
}