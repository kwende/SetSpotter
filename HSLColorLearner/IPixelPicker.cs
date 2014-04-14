using AForge.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSLColorLearner
{
    public interface IPixelPicker
    {
        HSL[] Get(Bitmap bmp, int take);
    }
}
