using SimpleChat.Core.ViewModel;
using System.ComponentModel.DataAnnotations;

namespace SimpleChat.ViewModel.Message
{
    public class MessageUpdateVM : UpdateVM
    {
        [Required]
        [MinLength(5)]
        [MaxLength(500)]
        public string Text { get; set; }
    }
}
