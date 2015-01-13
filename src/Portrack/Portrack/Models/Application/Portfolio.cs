using Newtonsoft.Json;
using System.Collections.Generic;

namespace Portrack.Models.Application
{
    public class Portfolio
    {
        public Portfolio()
        {

        }

        public Portfolio(string userName)
        {
            UserName = userName;
        }

        public int PortfolioId { get; set; }
        public string PortfolioName { get; set; }


        public string UserName { get; set; }


        public ICollection<Position> Positions { get; set; }
        public ICollection<Transaction> Transactions { get; set; }

        public PortfolioData PortfolioData { get; set; }    
    }

    public class PortfolioData
    {
        public PortfolioData(Portfolio portfolio)
        {
            Portfolio = portfolio;
        }

        public int PortfolioId { get; set; }
        [JsonIgnore]
        public Portfolio Portfolio { get; private set; }
    }
}