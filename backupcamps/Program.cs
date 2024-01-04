using System;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.IO;

namespace backupcamps
{
    class Program
    {
        public static string[] DatabaseName;
        static SqlConnection con;
        static SqlCommand cmd;
        static SqlDataReader dr;
        private static string connectionString;
        static string label;
        static TextReader tr;
        static string servidor;
        static string usuario;
        static string sqlsenha;
        static string pasta;
        static string emailTo;
        static string bcc;
        static string assunto;
        static string frommail;
        static string hostmail;
        static string usuariomail;
        static string senhamail;
        
        

        static void Main(string[] args)
        {
            tr = new StreamReader("config.ini");
            if (tr.ReadLine() == "[servidor]")
            {
                servidor = tr.ReadLine();
                if (tr.ReadLine() == "[usuario]")
                {
                    usuario = tr.ReadLine();
                    if (tr.ReadLine() == "[senha]")
                    {
                        sqlsenha = tr.ReadLine();
                        if (tr.ReadLine() == "[pasta]")
                        {
                            pasta = tr.ReadLine();
                            if (tr.ReadLine() == "[mailto]")
                            {
                                emailTo = tr.ReadLine();
                                if (tr.ReadLine() == "[assunto]")
                                {
                                    assunto = tr.ReadLine();
                                    if (tr.ReadLine() == "[from]")
                                    {
                                        frommail = tr.ReadLine();
                                        if (tr.ReadLine() == "[mailhost]")
                                        {
                                            hostmail = tr.ReadLine();
                                            if (tr.ReadLine() == "[usuariomail]")
                                            {
                                                usuariomail = tr.ReadLine();
                                                if (tr.ReadLine() == "[passwd]")
                                                {
                                                    senhamail = tr.ReadLine();
                                                    if (tr.ReadLine() == "[bcc]")
                                                        bcc = tr.ReadLine();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    System.Environment.Exit(1);
                }
            }


            ServerName(servidor,usuario,sqlsenha,pasta,emailTo,bcc,assunto,frommail,hostmail,usuariomail,senhamail);
        }

        public static void ServerName(string str,string usuario,string sqlsenha,string pasta,string emailto,string bcc,string assunto,string frommail,string hostmail,string usuariomail,string senhamail)
        {
            try
            {
                connectionString = "Data Source = " + str + "; User Id=" + usuario + "; Password="+ sqlsenha + ";";
                con = new SqlConnection(connectionString);
                con.Open();
                cmd = new SqlCommand("SELECT * FROM sys.databases d WHERE d.database_id > 4", con);
                dr = cmd.ExecuteReader();
                int i = 0;
                DatabaseName = new string[5];
                while (dr.Read())
                {
                    DatabaseName[i] = dr[0].ToString();
                    label = DatabaseName[i];
                    i++;
                }
                dr.Close();
                con.Close();
                
                for (int x = 0; x < i; x++)
                {

                    con = new SqlConnection(connectionString);
                    con.Open();
                    string sql = "BACKUP DATABASE " + DatabaseName[x] + " TO DISK = '"+ pasta + DatabaseName[x] + "_" +
                                 DateTime.Now.Ticks + ".bak'";
                    cmd = new SqlCommand(sql, con);
                    cmd.CommandTimeout = 0;
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

            }
            catch 
            {
                sendmail(false,emailto,bcc,assunto,frommail,hostmail,usuariomail,senhamail);
              
            }
            con.Close();
            sendmail(true,emailTo,bcc,assunto,frommail,hostmail,usuariomail,senhamail);
            System.Environment.Exit(0);
        }

        private static void sendmail(bool modo,string emailto,string bcc,string assunto,string frommail,string hostmail,string usuariomail,string senhamail)
        {
            MailMessage message = new MailMessage();
            
            message.To.Add(emailto);

            bool IsValidEmail(string bcc)
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(bcc);
                    return addr.Address == bcc;
                }
                catch
                {
                    return false;
                }
            }
            if (IsValidEmail(bcc))
            {
             message.CC.Add(bcc);
            }
            message.Subject = assunto;
            message.From = new MailAddress(frommail);
            if (modo)
            {
                message.Body = "Backup do banco do dia " + DateTime.Now +
                               " Executado com sucesso e salvo na pasta " + pasta;
            }
            else
            {
                message.Body = "Backup do banco do dia " + DateTime.Now +
                               " com ERRO!!!! Não Foi Feito!!!!";
            }


            SmtpClient smtp = new SmtpClient();
            smtp.Host = hostmail;
            smtp.UseDefaultCredentials = false;
            smtp.EnableSsl = true;
            smtp.Port = 587;

            smtp.Credentials = new NetworkCredential(usuariomail, senhamail);
            smtp.Send(message);
            // Attachment anexo = new Attachment(txtAttachmentPath.txt);
            //   message.Attachments.Add(anexo);  
        }





    }
}
