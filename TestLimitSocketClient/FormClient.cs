using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestLimitSocketCommon.Data;

namespace TestLimitSocketClient
{
    public partial class FormClient : Form
    {
        private System.Net.Sockets.TcpClient mClient = null;
        System.Net.Sockets.NetworkStream mStream = null;

        // DataAvailable Read 


        public FormClient()
        {
            InitializeComponent();

            Task.Run(() =>
            {
                /* サーバーと接続 */
                this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"1.サーバーと接続開始")));
                File.AppendAllText(@"../../../../client.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} 1.サーバーと接続開始\n");
                while (true)
                {
                    try
                    {
                        mClient = new System.Net.Sockets.TcpClient("192.168.1.8", 2001);
                        break;
                    }
                    catch (Exception e)
                    {
                        if (e is System.Net.Sockets.SocketException)
//                      if (e is System.Net.Sockets.SocketException || e is ArgumentNullException || e is ArgumentOutOfRangeException)
                        {
                            this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"想定内エラー。繰返します。")));
                            File.AppendAllText(@"../../../../client.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} 想定内エラー。繰返します。\n");
                            continue;
                        }
                        this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"想定外例外。e={e}")));
                        File.AppendAllText(@"../../../../client.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} 想定外例外。e={e}\n");
                    }
                }
                this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"2.サーバー({((System.Net.IPEndPoint)mClient.Client.RemoteEndPoint).Address}:{((System.Net.IPEndPoint)mClient.Client.RemoteEndPoint).Port})と接続しました({((System.Net.IPEndPoint)mClient.Client.LocalEndPoint).Address}:{((System.Net.IPEndPoint)mClient.Client.LocalEndPoint).Port})")));
                File.AppendAllText(@"../../../../client.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} 2.サーバー({((System.Net.IPEndPoint)mClient.Client.RemoteEndPoint).Address}:{((System.Net.IPEndPoint)mClient.Client.RemoteEndPoint).Port})と接続しました({((System.Net.IPEndPoint)mClient.Client.LocalEndPoint).Address}:{((System.Net.IPEndPoint)mClient.Client.LocalEndPoint).Port})\n");

                /* Stream取得 */
                this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"3.Stream取得")));
                File.AppendAllText(@"../../../../client.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} 3.Stream取得\n");
                mStream = mClient.GetStream();

                /* メッセージ受信スレッド起動 */
                Task.Run(() => { WaitforMessageLoop(); });

                /* 終了 */
                this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"4.このタスクは任務終了")));
                File.AppendAllText(@"../../../../client.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} 4.このタスクは任務終了\n");
                return;
            });

        }

        private void WaitforMessageLoop()
        {
            this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"6.受信スレッド起動")));
            File.AppendAllText(@"../../../../client.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} 6.受信スレッド起動\n");
            this.Invoke((MethodInvoker)(() => this.Logs.TopIndex = this.Logs.Items.Count - 1));

            byte[] rcvHead = new byte[sizeof(int)];
            byte[] rcvBody = new byte[100 * 1024 * 1024];
            while (true)
            {
                CommandBase database = null;
                using (System.IO.MemoryStream rcvBodyMs = new System.IO.MemoryStream())
                {
                    try
                    {
                        int intSize = mStream.Read(rcvHead, 0, rcvHead.Length);
                        int expectedRcvSize = BitConverter.ToInt32(rcvHead, 0);
                        this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"mStream.Read() ---受信予定サイズ {expectedRcvSize/(1000f*1000f)}MB")));
                        File.AppendAllText(@"../../../../client.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} mStream.Read() ---受信予定サイズ {expectedRcvSize/(1000f*1000f)}MB\n");
                        this.Invoke((MethodInvoker)(() => this.Logs.TopIndex = this.Logs.Items.Count - 1));
                        if (intSize != 0)
                        {
                            /* 本体受信 */
                            for(int retrycnt = 0; rcvBodyMs.Length < expectedRcvSize; retrycnt++)
                            {
                                int bodyRcvSize = mStream.Read(rcvBody, 0, rcvBody.Length);
                                rcvBodyMs.Write(rcvBody, 0, bodyRcvSize);
                                this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"mStream.Read() ---受信経過 {retrycnt}回目 {bodyRcvSize/(1000f*1000f)}MB/{rcvBodyMs.Length/(1000f*1000f)}MB")));
                                File.AppendAllText(@"../../../../client.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} mStream.Read() ---受信経過 {retrycnt}回目 {bodyRcvSize/(1000f*1000f)}MB/{rcvBodyMs.Length/(1000f*1000f)}MB\n");
                                this.Invoke((MethodInvoker)(() => this.Logs.TopIndex = this.Logs.Items.Count - 1));
                            }
                        }
                        else
                        {
                            /* 受信サイズが0ってどういうこと?? */
                            this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"mStream.Read 受信サイズが0ってどういうこと??")));
                            File.AppendAllText(@"../../../../client.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} mStream.Read 受信サイズが0ってどういうこと??\n");
                            this.Invoke((MethodInvoker)(() => this.Logs.TopIndex = this.Logs.Items.Count - 1));
                        }
                        rcvBodyMs.Seek(0, System.IO.SeekOrigin.Begin);
                        this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"mStream.Read() ---受信完了 総サイズ {rcvBodyMs.Length/(1000f*1000f)}MB")));
                        File.AppendAllText(@"../../../../client.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} mStream.Read() ---受信完了 総サイズ {rcvBodyMs.Length/(1000f*1000f)}MB\n");
                        this.Invoke((MethodInvoker)(() => this.Logs.TopIndex = this.Logs.Items.Count - 1));
                    }
                    catch (Exception e)
                    {
                        if (e is ArgumentException || e is ArgumentOutOfRangeException || e is System.IO.IOException || e is ObjectDisposedException)
                        {
                            this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"mStream.Read 起きうる例外!! 受信スレッド異常終了!! {e}")));
                            File.AppendAllText(@"../../../../client.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} mStream.Read 起きうる例外!! 受信スレッド異常終了!! {e}\n");
                            this.Invoke((MethodInvoker)(() => this.Logs.TopIndex = this.Logs.Items.Count - 1));
                            break;
                        }
                        else
                        {
                            this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"mStream.Read 正体不明例外!! 受信スレッド異常終了!! {e}")));
                            File.AppendAllText(@"../../../../client.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} mStream.Read 正体不明例外!! 受信スレッド異常終了!! {e}\n");
                            this.Invoke((MethodInvoker)(() => this.Logs.TopIndex = this.Logs.Items.Count - 1));
                            break;
                        }

                    }

                    this.Invoke((MethodInvoker)(() => this.Logs.TopIndex = this.Logs.Items.Count - 1));

                    /* データ復元 */
                    this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"受信データ復元開始...")));
                    File.AppendAllText(@"../../../../client.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} mStream.Read 受信データ復元開始...\n");
                    this.Invoke((MethodInvoker)(() => this.Logs.TopIndex = this.Logs.Items.Count - 1));
                    System.Runtime.Serialization.IFormatter fmt = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    database = (CommandBase)fmt.Deserialize(rcvBodyMs);
                    rcvBodyMs.Close();
                    this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"受信データ復元完了.")));
                    File.AppendAllText(@"../../../../client.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} 受信データ復元完了.\n");
                    this.Invoke((MethodInvoker)(() => this.Logs.TopIndex = this.Logs.Items.Count - 1));
                }

                CommandInspResult inpsresult = (CommandInspResult)database;
                /* 最初の画像を復元 */
                using (MemoryStream ms = new MemoryStream(inpsresult.ImgData))
                using (Bitmap bmp = new Bitmap(ms))
                {
                    bmp.Save($"../../../../{inpsresult.WorkNo}-ImgData1.png");
                    ms.Close();
                    this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"{inpsresult.WorkNo}の1枚目保存. {inpsresult.WorkNo}-ImgData1.png")));
                    File.AppendAllText(@"../../../../client.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} {inpsresult.WorkNo}の1枚目保存. {inpsresult.WorkNo}-ImgData1.png\n");
                    this.Invoke((MethodInvoker)(() => this.Logs.TopIndex = this.Logs.Items.Count - 1));
                }
                GC.Collect();

                /* 2番目の画像を復元 */
                using (MemoryStream ms = new MemoryStream(inpsresult.ImgData2))
                using (Bitmap bmp = new Bitmap(ms))
                {
                    bmp.Save($"../../../../{inpsresult.WorkNo}-ImgData2.png");
                    ms.Close();
                    this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"{inpsresult.WorkNo}の2枚目保存. {inpsresult.WorkNo}-ImgData2.png")));
                    File.AppendAllText(@"../../../../client.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} {inpsresult.WorkNo}の2枚目保存. {inpsresult.WorkNo}-ImgData2.png\n");
                    this.Invoke((MethodInvoker)(() => this.Logs.TopIndex = this.Logs.Items.Count - 1));
                }
                GC.Collect();
            }
            /* ListBoxをScroll */
            File.AppendAllText(@"../../../../client.log", $"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} 6.受信スレッド終了\n");
            this.Invoke((MethodInvoker)(() => this.Logs.Items.Add($"6.受信スレッド終了")));
        }
    }
}
