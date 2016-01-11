#!/bin/bash
cd ${0%/*}
g++ -std=c++11 ./project-ZIP.server.cpp -pthread -o ./server -Wall
