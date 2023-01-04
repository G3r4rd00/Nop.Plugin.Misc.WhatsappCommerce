using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.WhatsappCommerce
{
    /// <summary>
    /// Represents a plugin settings
    /// </summary>
    public class WhatsappCommerceSettings : ISettings
    {
        public string PhoneNumber { get; set; }
    }
}