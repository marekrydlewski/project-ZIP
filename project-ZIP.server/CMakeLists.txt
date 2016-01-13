cmake_minimum_required(VERSION 3.3)
project(project_ZIP_server)

include_directories(/usr/local/lib/libzip/include)
link_directories(/usr/local/lib/libzip/include)

set(CMAKE_EXE_LINKER_FLAGS "lzip")

set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++14 -pthread")
set(SOURCE_FILES
    project-ZIP.server.cpp ServerZip.cpp ServerZip.h ZipArchive.cpp ZipArchive.h)

add_executable(project_ZIP_server ${SOURCE_FILES})