using System.ComponentModel.DataAnnotations;

namespace OpenTranslator.ViewModels.Input
{
    public class LanguageInput
    {
        [UIHint("Hidden")]
        public string Id { get; set; }

        [Required]
        public string LanguageCode { get; set; }
		[Required]
        public string LanguageName { get; set; }
    }
}