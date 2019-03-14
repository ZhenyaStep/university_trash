using System;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;

namespace SmtpClient
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                String smtpHost = "smtp.mail.ru";
                int smtpPort = 587;
                String smtpUserName = "matveevilya1998";
                String smtpUserPass = "";
                
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(smtpHost, smtpPort);
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(smtpUserName, smtpUserPass);
                

                String msgFrom = "matveevilya1998@mail.ru";
                String msgTo = "matveevilya1998@mail.ru";
                String msgSubject = "TopicBSUIR";
                String msgBody = "MessageBSUIR";
                MailMessage message = new MailMessage(msgFrom, msgTo, msgSubject, msgBody);
                try
                {
                    client.Send(message);
                    MessageBox.Show("Заказ успешно отправлен ", "Отправка");
                }
                catch (SmtpException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            catch
            {
                MessageBox.Show("Неверный ввод данных!");
            }
        }
    }
}
