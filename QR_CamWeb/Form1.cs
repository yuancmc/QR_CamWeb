using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;
using BasselTech_CamCapture;

namespace QR_CamWeb
{
    public partial class Form1 : Form
    {
        Camera cam;
        Timer t;
        BackgroundWorker worker;
        Bitmap CapImage;

        public Form1()
        {
            InitializeComponent();

            t = new Timer();
            cam = new Camera (pictureBox1);
            worker = new BackgroundWorker();

            worker.DoWork += Worker_Dowork;
            t.Tick += T_Tick;
            t.Interval = 1;


        }

        private void  T_Tick(object sender, EventArgs e)
        {
            CapImage = cam.GetBitmap();
            if (CapImage != null && !worker.IsBusy)
                worker.RunWorkerAsync();
        }

        private void Worker_Dowork(object sender, DoWorkEventArgs e)
        {
            QRCodeDecoder Decoder = new QRCodeDecoder();

            try
            {
                // Decodificar el código QR
                string decodedText = Decoder.decode(new QRCodeBitmapImage(CapImage));

                // Comprobar si el texto decodificado es "Camila" o "Martina"
                if (decodedText == "Camila" || decodedText == "Martina")
                {
                    // Detener el temporizador y el proceso de captura para evitar múltiples aperturas de Form2
                    this.Invoke((MethodInvoker)delegate {
                        t.Stop();
                        cam.Stop();
                        worker.Dispose();

                        // Cambiar a Form2
                        Form2 form2 = new Form2();
                        form2.Show();
                        this.Hide();  
                    });
                }
                else
                {
                    // Mostrar mensaje de QR incorrecto
                    this.Invoke((MethodInvoker)delegate {
                        MessageBox.Show("QR incorrecto");
                    });
                }
            }
            catch (Exception)
            {
                
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                cam.Start();
                t.Start();
                button2.Enabled = true;
                button1.Enabled = false;
            }
            catch (Exception ex)
            {
                cam.Stop();
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cam.Stop();
            button2.Enabled = false;
            button1.Enabled = true;
        }
    }

}
