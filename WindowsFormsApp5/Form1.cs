using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using NAudio.Wave;
using System.Drawing;


namespace WindowsFormsApp5
{
    public partial class Form1 : Form
    {
        private Dictionary<string, IDisposable> activeListeners = new Dictionary<string, IDisposable>();
        private FirebaseClient firebaseClient;
        private string username;
        private HashSet<string> recipientsList = new HashSet<string>();
        private bool isManualTextChange = false;
        private List<string> groupList = new List<string>(); // Khai báo và khởi tạo groupList

        private Dictionary<int, string> imageUrlDictionary = new Dictionary<int, string>();

        public Form1(string username, string fullname)
        {

            InitializeComponent();
            this.username = username;
            lblUsername.Text = $"{fullname}!";
            pictureBox.BringToFront(); // Đảm bảo PictureBox nằm trên rtbChat
            firebaseClient = new FirebaseClient("https://ltma-e2e8e-default-rtdb.firebaseio.com/");
            btnSend.Enabled = false;
            LoadRecipients();
            lstRecipients.Click += new EventHandler(lstRecipients_Click);
            LoadRecentConversations();
            MessageBox.Show($"Chào mừng {fullname}!");
            btnSend.Enabled = true;
            LoadUserList();
            // Gán sự kiện LinkClicked cho RichTextBox
        }

        private async Task SendPrivateMessage(string sender, string recipient, string content, bool isGroup = false)
        {
            try
            {
                if (isGroup)
                {
                    // Gửi tin nhắn vào nhóm
                    var message = new Message
                    {
                        Sender = sender,
                        Content = content,
                        SentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };

                    await firebaseClient
                        .Child("GroupMessages")
                        .Child(recipient) // Sử dụng recipient như là groupId
                        .PostAsync(message);
                }
                else
                {
                    // Gửi tin nhắn cá nhân
                    string chatId = GetChatId(sender, recipient);

                    var message = new Message
                    {
                        Sender = sender,
                        Recipient = recipient,
                        Content = content,
                        SentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };

                    await firebaseClient
                        .Child("PrivateMessages")
                        .Child(chatId)
                        .PostAsync(message);
                }

                txtMessage.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi gửi tin nhắn: {ex.Message}");
            }
        }

