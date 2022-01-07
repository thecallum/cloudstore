using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentServiceListener.Gateways.Interfaces
{
    public interface IImageLoader
    {
        public Image<Rgba32> LoadImage(byte[] data);
        public void Dispose();
    }
    
}
