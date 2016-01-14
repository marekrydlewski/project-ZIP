
#include <zip.h>
#include "ZipArchive.h"
#include "ServerZip.h"
#include <iostream>
using namespace source;
int main(int argc, char **argv) {

	///test
	ServerZip* server;
	server = ServerZip::getInstance();
    server->connect();

}