        private async void LoadRecentConversations()
        {
            try
            {
                var conversations = await firebaseClient
                    .Child("PrivateMessages")
                    .OnceAsync<Message>();

                HashSet<string> recentContacts = new HashSet<string>();

                foreach (var conversation in conversations)
                {
                    var chatId = conversation.Key;
                    var users = chatId.Split('_');

                    if (users.Length == 2)
                    {
                        string otherUser = (users[0] == username) ? users[1] : (users[1] == username) ? users[0] : null;

                        if (!string.IsNullOrEmpty(otherUser) && !recentContacts.Contains(otherUser))
                        {
                            recentContacts.Add(otherUser);
                            lstRecipients.Items.Add(otherUser);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải các cuộc trò chuyện gần đây: {ex.Message}");
            }
        }

        private void lstRecipients_Click(object sender, EventArgs e)
        {
            if (lstRecipients.SelectedItem != null)
            {
                isManualTextChange = true;
                string selectedUser = lstRecipients.SelectedItem.ToString();
                txtRecipient.Text = selectedUser;

                // Kiểm tra nếu recipient là nhóm dựa trên groupList
                bool isGroup = groupList.Contains(selectedUser);

                if (isGroup)
                {
                    LoadChatHistory(selectedUser);
                    ListenForGroupMessages(selectedUser); // Lắng nghe tin nhắn nhóm
                }
                else
                {
                    LoadChatHistory(selectedUser);
                    ListenForPrivateMessages(username, selectedUser); // Lắng nghe tin nhắn cá nhân
                }

                isManualTextChange = false;
            }
        }



        private void lblUsername_Click(object sender, EventArgs e)
        {
            pictureBox.Visible = false;
        }
        private async void LoadChatHistory(string recipient)
        {
            string chatId = GetChatId(username, recipient);
            flowChatPanel.Controls.Clear();
            imageUrlDictionary.Clear();

            try
            {
                var messages = await firebaseClient
                    .Child("PrivateMessages")
                    .Child(chatId)
                    .OrderByKey()
                    .OnceAsync<Message>();

                foreach (var message in messages)
                {
                    var msg = message.Object;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải tin nhắn cũ: {ex.Message}");
            }
        }

        private async void btnSend_Click_1(object sender, EventArgs e)
        {
            string recipient = txtRecipient.Text;
            string messageContent = txtMessage.Text;

            if (string.IsNullOrWhiteSpace(recipient) || string.IsNullOrWhiteSpace(messageContent))
            {
                MessageBox.Show("Vui lòng nhập tên người nhận và tin nhắn.");
                return;
            }

            bool isGroup = groupList.Contains(recipient); // Kiểm tra nếu recipient là một nhóm

            try
            {
                await SendPrivateMessage(username, recipient, messageContent, isGroup);

                if (!lstRecipients.Items.Contains(recipient))
                {
                    lstRecipients.Items.Add(recipient);
                }

                txtMessage.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gửi tin nhắn thất bại: {ex.Message}");
            }
        }


        private async void LoadRecipients()
        {
            try
            {
                // Tải danh sách các cuộc trò chuyện gần đây của người dùng hiện tại
                var recentConversations = await firebaseClient
                    .Child("Users")
                    .Child(username)
                    .Child("RecentContacts")
                    .OnceAsync<object>();

                var autoCompleteCollection = new AutoCompleteStringCollection();
                lstRecipients.Items.Clear();
                HashSet<string> addedRecipients = new HashSet<string>();

                // Thêm người dùng từ các cuộc trò chuyện gần đây vào lstRecipients
                foreach (var contact in recentConversations)
                {
                    string contactUsername = contact.Key;
                    if (!string.IsNullOrEmpty(contactUsername) && !addedRecipients.Contains(contactUsername))
                    {
                        autoCompleteCollection.Add(contactUsername);
                        lstRecipients.Items.Add(contactUsername);
                        addedRecipients.Add(contactUsername); // Đánh dấu là đã thêm
                    }
                }

                txtRecipient.AutoCompleteCustomSource = autoCompleteCollection;

                // Tải danh sách nhóm từ Firebase
                var allGroups = await firebaseClient.Child("Groups").OnceAsync<Group>();
                groupList.Clear(); // Xóa danh sách cũ trước khi thêm mới

                foreach (var group in allGroups)
                {
                    string groupName = group.Object.GroupName;

                    // Chỉ thêm nhóm nếu người dùng hiện tại là thành viên của nhóm đó
                    if (group.Object.Members.Contains(username) && !addedRecipients.Contains(groupName))
                    {
                        lstRecipients.Items.Add(groupName);
                        groupList.Add(groupName); // Lưu tên nhóm vào groupList
                        addedRecipients.Add(groupName); // Đánh dấu là đã thêm
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách người dùng và nhóm: {ex.Message}");
            }
        }



        private void txtRecipient_TextChanged(object sender, EventArgs e)
        {
            string recipient = txtRecipient.Text.Trim();

            if (!string.IsNullOrEmpty(recipient))
            {
                LoadChatHistory(recipient);
                HighlightUserInListBox(recipient);
            }
        }

        private void HighlightUserInListBox(string recipient)
        {
            int index = lstRecipients.Items.IndexOf(recipient);

            if (index != -1)
            {
                lstRecipients.SelectedIndex = index;
            }
            else
            {
                lstRecipients.ClearSelected();
            }
        }

        private void txtMessage_Enter(object sender, EventArgs e)
        {
            if (txtMessage.Text == "Nhập tin nhắn vào đây...")
            {
                txtMessage.Text = "";
                txtMessage.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void txtMessage_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMessage.Text))
            {
                txtMessage.Text = "Nhập tin nhắn vào đây...";
                txtMessage.ForeColor = System.Drawing.Color.Gray;
            }
        }

        private void txtRecipient_Enter(object sender, EventArgs e)
        {
            if (txtRecipient.Text == "Nhập tên người nhận...")
            {
                txtRecipient.Text = "";
                txtRecipient.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void txtRecipient_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRecipient.Text))
            {
                txtRecipient.Text = "Nhập tên người nhận...";
                txtRecipient.ForeColor = System.Drawing.Color.Gray;
            }
        }

        private string GetChatId(string user1, string user2)
        {
            return string.Compare(user1, user2) < 0 ? $"{user1}_{user2}" : $"{user2}_{user1}";
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (activeListeners != null)
            {
                foreach (var listener in activeListeners.Values)
                {
                    listener.Dispose();
                }
                activeListeners.Clear();
            }

            this.Close();
            var loginForm = new Login();
            loginForm.Show();
        }

        private async void ListenForPrivateMessages(string sender, string recipient)
        {
            string chatId = GetChatId(sender, recipient);

            if (activeListeners.ContainsKey(chatId))
            {
                activeListeners[chatId].Dispose();
                activeListeners.Remove(chatId);
            }

            try
            {
                var listener = firebaseClient
                    .Child("PrivateMessages")
                    .Child(chatId)
                    .AsObservable<Message>()
                    .ObserveOn(SynchronizationContext.Current)
                    .Subscribe(message =>
                    {
                        if (message?.Object != null)
                        {
                            var msg = message.Object;
                            AppendFormattedMessage(msg.Sender, msg.Content, msg.Sender == username);
                        }
                    },
                    ex => MessageBox.Show($"Lỗi khi nhận tin nhắn mới: {ex.Message}"));

                activeListeners[chatId] = listener;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thiết lập lắng nghe tin nhắn: {ex.Message}");
            }
        }

        private void AppendFormattedMessage(string sender, string messageContent, bool isSender)
        {
            // Tạo một TableLayoutPanel với một cột để căn chỉnh tin nhắn
            TableLayoutPanel alignmentTable = new TableLayoutPanel
            {
                ColumnCount = 1,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Top,
                BackColor = flowChatPanel.BackColor,
                Padding = new Padding(5)
            };

            // Tạo một FlowLayoutPanel cho tin nhắn
            FlowLayoutPanel messagePanel = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(10),
                BackColor = isSender ? Color.LightGreen : Color.LightGray,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Margin = new Padding(5)
            };

            // Tạo Label cho tên người gửi
            Label senderLabel = new Label
            {
                Text = $"{sender}: ",
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Padding = new Padding(0, 0, 0, 5),
                ForeColor = Color.Black
            };
            messagePanel.Controls.Add(senderLabel);

            // Tạo LinkLabel cho các nội dung media
            if (Uri.IsWellFormedUriString(messageContent, UriKind.Absolute))
            {
                LinkLabel messageLink = new LinkLabel
                {
                    Text = GetDisplayText(messageContent),
                    AutoSize = true,
                    Tag = messageContent,
                    Padding = new Padding(0, 5, 0, 0)
                };

                messageLink.LinkClicked += (s, e) =>
                {
                    string fileUrl = (string)((LinkLabel)s).Tag;

                    if (fileUrl.EndsWith(".jpg") || fileUrl.EndsWith(".jpeg") || fileUrl.EndsWith(".png"))
                    {
                        // Hiển thị ảnh trong PictureBox
                        pictureBox.ImageLocation = fileUrl;
                        pictureBox.Visible = true;
                        pictureBox.BringToFront();
                    }
                    else if (fileUrl.EndsWith(".mp4") || fileUrl.EndsWith(".avi") || fileUrl.EndsWith(".mov"))
                    {
                        // Mở video trong trình duyệt mặc định
                        System.Diagnostics.Process.Start(fileUrl);
                    }
                    else if (fileUrl.EndsWith(".wav") || fileUrl.EndsWith(".mp3"))
                    {
                        // Phát audio trực tiếp
                        PlayAudio(fileUrl);
                    }
                    else
                    {
                        // Tải các file khác
                        DownloadAndSaveFile(fileUrl);
                    }
                };

                messagePanel.Controls.Add(messageLink);
            }
            else
            {
                // Nếu là văn bản bình thường, chỉ hiển thị trong một Label
                Label messageLabel = new Label
                {
                    Text = messageContent,
                    AutoSize = true,
                    Padding = new Padding(0, 5, 0, 0),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                messagePanel.Controls.Add(messageLabel);
            }

            // Sử dụng TableLayoutPanel để căn chỉnh tin nhắn
            alignmentTable.Controls.Add(messagePanel, 0, 0);
            alignmentTable.ColumnStyles.Add(new ColumnStyle(isSender ? SizeType.Absolute : SizeType.Percent, isSender ? 100 : 0));
            messagePanel.Anchor = isSender ? AnchorStyles.Right : AnchorStyles.Left;

            // Thêm alignmentTable vào flowChatPanel để hiển thị
            flowChatPanel.Controls.Add(alignmentTable);
            flowChatPanel.ScrollControlIntoView(alignmentTable);
        }

        // Phương thức hỗ trợ để lấy tên hiển thị cho các loại file
        private string GetDisplayText(string messageContent)
        {
            if (messageContent.EndsWith(".pdf")) return "PDF File";
            else if (messageContent.EndsWith(".docx")) return "Word Document";
            else if (messageContent.EndsWith(".xlsx")) return "Excel File";
            else if (messageContent.EndsWith(".jpg") || messageContent.EndsWith(".jpeg") || messageContent.EndsWith(".png")) return "Image";
            else if (messageContent.EndsWith(".mp4") || messageContent.EndsWith(".avi") || messageContent.EndsWith(".mov")) return "Video";
            else return "Voice";
        }


        private async Task DownloadAndSaveFile(string fileUrl)
        {
            try
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    // Tải file từ URL
                    var fileData = await client.GetByteArrayAsync(fileUrl);

                    // Xác định tên file từ URL
                    string fileName = Path.GetFileName(new Uri(fileUrl).LocalPath);

                    // Hộp thoại chọn nơi lưu file
                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.FileName = fileName;
                        saveFileDialog.Filter = "All Files|*.*";
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            // Lưu file xuống
                            File.WriteAllBytes(saveFileDialog.FileName, fileData);
                            MessageBox.Show("File đã được lưu thành công!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải file: {ex.Message}");
            }
        }






        private void OpenMediaFile(string fileUrl)
        {
            if (fileUrl.EndsWith(".wav") || fileUrl.EndsWith(".mp3"))
            {
                PlayAudio(fileUrl);
            }
            else if (fileUrl.EndsWith(".jpg") || fileUrl.EndsWith(".jpeg") || fileUrl.EndsWith(".png"))
            {
                pictureBox.ImageLocation = fileUrl;
                pictureBox.Visible = true;
                pictureBox.BringToFront();
            }
            else if (fileUrl.EndsWith(".mp4") || fileUrl.EndsWith(".avi") || fileUrl.EndsWith(".mov"))
            {
                try
                {
                    System.Diagnostics.Process.Start(fileUrl);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Không thể mở video: {ex.Message}");
                }
            }
            else if (fileUrl.EndsWith(".pdf") || fileUrl.EndsWith(".docx") || fileUrl.EndsWith(".xlsx"))
            {
                try
                {
                    System.Diagnostics.Process.Start(fileUrl);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Không thể mở file: {ex.Message}");
                }
            }
        }


        private async void PlayAudio(string audioUrl)
        {
            string tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.wav");

            using (var client = new System.Net.Http.HttpClient())
            {
                var audioData = await client.GetByteArrayAsync(audioUrl);
                File.WriteAllBytes(tempFilePath, audioData);
            }

            using (var audioFile = new AudioFileReader(tempFilePath))
            using (var outputDevice = new WaveOutEvent())
            {
                outputDevice.Init(audioFile);
                outputDevice.Play();

                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    await Task.Delay(1000);
                }
            }

            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }

        private async void rtbChat_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            string linkUrl = e.LinkText;

            if (linkUrl.EndsWith(".wav") || linkUrl.EndsWith(".mp3"))
            {
                try
                {
                    // Tạo đường dẫn file tạm để lưu file âm thanh
                    string tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.wav");

                    using (var client = new System.Net.Http.HttpClient())
                    {
                        var audioData = await client.GetByteArrayAsync(linkUrl);
                        File.WriteAllBytes(tempFilePath, audioData);  // Lưu file âm thanh vào đường dẫn tạm
                    }

                    // Phát âm thanh từ file cục bộ
                    using (var audioFile = new AudioFileReader(tempFilePath))
                    using (var outputDevice = new WaveOutEvent())
                    {
                        outputDevice.Init(audioFile);
                        outputDevice.Play();

                        // Chờ phát xong
                        while (outputDevice.PlaybackState == PlaybackState.Playing)
                        {
                            await Task.Delay(1000);
                        }
                    }

                    // Xóa file sau khi phát xong
                    if (File.Exists(tempFilePath))
                    {
                        File.Delete(tempFilePath);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Không thể phát âm thanh: {ex.Message}");
                }
            }
            else if (linkUrl.EndsWith(".jpg") || linkUrl.EndsWith(".jpeg") || linkUrl.EndsWith(".png"))
            {
                pictureBox.ImageLocation = linkUrl;
                pictureBox.Visible = true;
                pictureBox.BringToFront();
            }
            else if (linkUrl.EndsWith(".mp4") || linkUrl.EndsWith(".avi") || linkUrl.EndsWith(".mov"))
            {
                try
                {
                    System.Diagnostics.Process.Start(linkUrl);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Không thể mở video: {ex.Message}");
                }
            }
        }




        private async void btnHinhAnhvaVideo_Click_1(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Media Files|*.jpg;*.jpeg;*.png;*.mp4;*.avi;*.mov";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    string url = await UploadToCloudinary(filePath);
                    if (!string.IsNullOrEmpty(url))
                    {
                        await SendPrivateMessage(username, txtRecipient.Text, url);
                        MessageBox.Show("Đã gửi hình ảnh/video.");
                    }
                }
            }
        }


