#include <iostream>
#include <sstream>
#include <limits>
#include <list>

#include "variant.hpp"
#include "form.hpp"
#include "msgpack.hpp"
#include "json.hpp"

#include <cmath>


int main(int argc, char** argv)
{
  {
    bool passed = false;

    std::stringstream ss;
    goodform::variant var, var2;
    var = goodform::object
      {
        {"compact", true},
        {"schema", 0}
      };

    goodform::msgpack::serialize(var, ss);
    goodform::msgpack::deserialize(ss, var2);

    goodform::form form(var2);

    struct
    {
      bool compact;
      std::int32_t schema;
    } mpack;

    mpack.compact = form.at("compact").boolean().val();
    mpack.schema = form.at("schema").uint32().val();

    if (form.is_good())
    {
      passed = true;
      std::cout << "{ \"compact\": " << std::boolalpha << mpack.compact << ", \"schema\": " << mpack.schema << " }" << std::endl;
    }

    std::cout << "MsgPack " << (passed ? "Passed." : "FAILED!") << std::endl;
  }

  {
    goodform::variant var;
    std::stringstream ss;
    ss << "{" << std::endl
      << "\"first_name\":\"John\", // This is a comment" << std::endl
      << "\"last_name\":\"Smith\", " << std::endl
      << "\"age\": 23," << std::endl
      << "\"gpa\": 4.0," << std::endl
      << "\"email\":\"john.smith@example.com\"," << std::endl
      << "\"password_hash\":\"5f4dcc3b5aa765d61d8327deb882cf99\"," << std::endl
      << "\"interests\": [\"sailing\",\"swimming\",\"yoga\"]" << std::endl
      << "}" << std::endl;

    goodform::json::deserialize(ss, var);

    goodform::form form(var);

    struct
    {
      std::string first_name;
      std::string last_name;
      std::uint8_t age;
      float gpa;
      std::string email;
      std::string password_hash;
      bool subscribe_to_email_marketing;
      std::list<std::string> interests;
    } form_data;


    form_data.first_name = form.at("first_name").string().match(std::regex("^[a-zA-Z ]{1,64}$")).val();
    form_data.last_name = form.at("last_name").string().match(std::regex("^[a-zA-Z ]{1,64}$")).val();
    form_data.age = form.at("age").uint8().val();
    form_data.gpa = form.at("gpa").float32().gte(0).lte(4.0).val();
    form_data.email = form.at("email").string().match(std::regex("^.{3,256}$")).val();
    form_data.password_hash = form.at("password_hash").string().match(std::regex("^[a-fA-F0-9]{32}$")).val();
    form_data.subscribe_to_email_marketing = form.at("subscribe_to_email_marketing", true).boolean().val(); // Optional field defaults to true.

    form.at("interests").array().for_each([&form_data](goodform::sub_form& sf, std::size_t i)
    {
      form_data.interests.push_back(sf.string().val());
    });




    if (form.is_good())
    {
      std::cout << "JSON passed." << std::endl;
    }
    else
    {
      std::cout << "JSON FAILED!" << std::endl;
    }

  }


  return 0;
}