using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Telegram;
using Telegram.Bot;

namespace Matbot
{
    class Program
    {
        [XmlArray(ElementName = "Users")]
        [XmlArrayItem(ElementName = "User")]
        static List<BotUser> users = new List<BotUser>();

        static string ownerpass = "potato";

        static int FindUser(int id)
        {
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].ID == id) return i;
            }
            return -1;
        }

        static int createNewUser(int id)
        {
            BotUser u = new BotUser();
            u.ChangeID(id);
            u.ChangeRank(UserRank.User);

            users.Add(u);

            SaveChanges();

            return users.Count - 1;
        }

        static Backdoor backdoor = new Backdoor();

        static void doRegister(TelegramBotClient client, Telegram.Bot.Types.Message message, int index)
        {
            bool isGroup = message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Group;
            if (isGroup)
            {
                client.SendTextMessageAsync(message.Chat.Id, "Please send me the owner password in private.");
                client.SendTextMessageAsync(message.From.Id, "Send me the owner password.");
            }
            else client.SendTextMessageAsync(message.Chat.Id, "Please send me the owner password.");


            users[index].OperationQueue.Enqueue(new Command(message.Text));
        }

        static void SaveChanges()
        {
            var overrides = new XmlAttributeOverrides();
            var ignore = new XmlAttributes { XmlIgnore = true };
            overrides.Add(typeof(BotUser), "OperationQueue", ignore);

            XmlSerializer ser = new XmlSerializer(typeof(List<BotUser>), overrides);
            FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "MatBotUsers.xml", FileMode.Create);
            ser.Serialize(fs, users);
            fs.Close();
        }

        static void LoadChanges()
        {
            try
            {
                var overrides = new XmlAttributeOverrides();
                var ignore = new XmlAttributes { XmlIgnore = true };
                overrides.Add(typeof(BotUser), "OperationQueue", ignore);

                XmlSerializer ser = new XmlSerializer(typeof(List<BotUser>), overrides);
                FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "MatBotUsers.xml", FileMode.Open);
                System.Xml.XmlReader reader = new System.Xml.XmlTextReader(fs);
                if (ser.CanDeserialize(reader)) users = ser.Deserialize(reader) as List<BotUser>;
                fs.Close();
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (System.Xml.XmlException e)
            {
                Console.WriteLine(e.Message);

            }

        }

        public static string apiid = "220913134:AAHwC-5P5H14sFfx7eDlXYf9qobIx25OFWQ";

        static void Main(string[] args)
        {
            LoadChanges();

            ConsoleShower.HideConsole();

            TelegramBotClient client = new TelegramBotClient(apiid);
            Telegram.Bot.Types.User me = client.GetMeAsync().Result;

            backdoor.client = client;

            int offset = 0;

            while (true)
            {
                Telegram.Bot.Types.Update currupdate = null;
                try
                {

                    bool shouldSaveUsers = false;

                    Telegram.Bot.Types.Update[] updates = client.GetUpdatesAsync(offset).Result;


                    foreach (Telegram.Bot.Types.Update update in updates)
                    {
                        currupdate = update;
                        if (update.Message.Type == Telegram.Bot.Types.Enums.MessageType.TextMessage)
                        {
                            Command cmd = new Command(update.Message.Text);

                            Console.WriteLine(update.Message.From + " wrote: " + update.Message.Text);

                            int i = FindUser(update.Message.From.Id);
                            if (i == -1) i = createNewUser(update.Message.From.Id);

                            if(!cmd.Name.Equals("/register"))
                            {
                                bool registring = false;
                                if(users[i].OperationQueue.Count>0)
                                {
                                    if (users[i].OperationQueue.Peek().Name.Equals("/register")) registring = true;
                                }

                                if(!registring)
                                {
                                    if (users[i].Rank == UserRank.Banned && !cmd.Name.Equals("/register"))
                                    {
                                        users[i].OperationQueue.Clear();
                                        cmd.EmptyCommand();
                                    }
                                    else if (users[i].Rank == UserRank.Gali)
                                    {
                                        users[i].OperationQueue.Clear();
                                        client.SendTextMessageAsync(update.Message.Chat.Id, cmd.Raw());
                                        cmd.EmptyCommand();
                                    }
                                }
                            }
                            


                            if (users[i].OperationQueue.Count != 0)
                            {
                                if (users[i].OperationQueue.Peek().Name.Equals("/shell"))
                                {
                                    if (update.Message.Text.Equals("exit")) users[i].OperationQueue.Dequeue();
                                    backdoor.Input(update.Message.Text);
                                }
                                else if (users[i].OperationQueue.Peek().Name.Equals("/register") && update.Message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Private)
                                {
                                    if (update.Message.Text.ToLower().Equals("cancel"))
                                    {
                                        users[i].OperationQueue.Dequeue();
                                        client.SendTextMessageAsync(update.Message.Chat.Id, "Ownership request canceled.");
                                    }
                                    else if (update.Message.Text.Equals(ownerpass))
                                    {
                                        users[i].OperationQueue.Dequeue();
                                        users[i].ChangeRank(UserRank.Owner);
                                        shouldSaveUsers = true;
                                        client.SendTextMessageAsync(update.Message.Chat.Id, "You are now an Owner.");
                                    }
                                    else
                                    {
                                        client.SendTextMessageAsync(update.Message.Chat.Id, "That is not the correct password for you ownership request.\nTry again or type \"Cancel\".");
                                    }
                                }
                                else if (users[i].OperationQueue.Peek().Name.Equals("/writetofile"))
                                {
                                    try
                                    {
                                        File.WriteAllText(users[i].OperationQueue.Peek().parameters[0], update.Message.Text.Replace("\n", Environment.NewLine));
                                        client.SendTextMessageAsync(update.Message.Chat.Id, "Done!");
                                    }
                                    catch (Exception e)
                                    {
                                        client.SendTextMessageAsync(update.Message.Chat.Id, "Exception Occured!\nSending Info in Private.");
                                        client.SendTextMessageAsync(users[i].ID, "Exception Occured On writetofile: " + users[i].OperationQueue.Peek().Raw() + "\nError Message: " + e.Message);
                                    }
                                    users[i].OperationQueue.Dequeue();
                                }
                            }
                            else if (cmd.Name.Equals("/shell"))
                            {
                                if (users[i].Rank >= UserRank.Owner)
                                {
                                    if (!backdoor.Running)
                                    {
                                        backdoor.Start(update.Message.Chat.Id);
                                        users[i].OperationQueue.Enqueue(new Command("/shell"));
                                    }
                                }
                            }
                            else if (cmd.Name.Equals("/register"))
                            {
                                if (users[i].Rank != UserRank.Owner) doRegister(client, update.Message, i);
                                else client.SendTextMessageAsync(update.Message.Chat.Id, "I already registered you as an Owner!");
                            }
                            else if (cmd.Name.Equals("/myrank"))
                            {
                                if (users[i].Rank == UserRank.User) client.SendTextMessageAsync(update.Message.Chat.Id, "I am sorry, you don't have a rank yet...\nAsk an Owner to upgrade you or use the \"/register\" command.");
                                else client.SendTextMessageAsync(update.Message.Chat.Id, "Your rank is: " + users[i].Rank.ToString() + ".");
                            }
                            else if (cmd.Name.Equals("/myid"))
                            {
                                if (update.Message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Private)
                                {
                                    client.SendTextMessageAsync(update.Message.Chat.Id, "Your ID is: " + update.Message.From.Id + ".");
                                }
                                else
                                {
                                    client.SendTextMessageAsync(update.Message.From.Id, "Your ID is: " + update.Message.From.Id + ".");
                                    client.SendTextMessageAsync(update.Message.Chat.Id, "I sent your ID to you in private.");
                                }
                            }
                            else if (cmd.Name.Equals("/myidpublic"))
                            {
                                client.SendTextMessageAsync(update.Message.Chat.Id, "Your ID is: " + update.Message.From.Id + ".");
                            }
                            else if (cmd.Name.Equals("/speak"))
                            {
                                if (cmd.parameters.Count == 0) client.SendTextMessageAsync(update.Message.Chat.Id, "Usage: /speak [message]");
                                else
                                {
                                    string text = cmd.RawParams();
                                    Stream s = TextToSpeech.SpeakOgg(text);
                                    client.SendVoiceAsync(update.Message.Chat.Id, new Telegram.Bot.Types.FileToSend("matbotspeach.ogg", s));
                                }
                            }
                            else if (cmd.Name.Equals("/start"))
                            {
                                Telegram.Bot.Types.KeyboardButton[][] butt = new Telegram.Bot.Types.KeyboardButton[2][];
                                butt[0] = new Telegram.Bot.Types.KeyboardButton[2];
                                butt[1] = new Telegram.Bot.Types.KeyboardButton[2];
                                butt[0][0] = new Telegram.Bot.Types.KeyboardButton("/ac on 16 turbo");
                                butt[0][1] = new Telegram.Bot.Types.KeyboardButton("/ac off 16 turbo");
                                butt[1][0] = new Telegram.Bot.Types.KeyboardButton("/ac on 25 med");
                                butt[1][1] = new Telegram.Bot.Types.KeyboardButton("/ac off 25 med");
                                client.SendTextMessageAsync(update.Message.Chat.Id, "Hello! I am MatBot!", false, false, 0, new Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup(butt));
                            }
                            else if (cmd.Name.Equals("/upgrade"))
                            {
                                if (users[i].Rank != UserRank.Owner)
                                {
                                    client.SendTextMessageAsync(update.Message.Chat.Id, "You are not an Owner!");
                                }
                                else
                                {
                                    int x = -1;
                                    UserRank newrank;
                                    if (cmd.parameters.Count != 2) client.SendTextMessageAsync(update.Message.Chat.Id, "Invalid parameters!");
                                    else if (!int.TryParse(cmd.parameters[0], out x) || !Enum.TryParse<UserRank>(cmd.parameters[1], out newrank)) client.SendTextMessageAsync(update.Message.Chat.Id, "Invalid parameters!");
                                    else
                                    {
                                        int j = FindUser(x);
                                        if (j == -1) j = createNewUser(x);
                                        users[j].ChangeRank(newrank);
                                        shouldSaveUsers = true;
                                        Telegram.Bot.Types.ChatMember mem = client.GetChatMemberAsync(update.Message.Chat.Id, x).Result;
                                        while (mem == null) ;
                                        client.SendTextMessageAsync(update.Message.Chat.Id, "I upgraded " + mem.User.FirstName + " " + mem.User.LastName + "'s (" + x + ") rank to: " + users[j].Rank.ToString() + "!");
                                        if(users[j].Rank==UserRank.Gali) client.SendTextMessageAsync(update.Message.Chat.Id, "Pfffffttt. What a loser!");
                                    }
                                }
                            }
                            else if (cmd.Name.Equals("/lolmode"))
                            {
                                if (users[i].Rank >= UserRank.Admin)
                                {
                                    client.SendTextMessageAsync(update.Message.Chat.Id, "Never gonna give you up\nNever gonna let you down\nNever gonna run around and desert you\nNever gonna make you cry\nNever gonna say goodbye\nNever gonna tell a lie and hurt you!");
                                    System.Threading.Thread.Sleep(250);
                                }
                                else client.SendTextMessageAsync(update.Message.Chat.Id, "You need to be an admin to activate ze lolmode!!");
                            }
                            else if (cmd.Name.Equals("/sayhi"))
                            {
                                client.SendTextMessageAsync(update.Message.Chat.Id, "Hello!\nI am MatBot!");
                            }
                            else if (cmd.Name.Equals("/botconsole"))
                            {
                                if (users[i].Rank == UserRank.Owner)
                                {
                                    bool failed = false;
                                    if (cmd.parameters.Count != 1) failed = true;
                                    else if (cmd.parameters[0].ToLower().Equals("show")) ConsoleShower.ShowConsole();
                                    else if (cmd.parameters[0].ToLower().Equals("hide")) ConsoleShower.HideConsole();
                                    else failed = false;

                                    if (failed) client.SendTextMessageAsync(update.Message.Chat.Id, "Usage: /botconsole show/hide");
                                }
                                else client.SendTextMessageAsync(update.Message.Chat.Id, "Permission Denied!\nYou need to be an Owner!");
                            }
                            else if (cmd.Name.Equals("/clearusers"))
                            {
                                if (users[i].Rank == UserRank.Owner)
                                {
                                    users.Clear();
                                    shouldSaveUsers = true;

                                    client.SendTextMessageAsync(update.Message.Chat.Id, "Users Table Wiped!");
                                }
                                else client.SendTextMessageAsync(update.Message.Chat.Id, "Permission Denied!\nYou need to be an Owner!");
                            }
                            else if (cmd.Name.Equals("/writetofile"))
                            {
                                if (users[i].Rank == UserRank.Owner)
                                {
                                    bool failed = false;
                                    if (cmd.parameters.Count != 2) failed = true;
                                    else
                                    {
                                        try
                                        {
                                            Console.WriteLine(Environment.ExpandEnvironmentVariables(cmd.parameters[0]));
                                            string path = Path.GetFullPath(Environment.ExpandEnvironmentVariables(cmd.parameters[0]));
                                            if (!Path.IsPathRooted(path)) client.SendTextMessageAsync(update.Message.Chat.Id, "Invalid Path!");
                                            else
                                            {
                                                bool exists = File.Exists(path);
                                                bool create = false;
                                                if (cmd.parameters[1].Equals("new"))
                                                {
                                                    if (exists) client.SendTextMessageAsync(update.Message.Chat.Id, "File already exists.");
                                                    else
                                                    {
                                                        create = true;
                                                    }
                                                }
                                                else if (cmd.parameters[1].Equals("overwrite")) create = true;
                                                else failed = true;

                                                if (!failed && create)
                                                {
                                                    client.SendTextMessageAsync(update.Message.Chat.Id, "Everything good! Send the contents in one message.");
                                                    cmd.parameters[0] = path;
                                                    users[i].OperationQueue.Enqueue(cmd);
                                                }
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            client.SendTextMessageAsync(update.Message.Chat.Id, "Invalid Path!");
                                        }
                                    }

                                    if (failed) client.SendTextMessageAsync(update.Message.Chat.Id, "Usage: /writetofile <path> new/overwrite");
                                }
                                else client.SendTextMessageAsync(update.Message.Chat.Id, "Permission Denied!\nYou need to be an Owner!");
                            }
                            else if (cmd.Name.Equals("/msgp") || cmd.Name.Equals("/msg"))
                            {
                                bool allowed = false;
                                string str = "";
                                if (cmd.Name.Equals("/msgp"))
                                {
                                    if (users[i].Rank >= UserRank.Admin)
                                    {
                                        str = "MatBot: " + update.Message.From.FirstName + " " + update.Message.From.LastName + ": ";
                                        allowed = true;
                                    }
                                    else client.SendTextMessageAsync(update.Message.Chat.Id, "You have to be an Admin to open public message boxes!");
                                }
                                else
                                {
                                    if (users[i].Rank >= UserRank.Owner) allowed = true;
                                    else client.SendTextMessageAsync(update.Message.Chat.Id, "You have to be an Owner to open raw message boxes!");
                                }
                                if (allowed)
                                {
                                    foreach (string s in cmd.parameters) str += " " + s;
                                    Task.Factory.StartNew(() =>
                                    {
                                        MessageBox.Show(str, "", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                    });
                                }

                            }
                            else if (cmd.Name.Equals("/lord"))
                            {
                                string lord = "The path of the righteous man is beset on all sides by the inequities of the selfish and the tyranny of evil men. Blessed is he, who in the name of charity and good will, shepherds the weak through the valley of darkness, for he is truly his brother's keeper and the finder of lost children. And I will strike down upon thee with great vengeance and furious anger those who would attempt to poison and destroy my brothers. And you will know *my name is the Lord* when I lay my vengeance upon thee.";
                                client.SendTextMessageAsync(update.Message.Chat.Id, lord, false, false, 0, null, Telegram.Bot.Types.Enums.ParseMode.Markdown);
                            }
                            else if (cmd.Name.Equals("/mynameis"))
                            {
                                string lord = "Slim Shady!";
                                client.SendTextMessageAsync(update.Message.Chat.Id, lord, false, false, 0, null, Telegram.Bot.Types.Enums.ParseMode.Markdown);
                            }
                            else if (cmd.Name.Equals("/fuckherrightinthepussy"))
                            {
                                client.SendTextMessageAsync(update.Message.Chat.Id, "*Fucks her right in the pussy*", false, false, 0, null, Telegram.Bot.Types.Enums.ParseMode.Markdown);
                            }
                            else if (cmd.Name.Equals("/ac"))
                            {
                                if (users[i].Rank < UserRank.Admin) client.SendTextMessageAsync(update.Message.Chat.Id, "You have to be an Admin to control the AC!");
                                else
                                {
                                    if (cmd.parameters.Count < 1) client.SendTextMessageAsync(update.Message.Chat.Id, "Usage: /ac [on/off] (temp: 16-32) (low-med-high-turbo)");
                                    else
                                    {
                                        bool succeed = true;
                                        bool power = false;
                                        int temp = 16;
                                        ACPower level = ACPower.turbo;
                                        if (cmd.parameters[0].Equals("on")) power = true;
                                        else if (cmd.parameters[0].Equals("off")) power = false;
                                        else client.SendTextMessageAsync(update.Message.Chat.Id, "Usage: /ac [on/off]");

                                        if (cmd.parameters.Count >= 2)
                                        {
                                            succeed = int.TryParse(cmd.parameters[1], out temp);
                                        }
                                        if (cmd.parameters.Count >= 3 && succeed)
                                        {
                                            succeed = Enum.TryParse<ACPower>(cmd.parameters[2], out level);
                                        }
                                        if (succeed) succeed = ACManager.SendAC(temp, power, level);

                                        if (succeed) client.SendTextMessageAsync(update.Message.Chat.Id, "Transmitted to AC!");
                                        else client.SendTextMessageAsync(update.Message.Chat.Id, "Error Writing to Serial Port!");
                                    }
                                }
                            }
                            else if (cmd.Name.Equals("/lyrics"))
                            {
                                string lyrics = LyricsFinder.FindLyrics(cmd.RawParams());
                                client.SendTextMessageAsync(update.Message.Chat.Id, lyrics);
                            }
                            else if (cmd.Name.Equals("/play"))
                            {
                                if (users[i].Rank >= UserRank.Admin)
                                {
                                    if (cmd.parameters.Count == 0) client.SendTextMessageAsync(update.Message.Chat.Id, "Usage: /play [video name]");
                                    else
                                    {
                                        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(cmd.RawParams());
                                        string url = YoutubeParser.ParseVidFromName(cmd.RawParams()).URL;
                                        Backdoor bd = new Backdoor();
                                        bd.Start(0);
                                        bd.Input("start chrome.exe " + url + " --incognito");
                                        bd.Stop();
                                        client.SendTextMessageAsync(update.Message.Chat.Id, "I played the following video: " + url);

                                    }
                                }
                                else client.SendTextMessageAsync(update.Message.Chat.Id, "You have to be an Admin to play a video!");
                            }
                            else if (cmd.Name.Equals("/song"))
                            {
                                if (cmd.parameters.Count == 0) client.SendTextMessageAsync(update.Message.Chat.Id, "Usage: /song [name]");
                                else
                                {
                                    AudioDescriber audio = YoutubeDownloader.DownloadAudioWithProgress(YoutubeParser.ParseVidFromName(cmd.RawParams()), client, update.Message.Chat.Id);
                                    audio.Stream.Position = 0;
                                    //Telegram.Bot.Types.FileToSend f = new Telegram.Bot.Types.FileToSend(audio.Title+".mp3", audio.Stream);
                                    BotExtensions.SendAudioFile(apiid, update.Message.Chat.Id, audio);

                                    
                                }
                            }
                            else if (cmd.Name.Equals("/killchrome"))
                            {
                                if (users[i].Rank >= UserRank.Owner)
                                {
                                    if (cmd.parameters.Count != 0) client.SendTextMessageAsync(update.Message.Chat.Id, "Usage: /exceptionreg");
                                    else
                                    {
                                        Backdoor bd = new Backdoor();
                                        bd.Start(0);
                                        bd.Input("TASKKILL /IM chrome.exe /F");
                                        bd.Stop();
                                    }
                                }
                                else client.SendTextMessageAsync(update.Message.Chat.Id, "You have to be an Owner to kill chrome!");
                            }
                            else if (cmd.Name.Equals("/exceptionreg"))
                            {
                                if (users[i].Rank >= UserRank.Owner)
                                {
                                    if (cmd.parameters.Count != 1) client.SendTextMessageAsync(update.Message.Chat.Id, "Usage: /exceptionreg [on/off]");
                                    else
                                    {
                                        if (cmd.parameters[0].Equals("on")) users[i].exceptionRegister = true;
                                        else if (cmd.parameters[0].Equals("off")) users[i].exceptionRegister = false;
                                        else client.SendTextMessageAsync(update.Message.Chat.Id, "Usage: /exceptionreg [on/off]");

                                        shouldSaveUsers = true;

                                        client.SendTextMessageAsync(update.Message.Chat.Id, "Applied!");
                                    }
                                }
                                else client.SendTextMessageAsync(update.Message.Chat.Id, "You have to be an Owner to register for exceptions!");
                            }

                            if (shouldSaveUsers) SaveChanges();

                        }
                        else if (update.Message.Type == Telegram.Bot.Types.Enums.MessageType.ContactMessage)
                        {
                            client.SendTextMessageAsync(update.Message.Chat.Id, update.Message.Contact.FirstName + " " + update.Message.Contact.LastName + "'s Telegram ID is:" + update.Message.Contact.UserId);
                        }

                        offset = update.Id + 1;
                        System.Threading.Thread.Sleep(60);
                    }
                }
                catch (Exception ex)
                {
                    foreach (BotUser user in users)
                    {
                        if (user.exceptionRegister)
                        {
                            string s = "An exception occurred on: " + DateTime.Now.ToString() + ".\nMessage: " + ex.Message + ".\nInnerException: " + ex.InnerException;
                            client.SendTextMessageAsync(user.ID, s);
                        }
                        
                    }
                    offset = currupdate.Id + 1;
                }
            }
        }

    }
}