        private async Task<string> UploadToCloudinary(string filePath)
        {
            var account = new Account("dnuoh7v83", "863728563898176", "kkRyefN5sPgUJUysSthTVg7c-9Y");
            var cloudinary = new Cloudinary(account);

            if (filePath.EndsWith(".mp4") || filePath.EndsWith(".avi") || filePath.EndsWith(".mov"))
            {
                var uploadParams = new VideoUploadParams()
                {
                    File = new FileDescription(filePath),
                    Folder = "chat_videos"
                };
                var uploadResult = await cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl?.ToString();
            }
            else if (filePath.EndsWith(".jpg") || filePath.EndsWith(".jpeg") || filePath.EndsWith(".png"))
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(filePath),
                    Folder = "chat_images"
                };
                var uploadResult = await cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl?.ToString();
            }
            else
            {
                // Sử dụng RawUploadParams cho các file khác
                var uploadParams = new RawUploadParams()
                {
                    File = new FileDescription(filePath),
                    Folder = "chat_files"
                };
                var uploadResult = await cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl?.ToString();
            }
        }





        private async void button1_Click(object sender, EventArgs e)
        {
            // Khởi tạo đường dẫn lưu trữ file tạm thời để ghi âm
            string tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.wav");

            // Ghi âm giọng nói
            using (var waveIn = new NAudio.Wave.WaveInEvent())
            {
                waveIn.WaveFormat = new NAudio.Wave.WaveFormat(44100, 1);
                using (var writer = new NAudio.Wave.WaveFileWriter(tempFilePath, waveIn.WaveFormat))
                {
                    waveIn.DataAvailable += (s, a) => writer.Write(a.Buffer, 0, a.BytesRecorded);
                    waveIn.StartRecording();

                    MessageBox.Show("Đang ghi âm, nhấn OK để dừng và gửi.");

                    waveIn.StopRecording();
                }
            }

            // Tải file âm thanh lên Cloudinary
            string url = await UploadVoiceToCloudinary(tempFilePath);
            if (!string.IsNullOrEmpty(url))
            {
                await SendPrivateMessage(username, txtRecipient.Text, url);
                MessageBox.Show("Đã gửi tin nhắn giọng nói.");
            }

            // Xóa file âm thanh tạm thời
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
        private async Task<string> UploadVoiceToCloudinary(string filePath)
        {
            var account = new Account("dnuoh7v83", "863728563898176", "kkRyefN5sPgUJUysSthTVg7c-9Y");
            var cloudinary = new Cloudinary(account);

            var uploadParams = new RawUploadParams()
            {
                File = new FileDescription(filePath),
                Folder = "chat_voices"
            };
            var uploadResult = await cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl?.ToString();
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            clbUsers.Visible = false;
        }

        private void txtRecipient_TextChanged_1(object sender, EventArgs e)
        {

        }

        private async void btnFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "All Files|*.*|PDF Files|*.pdf|Word Documents|*.docx;*.doc|Excel Files|*.xlsx";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    string url = await UploadToCloudinary(filePath);
                    if (!string.IsNullOrEmpty(url))
                    {
                        await SendPrivateMessage(username, txtRecipient.Text, url);
                        MessageBox.Show("Đã gửi file.");
                    }
                }
            }
        }
        private async void LoadUserList()
        {
            try
            {
                // Lấy danh sách tất cả người dùng từ Firebase
                var allUsers = await firebaseClient.Child("Users").OnceAsync<User>();

                // Xóa tất cả mục hiện có trong clbUsers trước khi thêm mới
                clbUsers.Items.Clear();

                // Thêm từng người dùng vào CheckedListBox
                foreach (var user in allUsers)
                {
                    if (!string.IsNullOrEmpty(user.Object.Username) && user.Object.Username != username)
                    {
                        clbUsers.Items.Add(user.Object.Username);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách người dùng: {ex.Message}");
            }
        }
        private async void ListenForGroupMessages(string groupId)
        {
            if (activeListeners.ContainsKey(groupId))
            {
                activeListeners[groupId].Dispose();
                activeListeners.Remove(groupId);
            }

            try
            {
                var listener = firebaseClient
                    .Child("GroupMessages")
                    .Child(groupId)
                    .AsObservable<Message>()
                    .ObserveOn(SynchronizationContext.Current)
                    .Subscribe(message =>
                    {
                        if (message?.Object != null)
                        {
                            var msg = message.Object;
                            AppendFormattedMessage(msg.Sender, msg.Content, msg.Sender == username);
                        }
                    },
                    ex => MessageBox.Show($"Lỗi khi nhận tin nhắn nhóm: {ex.Message}"));

                activeListeners[groupId] = listener;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thiết lập lắng nghe tin nhắn nhóm: {ex.Message}");
            }
        }

        private async void btnTaoNhom_Click(object sender, EventArgs e)
        {
            // Lấy tên nhóm từ txtGroupName
            string groupName = txtGroupName.Text.Trim();

            // Lấy danh sách thành viên được chọn từ clbUsers
            List<string> selectedMembers = clbUsers.CheckedItems.Cast<string>().ToList();

            // Kiểm tra nếu tên nhóm hoặc danh sách thành viên bị trống
            if (string.IsNullOrEmpty(groupName) || selectedMembers.Count == 0)
            {
                MessageBox.Show("Vui lòng nhập tên nhóm và chọn ít nhất một thành viên.");
                return;
            }

            // Tạo đối tượng nhóm
            var group = new
            {
                GroupName = groupName,
                Members = selectedMembers
            };

            try
            {
                // Tạo nhóm mới trong Firebase
                var groupKey = await firebaseClient.Child("Groups").PostAsync(group);
                MessageBox.Show($"Nhóm '{groupName}' đã được tạo thành công!");

                // Xóa dữ liệu trong TextBox và CheckedListBox sau khi tạo nhóm
                txtGroupName.Clear();
                for (int i = 0; i < clbUsers.Items.Count; i++)
                {
                    clbUsers.SetItemChecked(i, false);
                }

                // Gọi lại LoadRecipients để làm mới danh sách người dùng và nhóm
                LoadRecipients();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo nhóm: {ex.Message}");
            }
        }

        private void lstRecipients_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }

}