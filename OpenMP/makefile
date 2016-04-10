#Makefile for Brute Force
#Figl, Winterhalder
#CFLAGS = -fopenmp -std=c++11
CC=gcc
CFLAGS = -fopenmp

all: bruteforce

bruteforce: bruteforce.o
	${CC} ${CFLAGS} -o bruteforce bruteforce.o
	
bruteforce.o: bruteforce.c
	${CC} ${CFLAGS} -c bruteforce.c

clean:
	rm bruteforce.exe bruteforce.o
