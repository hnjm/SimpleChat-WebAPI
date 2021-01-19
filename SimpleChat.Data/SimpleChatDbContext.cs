using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleChat.Data.Mapping;
using SimpleChat.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleChat.Data
{
    public class SimpleChatDbContext : IdentityDbContext<User, Role, Guid>
    {
        public SimpleChatDbContext(DbContextOptions<SimpleChatDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // new GroupMapping().Set(ref builder);

            builder.Entity<User>().ToTable("Users");
            builder.Entity<Role>().ToTable("Roles");
        }

        public virtual DbSet<ChatRoom> ChatRooms { get; set; }
        public virtual DbSet<ChatRoomUser> ChatRoomUsers { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
    }
}
