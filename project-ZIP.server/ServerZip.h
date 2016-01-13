//
// Created by marcin on 13.01.16.
//

#ifndef PROJECT_ZIP_SERVER_SERVERZIP_H
#define PROJECT_ZIP_SERVER_SERVERZIP_H

#include <netinet/in.h>
#include <signal.h>
#include <unistd.h>
#include <iostream>

class ServerZip {
    struct threadInfo {
        int connection_fd;
        sockaddr_in connectionAddress;
    };
public:
    static ServerZip * getInstance();
    static int socket_fd;
    static void sighandler(int signal);
    sockaddr_in fillAddress(int portNumber);
    void *threadFunction(void *info);

private:
    threadInfo* info;

    ServerZip(){};  // Private so that it can  not be called
    ServerZip(ServerZip const&){};             // copy constructor is private
    ServerZip & operator=(ServerZip const&){};  // assignment operator is private
    static ServerZip * m_pInstance;
};


#endif //PROJECT_ZIP_SERVER_SERVERZIP_H
