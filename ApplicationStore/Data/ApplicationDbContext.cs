using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ApplicationStore.Models;

namespace ApplicationStore.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Application> Applications { get; set; }
        public DbSet<ApplicationCategory> ApplicationCategories { get; set; }
        public DbSet<ApplicationPicture> ApplicationPictures { get; set; }
        public DbSet<ApplicationPublish> ApplicationPublishs { get; set; }
        public DbSet<ApplicationStoreUser> ApplicationStoreUsers { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentLike> CommentLikes { get; set; }
        public DbSet<DownloadApplication> DownloadApplications { get; set; }
        public DbSet<FavorieApplication> FavorieApplications { get; set; }
        public DbSet<Platform> Platforms { get; set; }
    }
}
