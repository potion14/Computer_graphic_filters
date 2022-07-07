using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Grafika4
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private byte[] t;
        private byte[] original;
        private int tmp, w, h;
        private double R = 0;
        private double G = 0;
        private double B = 0;

        private void Load_image(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            if(openFileDialog.FileName != "" && openFileDialog.FileName.Contains(".PNG") || openFileDialog.FileName.Contains(".png") || openFileDialog.FileName.Contains(".jpg"))
            {
                BitmapImage bitmap = new BitmapImage(new Uri(openFileDialog.FileName));
                PixelFormat pf = PixelFormats.Bgra32;
                int stride = (bitmap.PixelWidth * pf.BitsPerPixel + 7) / 8;
                byte[] bufor = new byte[stride * bitmap.PixelHeight];
                bitmap.CopyPixels(bufor, stride, 0);
                t = bufor;
                original = bufor;
                tmp = stride;
                w = bitmap.PixelWidth;
                h = bitmap.PixelHeight;
                BitmapSource new_bitmap = BitmapSource.Create(w, h, 96, 96, PixelFormats.Bgra32, null, t, tmp);
                Image_placeholder.Source = new_bitmap;
                SBrightness.Value = 100;
                Calculate_avarage(t);
            }
            else
            {
                MessageBox.Show("Nieprawidłowy format pliku!");
            }
        }

        private void Odcienie_szarosci(object sender, RoutedEventArgs e)
        {
            if(t != null)
            {
                Image_placeholder.Source = null;
                BitmapSource new_bitmap = BitmapSource.Create(w, h, 96, 96, PixelFormats.Bgra32, null, t, tmp);

                FormatConvertedBitmap newFormatedBitmapSource = new FormatConvertedBitmap();
                newFormatedBitmapSource.BeginInit();
                newFormatedBitmapSource.Source = new_bitmap;
                newFormatedBitmapSource.DestinationFormat = PixelFormats.Gray32Float;
                newFormatedBitmapSource.EndInit();
                Image_placeholder.Source = newFormatedBitmapSource;
                //newFormatedBitmapSource.CopyPixels(t, tmp, 0);
                t = original;
            }
        }

        private void Odcienie_szarosci2(object sender, RoutedEventArgs e)
        {
            if (t != null)
            {
                byte[] t2 = new byte[t.Length];
                Image_placeholder.Source = null;
                for (int i = 0; i < t.Length; i += 4)
                {
                    int tmp = (t[i] + t[i + 1] + t[i + 2]) / 3;
                    t2[i] = (byte)tmp;
                    t2[i + 1] = (byte)tmp;
                    t2[i + 2] = (byte)tmp;
                    t2[i + 3] = 255;
                }
                BitmapSource new_bitmap = BitmapSource.Create(w, h, 96, 96, PixelFormats.Bgra32, null, t2, tmp);
                Image_placeholder.Source = new_bitmap;
                t = original;
            }
        }

        private void Calculate_avarage(byte[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (i % 4 == 2) R += array[i];
                if (i % 4 == 1) G += array[i];
                if (i % 4 == 0) B += array[i];
            }

            R = R / array.Length;
            G = G / array.Length;
            B = B / array.Length;
        }

        private void Button_Red_Add(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(Red_TextBox.Text, out _) || !int.TryParse(Green_TextBox.Text, out _) || !int.TryParse(Blue_TextBox.Text, out _))
            {
                MessageBox.Show("Proszę podać liczbę");
                Red_TextBox.Text = "";
                Green_TextBox.Text = "";
                Blue_TextBox.Text = "";
            }
            else if (t != null)
            {
                for (int i = 0; i < t.Length; i++)
                {
                    if (i % 4 == 2 && Red_TextBox.Text != "" && Convert.ToInt32(Red_TextBox.Text) > 0 && int.TryParse(Red_TextBox.Text, out _))
                    {
                        if (Convert.ToInt32(t[i]) + Convert.ToInt32(Red_TextBox.Text) >= 255) t[i] = 255;
                        else t[i] += Convert.ToByte(Convert.ToInt32(Red_TextBox.Text));
                    }
                    if (i % 4 == 1 && Green_TextBox.Text != "" && Convert.ToInt32(Green_TextBox.Text) > 0)
                    {
                        if (Convert.ToInt32(t[i]) + Convert.ToInt32(Green_TextBox.Text) >= 255) t[i] = 255;
                        else t[i] += Convert.ToByte(Convert.ToInt32(Green_TextBox.Text));
                    }
                    if (i % 4 == 0 && Blue_TextBox.Text != "" && Convert.ToInt32(Blue_TextBox.Text) > 0)
                    {
                        if (Convert.ToInt32(t[i]) + Convert.ToInt32(Blue_TextBox.Text) >= 255) t[i] = 255;
                        else t[i] += Convert.ToByte(Convert.ToInt32(Blue_TextBox.Text));
                    }
                }
                Red_TextBox.Text = "";
                Green_TextBox.Text = "";
                Blue_TextBox.Text = "";
                BitmapSource new_bitmap = BitmapSource.Create(w, h, 96, 96, PixelFormats.Bgra32, null, t, tmp);
                Image_placeholder.Source = new_bitmap;
            }
            else
            {
                Red_TextBox.Text = "";
                Green_TextBox.Text = "";
                Blue_TextBox.Text = "";
            }
        }

        private void Button_Substract(object sender, RoutedEventArgs e)
        {
            if(t != null)
            {
                for (int i = 0; i < t.Length; i++)
                {
                    if (i % 4 == 2 && Red_TextBox.Text != "")
                    {
                        if (Convert.ToInt32(t[i]) - Convert.ToInt32(Red_TextBox.Text) <= 0) t[i] = 0;
                        else t[i] -= Convert.ToByte(Convert.ToInt32(Red_TextBox.Text));
                    }
                    if (i % 4 == 1 && Green_TextBox.Text != "")
                    {
                        if (Convert.ToInt32(t[i]) - Convert.ToInt32(Green_TextBox.Text) <= 0) t[i] = 0;
                        else t[i] -= Convert.ToByte(Convert.ToInt32(Green_TextBox.Text));
                    }
                    if (i % 4 == 0 && Blue_TextBox.Text != "")
                    {
                        if (Convert.ToInt32(t[i]) - Convert.ToInt32(Blue_TextBox.Text) <= 0) t[i] = 0;
                        else t[i] -= Convert.ToByte(Convert.ToInt32(Blue_TextBox.Text));
                    }
                }
                Red_TextBox.Text = "";
                Green_TextBox.Text = "";
                Blue_TextBox.Text = "";
                BitmapSource new_bitmap = BitmapSource.Create(w, h, 96, 96, PixelFormats.Bgra32, null, t, tmp);
                Image_placeholder.Source = new_bitmap;
            }
            else
            {
                Red_TextBox.Text = "";
                Green_TextBox.Text = "";
                Blue_TextBox.Text = "";
            }
        }

        private void Button_Multipla(object sender, RoutedEventArgs e)
        {
            if(t != null)
            {
                for (int i = 0; i < t.Length; i++)
                {
                    if (i % 4 == 2 && Red_TextBox.Text != "")
                    {
                        if (Convert.ToInt32(t[i]) * Convert.ToInt32(Red_TextBox.Text) >= 255) t[i] = 255;
                        else t[i] *= Convert.ToByte(Convert.ToInt32(Red_TextBox.Text));
                    }
                    if (i % 4 == 1 && Green_TextBox.Text != "")
                    {
                        if (Convert.ToInt32(t[i]) * Convert.ToInt32(Green_TextBox.Text) >= 255) t[i] = 255;
                        else t[i] *= Convert.ToByte(Convert.ToInt32(Green_TextBox.Text));
                    }
                    if (i % 4 == 0 && Blue_TextBox.Text != "")
                    {
                        if (Convert.ToInt32(t[i]) * Convert.ToInt32(Blue_TextBox.Text) >= 255) t[i] = 255;
                        else t[i] *= Convert.ToByte(Convert.ToInt32(Blue_TextBox.Text));
                    }
                }
                Red_TextBox.Text = "";
                Green_TextBox.Text = "";
                Blue_TextBox.Text = "";
                BitmapSource new_bitmap = BitmapSource.Create(w, h, 96, 96, PixelFormats.Bgra32, null, t, tmp);
                Image_placeholder.Source = new_bitmap;
            }
            else
            {
                Red_TextBox.Text = "";
                Green_TextBox.Text = "";
                Blue_TextBox.Text = "";
            }
        }

        private void Button_Divide(object sender, RoutedEventArgs e)
        {
            if (t != null)
            {
                for (int i = 0; i < t.Length; i++)
                {
                    if (i % 4 == 2 && Red_TextBox.Text != "")
                    {
                        if (Convert.ToInt32(t[i]) / Convert.ToInt32(Red_TextBox.Text) <= 0) t[i] = 0;
                        else t[i] /= Convert.ToByte(Convert.ToInt32(Red_TextBox.Text));
                    }
                    if (i % 4 == 1 && Green_TextBox.Text != "")
                    {
                        if (Convert.ToInt32(t[i]) / Convert.ToInt32(Green_TextBox.Text) <= 0) t[i] = 0;
                        else t[i] /= Convert.ToByte(Convert.ToInt32(Green_TextBox.Text));
                    }
                    if (i % 4 == 0 && Blue_TextBox.Text != "")
                    {
                        if (Convert.ToInt32(t[i]) / Convert.ToInt32(Blue_TextBox.Text) <= 0) t[i] = 0;
                        else t[i] /= Convert.ToByte(Convert.ToInt32(Blue_TextBox.Text));
                    }
                }
                BitmapSource new_bitmap = BitmapSource.Create(w, h, 96, 96, PixelFormats.Bgra32, null, t, tmp);
                Image_placeholder.Source = new_bitmap;
            }
            else
            {
                Red_TextBox.Text = "";
                Green_TextBox.Text = "";
                Blue_TextBox.Text = "";
            }
        }

        private void Brightness_Slider(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue > e.OldValue && t != null)
            {
                for (int i = 0; i < t.Length; i++)
                {
                    if (i % 4 == 2)
                    {
                        if (Convert.ToInt32(t[i]) + Convert.ToInt32((255 - R) / 255 * 2) >= 255) t[i] = 255;
                        else t[i] += Convert.ToByte(Convert.ToInt32((255 - R) / 255 * 2));
                    }
                    if (i % 4 == 1)
                    {
                        if (Convert.ToInt32(t[i]) + Convert.ToInt32((255 - G) / 255 * 2) >= 255) t[i] = 255;
                        else t[i] += Convert.ToByte(Convert.ToInt32((255 - G) / 255 * 2));
                    }
                    if (i % 4 == 0)
                    {
                        if (Convert.ToInt32(t[i]) + Convert.ToInt32((255 - B) / 255 * 2) >= 255) t[i] = 255;
                        else t[i] += Convert.ToByte(Convert.ToInt32((255 - B) / 255 * 2));
                    }
                }
                BitmapSource new_bitmap = BitmapSource.Create(w, h, 96, 96, PixelFormats.Bgra32, null, t, tmp);
                Image_placeholder.Source = new_bitmap;
            }
            else if(e.OldValue > e.NewValue && t != null)
            {
                for (int i = 0; i < t.Length; i++)
                {
                    if (i % 4 == 2)
                    {
                        if (Convert.ToInt32(t[i]) - Convert.ToInt32((255 - R) / 255 * 2) <= 0) t[i] = 0;
                        else t[i] -= Convert.ToByte(Convert.ToInt32((255 - R) / 255 * 2));
                    }
                    if (i % 4 == 1)
                    {
                        if (Convert.ToInt32(t[i]) - Convert.ToInt32((255 - G) / 255 * 2) <= 0) t[i] = 0;
                        else t[i] -= Convert.ToByte(Convert.ToInt32((255 - G) / 255 * 2));
                    }
                    if (i % 4 == 0)
                    {
                        if (Convert.ToInt32(t[i]) - Convert.ToInt32((255 - B) / 255 * 2) <= 0) t[i] = 0;
                        else t[i] -= Convert.ToByte(Convert.ToInt32((255 - B) / 255 * 2));
                    }
                }
                BitmapSource new_bitmap = BitmapSource.Create(w, h, 96, 96, PixelFormats.Bgra32, null, t, tmp);
                Image_placeholder.Source = new_bitmap;
            }
        }

        private void Filtr_wygladzajacy(object sender, RoutedEventArgs e)
        {
            int _B = 0;
            int _G = 0;
            int _R = 0;
            if (t != null)
            {
                byte[] t2 = new byte[t.Length];
                for (int i = w * 4 + 4; i < t.Length - ((w * 4) + 4); i += 4)
                {
                    if (i % (w * 4) != w * 4 - 4 && i % (w * 4) != 0)
                    {
                        _B = (t[i - (w * 4) - 4] + t[i - w * 4] + t[i - w * 4 + 4] + t[i - 4] + t[i] + t[i + 4] +
                            t[i + w * 4 - 4] + t[i + w * 4] + t[i + w * 4 + 4]) / 9;
                        _G = (t[i - (w * 4) - 4 + 1] + t[i - w * 4 + 1] + t[i - w * 4 + 4 + 1] + t[i - 4 + 1] + t[i + 1] + t[i + 4 + 1] +
                            t[i + w * 4 - 4 + 1] + t[i + w * 4 + 1] + t[i + w * 4 + 4 + 1]) / 9;
                        _R = (t[i - (w * 4) - 4 + 2] + t[i - w * 4 + 2] + t[i - w * 4 + 4 + 2] + t[i - 4 + 2] + t[i + 2] + t[i + 4 + 2] +
                            t[i + w * 4 - 4 + 2] + t[i + w * 4 + 2] + t[i + w * 4 + 4 + 2]) / 9;

                        t2[i] = (byte)_B;
                        t2[i + 1] = (byte)_G;
                        t2[i + 2] = (byte)_R;
                        t2[i + 3] = 255;
                    }
                }
                t = t2;
                BitmapSource new_bitmap = BitmapSource.Create(w, h, 96, 96, PixelFormats.Bgra32, null, t2, tmp);
                Image_placeholder.Source = new_bitmap;
            }
        }

        private void Filtr_medianowy(object sender, RoutedEventArgs e)
        {
            if (t != null)
            {
                byte[] t2 = new byte[t.Length];
                for (int i = (w * 4) + 4; i < t.Length - ((w * 4) + 4); i += 4)
                {
                    if (i % (w * 4) != (w * 4) - 4 && i % (w * 4) != 0)
                    {
                        byte[] tab_B = new byte[]
                        { t[i - (w * 4) - 4], t[i - (w * 4)], t[i - (w * 4) + 4], t[i - 4], t[i], t[i + 4], t[i + (w * 4) - 4], t[i - (w * 4)], t[i + (w * 4) + 4] };
                        byte[] tab_G = new byte[]
                        { t[i - (w * 4) - 4 + 1], t[i - (w * 4) + 1], t[i - (w * 4) + 4 + 1], t[i - 4 + 1], t[i + 1], t[i + 4 + 1], t[i + (w * 4) - 4 + 1], t[i - (w * 4) + 1], t[i + (w * 4) + 4 + 1] };
                        byte[] tab_R = new byte[]
                        { t[i - (w * 4) - 4 + 2], t[i - (w * 4) + 2], t[i - (w * 4) + 4 + 2], t[i - 4 + 2], t[i + 2], t[i + 4 + 2], t[i + (w * 4) - 4 + 2], t[i - (w * 4) + 2], t[i + (w * 4) + 4 + 2] };
                        Array.Sort(tab_B);
                        Array.Sort(tab_G);
                        Array.Sort(tab_R);
                        t2[i] = tab_B[4];
                        t2[i + 1] = tab_G[4];
                        t2[i + 2] = tab_R[4];
                        t2[i + 3] = 255;
                    }
                }
                t = t2;
                BitmapSource new_bitmap = BitmapSource.Create(w, h, 96, 96, PixelFormats.Bgra32, null, t2, tmp);
                Image_placeholder.Source = new_bitmap;
            }
        }

        private void Wykrywanie_krawedzi(object sender, RoutedEventArgs e)
        {
            if (t != null)
            {
                byte[] t2 = new byte[t.Length];
                for (int i = (w * 4) + 4; i < t.Length - ((w * 4) + 4); i += 4)
                {
                    if (i % (w * 4) != (w * 4) - 4 && i % (w * 4) != 0)
                    {
                        byte[] tab_B = new byte[]
                        { t[i - (w * 4) - 4], t[i - (w * 4)], t[i - (w * 4) + 4], t[i - 4], t[i], t[i + 4], t[i + (w * 4) - 4], t[i - (w * 4)], t[i + (w * 4) + 4] };
                        int gx = tab_B[6] + 2 * tab_B[7] + tab_B[8] - (tab_B[0] + 2 * tab_B[1] + tab_B[2]);
                        int gy = tab_B[2] + 2 * tab_B[5] + tab_B[8] - (tab_B[0] + 2 * tab_B[3] + tab_B[6]);
                        gx = Math.Abs(gx);
                        gy = Math.Abs(gy);
                        if (gx + gy >= 255) t2[i] = t2[i + 1] = t2[i + 2] = 255;
                        else t2[i] = t2[i + 1] = t2[i + 2] = (byte)(gx + gy);
                        t2[i + 3] = 255;
                    }
                }
                BitmapSource new_bitmap = BitmapSource.Create(w, h, 96, 96, PixelFormats.Bgra32, null, t2, tmp);
                Image_placeholder.Source = new_bitmap;
                //b22 = a13 - a11 + 2a23 - 2a21 + a33 - a31
            }
        }

        private void Filtr_wyostrzajacy(object sender, RoutedEventArgs e)
        {
            if(t != null)
            {
                int _B = 0;
                int _G = 0;
                byte[] t2 = new byte[t.Length];
                int _R = 0;
                for (int i = w * 4 + 4; i < t.Length - ((w * 4) + 4); i += 4)
                {
                    if (i % (w * 4) != w * 4 - 4 && i % (w * 4) != 0)
                    {
                        _B = t[i - (w * 4) - 4] * (-1) + t[i - w * 4] * (-1) + t[i - w * 4 + 4] * (-1) + t[i - 4] * (-1) + t[i] * 9 + t[i + 4] * (-1) +
                            t[i + w * 4 - 4] * (-1) + t[i + w * 4] * (-1) + t[i + w * 4 + 4] * (-1);
                        _G = t[i - (w * 4) - 4 + 1] * (-1) + t[i - w * 4 + 1] * (-1) + t[i - w * 4 + 4 + 1] * (-1) + t[i - 4 + 1] * (-1) + t[i + 1] * 9 + t[i + 4 + 1] * (-1) +
                            t[i + w * 4 - 4 + 1] * (-1) + t[i + w * 4 + 1] * (-1) + t[i + w * 4 + 4 + 1] * (-1);
                        _R = t[i - (w * 4) - 4 + 2] * (-1) + t[i - w * 4 + 2] * (-1) + t[i - w * 4 + 4 + 2] * (-1) + t[i - 4 + 2] * (-1) + t[i + 2] * 9 + t[i + 4 + 2] * (-1) +
                            t[i + w * 4 - 4 + 2] * (-1) + t[i + w * 4 + 2] * (-1) + t[i + w * 4 + 4 + 2] * (-1);

                        _B = Math.Abs(_B);
                        _G = Math.Abs(_G);
                        _R = Math.Abs(_R);
                        t2[i] = _B < 0 ? (byte)0 : _B > 255 ? (byte)255 : (byte)_B;
                        t2[i + 1] = _B < 0 ? (byte)0 : _B > 255 ? (byte)255 : (byte)_B;
                        t2[i + 2] = _B < 0 ? (byte)0 : _B > 255 ? (byte)255 : (byte)_B;

                        t2[i] = (byte)_B;
                        t2[i + 1] = (byte)_G;
                        t2[i + 2] = (byte)_R;
                        t2[i + 3] = 255;
                    }
                }
                t = t2;
                BitmapSource new_bitmap = BitmapSource.Create(w, h, 96, 96, PixelFormats.Bgra32, null, t2, tmp);
                Image_placeholder.Source = new_bitmap;
            }
            
        }

        private void Filtr_Gausa(object sender, RoutedEventArgs e)
        {
            int _B = 0;
            int _G = 0;
            int _R = 0;
            if (t != null)
            {
                byte[] t2 = new byte[t.Length];
                for (int i = w * 4 + 4; i < t.Length - ((w * 4) + 4); i += 4)
                {
                    if (i % (w * 4) != w * 4 - 4 && i % (w * 4) != 0)
                    {
                        _B = (t[i - (w * 4) - 4] + t[i - w * 4] * 2 + t[i - w * 4 + 4] + t[i - 4] * 2 + t[i] * 4 + t[i + 4] * 2 +
                            t[i + w * 4 - 4] + t[i + w * 4] * 2 + t[i + w * 4 + 4]) / 16;
                        _G = (t[i - (w * 4) - 4 + 1] + t[i - w * 4 + 1] * 2 + t[i - w * 4 + 4 + 1] + t[i - 4 + 1] * 2 + t[i + 1] * 4 + t[i + 4 + 1] * 2 +
                            t[i + w * 4 - 4 + 1] + t[i + w * 4 + 1] * 2 + t[i + w * 4 + 4 + 1]) / 16;
                        _R = (t[i - (w * 4) - 4 + 2] + t[i - w * 4 + 2] * 2 + t[i - w * 4 + 4 + 2] + t[i - 4 + 2] * 2 + t[i + 2] * 4 + t[i + 4 + 2] * 2 +
                            t[i + w * 4 - 4 + 2] + t[i + w * 4 + 2] * 2 + t[i + w * 4 + 4 + 2]) / 16;

                        t2[i] = (byte)_B;
                        t2[i + 1] = (byte)_G;
                        t2[i + 2] = (byte)_R;
                        t2[i + 3] = 255;
                    }
                }
                t = t2;
                BitmapSource new_bitmap = BitmapSource.Create(w, h, 96, 96, PixelFormats.Bgra32, null, t2, tmp);
                Image_placeholder.Source = new_bitmap;
                t = original;
            }
        }

        private void Load_original(object sender, RoutedEventArgs e)
        {
            if (original != null)
            {
                Image_placeholder.Source = null;
                BitmapSource original_bitmap = BitmapSource.Create(w, h, 96, 96, PixelFormats.Bgra32, null, original, tmp);
                Image_placeholder.Source = original_bitmap;
                t = original;
                SBrightness.Value = 100;
            }
        }
    }
}
