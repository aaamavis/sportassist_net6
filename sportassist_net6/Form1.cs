using System.Diagnostics;
using System.Runtime.InteropServices;

namespace sportassist_net6
{
    public partial class Form1 : Form
    {
        public string moviepath = "", moviename = ""; // 影片路徑,名稱

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)//按鈕選擇影片檔
        {
            OpenFileDialog dialog = new()
            {
                Title = "請選擇檔案",
                Filter = "mp4 files (*.mp4)|*.mp4|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                moviepath = dialog.FileName;
                moviename = Path.GetFileName(moviepath);
                textBox1.Text = moviepath;
                if (!Directory.Exists(GetFramedFilePath(moviename)))//檢查是否已經生成分析檔 沒有才給按分析按紐
                {
                    button2.Enabled = true;
                }
                else
                {
                    textBox1.Text = "分析檔已存在";
                    button2.Enabled = false;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (moviename == "")
            {
                textBox2.Text = "未選擇檔案";
            }
            else
            {
                string ffmpegpath = GetModuleFilePath("ffmpeg"); // ffmpeg目錄
                string openposepath = GetModuleFilePath("openpose"); // openpose所在資料夾
                string ffmpeg = Path.Combine(ffmpegpath, "ffmpeg.exe"); // ffmpeg檔案位置'
                string framecut = "-i " + moviepath + " " + Environment.CurrentDirectory + @"\Framed\" + moviename + @"\frames_%05d.jpg";// -i 影片.mp4 輸出目錄/檔案名稱.jpg(包含輸入及輸出目錄的切幀參數)
                string toavi = "-i " + moviepath + " " + "-vcodec copy -acodec copy " + Environment.CurrentDirectory + @"\Framed\" + moviename + @"\" + moviename + ".avi";
                string openposemark = @"--video " + Application.StartupPath + @"\Framed\" + moviename + @"\" + moviename + ".avi " + "--write_video " + Application.StartupPath + @"\Framed\" + moviename + @"\" + moviename + "_marked.avi";//輸出標記完影片

                #region 創建輸出資料夾
                textBox2.Text = "創建輸出資料夾";

                string framePath = $"{Environment.CurrentDirectory}\\Framed\\{moviename}";
                // 創建切幀輸出資料夾
                if (!Directory.Exists(framePath))
                    Directory.CreateDirectory(framePath);
                #endregion

                textBox2.Text = "影片格式轉換中";

                var proc_toavi = Process.Start(ffmpeg, toavi);//將影片轉換成avi供openpose工作喵
                proc_toavi.WaitForExit();

                textBox2.Text = "輸出標記影片";

                Console.WriteLine(openposemark);
                //呼叫 cmd 使用 openpose !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                var p = Process.Start(new ProcessStartInfo()
                {
                    FileName = "bin\\OpenPoseDemo.exe",
                    WorkingDirectory = openposepath,//在openpose根目錄運行
                    Arguments = openposemark,//啟動參數
                    UseShellExecute = true,
                    CreateNoWindow = false //跳出cmd視窗
                });
                p.WaitForExit();
                textBox2.Text = "輸出標記影片完成";

                //var proc = System.Diagnostics.Process.Start(ffmpeg, framecut);//執行切幀 filename(ffmpeg.exe)+parmeter(啟動參數)

                //openpose生成json

            }

        }

        public static string GetModuleFilePath(string fileName)
            => $"{AppDomain.CurrentDomain.BaseDirectory}module{GetPlatformSlash()}{fileName}";

        public static string GetFramedFilePath(string fileName)
            => $"{AppDomain.CurrentDomain.BaseDirectory}Framed{GetPlatformSlash()}{fileName}";

        public static string GetPlatformSlash()
            => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "\\" : "/";
    }
}
