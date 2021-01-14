using SimpleChat.Core.Validation;
using SimpleChat.Core.ViewModel;
using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleChat.Data.ViewModel.Message
{
    public record MessageAddVM : AddVM
    {
        [Required]
        [MinLength(5)]
        [MaxLength(500)]
        public string Text { get; set; }
        [Required]
        [GuidValidation]
        public Guid ChatRoomId { get; set; }
    }
}
