using System.Diagnostics;
using System.Runtime.InteropServices;

namespace sportassist_net6
{
    public partial class Form1 : Form
    {
        public string moviepath = "", moviename = ""; // �v�����|,�W��

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)//���s��ܼv����
        {
            OpenFileDialog dialog = new()
            {
                Title = "�п���ɮ�",
                Filter = "mp4 files (*.mp4)|*.mp4|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                moviepath = dialog.FileName;
                moviename = Path.GetFileName(moviepath);
                textBox1.Text = moviepath;
                if (!Directory.Exists(GetFramedFilePath(moviename)))//�ˬd�O�_�w�g�ͦ����R�� �S���~�������R����
                {
                    button2.Enabled = true;
                }
                else
                {
                    textBox1.Text = "���R�ɤw�s�b";
                    button2.Enabled = false;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (moviename == "")
            {
                textBox2.Text = "������ɮ�";
            }
            else
            {
                string ffmpegpath = GetModuleFilePath("ffmpeg"); // ffmpeg�ؿ�
                string openposepath = GetModuleFilePath("openpose"); // openpose�Ҧb��Ƨ�
                string ffmpeg = Path.Combine(ffmpegpath, "ffmpeg.exe"); // ffmpeg�ɮצ�m'
                string framecut = "-i " + moviepath + " " + Environment.CurrentDirectory + @"\Framed\" + moviename + @"\frames_%05d.jpg";// -i �v��.mp4 ��X�ؿ�/�ɮצW��.jpg(�]�t��J�ο�X�ؿ������V�Ѽ�)
                string toavi = "-i " + moviepath + " " + "-vcodec copy -acodec copy " + Environment.CurrentDirectory + @"\Framed\" + moviename + @"\" + moviename + ".avi";
                string openposemark = @"--video " + Application.StartupPath + @"\Framed\" + moviename + @"\" + moviename + ".avi " + "--write_video " + Application.StartupPath + @"\Framed\" + moviename + @"\" + moviename + "_marked.avi";//��X�аO���v��

                #region �Ыؿ�X��Ƨ�
                textBox2.Text = "�Ыؿ�X��Ƨ�";

                string framePath = $"{Environment.CurrentDirectory}\\Framed\\{moviename}";
                // �Ыؤ��V��X��Ƨ�
                if (!Directory.Exists(framePath))
                    Directory.CreateDirectory(framePath);
                #endregion

                textBox2.Text = "�v���榡�ഫ��";

                var proc_toavi = Process.Start(ffmpeg, toavi);//�N�v���ഫ��avi��openpose�u�@�p
                proc_toavi.WaitForExit();

                textBox2.Text = "��X�аO�v��";

                Console.WriteLine(openposemark);
                //�I�s cmd �ϥ� openpose !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                var p = Process.Start(new ProcessStartInfo()
                {
                    FileName = "bin\\OpenPoseDemo.exe",
                    WorkingDirectory = openposepath,//�bopenpose�ڥؿ��B��
                    Arguments = openposemark,//�ҰʰѼ�
                    UseShellExecute = true,
                    CreateNoWindow = false //���Xcmd����
                });
                p.WaitForExit();
                textBox2.Text = "��X�аO�v������";

                //var proc = System.Diagnostics.Process.Start(ffmpeg, framecut);//������V filename(ffmpeg.exe)+parmeter(�ҰʰѼ�)

                //openpose�ͦ�json

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
