using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestLimitSocketCommon.Data;

namespace TestLimitSocketServer
{
    public partial class FormServer : Form
    {
        private TcpListener mListener = null;
        private System.Net.Sockets.TcpClient mClient = null;
        private NetworkStream mStream = null;
        public FormServer()
        {
            InitializeComponent();
            mListener = new TcpListener(System.Net.IPAddress.Parse("192.168.1.8"), 2001);

            Task.Run(() =>
            {
                /* Listen開始 */
                this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"1.Listen開始")));
                File.AppendAllText(@"../../../../server.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} 1.Listen開始\n");
                mListener.Start();

                /* 接続待ち */
                this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"2.クライアントと接続待ち")));
                File.AppendAllText(@"../../../../server.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} 2.クライアントと接続待ち\n");
                mClient = mListener.AcceptTcpClient();
                this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"3.クライアントと接続 {((System.Net.IPEndPoint)mClient.Client.RemoteEndPoint).Address} {((System.Net.IPEndPoint)mClient.Client.RemoteEndPoint).Port}")));
                File.AppendAllText(@"../../../../server.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} 3.クライアントと接続 {((System.Net.IPEndPoint)mClient.Client.RemoteEndPoint).Address} {((System.Net.IPEndPoint)mClient.Client.RemoteEndPoint).Port}\n");

                /* NetworkStreamを取得 */
                this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"4.NetworkStreamを取得")));
                File.AppendAllText(@"../../../../server.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} 4.NetworkStreamを取得\n");
                mStream = mClient.GetStream();


                /* メッセージ送信スレッド起動 */
                Task.Run(() => { SndMessageLoop(); });

                /* 終了 */
                this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"5.このタスクは任務終了")));
                File.AppendAllText(@"../../../../server.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} 5.このタスクは任務終了\n");
                return;
            });
        }

        private void SndMessageLoop()
        {
            this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"6.送信スレッド起動")));
            File.AppendAllText(@"../../../../server.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} 6.送信スレッド起動\n");

            int workno = 0;
            while (true)
            {
                this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"CommandInspResult生成")));
                File.AppendAllText(@"../../../../server.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} CommandInspResult生成\n");
                this.Invoke((MethodInvoker)(() => this.Logs.TopIndex = this.Logs.Items.Count - 1));
                CommandInspResult snddata = new CommandInspResult()
                {
                    IsErr = false,
                    WorkNo = workno++,
                    //Scores = 何も設定しない.
                };

                this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"WorkNo={snddata.WorkNo} 1枚目画像設定 s")));
                File.AppendAllText(@"../../../../server.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} WorkNo={snddata.WorkNo} 1枚目画像設定 s\n");
                this.Invoke((MethodInvoker)(() => this.Logs.TopIndex = this.Logs.Items.Count - 1));
                /* 画像1枚目の設定 */
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    Bitmap bmp = new Bitmap("../../../../4k4k-67_1.png");
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    snddata.ImgData = ms.ToArray();
                    snddata.ImgSize = snddata.ImgData.Length;
                    bmp.Dispose();
                    ms.Close();
                }
                GC.Collect();

                this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"1枚目画像設定 e size={snddata.ImgSize/(1000f*1000f)}MB")));
                File.AppendAllText(@"../../../../server.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} 1枚目画像設定 e size={snddata.ImgSize / (1000f * 1000f)}MB\n");
                this.Invoke((MethodInvoker)(() => this.Logs.TopIndex = this.Logs.Items.Count - 1));

                this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"WorkNo={snddata.WorkNo} 2枚目画像設定 s")));
                File.AppendAllText(@"../../../../server.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} 2枚目画像設定 s\n");
                this.Invoke((MethodInvoker)(() => this.Logs.TopIndex = this.Logs.Items.Count - 1));
                /* 画像2枚目の設定 */
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    Bitmap bmp = new Bitmap("../../../../4k4k-67_2.png");
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    snddata.ImgData2 = ms.ToArray();
                    snddata.ImgSize2 = snddata.ImgData2.Length;
                    bmp.Dispose();
                    ms.Close();
                }
                GC.Collect();
                this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"2枚目画像設定 e size={snddata.ImgSize2/(1000*1000f)}MB")));
                File.AppendAllText(@"../../../../server.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} 2枚目画像設定 e size={snddata.ImgSize2 / (1000 * 1000f)}MB\n");
                this.Invoke((MethodInvoker)(() => this.Logs.TopIndex = this.Logs.Items.Count - 1));

                this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"WorkNo={snddata.WorkNo} 送信データをバイナリ変換")));
                File.AppendAllText(@"../../../../server.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} WorkNo={snddata.WorkNo} 送信データをバイナリ変換\n");
                this.Invoke((MethodInvoker)(() => this.Logs.TopIndex = this.Logs.Items.Count - 1));
                /* 送信データをシリアル化 */
                byte[] byteCommandBase = null;
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    System.Runtime.Serialization.IFormatter fmt = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    fmt.Serialize(ms, snddata);
                    byteCommandBase = ms.ToArray();
                    ms.Close();
                }

                byte[] sndDataSize = BitConverter.GetBytes(byteCommandBase.Length);
                byte[] sndPacketData = sndDataSize.Concat(byteCommandBase).ToArray();
                mStream.Write(sndPacketData, 0, sndPacketData.Length);
                this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"-----データ送信 WorkNo={snddata.WorkNo} 送信データ={(sndPacketData.Length-4)/(1000*1000f)}MB")));
                File.AppendAllText(@"../../../../server.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} -----データ送信 WorkNo={snddata.WorkNo} 送信データ={(sndPacketData.Length - 4) / (1000 * 1000f)}MB\n");
                this.Invoke((MethodInvoker)(() => this.Logs.TopIndex = this.Logs.Items.Count - 1));
            }

            this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"7.送信スレッド終了")));
            File.AppendAllText(@"../../../../server.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} 7.送信スレッド終了\n");
            this.Invoke((MethodInvoker)(() => this.Logs.TopIndex = this.Logs.Items.Count - 1));
        }
    }
}
