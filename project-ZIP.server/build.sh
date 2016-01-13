#!/bin/bash
cd ${0%/*}
g++ -std=c++14 ./ZipArchive.cpp ./project-ZIP.server.cpp -I/usr/local/lib/libzip/include -lzip -pthread -o ./server -Wall
