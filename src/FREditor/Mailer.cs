using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Net.Mail;

namespace FREditor
{	
	/// <summary>
	/// Класс, который умеет отправлять письма
	/// </summary>
	public class Mailer
	{
		private static string SmtpServerName = "mail.adc.analit.net";

		private static string EmailService = "service@analit.net";

		public static void SendNotificationLetter(MySqlConnection connection,
			string body, string priceName, string providerName, string regionName)
		{
			try
			{
				//Получаем e-mail оператора
				string operatorMail = (string)MySqlHelper.ExecuteScalar(
					connection,
@"SELECT 
  regionaladmins.email 
FROM 
  accessright.regionaladmins
WHERE  
  username = ?UserName", new MySqlParameter("?UserName", Environment.UserName));

				//Формируем сообщение
				MailMessage m = new MailMessage("register@analit.net",
#if DEBUG
 "d.dorofeev@analit.net",
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
				SmtpClient sm = new SmtpClient(SmtpServerName);
				sm.Send(m);
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					"Не удалось отправить уведомление об изменениях. Сообщение было отправлено разработчику.",
					"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Program.SendMessageOnException(null, new Exception("Ошибка при отправке уведомления.", ex));
			}
		}

		public static void SendWarningLetter(string body)
		{
			try
			{
				string messageBody = String.Format("Оператор: {0}\nТекст сообщения:{1}\n",
					Environment.UserName, body);
				//Формируем сообщение
				MailMessage m = new MailMessage(
					"service@analit.net", "service@analit.net", "Предупреждение в FREditor", messageBody);
				SmtpClient sm = new SmtpClient(SmtpServerName);
				sm.Send(m);
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					@"Не удалось отправить уведомление об изменениях. Сообщение было отправлено разработчику.",
					"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Program.SendMessageOnException(null, new Exception("Ошибка при отправке уведомления.", ex));
			}
		}

		public static void SendErrorMessageToService(string messageBody, Exception exception)
		{
			try
			{
				messageBody = String.Format("{0}\nКомпьютер: {1}\nОператор: {2}\nОшибка: {3}",
				                            messageBody, Environment.MachineName, Environment.UserName, exception);
				var mailMessage = new MailMessage(EmailService, EmailService, "Ошибка в FREditor", messageBody);
				var smtpClient = new SmtpClient(SmtpServerName);
				smtpClient.Send(mailMessage);
			}
			catch (Exception)
			{
				MessageBox.Show(@"Не удалось отправить разработчику сообщение об ошибке. Свяжитесь с разработчиком",
				                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
				
			}			
		}
	}
}
