using System;
using Newtonsoft.Json;

namespace TheGapFillers.Portrack.Models.Application
{
    public class Portfolio
    {
        [JsonIgnore]
        public int PortfolioId { get; set; }
        public string PortfolioName { get; set; }
        public string UserName { get; set; }

        public Holding PortfolioHolding { get; set; } 
    }



    public class PortfolioException : Exception
    {
        public PortfolioException()
        {
        }

        public PortfolioException(string message)
            : base(message)
        {
        }

        public PortfolioException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}