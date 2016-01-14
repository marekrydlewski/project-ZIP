#include <fstream>
#include "ServerZip.h"
#include "ZipArchive.h"

using namespace source;
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
    int on = 1;
    signal(SIGINT, this->signalHandler);

    socket_fd = socket(AF_INET, SOCK_STREAM, 0);

    setsockopt(socket_fd, SOL_SOCKET, SO_REUSEADDR, (char *) &on, sizeof(on));
    sockaddr_in socketAddress = this->fillAddress(1234);

    bind(socket_fd, (sockaddr *) &socketAddress, sizeof(socketAddress));

    listen(socket_fd, backlog);


    while (1) {
        pthread_t threadId;
        auto *info = new threadInfo();

        socklen_t socketAddressSize = sizeof(info->connectionAddress);
        info->connection_fd = accept(socket_fd, (sockaddr *) &info->connectionAddress, &socketAddressSize);

        pthread_create(&threadId, NULL, threadFunction, info);
        pthread_detach(threadId);
    }
}

void *ServerZip::threadFunction(void *info) {

    threadInfo *_info = (threadInfo *) info;
    std::cout << "Connection from: " << inet_ntoa(_info->connectionAddress.sin_addr) << std::endl;

    auto path = readData(_info->connection_fd);
    auto file = readData(_info->connection_fd);
    std::cout << path << std::endl;
    std::cout << "file size "<< file.length() << std::endl;

    try {
        ZipArchive archive{"output.zip", ZIP_CREATE};
        archive.add(Buffer{file}, path);
    } catch (const std::exception &ex) {
        std::cerr << ex.what() << std::endl;
        std::exit(1);
    }


    std::string str;

    std::ifstream t("output.zip");
    t.seekg(0, std::ios::end);
    std::cout<<"zip size"<<t.tellg() << std::endl;
    str.reserve(t.tellg());
    t.seekg(0, std::ios::beg);

    str.assign((std::istreambuf_iterator<char>(t)),
               std::istreambuf_iterator<char>());

    writeData(_info->connection_fd, std::to_string(str.length()));
    writeData(_info->connection_fd, str);

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


void ServerZip::readXBytes(int socket, unsigned int x, void *buffer) {
    unsigned int bytesRead = 0;
    int result;
    while (bytesRead < x) {
        result = read(socket, buffer + bytesRead, x - bytesRead);
        if (result < 1) {
            // Throw error
            std::cout << "Oh no problem with reading data!" << std::endl;
        }

        bytesRead += result;
    }
}

std::string ServerZip::readData(int socket_fd) {
    unsigned int length = 0;
    char *buffer = 0;
// we assume that sizeof(length) will return 4 here.
    readXBytes(socket_fd, sizeof(length), (void *) (&length));
    buffer = new char[length];
    readXBytes(socket_fd, length, (void *) buffer);

    std::string str = "";

    if (buffer)
        str = std::string(buffer);
    else {
        //throw error
    }

    delete[] buffer;
    return str;
}


void ServerZip::writeXBytes(int socket, unsigned int x, void *buffer) {
    unsigned int bytesWritten = 0;
    int result;
    while (bytesWritten < x) {
        result = write(socket, buffer + bytesWritten, x - bytesWritten);
        if (result < 1) {
            // Throw error
            std::cout << "Oh no problem with writing data!" << std::endl;
        }

        bytesWritten += result;
    }
}

void ServerZip::writeData(int socket_fd, std::string str) {
    writeXBytes(socket_fd, str.length(), (void *) str.c_str());
}
