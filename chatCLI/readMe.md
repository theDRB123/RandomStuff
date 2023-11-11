# chatCLI

A simple chat app made to be used in the terminal. made using webSockets in c#

this is just a practice project to learn how to use webSockets.

## How to use

Enter command `dotnet run` in chatServer dir to start the server.

Then do the same in another terminal , chatClient dir to start the client.
You can start as many clients as you want.

enter the name in the client terminal and start chatting.

## Commands

////// currently only limited things adding more /////
the messages can be structured as:-

`<command> 'toUser' <message>`

to send a broadcast : `bg Hi Friends`

to send a direct message : `dm 'user1' Hi user1`

## TODO

- [ ] add help
- [ ] add proper exit protocol
- [ ] add console buffer features
- [ ] add remote server
