//
// Created by marek on 05.01.16.
//

#ifndef PROJECT_ZIP_SERVER_THREADINFO_H
#define PROJECT_ZIP_SERVER_THREADINFO_H

#include <netinet/in.h>

struct threadInfo {
    int connection_fd;
    struct sockaddr_in connectionAddress;
};


#endif //PROJECT_ZIP_SERVER_THREADINFO_H
