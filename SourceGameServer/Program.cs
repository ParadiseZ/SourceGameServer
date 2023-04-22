using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SourceGameServer
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            /*Application.Run(new Form1());*/
            String gamePath = Config.ReadConfig("GamePath");
            String lastServer = Config.ReadConfig("LastLogin");
            if (gamePath == null)
            {
                MessageBox.Show("请先设置路径等信息！必须全部设置！");
                return;
            }
            if (lastServer == "OS")
            {
                String pathtemp = Config.ReadConfig("OSBackupPath");
                String[] gameFiles = Config.ReadConfig("OSFileNames").Split(',');
                String[] directory = Config.ReadConfig("OSDic").Split(',');
                //CN为空才能剪切，首次若不为空则删除对应文件
                foreach (String d in directory)
                {
                    if (!Directory.Exists(d))
                    {
                        Directory.CreateDirectory(pathtemp + "\\" + d);
                    }
                }
                foreach (String f in gameFiles)
                {
                    //遍历游戏文件内容，若游戏目录内存在则移动文件到CN文件夹
                    if (File.Exists(gamePath + "\\" + f))
                    {
                        if (File.Exists(pathtemp + "\\" + f))
                        {
                            File.Delete(pathtemp + "\\" + f);
                        }
                        try
                        {
                            File.Move(gamePath + "\\" + f, pathtemp + "\\" + f);
                        }
                        catch
                        {
                            MessageBox.Show(gamePath + "\\" + f + "移动到" + pathtemp + "\\" + f + "失败！");
                        }

                    }
                    else
                    {
                        MessageBox.Show("最后登录的是非CN服务！配置文件有误！");
                        return;
                    }
                }
                //修改文件名称
                Computer c = new Computer();
                c.FileSystem.RenameDirectory(gamePath + "\\" + Config.ReadConfig("OSDataName"), Config.ReadConfig("CNDataName"));
                gameFiles = Config.ReadConfig("CNFileNames").Split(',');
                pathtemp = Config.ReadConfig("CNBackupPath");
                foreach (String f in gameFiles)
                {
                    //遍历目标文件内容，若游戏目录内存在则移动文件到OS文件夹
                    if (File.Exists(pathtemp + "\\" + f))
                    {
                        File.Move(pathtemp + "\\" + f, gamePath + "\\" + f);
                    }
                }
            }

            RunGame(gamePath + "\\" + Config.ReadConfig("CNExeName"));
            Config.WriteConfig("LastLogin", "CN");
        }

        public static void RunGame(string processName)
        {
            Process myProcess = new Process();
            try
            {
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.FileName = processName;
                myProcess.StartInfo.CreateNoWindow = true;
                myProcess.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show("游戏启动" + processName + "+失败！文件替换已完成！");
                Trace.WriteLine(e.Message);
            }
        }
    }
}
