//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Marchegestite
    {
        public long idmarca { get; set; }
        public string descrizionemarca { get; set; }
        public Nullable<System.DateTime> dataaggiornamento { get; set; }
        public string descrizionebreve { get; set; }
        public Nullable<bool> selezionato { get; set; }
        public Nullable<long> idaziendamaster { get; set; }
        public string aziendamaster { get; set; }
        public string utente { get; set; }
        public Nullable<decimal> dataoramodifica { get; set; }
        public string inuso { get; set; }
        public Nullable<decimal> sconto1 { get; set; }
        public Nullable<decimal> sconto2 { get; set; }
        public Nullable<decimal> sconto3 { get; set; }
        public Nullable<decimal> sconto4 { get; set; }
        public Nullable<decimal> ricarico1 { get; set; }
        public Nullable<decimal> ricarico2 { get; set; }
        public string note { get; set; }
        public Nullable<long> idfornitoreassociato { get; set; }
        public string fornitoreassociato { get; set; }
        public string cittàlegale { get; set; }
        public string siglalegale { get; set; }
        public string sottozonalegale { get; set; }
    }
}
