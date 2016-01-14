#!/bin/bash
cd ${0%/*}

if [[ $1 = debug ]]; then
    g++ -std=c++14 ./ServerZip.cpp ./ZipArchive.cpp ./project-ZIP.server.cpp -I/usr/local/lib/libzip/include -lzip -pthread -o ./server -Wall -g
    gdb ./server
    #gdb run
    #gdb brake $2:$3
else
    g++ -std=c++14 ./ServerZip.cpp ./ZipArchive.cpp ./project-ZIP.server.cpp -I/usr/local/lib/libzip/include -lzip -pthread -o ./server -Wall
    ./server
fi

