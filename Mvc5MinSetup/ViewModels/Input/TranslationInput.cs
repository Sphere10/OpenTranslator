using Omu.AwesomeMvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Mvc5MinSetup.ViewModels.Input
{
    public class TranslationInput
    {
        public int Id { get; set; }

        [UIHint("Hidden")]
        public string TextId { get; set; }

        [Required]
        public string TranslationText { get; set; }

        [Required]
		[UIHint("Odropdown")]
        [AweUrl(Action = "GetAllLanguages", Controller = "Languages")]
        [DisplayName("Select Language")]
        public string LanguageCode { get; set; }
    }
}