using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication4
{
    public partial class Form1 : Form
    {
        string[] path = { "data1.txt", "data2.txt", "data3.txt", "data4.txt" };   //声明数据源
        byte[][] b = new byte[4][] { new byte[886], new byte[886], new byte[886], new byte[886] };
        string[][] strarray1 = new string[4][] { new string[886], new string[886], new string[886], new string[886] };
        public Form1()
        {
            InitializeComponent();
            //设置列头不可选
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            foreach (DataGridViewColumn column in dataGridView2.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        //设置“捕获数据”按钮的点击事件
        private void button1_Click(object sender, EventArgs e)
        {
            FileStream[] afile = new FileStream[4];
            for (int i = 0; i < 4; i++)
            {
                afile[i] = new FileStream(path[i], FileMode.Open);  //打开文件
                afile[i].Read(b[i], 0, b[i].Length);                //将文件存入交叉数组b中
                afile[i].Close();                                   //关闭流文件，释放资源
            }

            for (int i = 0; i < 4; i++)
            {
                //将数据按16进制存入交叉数组staraary1中
                for (int j = 0; j < 886; j++)
                {
                    strarray1[i][j] = b[i][j].ToString("X2");
                }
                //将数据按字节存入datagridview1中      
                int index1 = dataGridView1.Rows.Add();
                for (int j = 0; j < 13; j++)
                {
                    dataGridView1.Rows[index1].Cells[j].Value = strarray1[index1][j];
                }
            }

            string[] datastr = new string[4];
            string[][] strarray2 = new string[4][] { new string[6], new string[6], new string[6], new string[6] };
            for (int i = 0; i < 4; i++)
            {
                //根据数据帧结构将某一区间数据转化成二进制
                datastr[i] = Convert.ToString(b[i][8], 2).PadLeft(8, '0') + Convert.ToString(b[i][9], 2).PadLeft(8, '0');
                //初始化交叉数组staarray2用于存储按帧结构划分的数据
                strarray2[i][0] = strarray1[i][0] + strarray1[i][1] + strarray1[i][2] + strarray1[i][3];
                strarray2[i][1] = strarray1[i][4] + strarray1[i][5] + strarray1[i][6] + strarray1[i][7];
                strarray2[i][2] = datastr[i].Substring(0, 2);
                strarray2[i][3] = datastr[i].Substring(2, 8);
                strarray2[i][4] = datastr[i].Substring(10, 6);
                strarray2[i][5] = strarray1[i][10] + strarray1[i][11] + strarray1[i][12];
            }

            //将数据按帧结构存入datagridview2中
            for (int i = 0; i < 4; i++)
            {
                int index2 = dataGridView2.Rows.Add();
                for (int j = 0; j < 6; j++)
                {
                    dataGridView2.Rows[index2].Cells[j].Value = strarray2[index2][j];
                }
            }
            button1.Enabled = false;
        }
        //自定义函数GetOutput得出richtextbox1中的output
        private string GetOutput(int RowIndex)
        {
            string output = "";
            int j = 0;
            switch (RowIndex)
            {
                case 0:
                    for (int i = 0; i < 886; i++)
                    {
                        j = j + 1;
                        if (j % 16 == 0)
                            output += strarray1[0][i] + "\n";
                        else
                            output += strarray1[0][i] + " ";
                    }
                    break;
                case 1:
                    for (int i = 0; i < 886; i++)
                    {
                        j = j + 1;
                        if (j % 16 == 0)
                            output += strarray1[1][i] + "\n";
                        else
                            output += strarray1[1][i] + " ";
                    }
                    break;
                case 2:
                    for (int i = 0; i < 886; i++)
                    {
                        j = j + 1;
                        if (j % 16 == 0)
                            output += strarray1[2][i] + "\n";
                        else
                            output += strarray1[2][i] + " ";
                    }
                    break;
                case 3:
                    for (int i = 0; i < 886; i++)
                    {
                        j = j + 1;
                        if (j % 16 == 0)
                            output += strarray1[3][i] + "\n";
                        else
                            output += strarray1[3][i] + " ";
                    }
                    break;
            }
            return output;
        }

        //自定义函数将字符串转化为int32输出
        private int converttoint(string s)
        {
            int i = Convert.ToInt32(s);
            return i;
        }

        //设置CellClick事件将数据输入到richtextbox1里并用红色字体显示错误信息
        private void Dispaly_RichTextBox(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            if (i >= 0)
            {
                richTextBox1.Text = "Data Hex_Output: (Length = 886 Bytes)" + "\n";
                richTextBox1.Text = richTextBox1.Text + GetOutput(i);
                //检测表格中中同步头是否出错
                string line = richTextBox1.Lines[1];
                string[] strarray = line.Split(' ');
                string s1 = strarray[0] + strarray[1] + strarray[2] + strarray[3];
                if (s1 != "1ACFFC1D")
                {
                    richTextBox1.Select(38, 11);
                    richTextBox1.SelectionColor = Color.Red;
                }
                //检测数据帧是否不连续
                string s2 = (string)dataGridView2.Rows[i].Cells[5].Value;
                int x = converttoint(s2);
                //判断表格第一行中对应的计数器的计数值，不为0则用红色字体标出
                if (i == 0)
                {
                    if (x != 0)
                    {
                        richTextBox1.Select(68, 8);
                        richTextBox1.SelectionColor = Color.Red;
                    }
                }
                //判断行之间计数器的计数值是否为连续整数，不是则用红色字体标出
                else if (i > 0)
                {
                    string s3 = (string)dataGridView2.Rows[i - 1].Cells[5].Value;
                    int y = converttoint(s3);
                    if (x != y + 1)
                    {
                        richTextBox1.Select(68, 8);
                        richTextBox1.SelectionColor = Color.Red;
                    }
                }
            }
        }

        //设置“清空列表”按钮的点击事件
        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            this.dataGridView1.Rows.Clear();
            this.dataGridView2.Rows.Clear();
            richTextBox1.Text = string.Empty;

        }

        //设置MouseClick事件用于点击richtextbox即可在表中显示位置所示处的数据内容
        private void SelectData(object sender, MouseEventArgs e)
        {
            //清除两表格中所有cell的选定状态
            dataGridView1.ClearSelection();
            dataGridView2.ClearSelection();
            //获取光标的索引值
            int i = richTextBox1.SelectionStart;
            //设置光标位于数据帧的前13个字节内容时，点击事件才发生
            if (i > 38 && i <= 76)
            {
                richTextBox1.Select(38, i - 38);                //选中数据头至光标位置的数据
                string starter = richTextBox1.SelectedText;
                string median = starter.Replace(" ", "");       //去掉starter中的空格,得到选中文本的连续字符串形式               
                int j = median.Length;
                //为了同步两个datagridview的行显示，设置条件为俩表格“当前行”索引值相等
                if (dataGridView1.CurrentRow.Index == dataGridView2.CurrentRow.Index)
                {
                    int equal = dataGridView1.CurrentRow.Index;
                    //将richtextbox中选中内容显示在datagridview1中
                    if (j % 2 == 0)
                    {
                        for (int k = 0; k < j / 2; k++)
                        {
                            dataGridView1.Rows[equal].Cells[k].Selected = true;
                            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.Aqua;
                        }
                    }
                    if (j % 2 != 0)
                    {
                        for (int k = 0; k < (j + 1) / 2; k++)
                        {
                            dataGridView1.Rows[equal].Cells[k].Selected = true;
                            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.Aqua;
                        }
                    }
                    //同步将richtextbox中选中内容显示在datagridview2中
                    if (j <= 8)
                    {
                        dataGridView2.Rows[equal].Cells[0].Selected = true;
                        dataGridView2.DefaultCellStyle.SelectionBackColor = Color.Aqua;
                    }
                    if (j > 8 && j <= 16)
                    {
                        for (int k = 0; k < 2; k++)
                        {
                            dataGridView2.Rows[equal].Cells[k].Selected = true;
                            dataGridView2.DefaultCellStyle.SelectionBackColor = Color.Aqua;
                        }
                    }
                    if (j > 16 && j <= 18)
                    {
                        for (int k = 0; k < 4; k++)
                        {
                            dataGridView2.Rows[equal].Cells[k].Selected = true;
                            dataGridView2.DefaultCellStyle.SelectionBackColor = Color.Aqua;
                        }
                    }
                    if (j > 18 && j <= 20)
                    {
                        for (int k = 0; k < 5; k++)
                        {
                            dataGridView2.Rows[equal].Cells[k].Selected = true;
                            dataGridView2.DefaultCellStyle.SelectionBackColor = Color.Aqua;
                        }
                    }
                    if (j > 20 && j <= 26)
                    {
                        for (int k = 0; k < 6; k++)
                        {
                            dataGridView2.Rows[equal].Cells[k].Selected = true;
                            dataGridView2.DefaultCellStyle.SelectionBackColor = Color.Aqua;
                        }
                    }
                }
                //当两表格当前行索引值不相等时，提示用户选相同行
                else
                {
                    MessageBox.Show("请点击两表格的相同行(如先点击了datagridview1的第一行，应再点击下datagridview2的第一行，或先点击了datagridview2的第一行，则应再点击下datagridview1的第一行)", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }
    }
}