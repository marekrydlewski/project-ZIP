#include <fstream>
#include "ServerZip.h"
#include "ZipArchive.h"

using namespace source;
//Static values must be init
int ServerZip::socket_fd = -1;

void ServerZip::signalHandler(int signal) {
    std::cout << "ServerZip: Caught signal " << signal << ", free resources and exit" << std::endl;
    close(socket_fd);
    exit(0);
};

// Global static pointer used to ensure a single instance of the class.
ServerZip *ServerZip::m_pInstance = nullptr;

//  This function is called to create an instance of the class.
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

    bind(socket_fd, (sockaddr * ) & socketAddress, sizeof(socketAddress));

    listen(socket_fd, backlog);


    while (1) {
        pthread_t threadId;
        auto *info = new threadInfo();

        socklen_t socketAddressSize = sizeof(info->connectionAddress);
        info->connection_fd = accept(socket_fd, (sockaddr * ) & info->connectionAddress, &socketAddressSize);

        pthread_create(&threadId, NULL, threadFunction, info);
        pthread_detach(threadId);
    }
}

void *ServerZip::threadFunction(void *info) {

    threadInfo *_info = (threadInfo *) info;
    std::cout << "Connection from: " << inet_ntoa(_info->connectionAddress.sin_addr) << std::endl;

    std::string tempArchive = std::tmpnam(nullptr);
    tempArchive += ".zip";
    int numberOfFiles = std::stoi(readData(_info->connection_fd));


    for (int i = 0; i < numberOfFiles; i++) {
        auto path = readData(_info->connection_fd);
        auto file = readData(_info->connection_fd);
        std::cout << "path: " << path << std::endl;
        std::cout << "file size: " << file.length() << std::endl;
        try {
            ZipArchive archive{tempArchive, ZIP_CREATE};
            archive.add(Buffer{file}, path);
        } catch (const std::exception &ex) {
            std::cerr << ex.what() << std::endl;
            std::exit(1);
        }
    }

    std::string str;
    std::ifstream t(tempArchive);
    t.seekg(0, std::ios::end);
    std::cout << "zip size: " << t.tellg() << std::endl;
    str.reserve(t.tellg());
    t.seekg(0, std::ios::beg);
    str.assign((std::istreambuf_iterator<char>(t)),
               std::istreambuf_iterator<char>());

    char *buffer = 0;
    int length = (int) str.length();
    buffer = new char[length];
    std::copy(str.begin(), str.end(), buffer);
    writeData(_info->connection_fd, (unsigned int) length, (void *) buffer);

    write(1, "Ending connection\n", 18);
    close(_info->connection_fd);
    free(_info);

    if (std::remove(tempArchive.c_str()) != 0) std::perror("Error deleting temp archive");
    else std::cout << "Temp archive removed" << std::endl;

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
            throw std::length_error(std::string("Error: readXBytes -  read less than 1 byte"));
        }
        bytesRead += result;
    }
}

std::string ServerZip::readData(int socket_fd) {
    unsigned int length = 0;
    char *buffer = 0;
    readXBytes(socket_fd, sizeof(length), (void *) (&length));
    buffer = new char[length];

    readXBytes(socket_fd, length, (void *) buffer);
    std::string str = "";
    str = std::string(buffer, length);

    delete[] buffer;
    return str;
}


void ServerZip::writeXBytes(int socket, unsigned int x, void *buffer) {
    unsigned int bytesWritten = 0;
    int result;
    while (bytesWritten < x) {
        result = write(socket, buffer + bytesWritten, x - bytesWritten);
        if (result < 1) {
            throw std::length_error(std::string("Error: readXBytes -  wrote less than 1 byte"));
        }

        bytesWritten += result;
    }
}

void ServerZip::writeData(int socket_fd, int x, void *buffer) {
    writeXBytes(socket_fd, sizeof(x), (void *) &x);
    writeXBytes(socket_fd, (unsigned int) x, (void *) &buffer);
}