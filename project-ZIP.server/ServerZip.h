//
// Created by marcin on 13.01.16.
//

#ifndef PROJECT_ZIP_SERVER_SERVERZIP_H
#define PROJECT_ZIP_SERVER_SERVERZIP_H

class ServerZip {
public:
    static ServerZip * getInstance();

private:
    ServerZip(){};  // Private so that it can  not be called
    ServerZip(ServerZip const&){};             // copy constructor is private
    ServerZip & operator=(ServerZip const&){};  // assignment operator is private
    static ServerZip * m_pInstance;
};

#endif //PROJECT_ZIP_SERVER_SERVERZIP_H
