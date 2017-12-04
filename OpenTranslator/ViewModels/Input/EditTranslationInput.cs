using Omu.AwesomeMvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OpenTranslator.ViewModels.Input
{
    public class EditTranslationInput
    {
       
        [UIHint("Hidden")]
        public string TextId { get; set; }

        [Required]
        public string TranslationText { get; set; }
    }
}