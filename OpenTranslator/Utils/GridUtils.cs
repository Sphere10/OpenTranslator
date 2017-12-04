using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Omu.AwesomeMvc;

namespace OpenTranslator.Utils
{
    public static class GridUtils
    {
        private static string GetPopupName<T>(this HtmlHelper<T> html, string action, string gridId)
        {
            return action + html.Awe().GetContextPrefix() + gridId;
        }

        public static string InlineDeleteFormatForGrid<T>(this HtmlHelper<T> html, string gridId, string key = "Id", bool nofocus = false, string text = "Cancel")
        {
            var popupName = html.GetPopupName("delete", gridId);

            return DeleteFormat(popupName, key, btnClass: "inlDel", nofocus: nofocus)
                   + html.Awe()
                       .Button()
                       .Text(text)
                       .CssClass("gcancelbtn awe-nonselect")
                       .HtmlAttributes(new { style = "display:none;" });
        }

        public static IHtmlString InlineCreateButtonForGrid<T>(this HtmlHelper<T> html, string gridId, object parameters = null, string text = "Create")
        {
            gridId = html.Awe().GetContextPrefix() + gridId;
            var parms = DemoUtils.Encode(parameters);

            return html.Awe().Button()
                .Text(text)
                .CssClass("mbtn")
                .OnClick(string.Format("$('#{0}').data('api').inlineCreate({1})", gridId, parms));
        }

        public static IHtmlString CreateButtonForGrid<T>(this HtmlHelper<T> html, string gridId, object parameters = null, string text = "Create")
        {
            return html.Awe().Button()
                .Text(text)
                .CssClass("mbtn")
                .OnClick(html.Awe().OpenPopup(html.GetPopupName("create", gridId)).Params(parameters));
        }

        public static string EditFormatForGrid<T>(this HtmlHelper<T> html, string gridId, string key = "Id", bool setId = false, bool nofocus = false, int? height = null)
        {
            var popupName = html.GetPopupName("edit", gridId);

            var click = html.Awe().OpenPopup(popupName).Params(new { id = "." + key });
            if (height.HasValue)
            {
                click.Height(height.Value);
            }

            var button = html.Awe().Button()
                .CssClass("awe-nonselect editbtn")
                .Text("<span class='ico-edit'></span>")
                .OnClick(click);

            var attrdict = new Dictionary<string, object>();

            if (setId)
            {
                attrdict.Add("id", $"gbtn{popupName}.{key}");
            }

            if (nofocus)
            {
                attrdict.Add("tabindex", "-1");
            }

            button.HtmlAttributes(attrdict);

            return button.ToString();
        }

        public static string DeleteFormatForGrid<T>(this HtmlHelper<T> html, string gridId, string key = "Id", bool nofocus = false)
        {
            gridId = html.Awe().GetContextPrefix() + gridId;

            return DeleteFormatForGrid(gridId, key, nofocus);
        }

        public static string EditFormat(string popupName, string key = "Id", bool setId = false, bool nofocus = false)
        {
            var idattr = "";
            if (setId)
            {
                idattr = $"id = 'gbtn{popupName}.{key}'";
            }

            var tabindex = nofocus ? "tabindex = \"-1\"" : string.Empty;

            return string.Format("<button type=\"button\" class=\"awe-btn awe-nonselect editbtn\" {3} {2} onclick=\"awe.open('{0}', {{ params:{{ id: '.{1}' }} }}, event)\"><span class='ico-edit'></span></button>",
                popupName, key, idattr, tabindex);
        }

        public static string DeleteFormat(string popupName, string key = "Id", string deleteContent = null, string btnClass = null, bool nofocus = false)
        {
            if (deleteContent == null)
            {
                deleteContent = "<span class='ico-del'></span>";
            }

            var tabindex = nofocus ? "tabindex = \"-1\"" : string.Empty;

           var a = string.Format("<button type=\"button\" class=\"awe-btn awe-nonselect {3}\" {4} onclick=\"awe.open('{0}', {{ params:{{ id: '.{1}' }} }}, event)\">{2}</button>",
                popupName, key, deleteContent, btnClass, tabindex);
			 return a;
        }

        public static string InlineDeleteFormat(string popupName, string key = "Id", bool nofocus = false)
        {
            return DeleteFormat(popupName, key, btnClass: "inlDel", nofocus: nofocus)
                + "<button type=\"button\" class=\"awe-btn gcancelbtn awe-nonselect\" style=\"display:none;\">Cancel</button>";
        }

        public static string InlineEditFormat(bool nofocus = false)
        {
            var tabindex = nofocus ? "tabindex = \"-1\"" : string.Empty;
            return string.Format("<button type=\"button\" class=\"awe-btn geditbtn awe-nonselect\" {0} >Edit</button><button type=\"button\" class=\"awe-btn gsavebtn awe-nonselect\" style=\"display:none;\">Save</button>", tabindex);
        }

        public static string EditFormatForGrid(string gridId, string key = "Id", bool setId = false, bool nofocus = false)
        {
            return EditFormat("edit" + gridId, key, setId, nofocus);
        }

        public static string DeleteFormatForGrid(string gridId, string key = "Id", bool nofocus = false)
        {
            return DeleteFormat("delete" + gridId, key, null, null, nofocus);
        }

        public static string EditGridNestFormat()
        {
            return "<button type='button' class='awe-btn editnst'><span class='ico-edit'></span></button>";
        }

        public static string DeleteGridNestFormat()
        {
            return "<button type='button' class='awe-btn delnst'><span class='ico-del'></span></button>";
        }

        public static string AddChildFormat()
        {
            return "<button type='button' class='awe-btn awe-nonselect' onclick=\"awe.open('createNode', { params:{ parentId: '.Id' } })\">add child</button>";
        }

        public static string MoveBtn()
        {
            return "<button type=\"button\" class=\"awe-movebtn awe-btn\" tabindex=\"-1\"><i class=\"awe-icon\"></i></button>";
        }
    }
}