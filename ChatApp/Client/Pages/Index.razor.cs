using ChatApp.Client.Helper;
using ChatApp.Server.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using System;
using System.Net.Http.Json;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;

namespace ChatApp.Client.Pages
{
    public partial class Index : IAsyncDisposable
    {
        [Inject]
        public NavigationManager Navigation { get; set; }
        [Inject]
        public HttpClient Http { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        private string newMessageContent = string.Empty;
        private string newTagName = string.Empty;
        private bool tagCreationError;

        private List<Tag> selectedTags = new();
        private List<Tag>? allDbTags = new();
        private List<Tag> helpCompleteTags = new();
        private HubConnection? hubConnection;
        private List<Message>? messages;
        private ElementReference scrollableDiv;

        private const string apiBase = "api/Chat";

        protected override async Task OnAfterRenderAsync(bool firstRender) => await ScrollToLatestMessages();

        private async Task ScrollToLatestMessages() => await JSRuntime.InvokeVoidAsync("scrollToBottom", scrollableDiv);

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
            hubConnection = new HubConnectionBuilder()
                .WithUrl(Navigation.ToAbsoluteUri("/chathub"))
                .Build();
            RegisterHubHandlers();
            await hubConnection.StartAsync();
        }

        private void RegisterHubHandlers()
        {
            if (hubConnection == null) return;
            hubConnection.On<Message>("ReceiveMessage", message =>
            {
                if (message.Tags.Count == 0 || message.Tags.Any(t => selectedTags.Any(st => st.Name == t.Name)))
                    messages?.Add(message);
                StateHasChanged();
            });
            hubConnection.On<List<Tag>>("GetUpdatedTagsList", newTags =>
            {
                allDbTags?.AddRange(newTags);
                allDbTags = allDbTags?.OrderBy(t => t.Name).ToList();
                selectedTags.AddRange(newTags);
                selectedTags = selectedTags.OrderBy(t => t.Name).ToList();
                StateHasChanged();
            });
        }

        private async Task LoadData()
        {
            var uriAllMessages = Navigation.ToAbsoluteUri($"{apiBase}/GetAllMessages");
            messages = await Http.GetFromJsonAsync<List<Message>>(uriAllMessages);
            var uriAllTags = Navigation.ToAbsoluteUri($"/{apiBase}/GetAllTags");
            allDbTags = await Http.GetFromJsonAsync<List<Tag>>(uriAllTags);
            if (allDbTags != null)
                selectedTags = new List<Tag>(allDbTags);
        }

        private async Task SendMessage()
        {
            if (hubConnection is null || string.IsNullOrWhiteSpace(newMessageContent)) return;
            var newMessage = await PostMessageToApiAsync();
            var newTags = newMessage.Tags.Where(tg => !allDbTags.Any(mt => tg.Name == mt.Name)).ToList();
            if (newTags != null && newTags.Count > 0)
                await hubConnection.InvokeAsync("UpdateTagsList", newTags);
            foreach (var tag in newMessage.Tags.Where(t => !selectedTags.Any(st => st.Name == t.Name) && allDbTags.Any(dt => dt.Name == t.Name)).ToList()) // make a sent tag selected after sending a message
            {
                selectedTags.Add(tag);
                await GetMessagesByTags();
            }
            await hubConnection.SendAsync("SendMessage", newMessage);
        }

        private async Task<Message> PostMessageToApiAsync()
        {
            var uri = Navigation.ToAbsoluteUri($"{apiBase}/SendMessage");
            var newMessageTags = TagRegexHelper.GetTagsFromString(newMessageContent);
            var message = new Message { Content = newMessageContent, Tags = newMessageTags };
            await Http.PostAsJsonAsync(uri, message);
            newMessageContent = string.Empty;
            helpCompleteTags.Clear();
            return message;
        }

        public bool IsConnected =>
            hubConnection?.State == HubConnectionState.Connected;

