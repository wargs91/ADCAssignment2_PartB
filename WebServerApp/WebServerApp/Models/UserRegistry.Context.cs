//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebServerApp.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class WebServerDbEntities : DbContext
    {
        public WebServerDbEntities()
            : base("name=WebServerDbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<UserRegistry> UserRegistries { get; set; }

        public System.Data.Entity.DbSet<WebServerApp.Models.NetworkStatusTable> NetworkStatusTables { get; set; }
    }
}
