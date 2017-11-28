using System.ComponentModel.DataAnnotations;

namespace Mvc5MinSetup.ViewModels.Input
{
    public class TextInput
    {
        [UIHint("Hidden")]
        public string TextId { get; set; }

        [Required]
        public string OriginalText { get; set; }
    }
}