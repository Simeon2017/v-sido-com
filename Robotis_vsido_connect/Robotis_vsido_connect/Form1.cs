using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;


namespace Robotis_vsido_connect
{
    public partial class Form1 : Form
    {
        ComboBox comb;      //comポート一覧
        SerialPort port;    //しりあるポート
        List<byte> command_list = new List<byte>();
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comb = comboBox1;
            comb.Items.Add("COM1");
            comb.Items.Add("COM2");
            comb.Items.Add("COM3");
            comb.Items.Add("COM4");
            comb.Items.Add("COM5");
            comb.Items.Add("COM6");
            comb.Items.Add("COM7");
            comb.Items.Add("COM8");
            comb.Items.Add("COM9");
            comb.Items.Add("COM10");
            comb.Items.Add("COM11");
            comb.Items.Add("COM12");
            comb.Items.Add("COM13");
            comb.Items.Add("COM14");
            comb.SelectedIndex = 13; 
        }

        private void button1_Click(object sender, EventArgs e)
        {

            port = new SerialPort();
                   if (comb.SelectedItem == null) {
                     MessageBox.Show("COMポートを選択してください", "エラー",MessageBoxButtons.OK,MessageBoxIcon.Error);
                   return; 
             }
            port.PortName = comb.SelectedItem.ToString();

            port.BaudRate = 115200;
            port.StopBits = StopBits.One;
            port.Parity = Parity.None;
            port.DataBits = 8;

            try
            {
                port.Open();
                port.DataReceived += SerialPortReceivedData;
            }
            catch
            {
                MessageBox.Show("comが開けません", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            //接続コマンド送信
            try
            {
                byte[] serial_byte = new byte[5];
                serial_byte[0] = 0xff;
                serial_byte[1] = 0x67;
                serial_byte[2] = 0x05;
                serial_byte[3] = 0xfe;
                serial_byte[4] = 0x63;

                port.Write(serial_byte, 0, serial_byte.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            port.Close();
        }

        void SerialPortReceivedData(object sender, SerialDataReceivedEventArgs e)
        {
            // シリアルポートのバッファーの読み込む
        //    byte[] data = Encoding.ASCII.GetBytes(port.ReadExisting());
        //    string d = System.Text.Encoding.ASCII.GetString(data);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = @"C:\";
            ofd.Filter = "CSVファイル|*.csv";
            ofd.FilterIndex = 0;
            ofd.Title = "開くファイルを選択してください";
            ofd.CheckFileExists = true;
          
            if(ofd.ShowDialog() == DialogResult.OK){
                string file = ofd.FileName;
                try
                {
                    // csvファイルを開く
                    using (var sr = new System.IO.StreamReader(file))
                    {
                        // ストリームの末尾まで繰り返す
                        while (!sr.EndOfStream)
                        {
                            // ファイルから一行読み込む
                            var line = sr.ReadLine();
                            // 読み込んだ一行をカンマ毎に分けて配列に格納する
                            var values = line.Split(',');

                            foreach (var value in values)
                            {
                                if(value == "ff"){
                                    command_list = new List<byte>();
                                }
                               command_list.Add(Convert.ToByte(value,16));
                            }
                            foreach (var test in command_list)
                            {
                                listBox1.Items.Add(test.ToString("x"));
                            }

                         //1秒間隔でv-sido connectに送る
                            byte[] command = command_list.ToArray();
                            port.Write(command, 0, command.Length);
                            System.Threading.Thread.Sleep(1000); 
                        }
 
                    }
                }
                catch (System.Exception ee)
                {
                    // ファイルを開くのに失敗したとき
                    MessageBox.Show(ee.ToString(), "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }

        }
    }
}
