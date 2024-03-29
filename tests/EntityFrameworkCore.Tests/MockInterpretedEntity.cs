﻿namespace FuryTechs.BLM.EntityFrameworkCore.Tests
{
    public enum MockInterpretedValue
    {
        Default,
        CreateInterpreted,
        ModifyInterpreted
    }

    public class MockInterpretedEntity
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public MockInterpretedValue MockInterpretedValue { get; set; }
    }
}
