//
// Created by marcin on 13.01.16.
//

#ifndef PROJECT_ZIP_SERVER_SERVERZIP_H
#define PROJECT_ZIP_SERVER_SERVERZIP_H

#include <netinet/in.h>
#include <signal.h>
#include <unistd.h>
#include <pthread.h>
#include <string.h>
#include <arpa/inet.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <iostream>

class ServerZip {
    struct threadInfo {
        int connection_fd;
        sockaddr_in connectionAddress;
    };
public:
    static ServerZip * getInstance();
    static int socket_fd;
    static void signalHandler(int signal);
    static void *threadFunction(void *info);

    sockaddr_in fillAddress(int portNumber);
    void connect();

private:
    int backlog; // max length queue listen func

    ServerZip(){
        backlog = 5;
    };  // private so that it can  not be called
    ServerZip(ServerZip const&){};             // copy constructor is private
    static ServerZip * m_pInstance;
    static void readXBytes(int socket, unsigned int x, void* buffer);
    static std::string readData(int socket_fd);
    static void writeXBytes(int socket, unsigned int x, void* buffer);
    static void writeData(int socket_fd, int x, void *buffer);
};


#endif //PROJECT_ZIP_SERVER_SERVERZIP_H
