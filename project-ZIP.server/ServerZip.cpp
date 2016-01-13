
#include "ServerZip.h"


//Static values must be init
int ServerZip::socket_fd = -1;

void ServerZip:: sighandler(int signal)
{
    close(socket_fd);
    std::cout<<"ServerZip: Caught signal "<<signal<<" , coming out...\n"<<std::endl;
    exit(0);
};



// Global static pointer used to ensure a single instance of the class.
ServerZip* ServerZip::m_pInstance = nullptr;

/** This function is called to create an instance of the class.
    Calling the constructor publicly is not allowed. The constructor
    is private and is only called by this Instance function.
*/

ServerZip* ServerZip::getInstance()
{
    if (!m_pInstance)
        m_pInstance = new ServerZip;

    return m_pInstance;
}

sockaddr_in ServerZip::fillAddress(int portNumber) {
    sockaddr_in socketAddress;

    socketAddress.sin_family = AF_INET;
    socketAddress.sin_port = htons((uint16_t)portNumber);
    socketAddress.sin_addr.s_addr = INADDR_ANY;

    return socketAddress;
}

