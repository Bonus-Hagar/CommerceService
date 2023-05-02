using System;
using LSOmni.Common.Util;
using LSOmni.DataAccess.BOConnection.CentralPre.Dal;
using LSOmni.DataAccess.Interface.BOConnection;
using LSRetail.Omni.Domain.DataModel.Base;

namespace LSOmni.DataAccess.BOConnection.CentralPre
{
    public class NavCustom : NavBase, ICustomBO
    {
        public NavCustom(BOConfiguration config) : base(config)
        {
        }

        public virtual string MyCustomFunction(string data)
        {
            bool usedatabase = false;

            // using database lookup
            if (usedatabase)
            {
                MyCustomRepository rep = new MyCustomRepository(config, LSCVersion);
                return rep.GetMyData(data);
            }
            else
            {
                // using Web Service Lookup
                return LSCentralWSBase.MyCustomFunction(data);
            }
        }

        #region Terms

        public bool AcceptTerms(string accountId, string deviceId, string termsAndConditionsVersion, string privacyPolicyVersion, Statistics stat)
        {
            return LSCentralWSBase.AcceptTerms(accountId, deviceId, termsAndConditionsVersion, privacyPolicyVersion, stat);
        }

        #endregion
    }
}
