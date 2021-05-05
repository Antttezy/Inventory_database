using Inventory_database.Models;
using Inventory_database.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace Inventory_database.TagHelpers
{
    [HtmlTargetElement("anonymous")]
    public class AnonymousTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public IAuthenticationProvider AuthenticationProvider { get; }

        public AnonymousTagHelper(IAuthenticationProvider authenticationProvider)
        {
            AuthenticationProvider = authenticationProvider;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            string token = ViewContext.HttpContext.Request.Cookies["auth_token"];
            User user = await AuthenticationProvider.GetUserByTokenAsync(token);

            if (user == null)
            {
                output.TagName = "div";
            }
            else
            {
                output.SuppressOutput();
            }
        }
    }
}
