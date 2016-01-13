#include "ServerZip.h"


//Static values must be init
int ServerZip::socket_fd = -1;

void ServerZip::signalHandler(int signal) {
    close(socket_fd);
    std::cout << "ServerZip: Caught signal " << signal << " , coming out...\n" << std::endl;
    exit(0);
};

// Global static pointer used to ensure a single instance of the class.
ServerZip *ServerZip::m_pInstance = nullptr;

/** This function is called to create an instance of the class.
    Calling the constructor publicly is not allowed. The constructor
    is private and is only called by this Instance function.
*/
ServerZip *ServerZip::getInstance() {
    if (!m_pInstance)
        m_pInstance = new ServerZip;

    return m_pInstance;
}

void ServerZip::connect() {
    int backlog = 5;
    int on = 1;
    socket_fd = socket(AF_INET, SOCK_STREAM, 0);

    setsockopt(socket_fd, SOL_SOCKET, SO_REUSEADDR, (char *) &on, sizeof(on));
    sockaddr_in socketAddress = this->fillAddress(1234);

    bind(socket_fd, (sockaddr *) &socketAddress, sizeof(socketAddress));

    listen(socket_fd, backlog);
    signal(SIGINT, this->signalHandler);

    while (1) {
        pthread_t threadId;
        auto *info = new threadInfo();

        socklen_t socketAddressSize = sizeof(info->connectionAddress);
        info->connection_fd = accept(socket_fd, (sockaddr*) &info->connectionAddress, &socketAddressSize);

        pthread_create(&threadId, NULL, threadFunction, info);
        pthread_detach(threadId);
    }
}

void *ServerZip::threadFunction(void *info) {

    const char *responseValid = "Marcin Jablonski\n";
    const char *responseInvalid = "Unknown\n";
    int responseSize;
    threadInfo *_info = (threadInfo *) info;
    char buffer[1024];
    //int messageSize = (int) read(_info->connection_fd, &buffer, sizeof(&buffer));

    printf("Connection from: %s\n", inet_ntoa(_info->connectionAddress.sin_addr));

    if (strncmp("117270", buffer, 6) == 0) {
        responseSize = 17;
        write(_info->connection_fd, responseValid, (size_t) responseSize);
    }
    else {
        responseSize = sizeof(responseInvalid);
        write(_info->connection_fd, responseInvalid, (size_t) responseSize);
    }
    write(1, "Ending connection\n", 18);
    close(_info->connection_fd);
    free(_info);
    return 0;
}

sockaddr_in ServerZip::fillAddress(int portNumber) {
    sockaddr_in socketAddress;

    socketAddress.sin_family = AF_INET;
    socketAddress.sin_port = htons((uint16_t) portNumber);
    socketAddress.sin_addr.s_addr = INADDR_ANY;

    return socketAddress;
}

