﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using FtpSyn;
using GetVersion;

namespace ShadowSocksRUpdate
{
    public partial class Form1 : Form
    {
        int Error = 0;
        delegate void ChangeStates(String States);
        delegate void Closeall();
        delegate void ReStartLink();
        delegate void GetStates();
        struct StateStru
        {
            public String Value;
            public int col;
        }
        public Form1()
        {
            InitializeComponent();
            ThreadStart Father = new ThreadStart(FatherThread);
            Thread Fa = new Thread(Father);
            Fa.IsBackground = true;
            Fa.Start();
            
        }

        public void FatherThread()
        {
            ThreadStart GetStates = new ThreadStart(GetStatesMethod);
            Thread Get = new Thread(GetStates);
            Get.IsBackground = true;
            //Thread G = new Thread(Get);
            string Ts = "";
            while (true)
            {
                if (Error == 0)
                {
                    //GetStates = new ThreadStart(GetStatesMethod);
                    //Get = new Thread(GetStates);
                    try
                    {
                        Get = new Thread(GetStates);
                        Get.Start();
                        Error = 1;
                    }
                    catch (Exception)
                    {
                        Error = -1;
                    }
                }
                else if (Error == -1)
                {
                    Get.Abort();
                    Ts = Get.ThreadState.ToString();
                    Error = 0;
                }
            }
        }

