using Ratbags.Core.Settings;

namespace Ratbags.Comments.API.Models
{
    public class AppSettings : AppSettingsBase
    {   
        public MessagingExtensions MessagingExtensions { get; set; } = default!;
    }

    public class MessagingExtensions
    {
        public string CommentsListTopic { get; set; } = default!;
        public string CommentsCountTopic { get; set; } = default!;
    }
}
