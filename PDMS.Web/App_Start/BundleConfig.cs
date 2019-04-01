using System.Web;
using System.Web.Optimization;

namespace PDMS.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //JS..................................................................................................................................................

            #region Javascript
            //basic js framework
            bundles.Add(new ScriptBundle("~/bundles/basic_framework").Include(
                        "~/Content/AdminLTE/plugins/jQuery/jQuery-2.1.4.min.js",
                        "~/Content/AdminLTE/bootstrap/js/bootstrap.min.js",
                        "~/Content/AdminLTE/dist/js/bootstrap-hover-dropdown.min.js",
                        "~/Scripts/plugins/jquery-validate/jquery.validate.min.js",
                        "~/Scripts/plugins/jquery.form.js",
                        "~/Scripts/plugins/jquery.blockUI.js",
                        "~/Scripts/plugins/bootstrap-datetimepicker/js/bootstrap-datetimepicker.min.js",

                        "~/Content/AdminLTE/dist/js/app.js",
                        "~/Scripts/plugins/moment.min.js",
                        "~/Scripts/plugins/jquery.blockUI.js",
                        "~/Scripts/plugins/js.cookie.js",
                        "~/Scripts/plugins/jquery.storageapi.min.js",
                        "~/Scripts/plugins/select2.min.js",
                        "~/Scripts/plugins/jquery.dragsort-0.5.2.js"
                        //"~/Scripts/bootstrap-select.min.js"

                        ));

            //modernizr JS-->
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/plugins/modernizr-*"));

            //kendo JS-->
            bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
            "~/Scripts/plugins/kendo/2013.2.716/kendo.web.min.js",
            "~/Scripts/plugins/kendo/2013.2.716/cultures/kendo.culture.zh-TW.min.js"
            ));

            //datatables-->
            bundles.Add(new ScriptBundle("~/bundles/DataTables").Include(
            "~/Content/AdminLTE/plugins/DataTables/DataTables-1.10.9/js/jquery.dataTables.js",
            "~/Content/AdminLTE/plugins/DataTables/DataTables-1.10.9/js/dataTables.bootstrap.js",
            "~/Content/AdminLTE/plugins/DataTables/ColReorder-1.2.0/js/dataTables.colReorder.js",
            "~/Content/AdminLTE/plugins/DataTables/FixedColumns-3.1.0/js/dataTables.fixedColumns.js",
            "~/Content/AdminLTE/plugins/DataTables/FixedHeader-3.1.0/js/dataTables.fixedHeader.js",
            "~/Content/AdminLTE/plugins/DataTables/Responsive-1.0.7/js/dataTables.responsive.js",
            "~/Content/AdminLTE/plugins/DataTables/Select-1.0.1/js/dataTables.select.js",
            "~/Content/AdminLTE/plugins/DataTables/Select-1.0.1/js/dataTables.scroller.min.js",
            "~/Scripts/plugins/jquery.pagination-1.2.7.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/photoswipe").Include(
            "~/Scripts/plugins/photo-swipe/js/photoswipe.min.js",
            "~/Scripts/plugins/photo-swipe/js/photoswipe-ui-default.min.js"
            ));

            //spp framework-->
            bundles.Add(new ScriptBundle("~/bundles/spp-framework").Include(
            "~/Scripts/spp-common.js",
            "~/Scripts/spp.js"
            ));


            #endregion

            //CSS..................................................................................................................................................

            #region CSS
            //DataTables Css -->
            bundles.Add(new StyleBundle("~/Content/DataTables").Include(
            "~/Content/AdminLTE/plugins/DataTables/DataTables-1.10.9/css/dataTables.bootstrap.css",
            "~/Content/AdminLTE/plugins/DataTables/Buttons-1.0.3/css/buttons.bootstrap.css",
            "~/Content/AdminLTE/plugins/DataTables/ColReorder-1.2.0/css/colReorder.bootstrap.css",
            "~/Content/AdminLTE/plugins/DataTables/FixedColumns-3.1.0/css/fixedColumns.bootstrap.css",
            "~/Content/AdminLTE/plugins/DataTables/FixedHeader-3.1.0/css/fixedHeader.dataTables.css",
            "~/Content/AdminLTE/plugins/DataTables/Responsive-1.0.7/css/responsive.bootstrap.css",
            "~/Content/AdminLTE/plugins/DataTables/Select-1.0.1/css/select.bootstrap.css"
            ));

            //AdminLTE Theme style -->
            bundles.Add(new StyleBundle("~/Content/AdminLTE_Theme").Include(
            "~/Content/AdminLTE/dist/css/AdminLTE.css",
            "~/Content/AdminLTE/dist/css/skins/skin-blue.min.css"         
            ));

            bundles.Add(new StyleBundle("~/Content/css/css").Include(
                "~/Content/css/Portal.css",
                "~/Content/css/Special.css",
                "~/Content/css/Program.css",
                "~/Content/css/pdms-style.css",
               "~/Content/css/select2.css",
                "~/Content/css/select2-bootstrap.css",
                "~/Content/css/icomoon.css"
                //"~/Content/css/bootstrap-select.css"
                ));

            //kendo Css -->
            //todo:no css file
            bundles.Add(new StyleBundle("~/Content/kendo").Include(
            "~/Content/css/kendo/2013.2.716/kendo.common.min.css",
            "~/Content/css/kendo/2013.2.716/kendo.bootstrap.min.css"
            ));

            bundles.Add(new StyleBundle("~/Content/photoswipe").Include(
            "~/Scripts/plugins/photo-swipe/css/photoswipe.css",
            "~/Scripts/plugins/photo-swipe/css/default-skin/default-skin.css"
            ));
            #endregion
        }
    }
}
