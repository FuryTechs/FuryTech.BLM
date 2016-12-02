using BLM.Attributes;

namespace BLM.Tests
{
    public class MockEntity
    {
        public int Id { get; set; }
        public bool IsValid { get; set; }
        public bool IsVisible { get; set; }

        public bool IsVisible2 { get; set; }

        public string Guid { get; set; }
    }

    public class LogicalDeleteEntity : MockEntity
    {
        [LogicalDelete]
        public bool IsDeleted { get; set; }
    }
}