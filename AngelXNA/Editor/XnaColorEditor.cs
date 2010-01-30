#if WINDOWS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Design;

using Drawing = System.Drawing;
using XNA = Microsoft.Xna.Framework.Graphics;

namespace AngelXNA.Editor
{
    public class XnaColorEditor : ColorEditor
    {
        public override void PaintValue(PaintValueEventArgs e)
        {
            if(e.Value is XNA.Color)
                e = new PaintValueEventArgs(e.Context, Convert((XNA.Color)e.Value), e.Graphics, e.Bounds);           
            base.PaintValue(e);
        } 

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            XNA.Color xnaColor = (XNA.Color)value;
            Drawing.Color newColor = (Drawing.Color)base.EditValue(context, provider, Convert(xnaColor));
            return Convert(newColor);
        }

        private Drawing.Color Convert(XNA.Color xnaColor)
        {
            return Drawing.Color.FromArgb(xnaColor.A, xnaColor.R, xnaColor.G, xnaColor.B);
        }

        private XNA.Color Convert(Drawing.Color drawingColor)
        {
            return new XNA.Color(drawingColor.R, drawingColor.G, drawingColor.B, drawingColor.A);
        }
    }
}

#endif