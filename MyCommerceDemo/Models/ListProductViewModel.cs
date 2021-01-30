using System;
using System.Collections.Generic;

namespace MyCommerceDemo.Models
{
    public class ListProductViewModel
    {
        public ListProductViewModel()
        {
            Products = new List<Product>();
        }

        public List<Product> Products { get; set; }

        public string Search { get; set; }
        public string CategoryDesc { get; set; }
        public int Category { get; set; }

        public int ProductTotal { get; set; }
        public int ProductFrom { get; set; }
        public int ProductTo { get; set; }
        public int PageTotal { get; set; }
        public int CurrentPage { get; set; }
    }

    public class Product
    {
        public string unitàmisura { get; set; }
        public string statoarticolo { get; set; }
        public string codicearticolo { get; set; }
        public int idlistino { get; set; }
        public long idarticolo { get; set; }

        public Nullable<long> idmarca { get; set; }

        public Nullable<long> idaziendamaster { get; set; }

        public Nullable<decimal> prezzolistinoaut { get; set; }
        public Nullable<decimal> prezzolistinoman { get; set; }
        public string descrizionebrevearticolo { get; set; }
        public Nullable<decimal> ivavendita { get; set; }

        public string descrizioneestesaarticolo { get; set; }

        public Nullable<decimal> sconto1 { get; set; }
        public Nullable<decimal> sconto2 { get; set; }
        public Nullable<decimal> ricarico1 { get; set; }
        public Nullable<decimal> ricarico2 { get; set; }

        public decimal Price
        {
            get
            {
                decimal prezzo = prezzolistinoaut.Value;
                if (prezzolistinoman.HasValue && prezzolistinoman.Value != 0)
                {
                    prezzo = prezzolistinoman.Value;
                }

                return Math.Max(Math.Round(prezzo, 2), DiscountPrice);
            }
        }

        public decimal DiscountPrice
        {
            get
            {
                decimal prezzo = prezzolistinoaut.Value;
                if (prezzolistinoman.HasValue && prezzolistinoman.Value != 0)
                {
                    prezzo = prezzolistinoman.Value;
                }

                if (sconto1.HasValue && sconto1.Value != 0)
                {
                    prezzo -= (prezzo * sconto1.Value / 100);
                }

                if (sconto2.HasValue && sconto2.Value != 0)
                {
                    prezzo -= (prezzo * sconto2.Value / 100);
                }

                if (ricarico1.HasValue && ricarico1.Value != 0)
                {
                    prezzo += (prezzo * ricarico1.Value / 100);
                }

                if (ricarico2.HasValue && ricarico2.Value != 0)
                {
                    prezzo += (prezzo * ricarico2.Value / 100);
                }

                return Math.Round(prezzo, 2);
            }
        }

        public string Id
        {
            get
            {
                return $"{idlistino}-{idmarca}-{idarticolo}";
            }
        }
    }

}