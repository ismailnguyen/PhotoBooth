using System.Drawing;

namespace PhotoBooth
{
    interface IProcessing
    {
        Bitmap grayScale();

        Bitmap sepia();

        Bitmap negativ();

        Bitmap blue();

        Bitmap red();

        Bitmap green();

        /// <summary>
        ///     Permet de changer la luminosité par défaut 100
        ///         0 < 255  penche vers le blanc
        ///         0 < -255 penche vers le noir
        /// </summary>
        /// <param name="picture"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        Bitmap brightness(int range = 100);


        Bitmap contrast(double range = 50);

        Bitmap resize(int newWidth, int newHeight);
    }
}
