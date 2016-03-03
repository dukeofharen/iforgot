using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace IForgot
{
	public class Shooter
	{
		public string Shoot(string folder)
		{
			Rectangle totalSize = Rectangle.Empty;

			foreach (Screen s in Screen.AllScreens)
			{
				totalSize = Rectangle.Union(totalSize, s.Bounds);
			}

			using (Bitmap screenShot = new Bitmap(totalSize.Width, totalSize.Height, PixelFormat.Format32bppArgb))
			{
				using (Graphics screenShotGraphics = Graphics.FromImage(screenShot))
				{
					screenShotGraphics.CopyFromScreen(totalSize.Location.X, totalSize.Location.Y, 0, 0, totalSize.Size, CopyPixelOperation.SourceCopy);

					string filePath = string.Format("screen{0}.jpg", DateTime.Now.ToString("yyyyMMddHHmmss"));
					this.VaryQualityLevel(screenShot, Path.Combine(folder, filePath));

					return filePath;
				}
			}
		}

		private void VaryQualityLevel(Bitmap bitmap, string path)
		{
			// Get a bitmap.
			ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

			// Create an Encoder object based on the GUID 
			// for the Quality parameter category.
			Encoder myEncoder = Encoder.Quality;

			// Create an EncoderParameters object. 
			// An EncoderParameters object has an array of EncoderParameter 
			// objects. In this case, there is only one 
			// EncoderParameter object in the array.
			EncoderParameters myEncoderParameters = new EncoderParameters(1);

			EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 30L);
			myEncoderParameters.Param[0] = myEncoderParameter;
			bitmap.Save(path, jpgEncoder, myEncoderParameters);
		}

		private ImageCodecInfo GetEncoder(ImageFormat format)
		{
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

			foreach (ImageCodecInfo codec in codecs)
			{
				if (codec.FormatID == format.Guid)
				{
					return codec;
				}
			}
			return null;
		}
	}
}
