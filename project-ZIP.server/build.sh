#!/bin/bash
cd ${0%/*}
g++ ./project-ZIP.server.cpp -pthread -o ./server -Wall
