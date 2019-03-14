using System;
using System.Collections.Generic;
using System.Text;

namespace FuryTechs.BLM.NetStandard.Tests.Generic
{
    public interface IGenericEntity<T>
    {
        T Id { get; }
        bool IsValid { get; }
        bool IsVisible { get; }
        bool IsVisible2 { get; }
    }

    public class GenericEntity<T> : IGenericEntity<T>
    {
        public T Id { get; set; }

        public bool IsValid { get; set; }

        public bool IsVisible { get; set; }

        public bool IsVisible2 { get; set; }
    }

    public class GenericEntityInt : GenericEntity<int>
    {
    }

    public class GenericEntityGuid : GenericEntity<Guid>
    {

    }
}
