#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <netdb.h>
#include <csignal>
#include <cstdio>
#include <stdlib.h>
#include <unistd.h>
#include <string.h>
#include <pthread.h>
#include <zip.h>
#include "ZipArchive.h"
#include <iostream>


//declarations
void sighandler(int);
sockaddr_in FillAddress(int portNumber);
void *threadFunction(void *info);
int socket_fd;
//

struct threadInfo {
	int connection_fd;
	sockaddr_in connectionAddress;
};

int main(int argc, char **argv) {

	///test
	try {
		ZipArchive archive{"mydata.zip"};
		ZipStat stat = archive.stat("README");
		ZipFile file = archive.open("README");

		std::cout << "content of README:" << std::endl;
		std::cout << file.read(stat.size);
	} catch (const std::exception &ex) {
		std::cerr << ex.what() << std::endl;
		std::exit(1);
	}
	///

	int backlog = 5;
	int on = 1;
    socket_fd = socket(AF_INET, SOCK_STREAM, 0);

    setsockopt(socket_fd, SOL_SOCKET, SO_REUSEADDR, (char *) &on, sizeof(on));
    sockaddr_in socketAddress = FillAddress(1234);

    bind(socket_fd, (sockaddr *) &socketAddress, sizeof(socketAddress));

    listen(socket_fd, backlog);
    signal(SIGINT, sighandler);

    while (1) {
        pthread_t threadId;
				auto *info = new threadInfo();

        socklen_t socketAddressSize = sizeof(info->connectionAddress);
        info->connection_fd = accept(socket_fd, (sockaddr*) &info->connectionAddress, &socketAddressSize);

        pthread_create(&threadId, NULL, threadFunction, info);
        pthread_detach(threadId);
    }
}


//definitions, move to new files later



void sighandler(int signal)
{
    close(socket_fd);
    std::cout<<"Caught signal "<<signal<<" , coming out...\n"<<std::endl;
    exit(0);
}

void *threadFunction(void *info) {

	const char *responseValid = "Marcin Jablonski\n";
	const char *responseInvalid = "Unknown\n";
	int responseSize;
	threadInfo* _info = (threadInfo*)info;
	char buffer[1024];
	//int messageSize = (int) read(_info->connection_fd, &buffer, sizeof(&buffer));

	printf("Connection from: %s\n", inet_ntoa(_info->connectionAddress.sin_addr));

	if (strncmp("117270", buffer, 6) == 0) {
		responseSize = 17;
		write(_info->connection_fd, responseValid, (size_t)responseSize);
	}
	else {
		responseSize = sizeof(responseInvalid);
		write(_info->connection_fd, responseInvalid, (size_t)responseSize);
	}
	write(1, "Ending connection\n", 18);
	close(_info->connection_fd);
	free(_info);
	return 0;
}


sockaddr_in FillAddress(int portNumber) {
	sockaddr_in socketAddress;

	socketAddress.sin_family = AF_INET;
	socketAddress.sin_port = htons((uint16_t)portNumber);
	socketAddress.sin_addr.s_addr = INADDR_ANY;

	return socketAddress;
}
