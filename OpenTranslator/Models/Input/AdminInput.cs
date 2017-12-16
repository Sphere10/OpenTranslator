using Omu.AwesomeMvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OpenTranslator.Models.Input
{
    public class AdminInput
    {
        public int Id { get; set; }

        [Required]
        public string TextId { get; set; }

        [Required]
        [UIHint("TextArea")]
		[AweMeta("placeholder", "try Ma...")]
        public string Text { get; set; }

        [Required]
		[UIHint("Odropdown")]
        [AweUrl(Action = "GetAllLanguages", Controller = "Languages")]
        [DisplayName("Select Language")]
        public string LanguageCode { get; set; }
    }
	 public class GridArrayRow
    {
        public string TextId { get; set; }

        public string[] Values { get; set; }
    }
}