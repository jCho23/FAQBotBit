using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.ProjectOxford.Text.Sentiment;

namespace FAQBotBit.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        Microsoft.ProjectOxford.Text.Sentiment.SentimentClient _cognitiveClient;

        public RootDialog()
        {
            _cognitiveClient = new Microsoft.ProjectOxford.Text.Sentiment.SentimentClient(ConfigurationManager.AppSettings["2a2ab96ab54b4404a67e3ef637dbb6e4"]);
        }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            var document = new SentimentDocument()
            {
                Id= "23",
                Text = activity.Text,
                Language = "en"      
            };

            var request = new SentimentRequest();
            request.Documents.Add(document);

            var response = await _cognitiveClient.GetSentimentAsync(request);
            var retrivedDocument = response.Documents.FirstOrDefault();

            var sentimentScore = retrivedDocument.Score;
            
            // calculate something for us to return
            int length = (activity.Text ?? string.Empty).Length;

            // return our reply to the user
            await context.PostAsync($"You sent {activity.Text} which was {length} characters");

            context.Wait(MessageReceivedAsync);
        }
    }
}