﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Codice generato da un modello.
//
//     Le modifiche manuali a questo file potrebbero causare un comportamento imprevisto dell'applicazione.
//     Se il codice viene rigenerato, le modifiche manuali al file verranno sovrascritte.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MyCommerceDemo.Database
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class onelightnetEntities : DbContext
    {
        public onelightnetEntities()
            : base("name=onelightnetEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<listinimarche> listinimarche { get; set; }
        public virtual DbSet<listinoworktemp> listinoworktemp { get; set; }
        public virtual DbSet<tuteweb> tuteweb { get; set; }
        public virtual DbSet<articoliordineclienteweb> articoliordineclienteweb { get; set; }
        public virtual DbSet<datiordineclienteweb> datiordineclienteweb { get; set; }
        public virtual DbSet<Marchegestite> Marchegestite { get; set; }
        public virtual DbSet<CLIENTI> CLIENTI { get; set; }
    }
}
