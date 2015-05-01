using System.Linq;
using TheGapFillers.Portrack.Models.Application;

namespace TheGapFillers.Portrack.Tests.Repositories.DbSets
{
    class TestInstrumentDbSet : TestDbSet<Instrument>
    {
        public override Instrument Find(params object[] keyValues)
        {
            return this.SingleOrDefault(i => i.InstrumentId == (int)keyValues.Single());
        }
    }
}
