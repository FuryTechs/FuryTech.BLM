﻿using System;

namespace FuryTechs.BLM.NetStandard.Exceptions
{
    public class LogicalSecurityRiskException : BLMException
    {
        public LogicalSecurityRiskException(string message = null, Exception innerException = null) : base(message, innerException)
        {
        }
    }
}
