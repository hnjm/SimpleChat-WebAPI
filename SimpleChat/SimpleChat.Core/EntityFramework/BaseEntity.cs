using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SimpleChat.Core.EntityFramework
{
    public interface IBaseEntity
    {
        Guid Id { get; set; }
        bool IsDeleted { get; set; }
    }
    public record BaseEntity : IBaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
    }
}
