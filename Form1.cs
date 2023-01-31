using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Sunny.UI;

namespace DeleteRedundantFiles
{
    public partial class Form1 : UIForm
    {
        DataTable FolderDataTable = new DataTable();
        private List<string> filePath = new List<string>();
        private List<string> listTemp = new List<string>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DataColumn dc1 = new DataColumn("属性", Type.GetType("System.String"));
            DataColumn dc2 = new DataColumn("路径", Type.GetType("System.String"));
            FolderDataTable.Columns.Add(dc1);
            FolderDataTable.Columns.Add(dc2);
        }

        private void SelectFiles_Click(object sender, EventArgs e)
        {
            //获取选择目录
            FolderBrowserDialog dir = new FolderBrowserDialog();
            dir.ShowDialog();
            List<string> Path = new List<string>
            {
                dir.SelectedPath
            };
            GetFileLIstToDataGridView(Path, false);
        }

        /// <summary>
        /// 遍历文件夹获取目录
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="list"></param>
        public void Director(string dir, List<string> list)
        {
            try
            {
                DirectoryInfo d = new DirectoryInfo(dir);
                DirectoryInfo[] directs = d.GetDirectories();
                foreach (DirectoryInfo f in directs)
                {
                    list.Add(f.FullName);//添加文件名到列表中  
                }
                //获取子文件夹内的文件列表，递归遍历  
                foreach (DirectoryInfo dd in directs)
                {
                    Director(dd.FullName, list);
                }
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// 更新需要转换的list列表，以及DataGridView显示
        /// </summary>
        /// <param name="filePaths">需要重新操作的list集合</param>
        /// <param name="IsRemove">判断是否进行了删除操作</param>
        public void GetFileLIstToDataGridView(List<string> filePaths, bool IsRemove)
        {
            //判断是否有删除操作，如果是进行的删除，那么list列表数据等于DataGridView的数据，否则对现有list列表数据进行添加并去重
            if (IsRemove)
            {
                filePath = filePaths;
            }
            else
            {
                foreach (string item in filePaths)
                {
                    filePath.Add(item);
                }
            }
            filePath = RemoveDuplicate(filePath);

            //判断list数据更新后的数量，对界面进行选更新

            if (filePath.Count == 0) return;
            uiDataGridView2.DataSource = filePath.Select(x => new { Value = x }).ToList();
        }

        /// <summary>
        /// 利用哈希进行list去重
        /// </summary>
        /// <param name="list">需要去重的list</param>
        /// <returns></returns>
        public static List<String> RemoveDuplicate(List<String> list)
        {
            HashSet<String> h = new HashSet<String>(list);
            list.Clear();
            list = new List<string>(h);
            return list;
        }



        /// <summary>
        /// 判断当前目录下是否还有冗余的目录
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool GetNoRedundantFiles(string path)
        {
            if (Directory.GetDirectories(path).Count() == 1 && Directory.GetFiles(path).Count() == 0)
            {
                return false;
            }
            else if (Directory.GetDirectories(path).Count() == 0 && Directory.GetFiles(path).Count() == 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        /// <summary>
        /// 递归删除所有空文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void DeletDirectory(string path)
        {
            if (System.IO.Directory.Exists(path))
            {
                if (Directory.GetDirectories(path).Count() == 0 && Directory.GetFiles(path).Count() == 0)
                {
                    string ParentPath = Directory.GetParent(path).ToString();
                    Directory.Delete(path, true);
                    DeletDirectory(ParentPath);
                }
            }
        }

        /// <summary>
        /// 删除重冗余文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <param name="MoveFIle"></param>
        /// <param name="FileName"></param>
        public void DeleteRedundantFiles(string path, string MoveFIle, string FileName)
        {
            try
            {
                if (System.IO.Directory.Exists(path)) //path就是需要移动的文件夹
                {
                    string b = Directory.GetParent(path).ToString();  //获取父目录进行判断
                    string c = Directory.GetParent(b).ToString(); //获取父目录的父目录
                    //判断父目录的父目录是否为冗余目录，如果是，进行递归判断，知道不为冗余目录为止
                    if (Directory.GetDirectories(c).Count() == 1 && Directory.GetFiles(c).Count() == 0)
                    {
                        DeleteRedundantFiles(b, MoveFIle, FileName);
                    }
                    string e = c + "\\" + FileName;
                    if (System.IO.Directory.Exists(e))
                    {
                        string NewFiles = c + "\\" + FileName + "_去冗余目录";
                        Directory.Move(MoveFIle, NewFiles);
                        return;
                    }
                    else
                    {
                        string NewFiles = e;
                        Directory.Move(MoveFIle, NewFiles);
                        return;
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        private void Form1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                List<string> filePaths = new List<string>((string[])e.Data.GetData(DataFormats.FileDrop));
                GetFileLIstToDataGridView(filePaths, false);
            }
        }

        private void uiButton3_Click(object sender, EventArgs e)
        {
            filePath = null;
            uiDataGridView2.DataSource = null;
        }

        private void DeleteFiles_Click(object sender, EventArgs e)
        {
            int k = uiDataGridView2.SelectedRows.Count;
            if (k == 0) return;
            if (MessageBox.Show("您确认要删除这" + Convert.ToString(k) + "项吗？", "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)//给出提示
            {

            }
            else
            {
                for (int i = k; i >= 1; i--)//从下往上删，避免沙漏效应
                {
                    Int32 rowToDelete = uiDataGridView2.Rows.GetFirstRow(DataGridViewElementStates.Selected);
                    filePath.RemoveAt(rowToDelete);
                }
                uiDataGridView2.DataSource = filePath.Select(x => new { Value = x }).ToList();
            }
        }

        private void SelectList_Click(object sender, EventArgs e)
        {
            if (filePath.Count() == 0)
            {
                uiDataGridView1.DataSource = null;
                ShowWarningTip("待检索目录为空，请重新选择！");
                return;
            }

            listTemp.Clear();
            FolderDataTable.Clear();
            uiDataGridView1.DataSource = null;
            foreach (var item in filePath)
            {
                Director(item, listTemp);
            }
            foreach (var item in listTemp)
            {
                if (item.Contains("C:\\")) return;
                if (Directory.GetDirectories(item).Count() == 0 && Directory.GetFiles(item).Count() == 0)
                {
                    if (uiCheckBox2.Checked)
                    {
                        DataRow dr = FolderDataTable.NewRow();
                        dr["路径"] = item;
                        dr["属性"] = "空文件夹";
                        FolderDataTable.Rows.Add(dr);
                    }
                }
                else if (Directory.GetDirectories(item).Count() == 1 && Directory.GetFiles(item).Count() == 0)
                {
                    if (uiCheckBox1.Checked)
                    {
                        string GetDirectories = Directory.GetDirectories(item)[0];
                        if (GetNoRedundantFiles(GetDirectories))
                        {
                            DataRow dr = FolderDataTable.NewRow();
                            dr["路径"] = GetDirectories;
                            dr["属性"] = "冗余目录";
                            FolderDataTable.Rows.Add(dr);
                        }
                    }
                }
            }
            uiDataGridView1.DataSource = FolderDataTable;
            ShowSuccessTip("检索完成，请仔细核对列表后进行后续操作！");
        }

        private void DeleteList_Click(object sender, EventArgs e)
        {
            int k = uiDataGridView1.SelectedRows.Count;
            if (k == 0) return;

            if (MessageBox.Show("您确认要删除这" + Convert.ToString(k) + "项吗？", "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)//给出提示
            {

            }
            else
            {
                for (int i = k; i >= 1; i--)//从下往上删，避免沙漏效应
                {
                    Int32 rowToDelete = uiDataGridView1.Rows.GetFirstRow(DataGridViewElementStates.Selected);
                    FolderDataTable.Rows.RemoveAt(rowToDelete);
                }
                uiDataGridView1.DataSource = null;
                uiDataGridView1.DataSource = FolderDataTable;
            }
        }

        private void Start_Click(object sender, EventArgs e)
        {
            if (FolderDataTable.Rows.Count == 0) 
            {
                ShowWarningTip("待整理列表为空，请选择目录检索后在进行此操作！");
                return;
            }

            if (MessageBox.Show("您确认要操作这些文件吗？该操作不可逆！！！", "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)//给出提示
            {

            }
            else
            {
                foreach (DataRow item in FolderDataTable.Rows)
                {
                    if (Convert.ToString(item["属性"]) == "空文件夹")
                    {
                        DeletDirectory(Convert.ToString(item["路径"]));
                    }
                    if (Convert.ToString(item["属性"]) == "冗余目录")
                    {
                        string FileName = new DirectoryInfo(Convert.ToString(item["路径"])).Name;
                        DeleteRedundantFiles(Convert.ToString(item["路径"]), Convert.ToString(item["路径"]), FileName);
                        DeletDirectory(Directory.GetParent(Convert.ToString(item["路径"])).ToString());
                    }
                }
                SelectList_Click(sender, e);
                ShowSuccessTip("整理完成，请检查是否有误删等情况。");
            }

        }

        private void uiDataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string path = uiDataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                Process.Start(path);
            }
        }

        private void DeleteAll_Click(object sender, EventArgs e)
        {
            filePath.Clear();
            uiDataGridView2.DataSource = null;
        }


        private void About_Click(object sender, EventArgs e)
        {
            ShowWarningDialog("操作说明：\r\n" +
                "1、软件支持按钮选择和拖拽文件目录到软件进行添加。\r\n" +
                "2、检索后的文件列表，可以双击打开，查看对于的文件情况。\r\n" +
                "软件声明：\r\n"+
                "1、为防止系统出错，软件不会操作C盘下的文件！\r\n" +
                "2、文件操作不可逆，请谨慎使用！\r\n" +
                "3、软件操作逻辑如下：\r\n" +
                "①：找到空文件夹后，会继续查找父目录是否为空，如果为空，会一直循环删除。\r\n" +
                "②：找到冗余文件夹后，会继续查找父目录是否为冗余文件，如果是，会一直循环，直到不为冗余文件为止，然后将该文件夹移动到其目录下。\r\n" +
                "③：移动文件到目标位置如果有相同文件夹，会在移动文件夹名称后增加“_去冗余目录。”\r\n" +
                "④：执行冗余文件移动后，其父目录为空，会按照空文件夹逻辑进行删除。\r\n" +
                "4、该软件为个人兴趣开发，未做过多测试，请谨慎使用，如在使用中导致文件误删，本人不负法律责任！！！" +
                "5、什么是冗余文件:就是一个文件夹下只有一个文件夹，如此嵌套。除特殊软件需要外，该文件夹分类无特殊意义。");
        }
    }
}
