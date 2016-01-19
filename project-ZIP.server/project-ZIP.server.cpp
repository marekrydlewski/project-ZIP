
#include <zip.h>
#include "ZipArchive.h"
#include "ServerZip.h"
#include <iostream>
using namespace source;
int main(int argc, char **argv) {

	ServerZip* server;
	server = ServerZip::getInstance();
    try {
        server->connect();
    }
    catch (const std::exception& e) {
        std::cerr<<"Error: "<<e.what()<<std::endl;
    }

}
