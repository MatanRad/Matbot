using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Client;
using Matbot.Services;

namespace Matbot.Commands
{
    class ServiceCommand : Structure.Command
    {
        public enum ServiceCMD
        {
            register,
            unregister,
            kill
        }


        public ServiceCommand() : base("service")
        {
            
        }

        public override void Execute(Message message)
        {
            message.Reply("Running Services:\n" + message.Client.Bot.SrvManager.GetAllServicesString(message.Chat.Id));
        }

        public void Execute(Message m, ServiceCMD cmd, int serviceId)
        {
            Service s = m.Client.Bot.SrvManager.FindServiceByID(serviceId);
            if(s==null)
            {
                m.Reply("Couldn't find Service with ID: " + serviceId.ToString("D" + m.Client.Bot.SrvManager.MaxIDNum.ToString().Length));
                return;
            }

            if (!(s is RegisterableService))
            {
                m.Reply("This service is not a registerable service!");
                return;
            }

            RegisterableService ser = s as RegisterableService;

            if (m.User.BotRank < ser.RequiredRank) m.Reply("In order to register to this script you need have rank: "+ser.RequiredRank.ToString()+"!");
            else
            {
                switch (cmd)
                {
                    case ServiceCMD.register:
                        try
                        {
                            ser.Register(m.Chat);
                            m.Reply("Done!");
                        }
                        catch(Matbot.Services.Exceptions.UserAlreadyRegisteredException)
                        {
                            m.Reply("You are already registered to this service!");
                        }
                        break;
                    case ServiceCMD.unregister:
                        try
                        {
                            ser.Unregister(m.Chat);
                            m.Reply("Done!");
                        }
                        catch (Matbot.Services.Exceptions.UserNotRegisteredException)
                        {
                            m.Reply("You aren't registered to this service!");
                        }
                        break;
                    case ServiceCMD.kill:
                        if (m.User.BotRank < UserRank.Admin) m.Reply("You need to be an Admin in order to kill services!");
                        else
                        {
                            ser.Stop();
                            m.Reply("Serive "+serviceId+" was killed!");
                        }
                        break;
                }

            }
        }
    }
}
