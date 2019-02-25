using FuryTech.BLM.NetStandard;
using FuryTech.BLM.NetStandard.Interfaces;

namespace FuryTech.BLM.EntityFrameworkCore.Tests
{
    public class MockInterpretedEntityCreateInterpreter : InterpretBeforeCreate<MockInterpretedEntity>
    {
        public override MockInterpretedEntity DoInterpret(MockInterpretedEntity entity, IContextInfo context)
        {
            entity.MockInterpretedValue = MockInterpretedValue.CreateInterpreted;
            return entity;
        }
    }
}
