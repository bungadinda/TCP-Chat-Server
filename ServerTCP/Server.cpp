#include <iostream>
#include <WS2tcpip.h>
#include<string>
#include<string.h>
#pragma comment (lib, "ws2_32.lib")

using namespace std;

void main()
{
	//initialize winsock
	WSADATA wsData;
	WORD ver = MAKEWORD(2, 2);

	int wsock = WSAStartup(ver, &wsData);
	if (wsock != 0)
	{
		//cerr for show error
		cerr << "initialize error ! " << endl;
		return;
	}

	//create socket //for TCP is using SOCK_STREAM// AF_INET for version four
	SOCKET listening = socket(AF_INET, SOCK_STREAM, 0);
	if (listening == INVALID_SOCKET)
	{
		//cerr for show error
		cerr << "socket error ! " << endl;
		return;
	}

	//bind the socket and port to a socket usng hint structure
	sockaddr_in hint;
	hint.sin_family = AF_INET;
	//"Host TO Network Short"//convert values between host and network byte order, where Network byte order is big endian//
	hint.sin_port = htons(590);
	hint.sin_addr.S_un.S_addr = INADDR_ANY; //also can use inet_pton

	if (bind(listening, (sockaddr*)&hint, sizeof(hint)) != 0)
	{
		cout << "Error Binding" << endl;
	}
	//tell winsock the socket is for listening//server can send and receive
	if (listen(listening, SOMAXCONN) != 0)
	{
		cout << "Error Listening" << endl;
	}
	else
	{
		cout << "Server Connected^-^" << endl;
	}

	//wait for a connection
	sockaddr_in client;
	int clientsize = sizeof(client);
	SOCKET clientSocket = accept(listening, (sockaddr*)&client, &clientsize); //socket accept client and pass it to listening socket

	char host[NI_MAXHOST]; // client's remote name
	char service[NI_MAXHOST]; // service (i.e port) that connected the client

	ZeroMemory(host, NI_MAXHOST); //same as memset(host, 0, NI_MAXHOST)
	ZeroMemory(service, NI_MAXHOST);

	if (getnameinfo((sockaddr*)&client, sizeof(client), host, NI_MAXHOST, service, NI_MAXSERV, 0) == 0)
	{
		cout << host << " connected to port " << service << endl;
	}
	else
	{
		inet_ntop(AF_INET, &client.sin_addr, host, NI_MAXHOST);
		cout << host << " connected on port " << ntohs(client.sin_port) << endl;
	}
	//close listening socket
	closesocket(listening);
	//while loop : accept and response message back to client
	char buf[4096];
	string userInput;
	while (true)
	{
		ZeroMemory(buf, 4096);
		// wait for client
		int bytesReceived = recv(clientSocket, buf, 4096, 0);
		cout << "Client::: ";
		if (bytesReceived == SOCKET_ERROR)
		{
			cout << "Error in recv(). Quitting " << endl;
			break;
		}
		if (bytesReceived == 0)
		{
			cout << "Client disconnected" << endl;
			break;
		}
		cout << string(buf, 0, bytesReceived) << endl;
		//response message back to client
		cout << "> ";
		getline(cin, userInput);
		send(clientSocket, userInput.c_str(), bytesReceived + 1, 0);

	}

	//close the sock
	closesocket(clientSocket);
	//cleanup winsock
	WSACleanup;
}