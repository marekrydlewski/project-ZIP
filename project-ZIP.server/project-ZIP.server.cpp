
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
        std::cerror<<"Error: "<<e.what()<<endl;
    }

}
