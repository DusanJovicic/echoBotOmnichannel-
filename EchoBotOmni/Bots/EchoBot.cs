using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace EchoBotOmni.Bots
{
    public class EchoBot : ActivityHandler
    {
        private readonly ILogger _logger;
        public EchoBot(ILogger<EchoBot> logger)
        {
            _logger = logger;
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            Debug.WriteLine("Message received: " + turnContext.Activity.Text + " || " + DateTime.Now);
            _logger.LogCritical("Message received: " + turnContext.Activity.Text + " || " + DateTime.Now);
            
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                IActivity replyActivity = null;
                if (turnContext.Activity.Text.StartsWith("{"))
                {
                     replyActivity = MessageFactory.Text($"Welcome User");
                }
                else {
                     replyActivity = MessageFactory.Text($"Echoo: {turnContext.Activity.Text}");

                    // Replace with your own condition for bot escalation
                    if (turnContext.Activity.Text.Equals("escalate", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Debug.WriteLine("Escalate triggered");
                        _logger.LogDebug("Escalate triggered");

                        Dictionary<string, object> contextVars = new Dictionary<string, object>() { { "BotHandoffTopic", "CreditCard" } };
                        OmnichannelBotClient.AddEscalationContext(replyActivity, contextVars);
                    }
                    // Replace with your own condition for bot end conversation
                    else if (turnContext.Activity.Text.Equals("endconversation", StringComparison.InvariantCultureIgnoreCase))
                    {
                        _logger.LogDebug("End conversation triggered");
                        Debug.WriteLine("End conversation triggered");

                        OmnichannelBotClient.AddEndConversationContext(replyActivity);
                    }
                    // Call method BridgeBotMessage for every response that needs to be delivered to the customer.
                    else
                    {
                        var time = OmnichannelBotClient.BridgeBotMessage(replyActivity);
                        Debug.WriteLine("BridgeBotMessage time: " + time);
                        _logger.LogCritical("BridgeBotMessage time: " + time);
                    }
                }
                _logger.LogCritical("Message response: " + turnContext.Activity.Text + " || " + DateTime.Now);
                Debug.WriteLine("Message response: " + turnContext.Activity.Text + " || " + DateTime.Now);

                await turnContext.SendActivityAsync(replyActivity, cancellationToken);
            }
        }

        /// <summary>
        /// This method is called when there is a participant added to the chat.
        /// </summary>
        /// <param name="membersAdded">Member being added to the chat</param>
        /// <param name="turnContext">TurnContext</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns></returns>
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    //Set the bridge mode for every message that needs to be delivered to customer
                    OmnichannelBotClient.BridgeBotMessage(turnContext.Activity);
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Welcome to Echo Bot."), cancellationToken);
                }
            }
        }
    }
}