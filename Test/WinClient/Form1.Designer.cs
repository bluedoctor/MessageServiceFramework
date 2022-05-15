namespace WinClient
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.txtA = new System.Windows.Forms.TextBox();
            this.txtB = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblResult = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnSub = new System.Windows.Forms.Button();
            this.btnVoid = new System.Windows.Forms.Button();
            this.btnServerTime = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.txtSerivceUri = new System.Windows.Forms.TextBox();
            this.btnStartServer = new System.Windows.Forms.Button();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.btnAlarmClock = new System.Windows.Forms.Button();
            this.btnServerText = new System.Windows.Forms.Button();
            this.btnParallel = new System.Windows.Forms.Button();
            this.txtBlock = new System.Windows.Forms.TextBox();
            this.btnReqServerTime = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtA
            // 
            this.txtA.Location = new System.Drawing.Point(52, 104);
            this.txtA.Margin = new System.Windows.Forms.Padding(4);
            this.txtA.Name = "txtA";
            this.txtA.Size = new System.Drawing.Size(132, 25);
            this.txtA.TabIndex = 0;
            this.txtA.Text = "1";
            // 
            // txtB
            // 
            this.txtB.Location = new System.Drawing.Point(239, 102);
            this.txtB.Margin = new System.Windows.Forms.Padding(4);
            this.txtB.Name = "txtB";
            this.txtB.Size = new System.Drawing.Size(132, 25);
            this.txtB.TabIndex = 1;
            this.txtB.Text = "2";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 112);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "a=";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(193, 114);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = ",b=";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(416, 114);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "服务的结果是：";
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.Location = new System.Drawing.Point(416, 140);
            this.lblResult.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(79, 15);
            this.lblResult.TabIndex = 5;
            this.lblResult.Text = "lblResult";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(52, 159);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(4);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(183, 29);
            this.btnAdd.TabIndex = 6;
            this.btnAdd.Text = "（同步会话）计算 a+b";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnSub
            // 
            this.btnSub.Location = new System.Drawing.Point(239, 159);
            this.btnSub.Margin = new System.Windows.Forms.Padding(4);
            this.btnSub.Name = "btnSub";
            this.btnSub.Size = new System.Drawing.Size(133, 29);
            this.btnSub.TabIndex = 7;
            this.btnSub.Text = "(异常)计算 a-b";
            this.btnSub.UseVisualStyleBackColor = true;
            this.btnSub.Click += new System.EventHandler(this.btnSub_Click);
            // 
            // btnVoid
            // 
            this.btnVoid.Location = new System.Drawing.Point(52, 214);
            this.btnVoid.Margin = new System.Windows.Forms.Padding(4);
            this.btnVoid.Name = "btnVoid";
            this.btnVoid.Size = new System.Drawing.Size(183, 29);
            this.btnVoid.TabIndex = 8;
            this.btnVoid.Text = "无返回值的服务调用";
            this.btnVoid.UseVisualStyleBackColor = true;
            this.btnVoid.Click += new System.EventHandler(this.btnVoid_Click);
            // 
            // btnServerTime
            // 
            this.btnServerTime.Location = new System.Drawing.Point(419, 195);
            this.btnServerTime.Margin = new System.Windows.Forms.Padding(4);
            this.btnServerTime.Name = "btnServerTime";
            this.btnServerTime.Size = new System.Drawing.Size(203, 29);
            this.btnServerTime.TabIndex = 9;
            this.btnServerTime.Text = "订阅服务器时间";
            this.btnServerTime.UseVisualStyleBackColor = true;
            this.btnServerTime.Click += new System.EventHandler(this.btnServerTime_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(52, 16);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(302, 15);
            this.label5.TabIndex = 10;
            this.label5.Text = "iMSF消息服务框架测试程序 ，服务器地址：";
            // 
            // txtSerivceUri
            // 
            this.txtSerivceUri.Location = new System.Drawing.Point(52, 59);
            this.txtSerivceUri.Margin = new System.Windows.Forms.Padding(4);
            this.txtSerivceUri.Name = "txtSerivceUri";
            this.txtSerivceUri.Size = new System.Drawing.Size(319, 25);
            this.txtSerivceUri.TabIndex = 11;
            this.txtSerivceUri.Text = "net.tcp://127.0.0.1:8888";
            // 
            // btnStartServer
            // 
            this.btnStartServer.Location = new System.Drawing.Point(419, 59);
            this.btnStartServer.Margin = new System.Windows.Forms.Padding(4);
            this.btnStartServer.Name = "btnStartServer";
            this.btnStartServer.Size = new System.Drawing.Size(203, 29);
            this.btnStartServer.TabIndex = 12;
            this.btnStartServer.Text = "启动服务器";
            this.btnStartServer.UseVisualStyleBackColor = true;
            this.btnStartServer.Click += new System.EventHandler(this.btnStartServer_Click);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePicker1.Location = new System.Drawing.Point(52, 279);
            this.dateTimePicker1.Margin = new System.Windows.Forms.Padding(4);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.ShowUpDown = true;
            this.dateTimePicker1.Size = new System.Drawing.Size(265, 25);
            this.dateTimePicker1.TabIndex = 13;
            // 
            // btnAlarmClock
            // 
            this.btnAlarmClock.Location = new System.Drawing.Point(419, 231);
            this.btnAlarmClock.Margin = new System.Windows.Forms.Padding(4);
            this.btnAlarmClock.Name = "btnAlarmClock";
            this.btnAlarmClock.Size = new System.Drawing.Size(205, 32);
            this.btnAlarmClock.TabIndex = 14;
            this.btnAlarmClock.Text = "订阅闹钟";
            this.btnAlarmClock.UseVisualStyleBackColor = true;
            this.btnAlarmClock.Click += new System.EventHandler(this.btnAlarmClock_Click);
            // 
            // btnServerText
            // 
            this.btnServerText.Location = new System.Drawing.Point(417, 159);
            this.btnServerText.Margin = new System.Windows.Forms.Padding(4);
            this.btnServerText.Name = "btnServerText";
            this.btnServerText.Size = new System.Drawing.Size(203, 29);
            this.btnServerText.TabIndex = 15;
            this.btnServerText.Text = "订阅服务器文本";
            this.btnServerText.UseVisualStyleBackColor = true;
            this.btnServerText.Click += new System.EventHandler(this.btnServerText_Click);
            // 
            // btnParallel
            // 
            this.btnParallel.Location = new System.Drawing.Point(415, 272);
            this.btnParallel.Margin = new System.Windows.Forms.Padding(4);
            this.btnParallel.Name = "btnParallel";
            this.btnParallel.Size = new System.Drawing.Size(205, 32);
            this.btnParallel.TabIndex = 16;
            this.btnParallel.Text = "并发推送";
            this.btnParallel.UseVisualStyleBackColor = true;
            this.btnParallel.Click += new System.EventHandler(this.btnParallel_Click);
            // 
            // txtBlock
            // 
            this.txtBlock.Location = new System.Drawing.Point(55, 336);
            this.txtBlock.Margin = new System.Windows.Forms.Padding(4);
            this.txtBlock.Multiline = true;
            this.txtBlock.Name = "txtBlock";
            this.txtBlock.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtBlock.Size = new System.Drawing.Size(564, 164);
            this.txtBlock.TabIndex = 17;
            // 
            // btnReqServerTime
            // 
            this.btnReqServerTime.Location = new System.Drawing.Point(243, 214);
            this.btnReqServerTime.Margin = new System.Windows.Forms.Padding(4);
            this.btnReqServerTime.Name = "btnReqServerTime";
            this.btnReqServerTime.Size = new System.Drawing.Size(133, 29);
            this.btnReqServerTime.TabIndex = 18;
            this.btnReqServerTime.Text = "请求服务器时间";
            this.btnReqServerTime.UseVisualStyleBackColor = true;
            this.btnReqServerTime.Click += new System.EventHandler(this.btnReqServerTime_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(712, 526);
            this.Controls.Add(this.btnReqServerTime);
            this.Controls.Add(this.txtBlock);
            this.Controls.Add(this.btnParallel);
            this.Controls.Add(this.btnServerText);
            this.Controls.Add(this.btnAlarmClock);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.btnStartServer);
            this.Controls.Add(this.txtSerivceUri);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnServerTime);
            this.Controls.Add(this.btnVoid);
            this.Controls.Add(this.btnSub);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtB);
            this.Controls.Add(this.txtA);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "PDF.NET.MSF(http://www.pwmis.com/sqlmap)";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtA;
        private System.Windows.Forms.TextBox txtB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnSub;
        private System.Windows.Forms.Button btnVoid;
        private System.Windows.Forms.Button btnServerTime;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtSerivceUri;
        private System.Windows.Forms.Button btnStartServer;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Button btnAlarmClock;
        private System.Windows.Forms.Button btnServerText;
        private System.Windows.Forms.Button btnParallel;
        private System.Windows.Forms.TextBox txtBlock;
        private System.Windows.Forms.Button btnReqServerTime;
    }
}

