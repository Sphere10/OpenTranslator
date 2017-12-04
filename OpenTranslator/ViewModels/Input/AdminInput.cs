using Omu.AwesomeMvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OpenTranslator.ViewModels.Input
{
    public class AdminInput
    {
        public int Id { get; set; }

        [Required]
        public string TextId { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
		[UIHint("Odropdown")]
        [AweUrl(Action = "GetAllLanguages", Controller = "Languages")]
        [DisplayName("Select Language")]
        public string LanguageCode { get; set; }
    }
}