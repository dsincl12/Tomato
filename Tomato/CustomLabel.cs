using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tomato
{
    public partial class CustomLabel : Label
    {
        private TextRenderingHint hint = TextRenderingHint.AntiAlias;

        public TextRenderingHint TextRenderingHint
        {
            get 
            { 
                return this.hint;
            }
            set 
            { 
                this.hint = value;
            }
        }

        protected override void OnPaint(PaintEventArgs paintEventArgs)
        {
            paintEventArgs.Graphics.TextRenderingHint = TextRenderingHint;
            base.OnPaint(paintEventArgs);
        }
    }
}