        public void GetStatesMethod()
        {
            string Versions = "";
            FtpHelper Ftp = new FtpHelper();
            StateStru States;
            States.col = 0;
            States.Value = "";
            States = TimeUpdate(States);
            VersionGet Ver = new VersionGet();
            Ver.Send();
            States = LabelStick(States, "正在连接FTP服务器...");
            while (true)
            {
                if (Ver.SendStatus == -1)
                {
                    States = LabelStick(States, Ver.SendMessage);
                    ReStart();
                    return;
                }
                else if (Ver.SendStatus == 1)
                {
                    States = LabelStick(States, Ver.SendMessage);
                    while (true)
                    {
                        if (Ver.RecvStatus == -1)
                        {
                            States = LabelStick(States, Ver.RecvMessage);
                            ReStart();
                            return;
                        }
                        else if (Ver.RecvStatus == 1)
                        {
                            States = LabelStick(States, "版本号：" + Ver.RecvMessage);
                            Versions = Ver.RecvMessage;
                            break;
                        }
                    }
                    break;
                }
            }
            StateStru Vers = Read(".\\ssr-win\\version.txt");
            string vers = Vers.Value;
            if (Vers.Value.Substring(Vers.Value.IndexOf("version") + 8, 4) == Versions)
            {
                States = LabelStick(States, "服务端版本未更新..正在启动..");
                System.Diagnostics.Process.Start(".\\ssr-win\\ShadowsocksR-dotnet4.0.exe");
                Closed();
            }
            else
            {
                //int Large = -1;  //调试
                int Large = FtpHelper.GetFileSize("gui-config.json");
                if (Large == -1)
                {
                    States = LabelStick(States, "获取文件失败，请重试！");
                    ReStart();
                }
                else
                {
                    States = LabelStick(States, "正在获取文件大小...");
                    String SLarge = "获取成功，文件大小为" + Large.ToString() + "KB";
                    States = LabelStick(States, SLarge);
                    States = LabelStick(States, "正在下载...");
                    //int Suc = -1;
                    int Suc = Ftp.Download("gui-config.json");
                    if (Suc == -1)
                    {
                        States = LabelStick(States, "下载失败，请重试！");
                        ReStart();
                    }
                    else
                    {
                        States = LabelStick(States, "下载完成");
                        vers = vers.Substring(0, vers.IndexOf("version") + 8) + Versions + "\n";
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@".\\ssr-win\\version.txt", false))
                        {
                            WriteData(file, vers);
                        }
                        System.Diagnostics.Process.Start(".\\ssr-win\\ShadowsocksR-dotnet4.0.exe");
                        Closed();
                    }
                }
            }
            
            
        }

        //初始化与次数统计
        private StateStru TimeUpdate(StateStru States)
        {
            String Times = this.State.Text;
            int time;
            int first = Times.IndexOf(":");
            int last = Times.IndexOf("\r\n");
            Times = Times.Substring(first + 1, last - first - 4);
            time = int.Parse(Times) + 1;
            Times = time.ToString();
            States.Value = "State(Times:" + Times + ")..\r\n小心心么么哒.";
            F5(States.Value);
            //States.Value = this.State.Text;
            States.col = 2;
            return States;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.State.BackColor = Color.FromArgb(100, 192, 192, 192);
        }

        private void State_Click(object sender, EventArgs e)
        {

        }
        
        //输出结构体并更新
        private StateStru LabelStick(StateStru StateBefore, string ToStick)
        {
            StateStru StateAfter;
            StateAfter.Value = StateBefore.Value + "\r\n" + ToStick;
            if (StateBefore.col + 1 >= 12)
            {
                StateBefore.col = StateAfter.Value.IndexOf("\r\n");
                StateAfter.Value = StateAfter.Value.Substring(StateBefore.col + 4);
                StateAfter.col = StateBefore.col;
            }
            else
            {
                StateAfter.col = StateBefore.col + 1;
            }
            F5(StateAfter.Value);
            return StateAfter;
        }

        //读取文件内容
        private StateStru Read(string path)
        {
            StateStru Include;
            StreamReader sr = new StreamReader(path,Encoding.Default);
            String line;
            Include.Value = "";
            Include.col = 0;
            while ((line = sr.ReadLine()) != null) 
            {
                if (line != "")
                {
                    Include.Value = Include.Value + line + "\n";
                    Include.col += 1;
                }
            }
            sr.Close();
            return Include;
        }

        //内容逐行写入文件
        private void WriteData(System.IO.StreamWriter File, string Waiting)
        {
            //把文件打开转移到函数外否则不可递归
                string temp = "";
                int wlength = Waiting.Length;
                int tlength = 0;
                int Index = Waiting.IndexOf("\n");
                if (Index != 0 & wlength != 0)
                {
                    temp = Waiting.Substring(0, Waiting.IndexOf("\n"));
                    tlength = temp.Length;
                    Waiting = Waiting.Remove(0, tlength + 1);
                    wlength = Waiting.Length;
                    File.WriteLine(temp);
                }
                else if (Index == 0 & wlength != 0)
                {
                    File.WriteLine(Waiting);
                    wlength = 0;
                }
                else if (Index == 0 & wlength == 0)
                {
                    return;
                }
                if (wlength - 1 >= 0)
                {
                    WriteData(File, Waiting);
                    return;
                }
        }

        //public void Get()
        //{
        //    if (this.InvokeRequired)
        //    {
        //        this.BeginInvoke(new GetStates(Get));
        //    }
        //    else
        //    {
        //        GetStatesMethod();
        //    }
        //}

        public void Closed()
        {
            if(this.InvokeRequired)
            {
                this.BeginInvoke(new Closeall(Closed));
            }
            else
            {
                Close();
            }
        }

        public void F5(String States)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ChangeStates(F5), States);
            }
            else
            {
                this.State.Text = States;
            }
        }

        public void ReStart()
        {
            if(this.InvokeRequired)
            {
                this.BeginInvoke(new ReStartLink(ReStart));
            }
            else
            {
                this.ReLink.Enabled = true;
                this.Cancel.Enabled = true;
                this.ReLink.BackColor = Color.FromArgb(255, 255, 255, 255);
                this.ReLink.ForeColor = Color.FromArgb(255, 0, 0, 0);
                this.Cancel.BackColor = Color.FromArgb(255, 255, 255, 255);
                this.Cancel.ForeColor = Color.FromArgb(255, 0, 0, 0);
            }
        }

        private void ReLink_Click(object sender, EventArgs e)
        {
            Error = 0;
            this.ReLink.Enabled = false;
            this.ReLink.BackColor = Color.FromArgb(255, 132, 193, 193);
            this.ReLink.ForeColor = Color.FromArgb(255, 192, 192, 192);
            this.Cancel.Enabled = false;
            this.Cancel.BackColor = Color.FromArgb(255, 132, 193, 193);
            this.Cancel.ForeColor = Color.FromArgb(255, 192, 192, 192);
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(".\\ssr-win\\ShadowsocksR-dotnet4.0.exe");
            Closed();
        }

    }
}

namespace FtpSyn
{
    public class FtpHelper
    {
        //基本设置
        static private string path = @"ftp://" + "0.0.0.0" + "/";    //目标路径
        static private string ftpip = "0.0.0.0";    //ftp IP地址
        static private string username = "username";   //ftp用户名
        static private string password = "password";   //ftp密码

