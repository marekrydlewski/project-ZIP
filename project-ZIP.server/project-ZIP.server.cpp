
#include <zip.h>
#include "ZipArchive.h"
#include "ServerZip.h"
#include <iostream>
using namespace source;
int main(int argc, char **argv) {

	///test
	ServerZip* server;
	server = ServerZip::getInstance();

	try {
	ZipArchive archive{"output-new.zip", ZIP_CREATE};

	// Add a buffer to the base archive
	archive.add(Buffer{"Data from buffer"}, "documentFromBuffer");

	// Add a file with all its contents to the directory assets/ in the archive
	archive.addDirectory("assets");
} catch (const std::exception &ex) {
	std::cerr << ex.what() << std::endl;
	std::exit(1);
}
	///
    server->connect();

}
