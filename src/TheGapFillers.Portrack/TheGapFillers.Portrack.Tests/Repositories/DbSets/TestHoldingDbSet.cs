using System.Linq;
using TheGapFillers.Portrack.Models.Application;

namespace TheGapFillers.Portrack.Tests.Repositories.DbSets
{
    class TestHoldingDbSet : TestDbSet<Holding>
    {
        public override Holding Find(params object[] keyValues)
        {
            return this.SingleOrDefault(i => i.HoldingId == (int)keyValues.Single());
        }
    }
}
