using System;
using System.Windows.Forms;

namespace WindowsFormsApp5
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lstUser = new System.Windows.Forms.ListBox();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.txtRecipient = new System.Windows.Forms.TextBox();
            this.lblUsername = new System.Windows.Forms.Label();
            this.btnLogout = new System.Windows.Forms.Button();
            this.lstRecipients = new System.Windows.Forms.ListBox();
            this.btnHinhAnhvaVideo = new System.Windows.Forms.Button();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.btnVoice = new System.Windows.Forms.Button();
            this.flowChatPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.clbUsers = new System.Windows.Forms.CheckedListBox();
            this.btnFile = new System.Windows.Forms.Button();
            this.txtGroupName = new System.Windows.Forms.TextBox();
            this.btnTaoNhom = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // lstUser
            // 
            this.lstUser.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.lstUser.FormattingEnabled = true;
            this.lstUser.ItemHeight = 22;
            this.lstUser.Location = new System.Drawing.Point(14, 62);
            this.lstUser.Name = "lstUser";
            this.lstUser.Size = new System.Drawing.Size(175, 444);
            this.lstUser.TabIndex = 0;
            this.lstUser.Visible = false;
            // 
            // txtMessage
            // 
            this.txtMessage.Font = new System.Drawing.Font("Arial", 12F);
            this.txtMessage.ForeColor = System.Drawing.Color.Gray;
            this.txtMessage.Location = new System.Drawing.Point(237, 597);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(588, 30);
            this.txtMessage.TabIndex = 2;
            this.txtMessage.Text = "Nhập tin nhắn vào đây...";
            this.txtMessage.Enter += new System.EventHandler(this.txtMessage_Enter);
            this.txtMessage.Leave += new System.EventHandler(this.txtMessage_Leave);
            // 
            // btnSend
            // 
            this.btnSend.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Bold);
            this.btnSend.Location = new System.Drawing.Point(832, 595);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(94, 40);
            this.btnSend.TabIndex = 3;
            this.btnSend.Text = "Gửi";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click_1);
            // 
            // txtRecipient
            // 
            this.txtRecipient.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtRecipient.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtRecipient.Font = new System.Drawing.Font("Arial", 12F);
            this.txtRecipient.ForeColor = System.Drawing.Color.Gray;
            this.txtRecipient.Location = new System.Drawing.Point(236, 33);
            this.txtRecipient.Name = "txtRecipient";
            this.txtRecipient.Size = new System.Drawing.Size(654, 30);
            this.txtRecipient.TabIndex = 4;
            this.txtRecipient.Text = "Nhập tên người nhận...";
            this.txtRecipient.TextChanged += new System.EventHandler(this.txtRecipient_TextChanged_1);
            this.txtRecipient.Enter += new System.EventHandler(this.txtRecipient_Enter);
            this.txtRecipient.Leave += new System.EventHandler(this.txtRecipient_Leave);
            // 
            // lblUsername
            // 
            this.lblUsername.BackColor = System.Drawing.Color.White;
            this.lblUsername.Font = new System.Drawing.Font("Modern No. 20", 10.2F, System.Drawing.FontStyle.Bold);
            this.lblUsername.Location = new System.Drawing.Point(14, 17);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(176, 42);
            this.lblUsername.TabIndex = 6;
            this.lblUsername.Text = "label1";
            this.lblUsername.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblUsername.Click += new System.EventHandler(this.lblUsername_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Bold);
            this.btnLogout.Location = new System.Drawing.Point(14, 597);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(135, 40);
            this.btnLogout.TabIndex = 7;
            this.btnLogout.Text = "Đăng Xuất";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // lstRecipients
            // 
            this.lstRecipients.Font = new System.Drawing.Font("Arial", 10F);
            this.lstRecipients.ItemHeight = 19;
            this.lstRecipients.Location = new System.Drawing.Point(14, 63);
            this.lstRecipients.Name = "lstRecipients";
            this.lstRecipients.Size = new System.Drawing.Size(175, 156);
            this.lstRecipients.TabIndex = 8;
            this.lstRecipients.SelectedIndexChanged += new System.EventHandler(this.lstRecipients_SelectedIndexChanged);
            // 
            // btnHinhAnhvaVideo
            // 
            this.btnHinhAnhvaVideo.Location = new System.Drawing.Point(237, 546);
            this.btnHinhAnhvaVideo.Name = "btnHinhAnhvaVideo";
            this.btnHinhAnhvaVideo.Size = new System.Drawing.Size(138, 43);
            this.btnHinhAnhvaVideo.TabIndex = 9;
            this.btnHinhAnhvaVideo.Text = "HìnhẢnh/Video";
            this.btnHinhAnhvaVideo.UseVisualStyleBackColor = true;
            this.btnHinhAnhvaVideo.Click += new System.EventHandler(this.btnHinhAnhvaVideo_Click_1);
            // 
            // pictureBox
            // 
            this.pictureBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox.Location = new System.Drawing.Point(60, 62);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(729, 495);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 6;
            this.pictureBox.TabStop = false;
            this.pictureBox.Visible = false;
            this.pictureBox.Click += new System.EventHandler(this.pictureBox_Click);
            // 
            // btnVoice
            // 
            this.btnVoice.Location = new System.Drawing.Point(396, 546);
            this.btnVoice.Name = "btnVoice";
            this.btnVoice.Size = new System.Drawing.Size(138, 43);
            this.btnVoice.TabIndex = 10;
            this.btnVoice.Text = "Voice";
            this.btnVoice.UseVisualStyleBackColor = true;
            this.btnVoice.Click += new System.EventHandler(this.button1_Click);
            // 
            // flowChatPanel
            // 
            this.flowChatPanel.AutoScroll = true;
            this.flowChatPanel.BackColor = System.Drawing.Color.White;
            this.flowChatPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowChatPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowChatPanel.Location = new System.Drawing.Point(237, 68);
            this.flowChatPanel.Margin = new System.Windows.Forms.Padding(0);
            this.flowChatPanel.Name = "flowChatPanel";
            this.flowChatPanel.Padding = new System.Windows.Forms.Padding(8);
            this.flowChatPanel.Size = new System.Drawing.Size(653, 470);
            this.flowChatPanel.TabIndex = 11;
            this.flowChatPanel.WrapContents = false;
            // 
            // clbUsers
            // 
            this.clbUsers.FormattingEnabled = true;
            this.clbUsers.Location = new System.Drawing.Point(14, 337);
            this.clbUsers.Name = "clbUsers";
            this.clbUsers.Size = new System.Drawing.Size(178, 179);
            this.clbUsers.TabIndex = 0;
            // 
            // btnFile
            // 
            this.btnFile.Location = new System.Drawing.Point(566, 546);
            this.btnFile.Name = "btnFile";
            this.btnFile.Size = new System.Drawing.Size(138, 43);
            this.btnFile.TabIndex = 12;
            this.btnFile.Text = "File";
            this.btnFile.UseVisualStyleBackColor = true;
            this.btnFile.Click += new System.EventHandler(this.btnFile_Click);
            // 
            // txtGroupName
            // 
            this.txtGroupName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtGroupName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtGroupName.Font = new System.Drawing.Font("Arial", 12F);
            this.txtGroupName.ForeColor = System.Drawing.Color.Gray;
            this.txtGroupName.Location = new System.Drawing.Point(237, 1);
            this.txtGroupName.Name = "txtGroupName";
            this.txtGroupName.Size = new System.Drawing.Size(538, 30);
            this.txtGroupName.TabIndex = 13;
            this.txtGroupName.Text = "Nhập tên Nhóm";
            // 
            // btnTaoNhom
            // 
            this.btnTaoNhom.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTaoNhom.Location = new System.Drawing.Point(782, 1);
            this.btnTaoNhom.Name = "btnTaoNhom";
            this.btnTaoNhom.Size = new System.Drawing.Size(108, 29);
            this.btnTaoNhom.TabIndex = 14;
            this.btnTaoNhom.Text = "Tạo Nhóm";
            this.btnTaoNhom.UseVisualStyleBackColor = true;
            this.btnTaoNhom.Click += new System.EventHandler(this.btnTaoNhom_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::WindowsFormsApp5.Properties.Resources.Untitled;
            this.ClientSize = new System.Drawing.Size(926, 648);
            this.Controls.Add(this.btnTaoNhom);
            this.Controls.Add(this.clbUsers);
            this.Controls.Add(this.txtGroupName);
            this.Controls.Add(this.btnFile);
            this.Controls.Add(this.flowChatPanel);
            this.Controls.Add(this.btnVoice);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.btnHinhAnhvaVideo);
            this.Controls.Add(this.lstRecipients);
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.lblUsername);
            this.Controls.Add(this.txtRecipient);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.lstUser);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chat App";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

     

        #endregion

        private System.Windows.Forms.ListBox lstUser;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtRecipient;
        private Label lblUsername;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.ListBox lstRecipients;
        private Button btnHinhAnhvaVideo;
        private PictureBox pictureBox;
        private Button btnVoice;
        private FlowLayoutPanel flowChatPanel;
        private Button btnFile;
        private TextBox txtGroupName;
        private CheckedListBox clbUsers;
        private Button btnTaoNhom;
    }
}
