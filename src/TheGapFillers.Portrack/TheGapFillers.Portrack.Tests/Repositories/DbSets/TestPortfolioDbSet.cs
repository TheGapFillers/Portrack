using System.Linq;
using TheGapFillers.Portrack.Models.Application;

namespace TheGapFillers.Portrack.Tests.Repositories.DbSets
{
    class TestPortfolioDbSet : TestDbSet<Portfolio>
    {
        public override Portfolio Find(params object[] keyValues)
        {
            return this.SingleOrDefault(i => i.PortfolioId == (int)keyValues.Single());
        }
    }
}
