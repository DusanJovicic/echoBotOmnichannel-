using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System.Collections.Generic;
using EchoBotOmni.Command;
using System.Diagnostics;

namespace EchoBotOmni
{
    public static class OmnichannelBotClient
    {
        /// <summary>
        /// Delivery mode of bot's reply activity
        /// </summary>
        private const string DeliveryMode = "deliveryMode";
        /// <summary>
        /// Delivery Mode value bridged
        /// </summary>
        private const string Bridged = "bridged";
        /// <summary>
        /// Custom data tag
        /// </summary>
        private const string Tags = "tags";

        /// <summary>
        /// Adds Omnichannel for Customer Service escalation context to the bot's reply activity.
        /// </summary>
        /// <param name="activity">Bot's reply activity</param>
        /// <param name="contextVars">Omnichannel for Customer Service Workstream context variable value pairs</param>
        public static void AddEscalationContext(IActivity activity, Dictionary<string, object> contextVars)
        {
            Command.Command command = new Command.Command
            {
                Type = CommandType.Escalate,
                Context = contextVars
            };

            string serializedString = JsonConvert.SerializeObject(command);
            if (activity.ChannelData != null)
            {
                (activity as IActivity).ChannelData[Tags] = serializedString;
            }
            else
            {
                activity.ChannelData = new Dictionary<string, object>() { { Tags, serializedString } };
            }
        }

        /// <summary>
        /// Adds Omni-channel end conversation context to the bot's reply activity.
        /// </summary>
        /// <param name="activity">Bot's reply activity</param>
        public static void AddEndConversationContext(IActivity activity)
        {
            Command.Command command = new Command.Command
            {
                Type = CommandType.EndConversation,
                Context = new Dictionary<string, object>()
            };

            string serializedString = JsonConvert.SerializeObject(command);
            if (activity.ChannelData != null)
            {
                (activity as IActivity).ChannelData[Tags] = serializedString;
            }
            else
            {
                activity.ChannelData = new Dictionary<string, object>() { { Tags, serializedString } };
            }
        }

        /// <summary>
        /// Sets delivery mode for bot as bridged so that Customer can see bot messages
        /// </summary>
        /// <param name="activity">Bot's reply activity</param>
        public static string BridgeBotMessage(IActivity activity)
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            if (activity.ChannelData != null)
            {
                (activity as IActivity).ChannelData[DeliveryMode] = Bridged;
            }
            else
            {
                activity.ChannelData = new Dictionary<string, object>() { { DeliveryMode, Bridged } };
            }
            s.Stop();
            return s.Elapsed.ToString();
        }
    }
}
