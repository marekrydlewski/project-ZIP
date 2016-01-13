
#include <zip.h>
#include "ZipArchive.h"
#include "ServerZip.h"
#include <iostream>

int main(int argc, char **argv) {

	///test
	ServerZip* server;
	server = ServerZip::getInstance();

	try {
		ZipArchive archive{"mydata.zip"};
		ZipStat stat = archive.stat("README");
		ZipFile file = archive.open("README");

		std::cout << "content of README:" << std::endl;
		std::cout << file.read(stat.size);
	} catch (const std::exception &ex) {
		std::cerr << ex.what() << std::endl;
		std::exit(1);
	}
	///
    server->connect();

}
