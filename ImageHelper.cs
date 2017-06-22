using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Engine
{
    class ImageHelper
    {
        //Supports the resizing of an image with no smoothing and nearest neighbor interpolation.
        public static Image ResizeImage(Image Image, int Width, int Height)
        {
            Bitmap DestImage = new Bitmap(Width, Height);
            Rectangle DestRect = new Rectangle(0, 0, Width, Height);
            Graphics G = Graphics.FromImage(DestImage);

            G.InterpolationMode = InterpolationMode.NearestNeighbor;
            G.SmoothingMode = SmoothingMode.None;
            G.DrawImage(Image, DestRect, 0, 0, Image.Width, Image.Height, GraphicsUnit.Pixel);

            return DestImage;
        }

        public static Image ChangeOpacity(Image original, float opacity)
        {
            return AlterImageChannel(original, 3, opacity);
        }
        public static Image AdjustLuminosity(Image original, float change)
        {
            return AlterImageChannel(original, 2, change);
        }
        public static Image AlterImageChannel(Image original, int channel, float value)
        {
            if ((original.PixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed)
                return original;
            Bitmap bmp = original.Clone() as Bitmap;
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr ptr = data.Scan0;
            byte[] bytes = new byte[bmp.Width * bmp.Height * 4];
            System.Runtime.InteropServices.Marshal.Copy(ptr, bytes, 0, bytes.Length);
            for (int i = 0; i < bytes.Length; i += 4)
            {
                if (channel == 3)
                {
                    if (bytes[i + 3] == 0)
                        continue;
                    bytes[i + 3] = (byte)(value * 255);
                }
                if (channel == 2)// L*(value +1)                L+0.25*(1-L)
                {
                    for (var j = 0; j < 3; j++)
                    {
                        bytes[i + j] = value < 0 ? (byte)(bytes[i + j] * (value + 1)) : (byte)(bytes[i + j] + value * (255 - bytes[i + j]));
                    }
                }

            }
            System.Runtime.InteropServices.Marshal.Copy(bytes, 0, ptr, bytes.Length);
            bmp.UnlockBits(data);
            return bmp;
        }
    }
}
