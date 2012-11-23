using System;
using ServiceStack.ServiceHost;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using ServiceStack.Service;
using System.Collections.Generic;
using XamarinEvolveSSLibrary;

namespace XamarinEvolveSS
{
    public class UserAvatarService : RestServiceBase<UserAvatar>
    {
        public override object  OnGet(UserAvatar request)
        {
            try
            {
                EvolveUsersMySqlAccess sql = new EvolveUsersMySqlAccess();
                User user = sql.FindUser(request.username);

                if (request.size < 1)
                    request.size = 100;

                if (request.size > 800)
                    request.size = 800;

                if (string.IsNullOrEmpty(user.avatar) || !File.Exists(user.avatar))
                    throw new Exception(string.Format ("avatar for user {0} does not exist", user.username));

                Image image = Image.FromFile(user.avatar);
                try
                {
                    image = ResizeImage(image, (uint)request.size, false);

                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, ImageFormat.Jpeg);
                        byte[] imageData = m.ToArray(); //buffers
                        return new UserAvatarResponse(imageData, ImageFormat.Jpeg.ToString().ToLower());
                    }
                }
                finally
                {
                    image.Dispose();
                }
            }
            catch (Exception e)
            {
                return new UserAvatarResponse() { Exception = e };
            }
        }

        public override object OnPost(UserAvatar request)
        {
            try
            {
                EvolveUsersMySqlAccess sql = new EvolveUsersMySqlAccess();
                User user = sql.FindUser(request.username);

                if (request.data == null || request.data.Length == 0)
                    throw new ArgumentException("data sent was size zero");

                string filename = PathForUser(user);

                using (MemoryStream m = new MemoryStream())
                {
                    m.Write(request.data, 0, request.data.Length);
                    m.Position = 0;

                    Image image = Image.FromStream(m);
                    try
                    {
                        image = ResizeImage(image, SystemConstants.MaxAvatarSize, true);

                        using (Stream file = File.OpenWrite(filename))
                        {
                            image.Save(file, ImageFormat.Jpeg);

                            user = new User() { username = user.username, avatar = filename};
                            sql.UpdateUser(user);
                        }
                    }
                    finally
                    {
                        image.Dispose();
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                return new UserAvatarResponse() { Exception = e };
            }
        }

        public override object OnDelete(UserAvatar request)
        {
            try
            {
                EvolveUsersMySqlAccess sql = new EvolveUsersMySqlAccess();
                User user = sql.FindUser(request.username);

                string filename = PathForUser(user);

                if (File.Exists(filename))
                    File.Delete(filename);

                user = new User() { username = user.username, avatar = "" };
                sql.UpdateUser(user);

                return null;
            }
            catch (Exception e)
            {
                return new UserAvatarResponse() { Exception = e };
            }
        }

        private string PathForUser(User user)
        {
            if (!string.IsNullOrEmpty(user.avatar) && File.Exists(user.avatar))
                return user.avatar;

            string filename = string.Format("{0}.jpg", MD5Helper.CalculateMD5Hash(user.username));
            string directory = Path.Combine(SystemConstants.PathToRoot, "avatars");

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return Path.Combine(directory, filename);
        }

        private Image ResizeImage(Image srcImage, uint size, bool noopIfSmaller)
        {
            if (srcImage.Size.Width == size && srcImage.Size.Height == size)
                return srcImage;

            if (noopIfSmaller && srcImage.Size.Width < size && srcImage.Size.Height < size)
                return srcImage;

            Bitmap image = new Bitmap((int)size, (int)size);
            using (Graphics g = Graphics.FromImage(image))
            {
                g.DrawImage(srcImage, new RectangleF(0, 0, size, size));
            }

            srcImage.Dispose();

            return image;
        }
    }
}