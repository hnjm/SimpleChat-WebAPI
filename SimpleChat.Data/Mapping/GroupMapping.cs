using Microsoft.EntityFrameworkCore;
using SimpleChat.Core.EntityFramework;
using SimpleChat.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleChat.Data.Mapping
{
    public class GroupMapping : BaseDomainMapping
    {
        //public override void Set(ref ModelBuilder modelBuilder)
        //{
        //    base.Set(ref modelBuilder);

        //    modelBuilder.Entity<Group>()
        //        .Property(s => s.Name)
        //        .HasColumnName("Id")
        //        .HasDefaultValue(0)
        //        .IsRequired();
        //}
    }
}
