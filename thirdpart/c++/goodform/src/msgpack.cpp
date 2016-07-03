#include "msgpack.hpp"

#ifdef _WIN32
#include <WinSock2.h>
#else
#include <arpa/inet.h>
#endif

namespace goodform
{
  //----------------------------------------------------------------//
  bool msgpack::serialize(const variant& v, std::ostream& output)
  {
    bool ret = true;

    if (v.is<object>() && v.size() <= 15) // fixmap	1000xxxx
    {
      ret = output.put(static_cast<char>(0x80 | (0x0F & v.size()))).good();
      if (ret)
      {
        const goodform::object& tmp = v.get<object>();
        for (goodform::object::const_iterator it = tmp.begin(); it != tmp.end(); ++it)
        {
          variant key = it->first;
          ret = msgpack::serialize(key, output);
          if (ret)
            ret = msgpack::serialize(it->second, output);
        }
      }
    }
    else if (v.is<array>() && v.size() <= 15) // fixarray	1001xxxx
    {
      ret = output.put(static_cast<char>(0x90 | (0x0F & v.size()))).good();
      if (ret)
      {
        const goodform::array& tmp = v.get<array>();
        for (goodform::array::const_iterator it = tmp.begin(); it != tmp.end(); ++it)
        {
          ret = msgpack::serialize(*it, output);
        }
      }
    }
    else if (v.is<std::string>() && v.size() <= 31) // fixstr	101xxxxx
    {
      ret = output.put(static_cast<char>(0xA0 | (0x1F & v.size()))).good();
      if (ret)
      {
        ret = output.write(&(v.get<std::string>()[0]), v.size()).good();
      }
    }
    else if (v.is<std::nullptr_t>())
    {
      output.put(static_cast<char>(0xC0));
    }
    else if (v.is<bool>())
    {
      if (v.get<bool>())
        output.put(static_cast<char>(0xC3));
      else
        output.put(static_cast<char>(0xC2));
    }
    else if (v.is<binary>() && v.size() <= 0xFF) // Bin 8
    {
      if (!output.put(static_cast<char>(0xC4))
        || !output.put(static_cast<char>(0xFF & v.size()))
        || !output.write(v.get<binary>().data(), v.size()))
      {
        ret = false;
      }
    }
    else if (v.is<binary>() && v.size() <= 0xFFFF) // Bin 16
    {
      std::uint16_t sz_be(htons(0xFFFF & v.size()));
      if (!output.put(static_cast<char>(0xC5))
        || !output.write((char*)(&sz_be), sizeof(sz_be))
        || !output.write(v.get<binary>().data(), v.size()))
      {
        ret = false;
      }
    }
    else if (v.is<binary>() && v.size() <= 0xFFFFFFFF) // Bin 32
    {
      std::uint32_t sz_be(htonl(0xFFFFFFFF & v.size()));
      if (!output.put(static_cast<char>(0xC6))
        || !output.write((char*)(&sz_be), sizeof(sz_be))
        || !output.write(v.get<binary>().data(), v.size()))
      {
        ret = false;
      }
    }
    else if (v.type() == variant_type::floating_point)
    {
      double dbl = v.get<double>();

      if (static_cast<float>(dbl) == dbl) // Float 32
      {
        numeric_union32 nu;
        nu.f = static_cast<float>(dbl);
        std::uint32_t be(htonl(nu.i));
        ret = output.put(static_cast<char>(0xCA)).good();
        if (ret)
          ret = output.write((char*)&be, sizeof(be)).good();
      }
      else // Float 64
      {
        numeric_union64 nu;
        nu.f = dbl;
        std::uint64_t be(htonll(nu.i));
        ret = output.put(static_cast<char>(0xCB)).good();
        if (ret)
          ret = output.write((char*)&be, sizeof(be)).good();
      }
    }
    else if (v.type() == variant_type::unsigned_integer && v.get<std::uint64_t>() <= 0x7F) // positive fixint 0xxxxxxx
    {
      std::uint8_t val = static_cast<std::uint8_t>(v.get<std::uint64_t>());
      ret = output.put(static_cast<char>(std::uint8_t(0x7F) & val)).good();
    }
    else if (v.type() == variant_type::signed_integer && v.get<std::int64_t>() < 0 && v.get<std::int64_t>() >= -31) // negative fixint	111xxxxx
    {
      std::int8_t val = static_cast<std::int8_t>(v.get<std::int64_t>());
      ret = output.put(static_cast<char>(0xE0 | (0x1F & (val * -1)))).good();
    }
    else if (v.type() == variant_type::unsigned_integer && v.get<std::uint64_t>() <= std::numeric_limits<std::uint8_t>::max()) // Uint 8
    {
      std::uint8_t val = static_cast<std::uint8_t>(v.get<std::uint64_t>());
      ret = output.put(static_cast<char>(0xCC)).good();
      if (ret)
        ret = output.write((char*)&val, sizeof(std::uint8_t)).good();
    }
    else if (v.type() == variant_type::unsigned_integer && v.get<std::uint64_t>() <= std::numeric_limits<std::uint16_t>::max()) // Uint 16
    {
      std::uint16_t val = static_cast<std::uint16_t>(v.get<std::uint64_t>());
      std::uint16_t be(htons(val));
      ret = output.put(static_cast<char>(0xCD)).good();
      if (ret)
        ret = output.write((char*)&be, sizeof(be)).good();
    }
    else if (v.type() == variant_type::unsigned_integer && v.get<std::uint64_t>() <= std::numeric_limits<std::uint32_t>::max()) // Uint 32
    {
      std::uint32_t val = static_cast<std::uint32_t>(v.get<std::uint64_t>());
      std::uint32_t be(htonl(val));
      ret = output.put(static_cast<char>(0xCE)).good();
      if (ret)
        ret = output.write((char*)&be, sizeof(be)).good();
    }
    else if (v.type() == variant_type::unsigned_integer) // Uint 64
    {
      std::uint64_t be(htonll(v.get<std::uint64_t>()));
      ret = output.put(static_cast<char>(0xCF)).good();
      if (ret)
        ret = output.write((char*)&be, sizeof(be)).good();
    }
    else if (v.type() == variant_type::signed_integer && v.get<std::int64_t>() >= std::numeric_limits<std::int8_t>::min() && v.get<std::int64_t>() <= std::numeric_limits<std::int8_t>::max()) // Int 8
    {
      std::int8_t val = static_cast<std::int8_t>(v.get<std::int64_t>());
      ret = output.put(static_cast<char>(0xD0)).good();
      if (ret)
        ret = output.write((char*)&val, sizeof(std::int8_t)).good();
    }
    else if (v.type() == variant_type::signed_integer && v.get<std::int64_t>() >= std::numeric_limits<std::int16_t>::min() && v.get<std::int64_t>() <= std::numeric_limits<std::int16_t>::max()) // Int 16
    {
      std::int16_t val = static_cast<std::int16_t>(v.get<std::int64_t>());
      std::int16_t be(htons(val));
      ret = output.put(static_cast<char>(0xD1)).good();
      if (ret)
        ret = output.write((char*)&be, sizeof(be)).good();
    }
    else if (v.type() == variant_type::signed_integer && v.get<std::int64_t>() >= std::numeric_limits<std::int32_t>::min() && v.get<std::int64_t>() <= std::numeric_limits<std::int32_t>::max()) // Int 32
    {
      std::int32_t val = static_cast<std::int32_t>(v.get<std::int64_t>());
      std::int32_t be(htonl(val));
      ret = output.put(static_cast<char>(0xD2)).good();
      if (ret)
        ret = output.write((char*)&be, sizeof(be)).good();
    }
    else if (v.type() == variant_type::signed_integer) // Int 64
    {
      std::int64_t be(htonll(v.get<std::int64_t>()));
      ret = output.put(static_cast<char>(0xD3)).good();
      if (ret)
        ret = output.write((char*)&be, sizeof(be)).good();
    }
    else if (v.is<std::string>() && v.size() <= 0xFF) // str 8
    {
      if (!output.put(static_cast<char>(0xD9))
        || !output.put(static_cast<char>(0xFF & v.size()))
        || !output.write(v.get<std::string>().data(), v.size()))
      {
        ret = false;
      }
    }
    else if (v.is<std::string>() && v.size() <= 0xFFFF) // str 16
    {
      std::uint16_t sz_be(htons(0xFFFF & v.size()));
      if (!output.put(static_cast<char>(0xDA))
        || !output.write((char*)(&sz_be), sizeof(sz_be))
        || !output.write(v.get<std::string>().data(), v.size()))
      {
        ret = false;
      }
    }
    else if (v.is<std::string>() && v.size() <= 0xFFFFFFFF) // str 32
    {
      std::uint32_t sz_be(htonl(0xFFFFFFFF & v.size()));
      if (!output.put(static_cast<char>(0xDB))
        || !output.write((char*)(&sz_be), sizeof(sz_be))
        || !output.write(v.get<std::string>().data(), v.size()))
      {
        ret = false;
      }
    }
    else if (v.is<array>() && v.size() <= 0xFFFF) // array 16
    {
      std::uint16_t sz_be(htons(0xFFFF & v.size()));
      if (!output.put(static_cast<char>(0xDC))
        || !output.write((char*)(&sz_be), sizeof(sz_be)))
      {
        ret = false;
      }
      else
      {
        for (size_t i = 0; i < v.size() && ret; ++i)
          ret = msgpack::serialize(v[i], output);
      }
    }
    else if (v.is<array>() && v.size() <= 0xFFFFFFFF) // array 32
    {
      std::uint32_t sz_be(htonl(0xFFFFFFFF & v.size()));
      if (!output.put(static_cast<char>(0xDD))
        || !output.write((char*)(&sz_be), sizeof(sz_be)))
      {
        ret = false;
      }
      else
      {
        for (size_t i = 0; i < v.size() && ret; ++i)
          ret = msgpack::serialize(v[i], output);
      }
    }
    else if (v.is<object>() && v.size() <= 0xFFFF) // map 16
    {
      std::uint16_t sz_be(htons(0xFFFF & v.size()));
      if (!output.put(static_cast<char>(0xDE))
        || !output.write((char*)(&sz_be), sizeof(sz_be)))
      {
        ret = false;
      }
      else
      {
        const goodform::object& o = v.get<object>();
        for (goodform::object::const_iterator it = o.begin(); it != o.end() && ret; ++it)
        {
          variant key(it->first);
          if (!msgpack::serialize(key, output) || !msgpack::serialize(it->second, output))
          {
            ret = false;
          }
        }
      }
    }
    else if (v.is<object>() && v.size() <= 0xFFFFFFFF) // map 32
    {
      std::uint32_t sz_be(htonl(0xFFFFFFFF & v.size()));
      if (!output.put(static_cast<char>(0xDF))
        || !output.write((char*)(&sz_be), sizeof(sz_be)))
      {
        ret = false;
      }
      else
      {
        const goodform::object& o = v.get<object>();
        for (goodform::object::const_iterator it = o.begin(); it != o.end() && ret; ++it)
        {
          variant key(it->first);
          if (!msgpack::serialize(key, output) || !msgpack::serialize(it->second, output))
          {
            ret = false;
          }
        }
      }
    }
    else
    {
      ret = false;
    }

    return ret;
  }
  //----------------------------------------------------------------//

