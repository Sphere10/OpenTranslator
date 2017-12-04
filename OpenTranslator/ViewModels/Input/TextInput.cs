using System.ComponentModel.DataAnnotations;

namespace OpenTranslator.ViewModels.Input
{
    public class TextInput
    {
        [UIHint("Hidden")]
        public string TextId { get; set; }

        [Required]
        public string OriginalText { get; set; }
    }
}