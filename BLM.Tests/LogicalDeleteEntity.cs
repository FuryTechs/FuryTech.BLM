using BLM.Attributes;

namespace BLM.Tests
{
    public class LogicalDeleteEntity : MockEntity
    {
        [LogicalDelete]
        public bool IsDeleted { get; set; }
    }


    public interface ILogicalDeleteEntity
    {
        [LogicalDelete]
        bool IsDeleted { get; set; }
    }



    public class InheritedLogicalDeleteEntity :
        MockEntity, ILogicalDeleteEntity
    {
        public bool IsDeleted { get; set; }
    }
}