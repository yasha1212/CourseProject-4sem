﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    [Serializable]
    public class ImagePackage : Package
    {
        public Image Screenshot { get; private set; }

        public ImagePackage(Image screenshot, PackageType type) : base(type)
        {
            Screenshot = screenshot;
        }
    }
}