        //获取ftp上面的文件和文件夹
        public static string[] GetFileList(string dir)
        {
            string[] downloadFiles;
            StringBuilder result = new StringBuilder();
            FtpWebRequest request;
            try
            {
                request = (FtpWebRequest)FtpWebRequest.Create(new Uri(path));
                request.UseBinary = true;
                request.Credentials = new NetworkCredential(username, password);//设置用户名和密码
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.UseBinary = true;

                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());

                string line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    Console.WriteLine(line);
                    line = reader.ReadLine();
                }
                // to remove the trailing '\n'
                result.Remove(result.ToString().LastIndexOf('\n'), 1);
                reader.Close();
                response.Close();
                return result.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                Console.WriteLine("获取ftp上面的文件和文件夹：" + ex.Message);
                downloadFiles = null;
                return downloadFiles;
            }
        }

        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="file">ip服务器下的相对路径</param>
        /// <returns>文件大小</returns>
        public static int GetFileSize(string file)
        {
            StringBuilder result = new StringBuilder();
            FtpWebRequest request;
            try
            {
                request = (FtpWebRequest)FtpWebRequest.Create(new Uri(path + file));
                request.UseBinary = true;
                request.Credentials = new NetworkCredential(username, password);//设置用户名和密码
                request.Method = WebRequestMethods.Ftp.GetFileSize;

                int dataLength = (int)request.GetResponse().ContentLength;
                

                return dataLength;
            }
            catch (Exception ex)
            {
                Console.WriteLine("获取文件大小出错：" + ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="filePath">原路径（绝对路径）包括文件名</param>
        /// <param name="objPath">目标文件夹：服务器下的相对路径 不填为根目录</param>
        public static void FileUpLoad(string filePath, string objPath = "")
        {
            try
            {
                string url = path;
                if (objPath != "")
                    url += objPath + "/";
                try
                {

                    FtpWebRequest reqFTP = null;
                    //待上传的文件 （全路径）
                    try
                    {
                        FileInfo fileInfo = new FileInfo(filePath);
                        using (FileStream fs = fileInfo.OpenRead())
                        {
                            long length = fs.Length;
                            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(url + fileInfo.Name));

                            //设置连接到FTP的帐号密码
                            reqFTP.Credentials = new NetworkCredential(username, password);
                            //设置请求完成后是否保持连接
                            reqFTP.KeepAlive = false;
                            //指定执行命令
                            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
                            //指定数据传输类型
                            reqFTP.UseBinary = true;

                            using (Stream stream = reqFTP.GetRequestStream())
                            {
                                //设置缓冲大小
                                int BufferLength = 5120;
                                byte[] b = new byte[BufferLength];
                                int i;
                                while ((i = fs.Read(b, 0, BufferLength)) > 0)
                                {
                                    stream.Write(b, 0, i);
                                }
                                Console.WriteLine("上传文件成功");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("上传文件失败错误为" + ex.Message);
                    }
                    finally
                    {

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("上传文件失败错误为" + ex.Message);
                }
                finally
                {

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("上传文件失败错误为" + ex.Message);
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileName">服务器下的相对路径 包括文件名</param>
        public static void DeleteFileName(string fileName)
        {
            try
            {
                FileInfo fileInf = new FileInfo(ftpip + "" + fileName);
                string uri = path + fileName;
                FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                // 指定数据传输类型
                reqFTP.UseBinary = true;
                // ftp用户名和密码
                reqFTP.Credentials = new NetworkCredential(username, password);
                // 默认为true，连接不会被关闭
                // 在一个命令之后被执行
                reqFTP.KeepAlive = false;
                // 指定执行什么命令
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("删除文件出错：" + ex.Message);
            }
        }

        /// <summary>
        /// 新建目录 上一级必须先存在
        /// </summary>
        /// <param name="dirName">服务器下的相对路径</param>
        public static void MakeDir(string dirName)
        {
            try
            {
                string uri = path + dirName;
                FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                // 指定数据传输类型
                reqFTP.UseBinary = true;
                // ftp用户名和密码
                reqFTP.Credentials = new NetworkCredential(username, password);
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("创建目录出错：" + ex.Message);
            }
        }

        /// <summary>
        /// 删除目录 上一级必须先存在
        /// </summary>
        /// <param name="dirName">服务器下的相对路径</param>
        public static void DelDir(string dirName)
        {
            try
            {
                string uri = path + dirName;
                FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                // ftp用户名和密码
                reqFTP.Credentials = new NetworkCredential(username, password);
                reqFTP.Method = WebRequestMethods.Ftp.RemoveDirectory;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("删除目录出错：" + ex.Message);
            }
        }

        /// <summary>
        /// 从ftp服务器上获得文件夹列表
        /// </summary>
        /// <param name="RequedstPath">服务器下的相对路径</param>
        /// <returns></returns>
        public static List<string> GetDirctory(string RequedstPath)
        {
            List<string> strs = new List<string>();
            try
            {
                string uri = path + RequedstPath;   //目标路径 path为服务器地址
                FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                // ftp用户名和密码
                reqFTP.Credentials = new NetworkCredential(username, password);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                WebResponse response = reqFTP.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());//中文文件名

                string line = reader.ReadLine();
                while (line != null)
                {
                    if (line.Contains("<DIR>"))
                    {
                        string msg = line.Substring(line.LastIndexOf("<DIR>") + 5).Trim();
                        strs.Add(msg);
                    }
                    line = reader.ReadLine();
                }
                reader.Close();
                response.Close();
                return strs;
            }
            catch (Exception ex)
            {
                Console.WriteLine("获取目录出错：" + ex.Message);
            }
            return strs;
        }

        /// <summary>
        /// 从ftp服务器上获得文件列表
        /// </summary>
        /// <param name="RequedstPath">服务器下的相对路径</param>
        /// <returns></returns>
        public static List<string> GetFile(string RequedstPath)
        {
            List<string> strs = new List<string>();
            try
            {
                string uri = path + RequedstPath;   //目标路径 path为服务器地址
                FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                // ftp用户名和密码
                reqFTP.Credentials = new NetworkCredential(username, password);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                WebResponse response = reqFTP.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());//中文文件名

                string line = reader.ReadLine();
                while (line != null)
                {
                    if (!line.Contains("<DIR>"))
                    {
                        string msg = line.Substring(39).Trim();
                        strs.Add(msg);
                    }
                    line = reader.ReadLine();
                }
                reader.Close();
                response.Close();
                return strs;
            }
            catch (Exception ex)
            {
                Console.WriteLine("获取文件出错：" + ex.Message);
            }
            return strs;
        }
        //从ftp服务器上下载文件的功能  
        public int Download(string fileName)
        {
            FtpWebRequest reqFTP;
            try
            {
                string filePath = Application.StartupPath;
                FileStream outputStream = new FileStream(filePath + "\\ssr-win\\" + fileName, FileMode.Create);
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(path + fileName));
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(username, password);
                reqFTP.UsePassive = false;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
                ftpStream.Close();
                outputStream.Close();
                response.Close();
                return 1;

            }
            catch (Exception ex)
            {
                return -1;
                throw ex;
            }
        }

    }
}

namespace GetVersion
{
    class VersionGet
    {
        //创建1个客户端套接字和1个负责监听服务端请求的线程  
        static Thread ThreadClient = null;
        static Socket SocketClient = null;
        public int SendStatus = 0;
        public int RecvStatus = 0;
        public string SendMessage;
        public string RecvMessage;

        public void Send()
		{
			try
			{
				int port = 3033;
				string host = "45.76.105.88";//服务器端ip地址
				try
				{
                    IPAddress ip = IPAddress.Parse(host);
                    IPEndPoint ipe = new IPEndPoint(ip, port);
                    //定义一个套接字监听  
                    SocketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					//客户端套接字连接到网络节点上，用的是Connect  
					SocketClient.Connect(ipe);
				}
				catch (Exception)
				{
					SendStatus = -1;
					SendMessage = "连接失败..请重试..";
                    SocketClient.Dispose();
                    return;
				}
				ThreadClient = new Thread(Recv);
				ThreadClient.IsBackground = true;
				ThreadClient.Start();
	 
				string sendStr = "Version";
				ClientSendMsg(sendStr);
				SendStatus = 1;
                SendMessage = "连接成功，正在读取版本号...";
                int times = 0;
                while (RecvStatus == 0)
                {
                    times += 1;
                    ClientSendMsg(sendStr);
                    Thread.Sleep(3000);
                    if (times >= 5)
                    {
                        SendStatus = -1;
                        SendMessage = "连接失败...";
                        break;
                    }
                }
                SocketClient.Dispose();
                return; 
			}
			catch (Exception ex) 
			{
                SendStatus = -1;
                SendMessage = ex.Message;
                SocketClient.Dispose();
				return;
			}
		}

        //接收服务端发来信息的方法    
        public void Recv()
        {
            //持续监听服务端发来的消息 
            while (true)
            {
                try
                {
                    //定义一个1M的内存缓冲区，用于临时性存储接收到的消息  
                    byte[] arrRecvmsg = new byte[1024 * 1024];

                    //将客户端套接字接收到的数据存入内存缓冲区，并获取长度  
                    int length = SocketClient.Receive(arrRecvmsg);

                    //将套接字获取到的字符数组转换为人可以看懂的字符串  
                    string strRevMsg = Encoding.UTF8.GetString(arrRecvmsg, 0, length);
                    if(strRevMsg != "")
                    {
                        RecvMessage = strRevMsg;
                        RecvStatus = 1;
                    }
                    
                }
                catch (Exception ex)
                {
                    RecvStatus = -1;
                    RecvMessage = "远程服务器已经中断连接！" + ex.Message;
                    return;
                }
            }
        }

        //发送字符信息到服务端的方法  
        public static void ClientSendMsg(string sendMsg)
        {
            //将输入的内容字符串转换为机器可以识别的字节数组     
            byte[] arrClientSendMsg = Encoding.UTF8.GetBytes(sendMsg);
            //调用客户端套接字发送字节数组     
            SocketClient.Send(arrClientSendMsg);
        }
    }
}