        public async ValueTask DisposeAsync()
        {
            if (hubConnection is not null)
            {
                await hubConnection.DisposeAsync();
            }
        }

        private async void CheckInput(KeyboardEventArgs e)
        {
            var userInput = newMessageContent.Split(' ')[^1].ToLower();
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
                await SendMessage();
            else if (userInput.StartsWith('#'))
                AutocompleteTags(userInput);
            else
                helpCompleteTags.Clear();
        }

        private void AutocompleteTags(string userInput)
        {
            if (allDbTags == null)
                return;
            else
                helpCompleteTags = allDbTags.Where(t => t.Name.Contains(userInput)).ToList();
        }

        private void AddTagToText(string tagName)
        {
            int index = newMessageContent.LastIndexOf(' ');
            newMessageContent = newMessageContent.Remove(index < 0 ? 0 : index);
            newMessageContent += $"{(index < 0 ? "" : " ")}{tagName} "; // "if tag is the first word, no space before the tag
            helpCompleteTags.Clear();
        }

        private async Task AddSelectedTag(ChangeEventArgs e)
        {
            var tagName = e.Value?.ToString();
            if (tagName == null || allDbTags == null)
                return;
            var tag = allDbTags.FirstOrDefault(tag => tag.Name == tagName);
            if (tag != null && !selectedTags.Contains(tag))
            {
                selectedTags.Add(tag);
                await GetMessagesByTags();
            }
        }

        private async Task GetMessagesByTags()
        {
            var uriMessagesByTags = Navigation.ToAbsoluteUri($"{apiBase}/GetMessagesByTags");
            var response = await Http.PostAsJsonAsync(uriMessagesByTags, selectedTags);
            if (response != null && response.IsSuccessStatusCode)
                messages = await response.Content.ReadFromJsonAsync<List<Message>>();
        }

        private void RemoveSelectedTag(Tag tag)
        {
            selectedTags.Remove(tag);
            messages = messages?.Where(m => m.Tags.Any(t => selectedTags.Any(st => st.Name == t.Name)) || m.Tags.Count == 0).ToList();
        }

        private async Task SelectAllTags()
        {
            if (allDbTags == null) return;
            selectedTags = new List<Tag>(allDbTags);
            await GetMessagesByTags();
        }

        private async Task RemoveAllTags()
        {
            selectedTags.Clear();
            await GetMessagesByTags();
        }

        private async Task CreateTags()
        {
            var tags = TagRegexHelper.GetTagsFromString(newTagName);
            var newTags = tags?.Where(t => !allDbTags.Any(nt => t.Name == nt.Name)).ToList();
            if (newTags == null || !newTags.Any())
            {
                tagCreationError = true;
                return;
            }
            await PostTagToApiAsync(newTags);
        }

        private async Task PostTagToApiAsync(List<Tag> newTags)
        {
            if (hubConnection is null) return;
            var uri = Navigation.ToAbsoluteUri($"{apiBase}/CreateTag");
            await Http.PostAsJsonAsync(uri, newTags);
            await hubConnection.InvokeAsync("UpdateTagsList", new List<Tag>(newTags));
            newTagName = string.Empty;
        }

        private async Task CheckTagInput(KeyboardEventArgs e)
        {
            tagCreationError = false;
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
                await CreateTags();
        }

        private bool IsValidTagInput() => TagRegexHelper.ValidateRegexMatch(newTagName);

        private MarkupString StyleMessage(string message)
        {
            var matches = TagRegexHelper.GetRegexMatches(message);
            var result = new StringBuilder();
            int currentIndex = 0;
            foreach (Match match in matches)
            {
                result.Append(message.Substring(currentIndex, match.Index - currentIndex));
                result.Append($"<span class=\"text-primary\">{match.Value}</span> ");
                currentIndex = match.Index + match.Length;
            }
            result = new StringBuilder(result.ToString().TrimEnd());
            result.Append(message.Substring(currentIndex));
            return new MarkupString(result.ToString());
        }
    }
}
