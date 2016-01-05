#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <netdb.h>
#include <signal.h>
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <string.h>

struct threadInfo
{
  int connection_fd;
  struct sockaddr_in connectionAddress;
};

int backlog = 5;
char* responseValid = "Marcin Jablonski\n";
char* responseInvalid = "Unknown\n";
int responseSize;
int on = 1;

void* threadFunction(void* info)
{
  struct threadInfo* _info = (struct threadInfo*)info;
  char buffer[1024];
  int messageSize = read(_info->connection_fd, &buffer, sizeof(&buffer));

  printf("Connection from: %s\n", inet_ntoa((struct in_addr)_info->connectionAddress.sin_addr));

  if(strncmp("117270",buffer,6) == 0)
  {
    responseSize = 17;
    write(_info->connection_fd, responseValid, responseSize);
  }
  else
  {
    responseSize = sizeof(responseInvalid);
    write(_info->connection_fd, responseInvalid, responseSize);
  }
  write(1, "Ending connection\n", 18);
  close(_info->connection_fd);
  free(_info);
  return 0;
}


struct sockaddr_in FillAddress(int portNumber)
{
  struct sockaddr_in socketAddress;

  socketAddress.sin_family = AF_INET;
  socketAddress.sin_port = htons(portNumber);
  socketAddress.sin_addr.s_addr = INADDR_ANY;

  return socketAddress;
}

int main(int argc, char** argv)
{
  int socket_fd = socket(AF_INET, SOCK_STREAM, 0);
  setsockopt(socket_fd, SOL_SOCKET, SO_REUSEADDR, (char*)&on, sizeof(on));

  struct sockaddr_in connectionAddress;
  struct sockaddr_in socketAddress = FillAddress(1234);

  bind(socket_fd, (struct sockaddr*)&socketAddress, sizeof(socketAddress));

  listen(socket_fd, backlog);

  while(1)
  {
    pthread_t threadId;

    struct threadInfo* info = malloc(sizeof(struct threadInfo));

    int socketAddressSize = sizeof(info->connectionAddress);
    info->connection_fd = accept(socket_fd, (struct sockaddr*)&info->connectionAddress, &socketAddressSize);

    pthread_create(&threadId, NULL, threadFunction, info);
    pthread_detach(threadId);
  }

  close(socket_fd);
  return 0;
}