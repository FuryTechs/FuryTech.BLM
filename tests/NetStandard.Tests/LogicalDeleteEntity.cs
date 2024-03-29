using FuryTechs.BLM.NetStandard.Attributes;

namespace FuryTechs.BLM.NetStandard.Tests
{
    public class LogicalDeleteEntity : MockEntity
    {
        [LogicalDelete]
        public virtual bool IsDeleted { get; set; }
    }

    public interface ILogicalDeleteEntity
    {
        [LogicalDelete]
        bool IsDeleted { get; set; }
    }



    public class InheritedLogicalDeleteEntity : // LogicalDeleteEntity
        MockEntity, ILogicalDeleteEntity
    {
        public bool IsDeleted { get; set; }
    }
}