using Omu.AwesomeMvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace OpenTranslator.Models.Input
{
    public class TranslationInput 
    {
        public int Id { get; set; }

		[Required]
        public string TextId { get; set; }

		[Required]
        [UIHint("TextArea")]
        public string TranslationText { get; set; }

        [Required]
		[UIHint("Odropdown")]
		[Editable(false)]
        [AweUrl(Action = "GetAllLanguages", Controller = "Languages")]
        [DisplayName("Select Language")]
        public string LanguageCode { get; set; }
    }
}