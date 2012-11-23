using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using ServiceStack.ServiceClient.Web;
using XamarinEvolveSSLibrary;
using System.Drawing.Imaging;

namespace TestSSAPI
{
    public class AvatarTest
    {
        public void RunTests()
        {
            byte[] imageData;

            using (Image srcImage = Image.FromFile(@"D:\Bill\Code\XamarinEvolve2013Project\TestSSAPI\testavatar.jpg"))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    srcImage.Save(m, ImageFormat.Jpeg);
                    imageData = m.ToArray(); //buffers
                }
            }

            JsonServiceClient client = new JsonServiceClient(SystemConstants.WebServiceBaseURL);

            UserAvatar userAvatar = new UserAvatar()
            {
                username = "billholmes",
                data = imageData,
            };

            UserAvatarResponse response = client.Post<UserAvatarResponse>("useravatar", userAvatar);

            response = client.Delete<UserAvatarResponse>("useravatar/billholmes");

            return;
        }
    }
}
