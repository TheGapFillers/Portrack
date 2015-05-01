using System.Linq;
using TheGapFillers.Portrack.Models.Application;

namespace TheGapFillers.Portrack.Tests.Repositories.DbSets
{
    class TestTransactionDbSet : TestDbSet<Transaction>
    {
        public override Transaction Find(params object[] keyValues)
        {
            return this.SingleOrDefault(i => i.TransactionId == (int)keyValues.Single());
        }
    }
}
