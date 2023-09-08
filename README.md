# General Chat Web Application

This web application is a general chat platform built using ASP.NET .NET 6. It offers a chatroom accessible to all users without requiring registration or authentication. Users can participate in conversations and send messages with optional tags for message categorization. For quick access go to the deployed application go to https://ali-alo-chatapp.azurewebsites.net/

## Features

- **No Registration Required**: All users have immediate access to the chatroom without needing to register or log in.

- **Message Tagging**: Users can send messages with optional tags, allowing for message categorization.

- **My Tags Panel**: The screen is divided into two parts: a smaller "My Tags Panel" on the left and a larger chat window on the right. In the "My Tags Panel," users can enter a set of tags they want to enable for message filtering.

- **Tag-Based Filtering**: Messages in the chatroom are filtered based on the tags enabled by each user. Only messages marked with tags enabled for a user are visible in their chat view, essentially acting as message filters.

- **Persistent Messages**: All messages are stored in the database indefinitely, ensuring that previously sent messages are visible when users enter the site.

- **Messages Without Tags**: Messages sent without any tags are always visible, regardless of the tag-filter configuration.

- **Real-Time Messaging**: Messages sent by users appear "immediately" in the chatroom with minimal delay.

- **Tag Auto-Completion**: When entering tags, an auto-completion is used. Users can select from existing tags on the site or create new ones.

## Technologies

- ASP.NET .NET 6
- Blazor WebAssembly
- SignalR for real-time messaging

## Project Structure

- **Client.csproj**: The Blazor WebAssembly project responsible for the client-side application.

- **Server.csproj**: The ASP.NET server project handling server-side logic, including SignalR for real-time messaging.

- **Shared.csproj**: A shared project containing common code and components shared between the client and server.

## Getting Started

To get started with this project, follow these steps:

1. Clone the repository to your local machine:

   ```shell
   git clone https://github.com/ali-alo/ChatApp.git
    ```

2. Navigate to the repository project:

    ```shell
    cd ChatApp
    ```

3. Install the required dependencies in Client and Server folder projects by running:
    ```shell
    1. cd ChatApp/Client
    2. dotnet restore
    3. cd ../Server
    4. dotnet restore
    ```

4. Inside the Server project folder apply the database migrations:
    ```shell
    dotnet ef database update
    ```

5. Build and run the application:
    ```shell
    dotnet run
    ```
6. Access the application in your web browser at https://localhost:7053/

## Usage

1. Access the chatroom without the need for registration or login.

2. In the "My Tags Panel" on the left, enter the tags you want to enable for message filtering. You can remove tags by clicking on the red icon next to the tag.

3. Send messages with optional tag(s) by using '#' symbol to categorize your messages.

4. Chat in real-time with other users, and messages are filtered based on your enabled tags.

5. All messages are stored in the database, so you can view previously sent messages when you return to the site.

## Tagging

- Use tags for message categorization and filtering. Tags are applied to messages by specifying them while sending a message.

- Tags can be selected from existing ones on the site or created on the fly.
