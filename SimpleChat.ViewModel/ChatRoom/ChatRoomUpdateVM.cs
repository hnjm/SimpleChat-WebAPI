using SimpleChat.Core;
using SimpleChat.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SimpleChat.ViewModel.ChatRoom
{
    public record ChatRoomUpdateVM : UpdateVM
    {
        [Required(ErrorMessage= APIStatusCode.ERR03001)]
        [MinLength(5, ErrorMessage= APIStatusCode.ERR03002)]
        [MaxLength(100, ErrorMessage= APIStatusCode.ERR03003)]
        public string Name { get; set; }
        [MaxLength(250, ErrorMessage= APIStatusCode.ERR03003)]
        public string Description { get; set; }
        [Required(ErrorMessage= APIStatusCode.ERR03001)]
        [DefaultValue(false)]
        public bool IsMain { get; set; }
        [Required(ErrorMessage= APIStatusCode.ERR03001)]
        [DefaultValue(false)]
        public bool IsPrivate { get; set; }
        [Required(ErrorMessage= APIStatusCode.ERR03001)]
        public bool IsOneToOneChat { get; set; }

        public List<Guid> Users { get; set; }
    }
}
