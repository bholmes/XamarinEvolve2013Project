using System;
using ServiceStack.Service;
using ServiceStack.ServiceHost;
using System.Collections.Generic;
using System.IO;
using ServiceStack.Common.Web;


namespace XamarinEvolveSSLibrary
{
    public class UserAvatarResponse : IDisposable, IStreamWriter, IHasOptions
    {
        private readonly byte[] image;
        private readonly string imgFormat;
        public Exception Exception { get; set; }

        public UserAvatarResponse()
        {
            this.Options = new Dictionary<string, string>();
        }

        public UserAvatarResponse(byte[] image, string imgFormat)
        {
            this.image = image;
            this.imgFormat = imgFormat ?? "jpeg";
            this.Options = new Dictionary<string, string> {
                { HttpHeaders.ContentType, "image/" + this.imgFormat }
            };
        }

        public void WriteTo(Stream responseStream)
        {
            if (image != null && image.Length > 0 && responseStream != null)
                responseStream.Write(this.image, 0, this.image.Length);
        }

        public void Dispose()
        {

        }

        public IDictionary<string, string> Options { get; set; }
    }
}
