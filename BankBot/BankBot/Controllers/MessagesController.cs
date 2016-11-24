using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using BankBot.Models;

namespace BankBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                string userMessage = activity.Text;

                StateClient stateClient = activity.GetStateClient();
                BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);

                //hello checker
                string endOutput = "Hello";

               if (userData.GetProperty<bool>("SentGreeting"))
                {
                    endOutput = "Hello Again";
                }
                else
                {
                    userData.SetProperty<bool>("SentGreeting", true);
                    await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                }
               //clearing data
               if (userMessage.ToLower().Equals("clear"))
                {
                    endOutput = "User data has been cleared.";
                    await stateClient.BotState.DeleteStateForUserAsync(activity.ChannelId, activity.From.Id);
                }
               //set home currency
               if (userMessage.Length > 13)
                {
                    if (userMessage.ToLower().Substring(0, 12).Equals("set currency"))
                    {
                        string homeCurrency = userMessage.Substring(13);
                        userData.SetProperty<string>("HomeCurrency", homeCurrency);
                        await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                        endOutput = ($"Your home currency has been set to {homeCurrency}");
                    }
                }
                //check current home currency

                if (userMessage.ToLower().Equals("home"))
                {
                    string homecurrency = userData.GetProperty<string>("HomeCurrency");
                    if (homecurrency == null)
                    {
                        endOutput = "Your home currency is currently unassigned.";
                        
                    }
                    else
                    {
                        activity.Text = homecurrency;
                    }
                }


                Activity infoReply = activity.CreateReply(endOutput);
                await connector.Conversations.ReplyToActivityAsync(infoReply);


      //api things
                HttpClient client = new HttpClient();
                string x = await client.GetStringAsync(new Uri("http://api.fixer.io/latest?base=" + activity.Text));
                CurrencyObject.RootObject rootObject;
                rootObject = JsonConvert.DeserializeObject<CurrencyObject.RootObject>(x);

                string aus = rootObject.rates.AUD + "dollars";

    //reply to user
           
                Activity reply = activity.CreateReply($"Current AUS dollars = {aus}");
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}