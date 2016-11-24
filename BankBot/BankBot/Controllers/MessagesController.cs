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
using System.Collections.Generic;

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
                bool isCurrencyRequest = true;
                bool saidhello = false;

                StateClient stateClient = activity.GetStateClient();
                BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);




                //Greetings

                string Greeter = "Hello for the first time!";



                if (userData.GetProperty<bool>("SentGreeting"))
                {
                    Greeter = "Hello Again";
                    saidhello = true;
                    Activity repeatReply = activity.CreateReply(Greeter);

                }
                else
                {
                    userData.SetProperty<bool>("SentGreeting", true);
                    await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);

                }

                if (saidhello == false)
                {
                    Activity greetReply = activity.CreateReply(Greeter);
                    await connector.Conversations.ReplyToActivityAsync(greetReply);

                    // card test

                    Activity startReply = activity.CreateReply("I am ContosoBot, Contoso Bank's first account and currency exchange manager bot! Type 'help' for what I can do!");
                    startReply.Recipient = activity.From;
                    startReply.Type = "message";
                    startReply.Attachments = new List<Attachment>();

                    List<CardImage> startImages = new List<CardImage>();
                    startImages.Add(new CardImage(url: "http://orig15.deviantart.net/2f12/f/2016/328/c/9/contososmall_by_llamadoodle-dapj2n0.png"));

                    List<CardAction> startButtons = new List<CardAction>();
                    CardAction sButton = new CardAction()
                    {
                        Value = "http://msap2bankbot.azurewebsites.net/",
                        Type = "openUrl",
                        Title = "Visit Our Site"
                    };
                    startButtons.Add(sButton);


                    HeroCard sCard = new HeroCard()
                    {
                        Title = "Contoso Bank",
                        Images = startImages,
                        Buttons = startButtons,
                        Subtitle = "Contoso Bank - Connecting the world, one cent at a time",
                    };

                    Attachment sAttachment = sCard.ToAttachment();
                    startReply.Attachments.Add(sAttachment);


                    await connector.Conversations.SendToConversationAsync(startReply);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                // end greetings



                string endOutput = "Hello";

                //help tab

                if (userMessage.ToLower().Equals("help"))
                {

                    endOutput = "This is the **HELP** section." +

                    " Below is the list of things I can do:\n\n " +

                    "* **'help'** brings up this list.  \n\n " +

                    "* **'info'** gives you a link to our website.  \n\n " +

                    "* **'clear'** clears your current user data.  \n\n ";



                    isCurrencyRequest = false;

                }

                //help tab

                //clearing data
                if (userMessage.ToLower().Equals("clear"))
                {
                    endOutput = "User data has been cleared.";
                    await stateClient.BotState.DeleteStateForUserAsync(activity.ChannelId, activity.From.Id);
                    isCurrencyRequest = false;
                }
                //set home currency
                if (userMessage.Length > 13)
                {
                    if (userMessage.ToLower().Substring(0, 13).Equals("home currency"))
                    {
                        string homeCurrency = userMessage.Substring(14);
                        userData.SetProperty<string>("HomeCurrency", homeCurrency);
                        await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                        endOutput = ($"Your home currency has been set to {homeCurrency}");
                        isCurrencyRequest = false;
                    }
                }
                //check current home currency

                if (userMessage.ToLower().Equals("xchange home"))
                {
                    string homecurrency = userData.GetProperty<string>("HomeCurrency");
                    if (homecurrency == null)
                    {
                        endOutput = "Your home currency is currently unassigned.";
                        isCurrencyRequest = false;

                    }
                    else
                    {
                        activity.Text = homecurrency;
                    }
                }
                // msa card test

                if (userMessage.ToLower().Equals("info"))
                {
                    Activity startReply = activity.CreateReply("I am ContosoBot, Contoso Bank's first account and currency exchange manager bot! Type 'help' for what I can do!");
                    startReply.Recipient = activity.From;
                    startReply.Type = "message";
                    startReply.Attachments = new List<Attachment>();

                    List<CardImage> startImages = new List<CardImage>();
                    startImages.Add(new CardImage(url: "http://orig15.deviantart.net/2f12/f/2016/328/c/9/contososmall_by_llamadoodle-dapj2n0.png"));

                    List<CardAction> startButtons = new List<CardAction>();
                    CardAction sButton = new CardAction()
                    {
                        Value = "http://msap2bankbot.azurewebsites.net/",
                        Type = "openUrl",
                        Title = "Visit Our Site"
                    };
                    startButtons.Add(sButton);


                    HeroCard sCard = new HeroCard()
                    {
                        Title = "Contoso Bank",
                        Images = startImages,
                        Buttons = startButtons,
                        Subtitle = "Contoso Bank - Connecting the world, one cent at a time",
                    };

                    Attachment sAttachment = sCard.ToAttachment();
                    startReply.Attachments.Add(sAttachment);


                    await connector.Conversations.SendToConversationAsync(startReply);
                    return Request.CreateResponse(HttpStatusCode.OK);

                }





                // if weather request
                if (!isCurrencyRequest)
                {
                    Activity infoReply = activity.CreateReply(endOutput);
                    await connector.Conversations.ReplyToActivityAsync(infoReply);
                }

                else
                {

                    //api things

                    //if (userMessage.Length > 8)
                    //{
                    //    if (userMessage.ToLower().Substring(0, 8).Equals("xchange"))
                    //    {
                    //        string homeCurrency = userMessage.Substring(9);                         
                    //        endOutput = ($"Your home currency has been set to {homeCurrency}");
                    //        isCurrencyRequest = false;
                    //    }
                    //}







                    string baseCurrency = activity.Text;

                    HttpClient client = new HttpClient();
                    string x = await client.GetStringAsync(new Uri("http://api.fixer.io/latest?base=" + baseCurrency));
                    CurrencyObject.RootObject rootObject;
                    rootObject = JsonConvert.DeserializeObject<CurrencyObject.RootObject>(x);

                    string aus = rootObject.rates.AUD + "dollars";

                    //reply to user

                    Activity reply = activity.CreateReply($"Current AUS dollars = {aus}");
                    await connector.Conversations.ReplyToActivityAsync(reply);
                }
            }


            else
            {
                HandleSystemMessage(activity);

            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }




        //the rest
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