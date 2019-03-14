using System;

namespace FuryTechs.BLM.NetStandard.Tests
{
    public interface Entity<T>
    {
        T Id { get; }

        bool IsVisible { get; }
        bool IsVisible2 { get; }
    }

    public class MockGenericEntity<T>: Entity<T>
    {
        public T Id { get; set; }
        public bool IsValid { get; set; }
        public bool IsVisible { get; set; }

        public bool IsVisible2 { get; set; }
    }

    public class MockGenericIntEntity : MockGenericEntity<int>
    {

    }
    public class MockGenericGuidEntity : MockGenericEntity<Guid>
    {

    }
}