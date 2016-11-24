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
using System.Text.RegularExpressions;

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
                string currencyResult = null;

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
                string errOutput = "Hello";
                string homeCurrency = null;

                //help tab

                if (userMessage.ToLower().Equals("help"))
                {

                    endOutput = "This is the **HELP** section." +

                    " Below is the list of things I can do:\n\n " +

                    "* **'help'** brings up this list.  \n\n " +

                    "* **'info'** gives you a link to our website.  \n\n " +

                    "* **'currencies'** gives you a list of supported currencies.  \n\n " +

                    "* **'base currency'** tells you which baseline currency you are converting against.  \n\n " +

                    "* **'exchange'** followed by a currency will convert your **base currency** against it.  \n\n " +

                    "* **'clear'** clears your current user data, such as logged base currencies.  \n\n ";



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

                //currency list

                if (userMessage.ToLower().Equals("currencies"))
                {

                    endOutput = "**Supported Currencies**:" +

                    " Below is the list of currencies I can calculate:\n\n " +

                    "* **AUD**: Australian Dollars  \n\n " +

                    "* **CAD**: Canadian Dollars  \n\n " +

                    "* **CNY**: Chinese Yuan  \n\n " +

                    "* **EUR**: Euro  \n\n " +

                     "* **GBP**: British Pounds  \n\n " +

                    "* **HKD**: Hong Kong Dollars  \n\n " +

                    "* **INR**: Indian Rupee  \n\n " +

                    "* **KRW**: South Korean Won  \n\n " +

                    "* **JPY**: Japanese Yen  \n\n " +

                     "* **NZD**: New Zealand Dollars  \n\n " +

                     "* **USD**: US Dollars \n\n ";
                    

                    isCurrencyRequest = false;


                }


                //currency list



                //set home currency
                if (userMessage.Length > 8)
                {
                    if (userMessage.ToLower().Substring(0, 8).Equals("set base"))
                    {
                        homeCurrency = userMessage.Substring(9);
                        userData.SetProperty<string>("HomeCurrency", homeCurrency);
                        await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                        endOutput = ($"Your base currency has been set to {homeCurrency}");
                        isCurrencyRequest = false;
                    }
                }
                //check current home currency

                if (userMessage.ToLower().Equals("base currency"))
                {
                    string homecurrency = userData.GetProperty<string>("HomeCurrency");
                    if (homecurrency == null)
                    {
                        endOutput = "Your base currency is currently unassigned. You can assign a currency by typing 'set base *your currency here*'";
                        isCurrencyRequest = false;

                    }
                    else
                    {
                        endOutput = ($"Your base currency is set to {homecurrency}");
                        activity.Text = homecurrency;
                        isCurrencyRequest = false;
                    }
                }

                string toExchange;

          

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

            

                   
                        // if request
                        if (!isCurrencyRequest)
                {
                    Activity infoReply = activity.CreateReply(endOutput);
                    await connector.Conversations.ReplyToActivityAsync(infoReply);
                }

                


                else
                {
                    //api things


                    //currency exchange test
                    if (userMessage.Length > 8)
                    {
                        if (userMessage.ToLower().Substring(0, 8).Equals("exchange"))
                        {
                            {
                                string homecurrency = userData.GetProperty<string>("HomeCurrency");
                                if (homecurrency == null)
                                {
                                    errOutput = "Your base currency is currently unassigned. You can assign a currency by typing 'set base *your currency here*'";
                                    Activity errorReply = activity.CreateReply(errOutput);
                                    await connector.Conversations.ReplyToActivityAsync(errorReply);


                                }
                                else
                                {
                                    endOutput = ($"Your base currency is set to {homecurrency}");
                                    activity.Text = homecurrency;

                                }
                            }

                            //api things

                            HttpClient client = new HttpClient();
                            string x = await client.GetStringAsync(new Uri("http://api.fixer.io/latest?base=" + activity.Text));
                            CurrencyObject.RootObject rootObject;
                            rootObject = JsonConvert.DeserializeObject<CurrencyObject.RootObject>(x);

                            toExchange = userMessage.Substring(9);

                            //convert to array maybe?

                            if (toExchange.ToLower().Equals("aud"))
                                currencyResult = rootObject.rates.AUD + (" Australian Dollars");
                            if (toExchange.ToLower().Equals("cad"))
                                currencyResult = rootObject.rates.CAD + (" Canadian Dollars");
                            if (toExchange.ToLower().Equals("cny"))
                                currencyResult = rootObject.rates.CNY + (" Chinese Yuan");
                            if (toExchange.ToLower().Equals("eur"))
                                currencyResult = rootObject.rates.EUR + (" Euros");
                            if (toExchange.ToLower().Equals("hkd"))
                                currencyResult = rootObject.rates.HKD + (" Hong Kong Dollars");
                            if (toExchange.ToLower().Equals("inr"))
                                currencyResult = rootObject.rates.INR + (" Indian Rupees");
                            if (toExchange.ToLower().Equals("krw"))
                                currencyResult = rootObject.rates.KRW + (" South Korean Won");
                            if (toExchange.ToLower().Equals("jpy"))
                                currencyResult = rootObject.rates.JPY + (" Japanese Yen");
                            if (toExchange.ToLower().Equals("nzd"))
                                currencyResult = rootObject.rates.NZD + (" New Zealand Dollars");
                            if (toExchange.ToLower().Equals("gbp"))
                                currencyResult = rootObject.rates.GBP + (" British Pounds");
                            if (toExchange.ToLower().Equals("usd"))
                                currencyResult = rootObject.rates.USD + (" US Dollars");

                            if (currencyResult.Equals(""))

                            {
                                Activity noreply = activity.CreateReply("that currency isn't supported.");
                            }
                            
                           

                            Activity reply = activity.CreateReply($"1 {activity.Text} is currently worth {currencyResult}.");
                            await connector.Conversations.ReplyToActivityAsync(reply);

                        }

                        else {
                            Activity noreply = activity.CreateReply("that currency isn't supported.");
                    }
                    }




                    //end fluff


                
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