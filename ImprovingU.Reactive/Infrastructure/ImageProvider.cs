using System;
using System.Windows.Media.Imaging;

namespace ImprovingU.Reactive.Infrastructure
{
    public class ImageProvider
    {
        public BitmapSource GetIcon(Uri uri)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = uri;
            bitmapImage.EndInit();
            return bitmapImage;
        }
    }
}
