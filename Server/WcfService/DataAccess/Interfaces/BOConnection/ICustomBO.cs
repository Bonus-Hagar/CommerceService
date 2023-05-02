using LSOmni.Common.Util;
using System;

namespace LSOmni.DataAccess.Interface.BOConnection
{
    public interface ICustomBO
    {
        // Interface for NavCustom functions
        string MyCustomFunction(string data);
        bool AcceptTerms(string accountId, string deviceId, string termsAndConditionsVersion, string privacyPolicyVersion, Statistics stat);
    }
}
