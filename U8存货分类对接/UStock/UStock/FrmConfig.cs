using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace UStock
{
    public partial class FrmConfig : Form
    {

        //select '' as cInvCName,1 as cInvCCode into InventoryClass
        //select '' as cInvCName,1 as cInvCode,11 as cInvCCode into Inventory
        //select '' as AutoID,1 as cInvCode,'' as cDefine1,GETDATE() as dCreateDate into IA_Subsidiary


        public string strConn = ConfigurationManager.ConnectionStrings["dbu8"].ConnectionString+ "Initial Catalog=master;";//"Data Source=.;Initial Catalog=master;Integrated Security=True";//
        //public string strConn = "Data Source=.;Initial Catalog=master;Persist Security Info=True;User ID=sa;Password=1";
        public FrmConfig()
        {
            InitializeComponent();
            this.Text += "(2017版)";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                dt = ReturnDataTable("select Name from sys.databases where database_id > 4 ORDER BY Name");
                if(dt==null)
                {
                    dt.Columns.Add("Name", typeof(string));
                    dt.Rows.Add(ConfigurationManager.AppSettings["dbname"]);
                }
                else
                {
                    if(dt.Rows.Count==0)
                    {
                        dt.Columns.Add("Name", typeof(string));
                        dt.Rows.Add(ConfigurationManager.AppSettings["dbname"]);
                    }
                }
                
                comboBox1.DataSource = dt;
                comboBox1.DisplayMember = "Name";//这是text值
                comboBox1.ValueMember = "Name";//这是value值
                groupBox1.Enabled = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show("信息出错，请退出重新打开！\r\n" + ex.Message.ToString(), "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            groupBox1.Enabled = false;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 执行SqlServer下的SQL语句并返回DataTable
        /// </summary>
        /// <param name="strSql">SqlServer下的SQL语句</param>
        /// <returns>DataTable</returns>
        public DataTable ReturnDataTable(string strSql)
        {
            DataTable datatable = new DataTable();
            SqlConnection odconn = new SqlConnection(strConn);
            try
            {
                odconn.Open();
                SqlDataAdapter SqlDbDA = new SqlDataAdapter(strSql, odconn);
                SqlDbDA.Fill(datatable);
                odconn.Close();
            }
            catch(Exception ex)
            {
                odconn.Close();
                //MessageBox.Show(ex.Message);
            }

            return datatable;
        }
        /// <summary>
        /// 执行SqlServer下的SQL语句组[用于insert、delete、update]
        /// </summary>
        /// <param name="strSql">多条SqlServer下的SQL语句</param>
        /// <returns>成功true,失败false</returns>
        public bool ExecuteNonQuery(string[] strSql)
        {
            SqlConnection conn = new SqlConnection(strConn);
            conn.Open();
            using (SqlTransaction trans = conn.BeginTransaction())
            {
                SqlCommand cmdStr = new SqlCommand();
                cmdStr.Connection = conn;
                cmdStr.Transaction = trans;//指定事务
                try
                {
                    for (int i = 0; i < strSql.Length; i++)
                    {
                        if (strSql[i] != null)
                        {
                            if (strSql[i].Trim().Length != 0)
                            {
                                cmdStr.CommandText = strSql[i];
                                cmdStr.ExecuteNonQuery();
                            }
                        }
                    }

                }
                catch
                {
                    trans.Rollback();//插入失败则回滚操作
                    conn.Close();
                    return false;

                }
                trans.Commit();//提交事务
            }
            conn.Close();
            return true;

        }
        /// <summary>
        /// 执行SqlServer下的SQL语句并返回受影响的行数[用于insert、delete、update]
        /// </summary>
        /// <param name="strSql">SqlServer下的SQL语句</param>
        /// <returns>影响行数，0为未影响或执行失败</returns>
        public int ExeSql(string strSql)
        {
            int iReturn = 0;
            SqlConnection conn = new SqlConnection(strConn);
            conn.Open();
            using (SqlTransaction trans = conn.BeginTransaction())
            {
                SqlCommand cmdStr = new SqlCommand();
                cmdStr.Connection = conn;
                cmdStr.Transaction = trans;//指定事务
                try
                {
                    cmdStr.CommandText = strSql;
                    iReturn = cmdStr.ExecuteNonQuery();
                    trans.Commit();//提交事务
                    conn.Close();
                }
                catch
                {
                    trans.Rollback();//插入失败则回滚操作
                    conn.Close();
                    iReturn = 0;
                }

            }

            return iReturn;

        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            try
            {
                if (comboBox1.Text != "System.Data.DataRowView")
                {
                    string strSqltr = "";
                    strSqltr = "SELECT name FROM " + comboBox1.Text + ".dbo.sysobjects where type='TR' and (name='TRIGGERDEF1_UPDATE' or name='TRIGGERDEF1_INSERT')";
                    DataTable dt = new DataTable();
                    dt = ReturnDataTable(strSqltr);
                    if (dt.Rows.Count == 2)
                    {
                        label3.Text = "存货分类功能已添加";
                        label3.ForeColor = Color.Blue;
                        button3.Enabled = true;
                    }
                    else
                    {
                        label3.Text = "存货分类功能未添加";
                        label3.ForeColor = Color.Black;
                        button3.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("信息出错，请退出重新打开！\r\n"+ex.Message.ToString(),"系统提示",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            if (label3.Text == "存货分类功能已添加")
            {
                MessageBox.Show("存货分类功能已添加，不能再次添加！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                try
                {
                    //strConn = "Data Source=.;Initial Catalog=" + comboBox1.Text + ";Integrated Security=True";
                    //strConn = "Data Source=.;Initial Catalog=" + comboBox1.Text + ";Persist Security Info=True;User ID=sa;Password=1";
                    strConn = ConfigurationManager.ConnectionStrings["dbu8"].ConnectionString + "Initial Catalog="+ comboBox1.Text + ";";
                    StringBuilder strBSql = new StringBuilder();
                    string strSqltr = "";
                    DataTable dt = new DataTable();
                    strSqltr = "SELECT name FROM " + comboBox1.Text + ".dbo.sysobjects where type='TR' and name='DEF1_Insert' ";
                    dt = ReturnDataTable(strSqltr);
                    if (dt.Rows.Count == 1)
                    {
                        strBSql = new StringBuilder();
                        //存在，先删除
                        strBSql.Append("DROP TRIGGER [DEF1_Insert]; ");
                        ExeSql(strBSql.ToString());
                    }
                    strSqltr = "SELECT name FROM " + comboBox1.Text + ".dbo.sysobjects where type='TR' and name='DEF1_Update' ";
                    dt = ReturnDataTable(strSqltr);
                    if (dt.Rows.Count == 1)
                    {
                        strBSql = new StringBuilder();
                        //存在，先删除
                        strBSql.Append("DROP TRIGGER [DEF1_Update]; ");
                        ExeSql(strBSql.ToString());
                    }

                    strSqltr = "SELECT name FROM " + comboBox1.Text + ".dbo.sysobjects where type='TR' and name='TRIGGERDEF1_UPDATE' ";
                    dt = ReturnDataTable(strSqltr);
                    if (dt.Rows.Count == 1)
                    {
                        strBSql = new StringBuilder();
                        //存在，先删除
                        strBSql.Append("DROP TRIGGER [TRIGGERDEF1_UPDATE]; ");
                        ExeSql(strBSql.ToString());
                    }
                    strSqltr = "SELECT name FROM " + comboBox1.Text + ".dbo.sysobjects where type='TR' and name='TRIGGERDEF1_INSERT' ";
                    dt = ReturnDataTable(strSqltr);
                    if (dt.Rows.Count == 1)
                    {
                        strBSql = new StringBuilder();
                        //存在，先删除
                        strBSql.Append("DROP TRIGGER [TRIGGERDEF1_INSERT]; ");
                        ExeSql(strBSql.ToString());
                    }
                    strBSql = new StringBuilder();
                    strBSql.Append("CREATE TRIGGER [dbo].[TRIGGERDEF1_INSERT] \n");
                    strBSql.Append("   ON  [dbo].[IA_Subsidiary] \n");
                    strBSql.Append("   AFTER INSERT \n");
                    strBSql.Append("AS  \n");
                    strBSql.Append("BEGIN  \n");
                    strBSql.Append("    Declare @cInvCode varchar(500)   \n");
                    strBSql.Append("    Declare @cDefine1 varchar(500)   \n");
                    strBSql.Append("    select @cInvCode=cInvCode,@cDefine1=cDefine1 From inserted  \n");
                    strBSql.Append("    IF @cInvCode IS NOT NULL  \n");
                    strBSql.Append("    BEGIN  \n");
                    strBSql.Append("		UPDATE  IA_Subsidiary SET cDefine1=BB.CName  \n");
                    strBSql.Append("		FROM IA_Subsidiary AA,  \n");
                    strBSql.Append("		(SELECT * FROM  \n");
                    strBSql.Append("		(  \n");
                    strBSql.Append("		select AutoID,cInvCode,  \n");
                    strBSql.Append("			(SELECT (SELECT cInvCName FROM InventoryClass B   \n");
                    strBSql.Append("					 WHERE B.cInvCCode=LEFT(A.cInvCCode,7))   \n");
                    strBSql.Append("			 FROM Inventory A WHERE A.cInvCode=C.cInvCode) AS CName  \n");
                    strBSql.Append("		from IA_Subsidiary C   \n");
                    strBSql.Append("		where dCreateDate>=cast(CONVERT(varchar,GETDATE(),111) as datetime)   \n");
                    strBSql.Append("		and (cInvCode is not null or len(cInvCode)<>0)   \n");
                    strBSql.Append("		and (cDefine1 is null or len(cDefine1)=0)  \n");
                    strBSql.Append("		) D) BB  \n");
                    strBSql.Append("		WHERE AA.AutoID=BB.AutoID  \n");
                    strBSql.Append("    END  \n");
                    strBSql.Append("END  \n");
                    ExeSql(strBSql.ToString());

                    strBSql = new StringBuilder();
                    strBSql.Append("CREATE TRIGGER [dbo].[TRIGGERDEF1_UPDATE] \n");
                    strBSql.Append("   ON  [dbo].[IA_Subsidiary] \n");
                    strBSql.Append("   AFTER UPDATE \n");
                    strBSql.Append("AS  \n");
                    strBSql.Append("BEGIN  \n");
                    strBSql.Append("    Declare @cInvCode varchar(500)   \n");
                    strBSql.Append("    Declare @cDefine1 varchar(500)   \n");
                    strBSql.Append("    select @cInvCode=cInvCode,@cDefine1=cDefine1 From inserted  \n");
                    strBSql.Append("    IF @cInvCode IS NOT NULL  \n");
                    strBSql.Append("    BEGIN  \n");
                    strBSql.Append("		UPDATE  IA_Subsidiary SET cDefine1=BB.CName  \n");
                    strBSql.Append("		FROM IA_Subsidiary AA,  \n");
                    strBSql.Append("		(SELECT * FROM  \n");
                    strBSql.Append("		(  \n");
                    strBSql.Append("		select AutoID,cInvCode,  \n");
                    strBSql.Append("			(SELECT (SELECT cInvCName FROM InventoryClass B   \n");
                    strBSql.Append("					 WHERE B.cInvCCode=LEFT(A.cInvCCode,7))   \n");
                    strBSql.Append("			 FROM Inventory A WHERE A.cInvCode=C.cInvCode) AS CName  \n");
                    strBSql.Append("		from IA_Subsidiary C   \n");
                    strBSql.Append("		where dCreateDate>=cast(CONVERT(varchar,GETDATE(),111) as datetime)   \n");
                    strBSql.Append("		and (cInvCode is not null or len(cInvCode)<>0)   \n");
                    strBSql.Append("		and (cDefine1 is null or len(cDefine1)=0)  \n");
                    strBSql.Append("		) D) BB  \n");
                    strBSql.Append("		WHERE AA.AutoID=BB.AutoID  \n");
                    strBSql.Append("    END  \n");
                    strBSql.Append("END  \n");
                    ExeSql(strBSql.ToString());

                    string strSql = "SELECT name FROM " + comboBox1.Text + ".dbo.sysobjects where type='TR' and (name='TRIGGERDEF1_UPDATE' or name='TRIGGERDEF1_INSERT')";
                    DataTable dta = new DataTable();
                    dta = ReturnDataTable(strSql);
                    if (dta.Rows.Count == 2)
                    {
                        label3.Text = "存货分类功能已添加";
                        label3.ForeColor = Color.Blue;
                        button3.Enabled = true;
                        MessageBox.Show("存货分类功能添加成功！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        label3.Text = "存货分类功能未添加";
                        label3.ForeColor = Color.Black;
                        button3.Enabled = false;
                        MessageBox.Show("存货分类功能添加失败，请尝试重新操作！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("信息出错，系统退出后请重新打开！\r\n" + ex.Message.ToString(), "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.Close();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                strConn = "Data Source=.;Initial Catalog=" + comboBox1.Text + ";Integrated Security=True";
                //strConn = "Data Source=.;Initial Catalog=" + comboBox1.Text + ";Persist Security Info=True;User ID=sa;Password=1";

                StringBuilder strBSql = new StringBuilder();
                string strSqltr = "";
                DataTable dt = new DataTable();
                strSqltr = "SELECT name FROM " + comboBox1.Text + ".dbo.sysobjects where type='TR' and name='TRIGGERDEF1_UPDATE' ";
                dt = ReturnDataTable(strSqltr);
                if (dt.Rows.Count == 1)
                {
                    //存在，先删除
                    strBSql = new StringBuilder();
                    strBSql.Append("DROP TRIGGER [TRIGGERDEF1_UPDATE]; ");
                    ExeSql(strBSql.ToString());
                }
                strSqltr = "SELECT name FROM " + comboBox1.Text + ".dbo.sysobjects where type='TR' and name='TRIGGERDEF1_INSERT' ";
                dt = ReturnDataTable(strSqltr);
                if (dt.Rows.Count == 1)
                {
                    //存在，先删除
                    strBSql = new StringBuilder();
                    strBSql.Append("DROP TRIGGER [TRIGGERDEF1_INSERT]; ");
                    ExeSql(strBSql.ToString());
                }
                label3.Text = "存货分类功能未添加";
                label3.ForeColor = Color.Black;
                MessageBox.Show("存货分类功能删除成功！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("信息出错，系统退出后请重新打开！\r\n" + ex.Message.ToString(), "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.Close();
            }
        }
    }
}
