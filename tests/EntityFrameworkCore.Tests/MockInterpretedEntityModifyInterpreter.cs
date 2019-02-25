using FuryTechs.BLM.NetStandard;
using FuryTechs.BLM.NetStandard.Interfaces;

namespace FuryTechs.BLM.EntityFrameworkCore.Tests
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
