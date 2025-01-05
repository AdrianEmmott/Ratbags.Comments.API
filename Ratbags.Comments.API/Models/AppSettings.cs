using Ratbags.Core.Settings;

namespace Ratbags.Comments.API.Models
{
    public class AppSettings : AppSettingsBase
    {
        public string AZSBTestConnection { get; set; } = default!;
    }
}
