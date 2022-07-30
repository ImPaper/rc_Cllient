using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace chart_CLT
{
    public partial class MainSetdata : Form
    {
        Socket m_sck = null;
        public LoginView m_lv = null;
        public MainSetdata()
        {
            InitializeComponent();

            
        }

        private void MainSetdata_Load(object sender, EventArgs e)
        {
            ConnectServer();
        }
        public void ConnectServer()
        {
            try
            {


                if (m_sck == null)
                {
                    m_sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    //string ServerIP = "192.168.117.41";
                    //string ServerIP = "192.168.117.113";


                    // real ip, port
                    //string ServerIP = "218.159.115.154";    Int32 Port = 10000;
                    //

                    // test ip, port
                    //string ServerIP = "192.168.43.44";    Int32 Port = 10001;
                    //string ServerIP = "192.168.117.41";    Int32 Port = 10001;
                    //string ServerIP = "192.168.117.42";    Int32 Port = 10001;
                    //string ServerIP = "115.91.67.98"; Int32 Port = 10001;
                    string ServerIP = "192.168.78.202"; Int32 Port = 12221;


                    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(ServerIP), Port);
                    m_sck.Connect(remoteEP);

                    //byte[] b = { 0xFF, 0x0F, 0x0F, 0xFF };
                    //int bytesSent = m_sck.Send(b);
                    //int bytesSent = sck.Send(Encoding.UTF8.GetBytes("test"));
                    //toolStripStatusLabel1.Text = "Connected";
                    //this.Text = this.Text + " - " + sck.LocalEndPoint.ToString();
                    // read the socket in thread
                    Thread t_handler = new Thread(readScoket);
                    t_handler.IsBackground = true;
                    t_handler.Start();
                }
                else
                {
                    MessageBox.Show("Socket is not null");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // close the socket
                CloseClient();
                Application.Exit();
            }
        }

        public int m_isPTZ = 0;
        public int m_codeLevel = -1;
        public bool idcheckOK = false;
        public void readScoket()            //@@@@@@@@@@@@@@@@@@@@@서버에서 받는거 일로 떨어짐.
        {
            Rectangle screenSet = Screen.PrimaryScreen.Bounds;
            while (true)
            {
                try
                {
                    //201204N5XN96SGV



                    byte[] buffer = new byte[2048];
                    //byte[] buffer = new byte[256];
                    int bytesRead = m_sck.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                    string message = Encoding.UTF8.GetString(buffer);
                    if (bytesRead <= 0)
                    {
                        throw new SocketException();
                    }
                    else
                    {
                        m_lv.SetBounds(0, 0, 1, 1);
                        if (message.Contains("***idcheckOK***"))
                        {
                            message = message.Substring(0, message.IndexOf("***idcheckOK***"));
                            idcheckOK = true;
                            MessageBox.Show(message, "TH_Client", MessageBoxButtons.OK,
                            MessageBoxIcon.None, MessageBoxDefaultButton.Button1);

                        }
                        else if (message.Contains("***complete***"))     //로그인 성공
                        {

                            message = message.Substring(0, message.IndexOf("***complete***"));

                            MessageBox.Show(message, "TH_Client", MessageBoxButtons.OK,
                            MessageBoxIcon.None, MessageBoxDefaultButton.Button1);

                            Invoke((MethodInvoker)delegate
                            {
                                //m_lv.timerLoginBar.Enabled = false;
                                m_lv.SetBounds(0, 0, 1, 1);
                                //m_lv.SetBounds(-305, 0, 305, 600);

                                //this.SetBounds(0, 0, 1920, 1080);

                                /*if (screenSet.Width == 1920 && screenSet.Height == 1080)
                                {
                                    this.SetBounds(0, 0, 1920, 1080);
                                }
                                else if (screenSet.Width == 1280 && screenSet.Height == 1024)
                                {
                                    this.SetBounds(0, 0, 1280, 1040);
                                }*/
                            });
                        }
                        else if (message.Contains("***alreadyLogin***"))
                        {
                            message = message.Substring(0, message.IndexOf("***alreadyLogin***"));

                            MessageBox.Show(message, "TH_Client", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);

                            Application.ExitThread();
                            Environment.Exit(0);
                        }
                        else if (message.Contains("***signComplete***"))    //회원가입 성공
                        {

                            message = message.Substring(0, message.IndexOf("***signComplete***"));

                            MessageBox.Show(message, "TH_Client", MessageBoxButtons.OK,
                            MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
                            Invoke((MethodInvoker)delegate
                            {
                                //m_lv.fn_LoginSet();
                            });

                        }
                        else if (message.Contains("***alreadyID***"))
                        {
                            message = message.Substring(0, message.IndexOf("***alreadyID***"));
                            idcheckOK = false;
                            MessageBox.Show(message, "TH_Client", MessageBoxButtons.OK,
                            MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
                        }
                        else if (message.Contains("***fail***"))
                        {
                            message = message.Substring(0, message.IndexOf("***fail***"));
                            MessageBox.Show(message, "TH_Client", MessageBoxButtons.OK,
                            MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
                        }
                        // code state
                        else if (message.Contains("***codeBefore***"))
                        {

                            m_codeLevel = 0;
                            message = "It is still before the test. Close Application.";
                            MessageBox.Show(message, "TH_Client", MessageBoxButtons.OK,
                            MessageBoxIcon.None, MessageBoxDefaultButton.Button1);


                            Application.Exit();
                        }
                        else if (message.Contains("***codeLive***"))
                        {

                            m_codeLevel = 1;
                            message = "You are a real-time test member.";
                            MessageBox.Show(message, "TH_Client", MessageBoxButtons.OK,
                            MessageBoxIcon.None, MessageBoxDefaultButton.Button1);

                            this.SetBounds(0, 0, 1920, 1080);


                            
                        }
                        
                    }

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    MessageBox.Show("Client Disconnect: " + e.Message);

                    // close the socket
                    //CloseClient();
                    //Application.Exit();
                    break;
                }
            }
            CloseClient();
            Application.Exit();

        }
        
        public void CloseClient()
        {
            if(m_sck != null)
            {
                m_sck.Close();
                m_sck.Dispose();
                m_sck = null;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SendClientMSG("Test");
        }

        public void SendClientMSG(string msg)
        {
            if (m_sck == null) {
                MessageBox.Show("socket null");
                return; }
            m_sck.Send(Encoding.UTF8.GetBytes(msg));
        }
    }
}
