cmake_minimum_required(VERSION 3.3)
project(project_ZIP_server)

set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++11 -s -lpthread")

set(SOURCE_FILES
    project-ZIP.server.cpp threadInfo.h)

add_executable(project_ZIP_server ${SOURCE_FILES})