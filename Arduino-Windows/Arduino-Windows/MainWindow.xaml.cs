using System;
using System.Windows.Threading;
using System.Windows;
using System.IO.Ports;

namespace Arduino_Windows
{
    /// Lógica de interacción para MainWindow.xaml
    public partial class MainWindow : Window
    {
        SerialPort sp = new SerialPort();

        const int baudRateDefault = 9600;
        const int readT = 200;
        const int writeT = 50;
        const int dataBits = 8;

        String datosM = string.Empty;

        String conectadoText = "Conectado";
        String desconectadoText = "Desconectado";

        String error1 = "ERROR 001:";
        String error2 = "ERROR 002:";
        String error3 = "ERROR 003:";
        String error4 = "ERROR 004:";
        String error5 = "ERROR 005:";

        String error1Text = "Teclea el puerto COM'X' válido o revisa la conexión con el dispositivo.";
        String error2Text = "Primero teclea el botón para Conectar y después desconecta el dispositivo.";
        String error3Text = "Debes iniciar la conexión con el dispositivo.";
        String error4Text = "Error en la transmisión.";
        String error5Text = "Error con las operaciones numéricas.";

        public MainWindow()
        {
            sp.DataBits = dataBits;
            sp.ReadTimeout = readT;
            sp.WriteTimeout = writeT;
            sp.Handshake = Handshake.None;
            sp.Parity = Parity.None;
            sp.StopBits = StopBits.One;

            InitializeComponent();
        }
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sp.PortName = COMPort.Text;
                sp.BaudRate = baudRateDefault;

                if (!sp.IsOpen)
                {
                    sp.Open();
                    status.Text = conectadoText;
                    sp.DataReceived += new SerialDataReceivedEventHandler(transmisionDatos);
                }
            }
            catch (Exception)
            {

                MessageBox.Show(error1Text, error1);
            }
        }
        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                eraseAll();
                if (sp.IsOpen)
                {
                    sp.Write("2");
                    sp.Close();
                    status.Text = desconectadoText;
                }
            }
            catch (Exception)
            {
                MessageBox.Show(error2Text, error2);
            }
        }
        private void on_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sp.Write("1");
            }
            catch (Exception)
            {
                MessageBox.Show(error3Text, error3);
            }
        }
        private void off_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sp.Write("0");
                eraseAll();
            }
            catch (Exception)
            {
                MessageBox.Show(error3Text, error3);
            }
        }
        private void transmisionDatos(object sender, SerialDataReceivedEventArgs dataEvent)
        {
            if (sp.IsOpen)
            {
                try
                {
                    datosM = sp.ReadExisting();
                    Dispatcher.Invoke(DispatcherPriority.Send, new newDelegate(cambiarOperaciones), datosM);
                }
                catch (Exception)
                {
                    MessageBox.Show(error4Text, error4);
                }
            }
        }
        private delegate void newDelegate(string t);
        private void cambiarOperaciones(string datosM)
        {
            try
            {
                double var = Double.Parse(datosM);
                var = var * 5 / 1.023;
                double varNivel = var / 54;
                double varNPresion = varNivel * 7.5;
                double varNPNormal = var / 10;

                nivelVolts.Text = String.Format("{0:#,0.##}", var);
                nivel.Text = String.Format("{0:0.##}", varNivel);
                nivelPresion.Text = String.Format("{0:0.##}", varNPresion);
                nivelPNormalizado.Text = String.Format("{0:0.##}", varNPNormal);
            }
            catch (Exception)
            {
                sp.Close();
                status.Text = desconectadoText;
                MessageBox.Show(error5Text, error5);
            }
        }

        private void eraseAll()
        {
            datosM = "0";
            nivelVolts.Text = "0";
            nivel.Text = "0";
            nivelPresion.Text = "0";
            nivelPNormalizado.Text = "0";
        }
    }
}

