using System;
using System.Runtime.Serialization;

namespace Escc.Umbraco.UserAccessManager.Utility
{
    [Serializable]
    public class PostMessageError : Exception
    {
        public string ErrorMessage { get; set; }
        public string ClassName { get; set; }

        public PostMessageError(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info != null)
                ErrorMessage = info.GetString("Message");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            //if (info != null)
                info.AddValue("ErrorMessage", ErrorMessage);
            info.AddValue("ClassName", "test");
        }
    }
}