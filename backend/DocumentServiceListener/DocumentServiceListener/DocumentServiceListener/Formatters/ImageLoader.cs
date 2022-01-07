using DocumentServiceListener.Gateways.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentServiceListener.Gateways
{
    public class ImageLoader : IImageLoader
    {
        private  Image<Rgba32> _image;

        public void Dispose()
        {
            _image.Dispose();
        }

        public Image<Rgba32> LoadImage(byte[] data)
        {
            _image = Image.Load(data);
            return _image;

        }
    }
}
