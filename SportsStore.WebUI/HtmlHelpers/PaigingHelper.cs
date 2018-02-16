using SportsStore.WebUI.Models;
using System;
using System.Text;
using System.Web.Mvc;

namespace SportsStore.WebUI.HtmlHelpers
{
    public static class PaigingHelper
    {
        public static MvcHtmlString PageLinks(this HtmlHelper htmlHelper,
                                              PagingInfo paigingInfo,
                                              Func<int, string> pageUrl)
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 1; i <= paigingInfo.TotalPages; i++)
            {
                TagBuilder tagBuilder = new TagBuilder("a");
                tagBuilder.MergeAttribute("href", pageUrl(i));
                tagBuilder.InnerHtml = i.ToString();

                if (i == paigingInfo.CurrentPage)
                {
                    tagBuilder.AddCssClass("selected");
                    tagBuilder.AddCssClass("btn-primary");
                }
                tagBuilder.AddCssClass("btn btn-default");

                stringBuilder.Append(tagBuilder);
            }

            return MvcHtmlString.Create(stringBuilder.ToString());
        }
    }
}