  //----------------------------------------------------------------//
  bool msgpack::deserialize(std::istream& input, variant& v)
  {
    unsigned int typeByte = static_cast<unsigned int>(0xFF & input.get());
    bool ret = input.good();
    if (ret)
    {
      if (/*typeByte >= 0x00 &&*/ typeByte <= 0x7F) // positive fixint 0xxxxxxx
      {
        v = std::uint8_t(0x7F & typeByte);
      }
      else if (typeByte >= 0xE0 && typeByte <= 0xFF) // negative fixint	111xxxxx
      {
        v = std::int8_t((0x1F & typeByte ) * -1);
      }
      else if (typeByte >= 0x80 && typeByte <= 0x8F) // fixmap	1000xxxx
      {
        std::size_t sz(0x0F & typeByte);
        v = variant_type::object;
        for (size_t i = 0; i < sz; ++i)
        {
          variant key;
          if (!msgpack::deserialize(input, key) || !key.is<std::string>()) // Only supporting string keys.
            ret = false;
          else
            ret = msgpack::deserialize(input, v[key.get<std::string>()]);
        }
      }
      else if (typeByte >= 0x90 && typeByte <= 0x9F) // fixarray	1001xxxx
      {
        std::size_t sz(0x0F & typeByte);
        goodform::array a(sz);
        for (size_t i = 0; i < sz; ++i)
          msgpack::deserialize(input, a[i]);
        v = std::move(a);
      }
      else if (typeByte >= 0xA0 && typeByte <= 0xBF) // fixstr	101xxxxx
      {
        std::size_t sz(0x1F & typeByte);
        std::string s;
        s.resize(sz);
        input.read(&s[0], sz);
        if (!input.good())
          ret = false;
        else
          v = std::move(s);
      }
      else if (typeByte == 0xC0) // null
      {
        v = nullptr;
      }
      else if (typeByte == 0xC2) // false
      {
        v = false;
      }
      else if (typeByte == 0xC3) // true
      {
        v = true;
      }
      else if (typeByte == 0xC4) // Bin 8
      {
        std::size_t sz (0xFF & static_cast<unsigned int>(input.get()));
        ret = input.good();
        if (ret)
        {
          goodform::binary b(sz);
          if (!input.read(b.data(), sz).good())
            ret = false;
          else
            v = std::move(b);
        }
      }
      else if (typeByte == 0xC5) // Bin 16
      {
        std::uint16_t sz_be;
        ret = input.read((char*)&sz_be, sizeof(sz_be)).good();
        if (ret)
        {
          goodform::binary b(ntohs(sz_be));
          if (!input.read(b.data(), b.size()).good())
            ret = false;
          else
            v = std::move(b);
        }
      }
      else if (typeByte == 0xC6) // Bin 32
      {
        std::uint32_t sz_be;
        ret = input.read((char*)&sz_be, sizeof(sz_be)).good();
        if (ret)
        {
          goodform::binary b(ntohl(sz_be));
          if (!input.read(b.data(), b.size()).good())
            ret = false;
          else
            v = std::move(b);
        }
      }
      else if (typeByte == 0xCA) // Float 32
      {
        std::uint32_t be;
        ret = input.read((char*)&be, sizeof(be)).good();
        if (ret)
        {
          numeric_union32 u;
          u.i = ntohl(be);
          v = u.f;
        }
      }
      else if (typeByte == 0xCB) // Float 64
      {
        std::uint64_t be;
        ret = input.read((char*)&be, sizeof(be)).good();
        if (ret)
        {
          numeric_union64 u;
          u.i = ntohll(be);
          v = u.f;
        }
      }
      else if (typeByte == 0xCC) // Uint 8
      {
        std::uint8_t val;
        ret = input.read((char*)&val, sizeof(val)).good();
        if (ret)
          v = val;
      }
      else if (typeByte == 0xCD) // Uint 16
      {
        std::uint16_t be;
        ret = input.read((char*)&be, sizeof(be)).good();
        if (ret)
        {
          std::uint16_t val(ntohs(be));
          v = val;
        }
      }
      else if (typeByte == 0xCE) // Uint 32
      {
        std::uint32_t be;
        ret = input.read((char*)&be, sizeof(be)).good();
        if (ret)
        {
          std::uint32_t val(ntohl(be));
          v = val;
        }
      }
      else if (typeByte == 0xCF) // Uint 64
      {
        std::uint64_t be;
        ret = input.read((char*)&be, sizeof(be)).good();
        if (ret)
        {
          std::uint64_t val(ntohll(be));
          v = val;
        }
      }
      else if (typeByte == 0xD0) // Int 8
      {
        std::int8_t val;
        ret = input.read((char*)&val, sizeof(val)).good();
        if (ret)
          v = val;
      }
      else if (typeByte == 0xD1) // Int 16
      {
        std::int16_t be;
        ret = input.read((char*)&be, sizeof(be)).good();
        if (ret)
        {
          std::int16_t val(ntohs(be));
          v = val;
        }
      }
      else if (typeByte == 0xD2) // Int 32
      {
        std::int32_t be;
        ret = input.read((char*)&be, sizeof(be)).good();
        if (ret)
        {
          std::int32_t val(ntohl(be));
          v = val;
        }
      }
      else if (typeByte == 0xD3) // Int 64
      {
        std::int64_t be;
        ret = input.read((char*)&be, sizeof(be)).good();
        if (ret)
        {
          std::int64_t val(ntohll(be));
          v = val;
        }
      }
      else if (typeByte == 0xD9) // str 8
      {
        std::size_t sz(0xFF & static_cast<unsigned int>(input.get()));
        ret = input.good();
        if (ret)
        {
          std::string s;
          s.resize(sz);
          if (!input.read(&s[0], sz).good())
            ret = false;
          else
            v = std::move(s);
        }
      }
      else if (typeByte == 0xDA) // str 16
      {
        std::uint16_t sz_be;
        ret = input.read((char*)&sz_be, sizeof(sz_be)).good();
        if (ret)
        {
          std::string s;
          s.resize(ntohs(sz_be));
          if (!input.read(&s[0], s.size()).good())
            ret = false;
          else
            v = std::move(s);
        }
      }
      else if (typeByte == 0xDB) // str 32
      {
        std::uint32_t sz_be;
        ret = input.read((char*)&sz_be, sizeof(sz_be)).good();
        if (ret)
        {
          std::string s;
          s.resize(ntohl(sz_be));
          if (!input.read(&s[0], s.size()).good())
            ret = false;
          else
            v = std::move(s);
        }
      }
      else if (typeByte == 0xDC) // array 16
      {
        std::uint16_t sz_be;
        ret = input.read((char*)&sz_be, sizeof(sz_be)).good();
        if (ret)
        {
          goodform::array a(ntohs(sz_be));
          for (size_t i = 0; ret && i < a.size(); ++i)
            ret = msgpack::deserialize(input, a[i]);
          v = std::move(a);
        }
      }
      else if (typeByte == 0xDD) // array 32
      {
        std::uint32_t sz_be;
        ret = input.read((char*)&sz_be, sizeof(sz_be)).good();
        if (ret)
        {
          goodform::array a(ntohl(sz_be));
          for (size_t i = 0; ret && i < a.size(); ++i)
            ret = msgpack::deserialize(input, a[i]);
          v = std::move(a);
        }
      }
      else if (typeByte == 0xDE) // map 16
      {
        std::uint16_t sz_be;
        ret = input.read((char*)&sz_be, sizeof(sz_be)).good();
        if (ret)
        {
          v = variant_type::object;
          std::uint16_t sz(ntohs(sz_be));
          for (size_t i = 0; ret && i < sz; ++i)
          {
            variant key;
            if (!msgpack::deserialize(input, key) || !key.is<std::string>()) // Only supporting string keys.
              ret = false;
            else
              ret = msgpack::deserialize(input, v[key.get<std::string>()]);
          }
        }
      }
      else if (typeByte == 0xDF) // map 32
      {
        std::uint32_t sz_be;
        ret = input.read((char*)&sz_be, sizeof(sz_be)).good();
        if (ret)
        {
          v = variant_type::object;
          std::uint32_t sz(ntohl(sz_be));
          for (size_t i = 0; ret && i < sz; ++i)
          {
            variant key;
            if (!msgpack::deserialize(input, key) || !key.is<std::string>()) // Only supporting string keys.
              ret = false;
            else
              ret = msgpack::deserialize(input, v[key.get<std::string>()]);
          }
        }
      }
      else // Not Supported
      {
        ret = false;
      }
    }

    return ret;
  }
  //----------------------------------------------------------------//
}