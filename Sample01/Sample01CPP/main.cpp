#include "stdio.h"
#include <string>

class Person
{
public:
	int m_old;
	std::string m_name;

	Person(){
		m_old = 10;
		m_name = std::string("Hoge");
	}
};

void main()
{
	Person hoges[10];
	for (int i = 0; i < 10; ++i)
	{
		printf("%d : ", i);
		printf("Old = %d  ", hoges[i].m_old);
		printf("Name = %s\n", hoges[i].m_name.c_str());
	}
}

