using FuryTech.BLM.NetStandard;
using FuryTech.BLM.NetStandard.Interfaces;

namespace FuryTech.BLM.EntityFrameworkCore.Tests
{
    public class MockInterpretedEntityModifyInterpreter : InterpretBeforeModify<MockInterpretedEntity>
    {
        public override MockInterpretedEntity DoInterpret(MockInterpretedEntity originalEntity, MockInterpretedEntity modifiedEntity,
            IContextInfo context)
        {
            modifiedEntity.MockInterpretedValue = MockInterpretedValue.ModifyInterpreted;
            return modifiedEntity;
        }
    }
}
