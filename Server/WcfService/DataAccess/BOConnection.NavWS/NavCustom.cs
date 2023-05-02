using System;
using LSOmni.Common.Util;
using LSOmni.DataAccess.Interface.BOConnection;
using LSRetail.Omni.Domain.DataModel.Base;

namespace LSOmni.DataAccess.BOConnection.NavWS
{
    public class NavCustom : NavBase, ICustomBO
    {
        public NavCustom(BOConfiguration config) : base(config)
        {
        }

        public virtual string MyCustomFunction(string data)
        {
            // using Web Service Lookup
            if (NAVVersion < new Version("17.5"))
                return NavWSBase.MyCustomFunction(data);

            return LSCWSBase.MyCustomFunction(data);
        }

        #region Terms

        public bool AcceptTerms(string accountId, string deviceId, string termsAndConditionsVersion, string privacyPolicyVersion, Statistics stat)
        {
            return LSCWSBase.AcceptTerms(accountId, deviceId, termsAndConditionsVersion, privacyPolicyVersion, stat);
        }

        #endregion
    }
}
