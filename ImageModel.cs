using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PhotoBooth
{
    class ImageModel
    {
        public string Name { get; set; }
        public BitmapImage Path { get; set; }
        public string FileName { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public long Size { get; set; }

    }
}
