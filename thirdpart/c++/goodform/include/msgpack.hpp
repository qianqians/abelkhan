#pragma once
#ifndef GOODFORM_MSGPACK_HPP
#define GOODFORM_MSGPACK_HPP

#include <iostream>
#include <iterator>

#include "variant.hpp"

//######################################################################//
namespace goodform
{
  //**********************************************************************//
  class msgpack
  {
  private:
    union numeric_union32
    {
      float f;
      std::uint32_t i;
    };

    union numeric_union64
    {
      double f;
      std::uint64_t i;
    };

    msgpack() = delete;
    //static bool deserializeobject();

  public:
    static bool serialize(const variant& v, std::ostream& output);
    static bool deserialize(std::istream& input, variant& v);
  };
}

#endif //GOODFORM_MSGPACK_HPP
