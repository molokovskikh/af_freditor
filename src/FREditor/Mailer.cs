using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Net.Mail;
using FREditor.Properties;

namespace FREditor
{
	/// <summary>
	/// Класс, который умеет отправлять письма
	/// </summary>
	public class Mailer
	{
		private static string SmtpServerName = "box.analit.net";

		private static string EmailService = Settings.Default.EmailService;

		public static void SendNotificationLetter(MySqlConnection connection,
			string body, string priceName, string providerName, string regionName)
		{
			try {
				//Получаем e-mail оператора
				var operatorMail = (string)MySqlHelper.ExecuteScalar(
					connection,
					@"SELECT
  regionaladmins.email 
FROM 
  accessright.regionaladmins
WHERE  
  username = ?UserName", new MySqlParameter("?UserName", Environment.UserName));

				//Формируем сообщение
				var m = new MailMessage("register@analit.net",
#if DEBUG
					"KvasovTest@analit.net",
#else
					"RegisterList@subscribe.analit.net",
#endif
					String.Format("\"{0}\" - изменения в списке ценовых колонок", providerName),
					String.Format(@"
Оператор   : {0} 
Поставщик  : {1}
Регион     : {2}
Прайс-лист : {3}

{4}

С уважением,
  АК Инфорум.",
						Environment.UserName,
						providerName,
						regionName,
						priceName,
						body));
				if (!String.IsNullOrEmpty(operatorMail))
					m.Bcc.Add(operatorMail);
				var sm = new SmtpClient(SmtpServerName);
				sm.Send(m);
			}
			catch (Exception ex) {
				MessageBox.Show(
					"Не удалось отправить уведомление об изменениях. Сообщение было отправлено разработчику.",
					"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Program.SendMessageOnException(null, new Exception("Ошибка при отправке уведомления.", ex));
			}
		}

		public static void SendWarningLetter(string body)
		{
			try {
				string messageBody = String.Format("Оператор: {0}\nТекст сообщения:{1}\n",
					Environment.UserName, body);
				//Формируем сообщение
				var m = new MailMessage(EmailService, EmailService, "Предупреждение в FREditor", messageBody);
				var sm = new SmtpClient(SmtpServerName);
				sm.Send(m);
			}
			catch (Exception ex) {
				MessageBox.Show(
					@"Не удалось отправить уведомление об изменениях. Сообщение было отправлено разработчику.",
					"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Program.SendMessageOnException(null, new Exception("Ошибка при отправке уведомления.", ex));
			}
		}
	}
}