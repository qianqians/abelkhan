#include <boost/any.hpp>
#include <vector>
#include <iostream>

void main() {
	std::vector<boost::any> v;

	v.push_back(1);
	v.push_back("123");

	for (auto item : v) {
		//std::cout << item << std::endl;
	}
}