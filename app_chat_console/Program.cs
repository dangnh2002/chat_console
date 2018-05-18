using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace app_chat_console
{
    class Program
    {
        static Timer timer = new Timer(1000);
        public static app_chat_consoleEntities context = new app_chat_consoleEntities();//kết nối tới database
        private static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            list_mess(Session.channel_name, Session.pass, Session.username);
        }
        public static List<mess> list_chat { get; set; }
        private static void list_mess(string channel_name, string pass, string username)
        {
            //lấy danh sách tin nhắn và in ra
            if (Session.firstload == true)
            {
                //nếu load list lần đầu
                list_chat = context.messes.Where(x => x.channel_name == channel_name).ToList();
                if (list_chat != null)
                {
                    foreach (var line in list_chat)
                    {
                        Console.WriteLine(line.username + ": " + line.content);
                        if (list_chat.OrderBy(x => x.id_mess).Take(1).SingleOrDefault().id_mess != 0)
                        {
                            Session.id_mess_last = list_chat.OrderByDescending(x => x.id_mess).Take(1).SingleOrDefault().id_mess;//lấy id_mess của bản ghi cuối cùng trong bảng mess
                        }
                    }
                    Session.firstload = false;
                }
            }
            else if (Session.firstload == false)
            {
                //load lần 2 trở đi
                //lấy id_mess của tin nhắn cuối cùng của bản ghi trong bảng mess và đem so sánh với id_mess_firstload xem nó đã được in ra chưa
                list_chat = context.messes.Where(x => x.channel_name == channel_name).OrderByDescending(x => x.id_mess).Take(1).ToList();
                if (list_chat.Count() != 0)
                {
                    Session.id_mess = list_chat.SingleOrDefault().id_mess;
                    Session.user = list_chat.SingleOrDefault().username;
                    if (Session.user != username && Session.id_mess != Session.id_mess_last)
                    {
                        //trường hợp bản ghi chưa được in ra và cũng không phải là của mình chat thì in ra màn hình
                        //trường hợp bản ghi chưa được in ra và nó là của mình thì không in gì cả
                        Console.WriteLine(list_chat.SingleOrDefault().username + ": " + list_chat.SingleOrDefault().content);
                        Session.id_mess_last = Session.id_mess;
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            try
            {
                //khai báo biến
                string mess, create, statement;
                Console.OutputEncoding = Encoding.UTF8;//hiển thị viết tiếng việt có dấu
                Console.InputEncoding = Encoding.Unicode;//viết dấu khi gửi tin chat
                Session.username = Console.ReadLine();//tạo tài khoản
                begin:
                Console.WriteLine("Tạo kênh chat hay không ?");
                statement = Console.ReadLine();//xử lý việc tạo hay không vả xử lý biến channel_name, pass truyền vào
                string[] arr = statement.Trim().Split(' ');
                create = arr[0];
                Session.channel_name = arr[1];
                Session.pass = arr[2];
                if (create == "/yes")
                {
                    //trường hợp có tạo kênh chat
                    var query = context.channels.Where(x => x.channel_name == Session.channel_name).SingleOrDefault();
                    if (query == null)
                    {
                        //trường họp chưa tồn tại kênh chat
                        //tạo kênh chat mới
                        var c = new channel
                        {
                            channel_name = Session.channel_name,
                            pass = Session.pass,
                            total_user = 1,
                        };
                        context.Entry(c).State = System.Data.Entity.EntityState.Added;
                        context.SaveChanges();
                        Console.WriteLine("Tạo kênh chat thành công !!!");
                        //đưa ra danh sách tin nhắn
                        timer.Elapsed += timer_Elapsed;
                        timer.Start();
                        //xử lý đoạn chat của người dùng
                        chat_again:
                        mess = Console.ReadLine();
                        if (mess == "/quit")
                        {
                            //trường hợp muốn rời kênh chat
                            var channel = context.channels.Where(x => x.channel_name == Session.channel_name && x.pass == Session.pass).FirstOrDefault();
                            channel.total_user -= 1;
                            if (channel.total_user != 0)
                                context.Entry(channel).State = System.Data.Entity.EntityState.Modified;
                            else
                                context.Entry(channel).State = System.Data.Entity.EntityState.Deleted;
                            context.SaveChanges();
                            Session.channel_name = "";
                            Session.pass = "";
                            Console.WriteLine("Thoát kênh chat thành công");
                            goto begin;
                        }
                        //đổi màu chữ
                        else if (mess == "/red")
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            goto chat_again;
                        }
                        else if (mess == "/blue")
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            goto chat_again;
                        }
                        else if (mess == "/white")
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            goto chat_again;
                        }
                        else if (mess == "/darkcyan")
                        {
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            goto chat_again;
                        }
                        else if (mess == "/darkblue")
                        {
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            goto chat_again;
                        }
                        else if (mess == "/darkgreen")
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            goto chat_again;
                        }
                        else
                        {
                            //thêm tin nhắn vào database
                            var m = new mess
                            {
                                channel_name = Session.channel_name,
                                username = Session.username,
                                content = mess,
                            };
                            context.Entry(m).State = System.Data.Entity.EntityState.Added;
                            context.SaveChanges();
                            goto chat_again;//quay lại đoạn xử lý tin nhắn 
                        }
                    }
                    else
                    {
                        //trường hợp đã tồn tại kênh chat
                        Console.WriteLine("Kênh chat đã tồn tại");
                        goto begin;
                    }
                }
                if (create == "/no")
                {
                    //trường hợp không tạo kênh chat
                    var query = context.channels.Where(x => x.channel_name == Session.channel_name && x.pass == Session.pass).SingleOrDefault();
                    if (query != null)
                    {
                        //trường hợp tồn tại kênh chat và tài khoản đúng
                        query.total_user += 1;
                        context.Entry(query).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                        Console.WriteLine("Vào kênh chat thành công !!!");
                        timer.Elapsed += timer_Elapsed;
                        timer.Start();
                        chat_again:
                        mess = Console.ReadLine();
                        if (mess == "/quit")
                        {
                            //trường hợp muốn rời kênh chat
                            var channel = context.channels.Where(x => x.channel_name == Session.channel_name && x.pass == Session.pass).FirstOrDefault();
                            channel.total_user -= 1;
                            if (channel.total_user != 0)
                                context.Entry(channel).State = System.Data.Entity.EntityState.Modified;
                            else
                                context.Entry(channel).State = System.Data.Entity.EntityState.Deleted;
                            context.SaveChanges();
                            Session.channel_name = "";
                            Session.pass = "";
                            Console.WriteLine("Thoát kênh chat thành công");
                            goto begin;
                        }
                        else if (mess == "/red")
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            goto chat_again;
                        }
                        else if (mess == "/blue")
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            goto chat_again;
                        }
                        else if (mess == "/white")
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            goto chat_again;
                        }
                        else if (mess == "/darkcyan")
                        {
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            goto chat_again;
                        }
                        else if (mess == "/darkblue")
                        {
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            goto chat_again;
                        }
                        else if (mess == "/darkgreen")
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            goto chat_again;
                        }
                        else
                        {
                            //thêm tin nhắn vào database
                            var m = new mess
                            {
                                channel_name = Session.channel_name,
                                username = Session.username,
                                content = mess,
                            };
                            context.Entry(m).State = System.Data.Entity.EntityState.Added;
                            context.SaveChanges();
                            goto chat_again;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Vào kênh chat không thành công !!!");
                        goto begin;
                    }
                }
                else
                {
                    //trường hợp nhập sai cú pháp
                    Console.WriteLine("Bạn đã nhập sai cú pháp");
                    goto begin;
                }
            }
            catch (Exception ex)
            {
                //trường hợp nhập sai cú pháp
                Console.WriteLine("Có lỗi xảy ra. Phần mềm sẽ đóng sau khi ấn phím bất kỳ!");
                Console.Read();
            }
        }
    }
}