
#include <sstream>
#include <codecvt>

#include "json.hpp"

//######################################################################//
namespace goodform
{
  //----------------------------------------------------------------------//
  void json::write_string(const std::string& input, std::ostream& output)
  {
    output << "\"";
    for (unsigned long i = 0; i < input.length(); ++i)
    {
      char c = input[i];
      if (c == '\\')
        output << "\\\\";
      else if (c == '/')
        output << "\\/";
      else if (c == '"')
        output << "\\\"";
      else if (c == '\b')
        output << "\\b";
      else if (c == '\f')
        output << "\\f";
      else if (c == '\n')
        output << "\\n";
      else if (c == '\r')
        output << "\\r";
      else if (c == '\t')
        output << "\\t";
      else
        output << c;
    }
    output << "\"";
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  void json::stringify_object(const variant& v, std::ostream& output)
  {
    output << "{";
    for (auto it = v.get<goodform::object>().begin(); it != v.get<goodform::object>().end(); ++it)
    {
      if (it != v.get<goodform::object>().begin())
        output << ",";
      write_string(it->first, output);
      output << ":";
      serialize(it->second, output);
    }
    output << "}";
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  void json::stringify_array(const variant& v, std::ostream& output)
  {
    output << "[";
    for (auto it = v.get<goodform::array>().begin(); it != v.get<goodform::array>().end(); ++it)
    {

      if (it != v.get<goodform::array>().begin())
        output << ",";
      serialize(*it, output);
    }
    output << "]";
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  void json::serialize(const variant& v, std::ostream& output)
  {
    if (v.is<std::nullptr_t>())
    {
      output << "null";
    }
    else if (v.is<bool>())
    {
      output << (v.get<bool>() ? "true":"false");
    }
    else if (v.type() == variant_type::signed_integer) output << v.get<std::int64_t>();
    else if (v.type() == variant_type::unsigned_integer) output << v.get<std::uint64_t>();
    else if (v.type() == variant_type::floating_point) output << v.get<double>();
    else if (v.is<std::string>())
    {
      write_string(v.get<std::string>(), output);
    }
    else if (v.is<goodform::array>())
    {
      stringify_array(v, output);
    }
    else if (v.is<goodform::object>())
    {
      stringify_object(v, output);
    }
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  char json::parse_escaped_char(char input)
  {
    char returnVal = -1;
    if (input == '\\')
      returnVal = input;
    else if (input == '/')
      returnVal = input;
    else if (input == '"')
      returnVal = input;
    else if (input == 'b')
      returnVal = '\b';
    else if (input == 'f')
      returnVal = '\f';
    else if (input == 'n')
      returnVal = '\n';
    else if (input == 'r')
      returnVal = '\r';
    else if (input == 't')
      returnVal = '\t';
    else
      returnVal = input;
    return returnVal;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  void json::strip_white_space(std::istream& input)
  {
    for (char c = (char)input.peek();
      c == ' '
      || c == '\t'
      || c == '\n'
      || c == '\r'
      || c == '/';
      c = (char)input.peek())
    {
      if (input.get() == '/')
      {
        if (input.peek() != '/')
          input.unget();
        else
        {
          std::string comment;
          std::getline(input, comment);
        }
      }
    }
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  int json::convert_hex_to_dec(char input)
  {
    int iReturnValue = -1;
    if(input == '0')
      iReturnValue = 0;
    else if(input == '1')
      iReturnValue = 1;
    else if(input == '2')
      iReturnValue = 2;
    else if(input == '3')
      iReturnValue = 3;
    else if(input == '4')
      iReturnValue = 4;
    else if(input == '5')
      iReturnValue = 5;
    else if(input == '6')
      iReturnValue = 6;
    else if(input == '7')
      iReturnValue = 7;
    else if(input == '8')
      iReturnValue = 8;
    else if(input == '9')
      iReturnValue = 9;
    else if(input == 'a' || input == 'A')
      iReturnValue = 10;
    else if(input == 'b' || input == 'B')
      iReturnValue = 11;
    else if(input == 'c' || input == 'C')
      iReturnValue = 12;
    else if(input == 'd' || input == 'D')
      iReturnValue = 13;
    else if(input == 'e' || input == 'E')
      iReturnValue = 14;
    else if(input == 'f' || input == 'F')
      iReturnValue = 15;
    return iReturnValue;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  bool json::parse_object(std::istream& input, variant& v)
  {
    v = variant_type::object;
    char c = (char)input.get();
    bool closingBraceFound = false;
    if (c == '{')
    {
      bool parseError = false;
      while(!closingBraceFound && !parseError && input.good())
      {
        strip_white_space(input);
        if ((closingBraceFound = (input.peek() == '}')))
        {
          input.seekg(1, std::ios::cur);
        }
        else
        {
          std::string key;
          if (!(parseError = !read_string(input, key)))
          {
            strip_white_space(input);
            if (!(parseError = (input.peek() != ':')))
            {
              input.seekg(1, std::ios::cur);
              strip_white_space(input);

              variant value;
              parseError = !deserialize(input, value);
              v[key] = std::move(value);

              if (!parseError)
              {
                strip_white_space(input);
                c = (char)input.peek();
                if ((closingBraceFound = (c == '}')) || !(parseError = (c != ',')))
                  input.seekg(1, std::ios::cur);
              }
            }
          }
        }
      }
    }
    return closingBraceFound;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  bool json::parse_array(std::istream& input, variant& v)
  {
    v = variant_type::array;
    char c = (char)input.get();
    bool closingBraceFound = false;
    if (c == '[')
    {
      bool parseError = false;
      while(!closingBraceFound && !parseError && input.good())
      {
        strip_white_space(input);
        if ((closingBraceFound = (input.peek() == ']')))
        {
          input.seekg(1, std::ios::cur);
        }
        else
        {
          variant elem;
          parseError = !deserialize(input, elem);
          v.push(std::move(elem));
          if (!parseError)
          {
            strip_white_space(input);
            c = (char)input.peek();
            if ((closingBraceFound = (c == ']')) || !(parseError = (c != ',')))
              input.seekg(1, std::ios::cur);
          }
        }
      }
    }
    return closingBraceFound;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  bool json::read_string(std::istream& input, std::string& destination)
  {
    bool ret = false;
    char c = (char)input.get();
    if (c == '"')
    {
      for (c = (char)input.get(); c != '"' && input.good(); c = (char)input.get())
      {
        if (c != '\\')
        {
          destination += c;
        }
        else
        {
          c = (char)input.get();
          if (c != 'u')
          {
            destination += parse_escaped_char(c);
          }
          else
          {
            //handle unicode character
            char uniArray[4];
            input.read(uniArray, 4);

            char32_t uniChar = (unsigned char)convert_hex_to_dec(uniArray[0]);
            uniChar = uniChar << 8; // was << 4

            uniChar |= (unsigned char)convert_hex_to_dec(uniArray[1]);
            uniChar = uniChar << 8; // was << 4

            uniChar |= (unsigned char)convert_hex_to_dec(uniArray[2]);
            uniChar = uniChar << 8; // was << 4

            uniChar |= (unsigned char)convert_hex_to_dec(uniArray[3]);
#ifdef _MSC_VER
            std::basic_string<__int32, std::char_traits<__int32>> utf32String;
#else
            std::u32string utf32String;
#endif
            utf32String.push_back(uniChar);
#ifdef _MSC_VER
            std::wstring_convert<std::codecvt_utf8<__int32>, __int32> convert;
#else
            std::wstring_convert<std::codecvt_utf8<char32_t>, char32_t> convert;
#endif
            destination += convert.to_bytes(utf32String);
          }
        }
      }
      if (c == '"')
        ret = true;
    }
    return ret;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  bool json::deserialize(std::istream& input, variant& v)
  {
    bool ret = false;

    char drain[5];

    strip_white_space(input);

    char c = (char)input.peek();
    if (c == '{')
    {
      ret = parse_object(input, v);
    }
    else if (c == '[')
    {
      ret = parse_array(input, v);
    }
    else if (c == '"')
    {
      std::string s;
      ret = read_string(input, s);
      v = std::move(s);
    }
    else if (isdigit(c) || c == '-')
    {
      bool is_signed = (c == '-');

      std::string str_number;
      str_number += c;
      input.seekg(1, std::ios::cur);
      bool done = false;
      bool found_period = false;
      while (!done && input.good())
      {
        c = (char)input.peek();

        if (c == '.')
          found_period = true;

        if (isdigit(c) || c == '.' || c == 'e' || c == 'E' || c == '-' || c == '+')
        {
          str_number += c;
          input.seekg(1, std::ios::cur);
        }
        else
        {
          done = true;
        }
      }

      std::stringstream ss(str_number);
      if (found_period)
      {
        double tmp;
        ss >> tmp;
        v = (tmp);
      }
      else if (is_signed)
      {
        std::int64_t tmp;
        ss >> tmp;
        v = (tmp);
      }
      else
      {
        std::uint64_t tmp;
        ss >> tmp;
        v = (tmp);
      }

      ret = true;
    }
    else if (c == 't' || c == 'T')
    {
      input.read(drain, 4);
      v = (true);
      ret = true;
    }
    else if (c == 'f' || c == 'F')
    {
      input.read(drain, 5);
      v = (false);
      ret = true;
    }
    else if (c == 'n' || c == 'N')
    {
      input.read(drain, 4);
      v = (nullptr);
      ret = true;
    }

    return ret;
  }
  //----------------------------------------------------------------------//
}
//######################################################################//
