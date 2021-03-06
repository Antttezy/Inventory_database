using Inventory_database.Models;
using Inventory_database.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Inventory_database.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("username")]
    public class UsernameTagHelper : TagHelper
    {
        [HtmlAttributeNotBound, ViewContext]
        public ViewContext ViewContext { get; set; }
        public IAuthenticationProvider AuthenticationProvider { get; }

        public UsernameTagHelper(IAuthenticationProvider authenticationProvider)
        {
            AuthenticationProvider = authenticationProvider;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            string token = ViewContext.HttpContext.Request.Cookies["auth_token"];
            User user = await AuthenticationProvider.GetUserByTokenAsync(token);

            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "p";
            output.AddClass("nav-link", HtmlEncoder.Default);

            if (user != null)
            {
                output.Content.AppendHtml(user.FirstName + " " + user.SecondName);
            }
            else
            {
                output.Content.AppendHtml("Аноним");
            }
        }
    }
}
