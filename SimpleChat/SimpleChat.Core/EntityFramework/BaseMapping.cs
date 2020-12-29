using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleChat.Core.EntityFramework
{
    public interface IBaseDomainMapping
    {
        void Set(ref ModelBuilder modelBuilder);
    }

    public abstract class BaseDomainMapping : IBaseDomainMapping
    {
        //public virtual void Set(ref ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<BaseEntity>()
        //        .Property(s => s.Id)
        //        .HasKey()
        //        .IsRequired();

        //    modelBuilder.Entity<BaseEntity>()
        //        .Property(s => s.IsDeleted)
        //        .ValueGeneratedOnAdd()
        //        .IsRequired();
        //}
    }
}
