///----------------------------------------------------------------------------
/// Class     : Jiedy.Magnifier.PicMagnifier
/// Purpose   : 放大图片，按像素放大，并不是直接放大屏幕，避免模糊
/// Written by: HCJ.Jiedy   Jiedy@outlook.com
/// Date   : 2020/09/01
/// 直接引用此类请标明来源~

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Jiedy.Magnifier
{
    public class PicMagnifier
    {
        Form formMagnifier = null;
        PictureBox picShow = null;
        Bitmap bitmap = null;
        DateTime dt = DateTime.Now;
        PictureBox picBox;
        private Image mImageMagnifier;
        public PicMagnifier(PictureBox _picBox)
        {
            formMagnifier = new Form();
            formMagnifier.Size = new Size(300, 300);
            formMagnifier.VisibleChanged += (_s, _e) =>
            {
                if (!formMagnifier.Visible)
                    Cursor.Show();
                else
                    Cursor.Hide();
            };
            picShow = new PictureBox();
            picShow.SizeMode = PictureBoxSizeMode.Zoom;
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(new Rectangle(0, 0, 295, 295));
            formMagnifier.Region = new Region(path);
            formMagnifier.ShowIcon = false;
            formMagnifier.ShowInTaskbar = false;
            formMagnifier.FormBorderStyle = FormBorderStyle.None;
            formMagnifier.Controls.Add(picShow);
            picShow.Dock = DockStyle.Fill;
            picBox = _picBox;
            mImageMagnifier = WindowsFormsApp1.Properties.Resources.magnifierGlass;
            picBox.MouseDown += picBox_MouseDown;
            picBox.MouseUp += picBox_MouseUp;
            picBox.MouseMove += picBox_MouseMove;
            picBox.MouseLeave += picBox_MouseLeave;
        }

        void picBox_MouseUp(object sender, MouseEventArgs e)
        {
            formMagnifier.Hide();
        }

        void picBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || picBox.Image == null) return;
            formMagnifier.Show();
        }

        void picBox_MouseLeave(object sender, EventArgs e)
        {
            formMagnifier.Hide();
        }

        void picBox_MouseMove(object sender, MouseEventArgs e)
        {
            if ((DateTime.Now - dt).TotalMilliseconds < 5 || picBox.Image == null) return;
            dt = DateTime.Now;
            Point p = e.Location;
            Point screenPoint = picBox.PointToScreen(p);
            Point loc = new Point(screenPoint.X - formMagnifier.Width / 2, screenPoint.Y - formMagnifier.Height / 2);
            formMagnifier.Location = loc;
            PropertyInfo _ImageRectanglePropert = picBox.GetType().GetProperty("ImageRectangle", BindingFlags.Instance | BindingFlags.NonPublic);
            Rectangle _Rectangle = (Rectangle)_ImageRectanglePropert.GetValue(picBox, null);

            float wR = (float)picBox.Image.Width / (float)_Rectangle.Width;
            bitmap = new Bitmap((int)(100 * wR), (int)(100 * wR));
            float l = 0, t = 0;
            if (_Rectangle.Left > 0)
            {
                l = (p.X - _Rectangle.Left - 50) * wR;
                t = (p.Y - 50) * wR;
            }
            if (_Rectangle.Top > 0)
            {
                l = (p.X - 50) * wR;
                t = (p.Y - _Rectangle.Top - 50) * wR;
            }
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(picBox.Image, new RectangleF(0, 0, 100 * wR, 100 * wR), new RectangleF(l, t, 100 * wR, 100 * wR), GraphicsUnit.Pixel);
                g.DrawImage(mImageMagnifier, 0, 0, bitmap.Width, bitmap.Height);
            }
            picShow.Image = bitmap;
        }
    }
}
