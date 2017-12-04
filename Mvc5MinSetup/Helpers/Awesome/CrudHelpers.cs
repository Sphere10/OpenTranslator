using System.Web;
using System.Web.Mvc;
using Omu.AwesomeMvc;
using Omu.Awem.Helpers;

namespace Mvc5MinSetup.Helpers.Awesome
{
    public static class CrudHelpers
    {
        private static UrlHelper GetUrlHelper<T>(HtmlHelper<T> html)
        {
            return new UrlHelper(html.ViewContext.RequestContext);
        }

        /*beging*/
        public static IHtmlString InitCrudPopupsForGrid<T>(this HtmlHelper<T> html, string gridId, string crudController, int createPopupHeight = 430, int maxWidth = 0, string Title=" Item")
        {
            var url = GetUrlHelper(html);

            gridId = html.Awe().GetContextPrefix() + gridId;

            var result =
            html.Awe()
                .InitPopupForm()
                .Name("create" + gridId)
                .Group(gridId)
                .Height(createPopupHeight)
                .MaxWidth(maxWidth)
                .Url(url.Action("Create", crudController))
                .Title("Add " + Title)
                .Modal()
                .Success("utils.itemCreated('" + gridId + "')")
                .ToString()

            + html.Awe()
                  .InitPopupForm()
                  .Name("edit" + gridId)
                  .Group(gridId)
                  .Height(createPopupHeight)
                  .MaxWidth(maxWidth)
                  .Url(url.Action("Edit", crudController))
                  .Title("Edit " + Title)
                  .Modal()
                  .Success("utils.itemEdited('" + gridId + "')")

            + html.Awe()
                  .InitPopupForm()
                  .Name("delete" + gridId)
                  .Group(gridId)
                  .Url(url.Action("Delete", crudController))
                  .Success("utils.itemDeleted('" + gridId + "')")
				  .Title("Delete " + Title)
                  .OnLoad("utils.delConfirmLoad('" + gridId + "')") // calls grid.api.select and animates the row
                  .Height(200)
                  .Modal();

            return new MvcHtmlString(result);
        }
        /*endg*/

        public static IHtmlString InitCrudForGridNest<T>(this HtmlHelper<T> html, string gridId, string crudController)
        {
            var url = GetUrlHelper(html);
            gridId = html.Awe().GetContextPrefix() + gridId;

            var result =
                html.Awe()
                    .InitPopupForm()
                    .Name("create" + gridId)
                    .Group(gridId)
                    .Url(url.Action("Create", crudController))
                    .Mod(o => o.Inline().ShowHeader(false))
                    .Success("utils.itemCreated('" + gridId + "')")
                    .ToString()
                + html.Awe()
                      .InitPopupForm()
                      .Name("edit" + gridId)
                      .Group(gridId)
                      .Url(url.Action("Edit", crudController))
                      .Mod(o => o.Inline().ShowHeader(false))
                      .Success("utils.itemEdited('" + gridId + "')")
                + html.Awe()
                      .InitPopupForm()
                      .Name("delete" + gridId)
                      .Group(gridId)
                      .Url(url.Action("Delete", crudController))
                      .Mod(o => o.Inline().ShowHeader(false))
                      .Success("utils.itemDeleted('" + gridId + "')");

            return new MvcHtmlString(result);
        }

        /*beginal*/
        public static IHtmlString InitCrudPopupsForAjaxList<T>(
           this HtmlHelper<T> html, string ajaxListId, string controller, string popupName)
        {
            var url = GetUrlHelper(html);

            var result =
                html.Awe()
                    .InitPopupForm()
                    .Name("create" + popupName)
                    .Url(url.Action("Create", controller))
                    .Height(200)
                    .Success("utils.itemCreatedAlTbl('" + ajaxListId + "')")
                    .Group(ajaxListId)
                    .Title("Translate String")
                    .ToString()

                + html.Awe()
                      .InitPopupForm()
                      .Name("edit" + popupName)
                      .Url(url.Action("Edit", controller))
                      .Height(200)
                      .Success("utils.itemEditedAl('" + ajaxListId + "')")
                      .Group(ajaxListId)
                      .Title("edit item")

                + html.Awe()
                      .InitPopupForm()
                      .Name("delete" + popupName)
                      .Url(url.Action("Delete", controller))
                      .Success("utils.itemDeletedAl('" + ajaxListId + "')")
                      .Group(ajaxListId)
                      .OkText("Yes")
                      .CancelText("No")
                      .Height(200)
                      .Title("delete item");

            return new MvcHtmlString(result);
        }
        /*endal*/

        public static IHtmlString InitDeletePopupForGrid<T>(this HtmlHelper<T> html, string gridId, string crudController, string action = "Delete")
        {
            var url = GetUrlHelper(html);
            gridId = html.Awe().GetContextPrefix() + gridId;

            var result =
                html.Awe()
                  .InitPopupForm()
                  .Name("delete" + gridId)
                  .Group(gridId)
                  .Url(url.Action(action, crudController))
                  .Success("utils.itemDeleted('" + gridId + "')")
                  .OnLoad("utils.delConfirmLoad('" + gridId + "')") // calls grid.api.select and animates the row
                  .Height(200)
                  .Modal()
                  .ToString();

            return new MvcHtmlString(result);
        }
    }
}