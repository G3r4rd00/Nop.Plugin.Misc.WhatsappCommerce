using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.WhatsappCommerce.Models
{
    /// <summary>
    /// Represents plugin configuration model
    /// </summary>
    public record ConfigurationModel : BaseNopModel
    {
        [Required]
        [NopResourceDisplayName("Plugins.Misc.WhatsappCommerce.Fields.PhoneNumber")]
        public string PhoneNumber { get; set; }

    }
}