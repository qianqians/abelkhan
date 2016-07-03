
#include "variant.hpp"

#include <assert.h>

//######################################################################//
namespace goodform
{
  //----------------------------------------------------------------------//
  const variant null_variant = goodform::variant(nullptr);

  const std::nullptr_t const_null = nullptr;
  const bool const_bool = false;
  const std::int64_t  const_int64  = 0;
  const std::uint64_t const_uint64 = 0;
  const double const_double = 0.0;
  const std::string const_string;
  const binary const_binary;
  const array const_array;
  const object const_object;
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  void variant::destroy()
  {
    if (this->type_ == variant_type::string)
      this->data_.string_.~basic_string();     //~string();
    else if (this->type_ == variant_type::binary)
      this->data_.binary_.~vector();
    else if (this->type_ == variant_type::array)
      this->data_.array_.~vector();
    else if (this->type_ == variant_type::object)
      this->data_.object_.~map();
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  void variant::move(variant&& source)
  {
    this->type_ = source.type_;

    if (this->type_ == variant_type::boolean)
      this->data_.boolean_ = source.data_.boolean_;
    //else if (this->type_ == variant_type::int8)
    //  this->data_.int8_ = source.data_.int8_;
    //else if (this->type_ == variant_type::int16)
    //  this->data_.int16_ = source.data_.int16_;
    //else if (this->type_ == variant_type::int32)
    //  this->data_.int32_ = source.data_.int32_;
    //else if (this->type_ == variant_type::int64)
    //  this->data_.int64_ = source.data_.int64_;
    //else if (this->type_ == variant_type::uint8)
    //  this->data_.uint8_ = source.data_.uint8_;
    //else if (this->type_ == variant_type::uint16)
    //  this->data_.uint16_ = source.data_.uint16_;
    //else if (this->type_ == variant_type::uint32)
    //  this->data_.uint32_ = source.data_.uint32_;
    //else if (this->type_ == variant_type::uint64)
    //  this->data_.uint64_ = source.data_.uint64_;
    //else if (this->type_ == variant_type::float32)
    //  this->data_.float32_ = source.data_.float32_;
    //else if (this->type_ == variant_type::float64)
    //  this->data_.float64_ = source.data_.float64_;
    else if (this->type_ == variant_type::signed_integer)
      this->data_.signed_integer_ = source.data_.signed_integer_;
    else if (this->type_ == variant_type::unsigned_integer)
      this->data_.unsigned_integer_ = source.data_.unsigned_integer_;
    else if (this->type_ == variant_type::floating_point)
      this->data_.floating_point_ = source.data_.floating_point_;
    else if (this->type_ == variant_type::string)
      new(&this->data_.string_) std::string(std::move(source.data_.string_));
    else if (this->type_ == variant_type::binary)
      new(&this->data_.binary_) binary(std::move(source.data_.binary_));
    else if (this->type_ == variant_type::array)
      new(&this->data_.array_) array(std::move(source.data_.array_));
    else if (this->type_ == variant_type::object)
      new(&this->data_.object_) object(std::move(source.data_.object_));
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  void variant::copy(const variant& source)
  {
    this->type_ = source.type_;

    if (this->type_ == variant_type::boolean)
      this->data_.boolean_ = source.data_.boolean_;
    //else if (this->type_ == variant_type::int8)
    //  this->data_.int8_ = source.data_.int8_;
    //else if (this->type_ == variant_type::int16)
    //  this->data_.int16_ = source.data_.int16_;
    //else if (this->type_ == variant_type::int32)
    //  this->data_.int32_ = source.data_.int32_;
    //else if (this->type_ == variant_type::int64)
    //  this->data_.int64_ = source.data_.int64_;
    //else if (this->type_ == variant_type::uint8)
    //  this->data_.uint8_ = source.data_.uint8_;
    //else if (this->type_ == variant_type::uint16)
    //  this->data_.uint16_ = source.data_.uint16_;
    //else if (this->type_ == variant_type::uint32)
    //  this->data_.uint32_ = source.data_.uint32_;
    //else if (this->type_ == variant_type::uint64)
    //  this->data_.uint64_ = source.data_.uint64_;
    //else if (this->type_ == variant_type::float32)
    //  this->data_.float32_ = source.data_.float32_;
    //else if (this->type_ == variant_type::float64)
    //  this->data_.float64_ = source.data_.float64_;
    else if (this->type_ == variant_type::signed_integer)
      this->data_.signed_integer_ = source.data_.signed_integer_;
    else if (this->type_ == variant_type::unsigned_integer)
      this->data_.unsigned_integer_ = source.data_.unsigned_integer_;
    else if (this->type_ == variant_type::floating_point)
      this->data_.floating_point_ = source.data_.floating_point_;
    else if (this->type_ == variant_type::string)
      new(&this->data_.string_) std::string(source.data_.string_);
    else if (this->type_ == variant_type::binary)
      new(&this->data_.binary_) binary(source.data_.binary_);
    else if (this->type_ == variant_type::array)
      new(&this->data_.array_) array(source.data_.array_);
    else if (this->type_ == variant_type::object)
      new(&this->data_.object_) object(source.data_.object_);
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant::variant(variant&& source)
  {
    this->move(std::move(source));
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant::variant(const variant& source)
  {
    this->copy(source);
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant& variant::operator=(variant&& source)
  {
    if (this != &source)
    {
      this->destroy();
      this->move(std::move(source));
    }
    return *this;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant& variant::operator=(const variant& source)
  {
    if (this != &source)
    {
      this->destroy();
      this->copy(source);
    }
    return *this;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant::variant(std::nullptr_t value)
  {
    this->type_ = variant_type::null;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant::~variant()
  {
    this->destroy();
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant::variant(bool value)
  {
    this->type_ = variant_type::boolean;
    this->data_.boolean_ = value;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant::variant(std::int8_t value)
  {
    this->type_ = variant_type::signed_integer;
    this->data_.signed_integer_ = value;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant::variant(std::int16_t value)
  {
    this->type_ = variant_type::signed_integer;
    this->data_.signed_integer_ = value;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant::variant(std::int32_t value)
  {
    this->type_ = variant_type::signed_integer;
    this->data_.signed_integer_ = value;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant::variant(std::int64_t value)
  {
    this->type_ = variant_type::signed_integer;
    this->data_.signed_integer_ = value;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant::variant(std::uint8_t value)
  {
    this->type_ = variant_type::unsigned_integer;
    this->data_.unsigned_integer_ = value;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant::variant(std::uint16_t value)
  {
    this->type_ = variant_type::unsigned_integer;
    this->data_.unsigned_integer_ = value;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant::variant(std::uint32_t value)
  {
    this->type_ = variant_type::unsigned_integer;
    this->data_.unsigned_integer_ = value;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant::variant(std::uint64_t value)
  {
    this->type_ = variant_type::unsigned_integer;
    this->data_.unsigned_integer_ = value;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant::variant(float value)
  {
    this->type_ = variant_type::floating_point;
    this->data_.floating_point_ = value;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant::variant(double value)
  {
    this->type_ = variant_type::floating_point;
    this->data_.floating_point_ = value;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant::variant(const char*const value)
  {
    this->type_ = variant_type::string;
    new(&this->data_.string_) std::string(value);
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant::variant(const char*const value, size_t size)
  {
    this->type_ = variant_type::string;
    new(&this->data_.string_) std::string(value, size);
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant::variant(std::string&& value)
  {
    this->type_ = variant_type::string;
    new(&this->data_.string_) std::string(std::move(value));
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant::variant(goodform::binary&& value)
  {
    this->type_ = variant_type::binary;
    new (&this->data_.binary_) binary(std::move(value));
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant::variant(goodform::array&& value)
  {
    this->type_ = variant_type::array;
    new (&this->data_.array_) array(std::move(value));
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant::variant(goodform::object&& value)
  {
    this->type_ = variant_type::object;
    new(&this->data_.object_) object(std::move(value));
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant::variant(const std::string& value)
  {
    this->type_ = variant_type::string;
    new(&this->data_.string_) std::string(value);
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant::variant(const binary& value)
  {
    this->type_ = variant_type::binary;
    new (&this->data_.binary_) binary(value);
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant::variant(const array& value)
  {
    this->type_ = variant_type::array;
    new (&this->data_.array_) array(value);
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant::variant(const object& value)
  {
    this->type_ = variant_type::object;
    new(&this->data_.object_) object(value);
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant::variant(variant_type type)
  {
    this->type_ = type;
    if (this->type_ == variant_type::string)
      new (&this->data_.string_) std::string();
    else if (this->type_ == variant_type::binary)
      new (&this->data_.binary_) goodform::binary();
    else if (this->type_ == variant_type::array)
      new (&this->data_.array_) goodform::array();
    else if (this->type_ == variant_type::object)
      new (&this->data_.object_) goodform::object();
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant& variant::operator=(std::nullptr_t value)
  {
    this->destroy();
    this->type_ = variant_type::null;
    return *this;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant& variant::operator=(bool value)
  {
    operator=(variant(value));
    return *this;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant& variant::operator=(std::int8_t value)
  {
    operator=(variant(value));
    return *this;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant& variant::operator=(std::int16_t value)
  {
    operator=(variant(value));
    return *this;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant& variant::operator=(std::int32_t value)
  {
    operator=(variant(value));
    return *this;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant& variant::operator=(std::int64_t value)
  {
    operator=(variant(value));
    return *this;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant& variant::operator=(std::uint8_t value)
  {
    operator=(variant(value));
    return *this;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant& variant::operator=(std::uint16_t value)
  {
    operator=(variant(value));
    return *this;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant& variant::operator=(std::uint32_t value)
  {
    operator=(variant(value));
    return *this;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant& variant::operator=(std::uint64_t value)
  {
    operator=(variant(value));
    return *this;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant& variant::operator=(float value)
  {
    operator=(variant(value));
    return *this;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant& variant::operator=(double value)
  {
    operator=(variant(value));
    return *this;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant& variant::operator=(const char* value)
  {
    operator=(variant(value));
    return *this;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant& variant::operator=(std::string&& value)
  {
    operator=(variant(std::move(value)));
    return *this;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant& variant::operator=(goodform::binary&& value)
  {
    operator=(variant(std::move(value)));
    return *this;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant& variant::operator=(goodform::array&& value)
  {
    operator=(variant(std::move(value)));
    return *this;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant& variant::operator=(goodform::object&& value)
  {
    operator=(variant(std::move(value)));
    return *this;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant& variant::operator=(const std::string& value)
  {
    operator=(variant(value));
    return *this;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant& variant::operator=(const binary& value)
  {
    operator=(variant(value));
    return *this;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant& variant::operator=(const array& value)
  {
    operator=(variant(value));
    return *this;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant& variant::operator=(const object& value)
  {
    operator=(variant(value));
    return *this;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant& variant::operator=(variant_type type)
  {
    operator=(variant(type));
    return *this;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant_type variant::type() const
  {
    return this->type_;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  void variant::type(variant_type type)
  {
    if (this->type_ != type)
    {
      this->destroy();
      if (type == variant_type::string)
        new (&this->data_.string_) std::string();
      else if (type == variant_type::binary)
        new (&this->data_.binary_) goodform::binary();
      else if (type == variant_type::array)
        new (&this->data_.array_) goodform::array();
      else if (type == variant_type::object)
        new (&this->data_.object_) goodform::object();
      this->type_ = type;
    }
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  template<>
  bool variant::is<std::nullptr_t>() const { return (this->type_ == variant_type::null); }
  template<>
  bool variant::is<bool>() const { return (this->type_ == variant_type::boolean); }
  template<>
  bool variant::is<std::int64_t>() const { return (this->type_ == variant_type::signed_integer); }
  template<>
  bool variant::is<std::uint64_t>() const { return (this->type_ == variant_type::unsigned_integer); }
  template<>
  bool variant::is<double>() const { return (this->type_ == variant_type::floating_point); }
  template<>
  bool variant::is<std::string>() const { return (this->type_ == variant_type::string); }
  template<>
  bool variant::is<goodform::binary>() const { return (this->type_ == variant_type::binary); }
  template<>
  bool variant::is<goodform::array>() const { return (this->type_ == variant_type::array); }
  template<>
  bool variant::is<goodform::object>() const { return (this->type_ == variant_type::object);  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  template<typename T>
  bool variant::can_be() const
  {
    return (this->type() == variant_type::signed_integer && this->data_.signed_integer_ >= std::numeric_limits<T>::min() && this->data_.signed_integer_ <= std::numeric_limits<T>::max())
      || (this->type() == variant_type::unsigned_integer && this->data_.unsigned_integer_ <= std::numeric_limits<T>::max());
  }
  template bool variant::can_be<std::int8_t>() const;
  template bool variant::can_be<std::int16_t>() const;
  template bool variant::can_be<std::int32_t>() const;
  template bool variant::can_be<std::int64_t>() const;
  template bool variant::can_be<std::uint8_t>() const;
  template bool variant::can_be<std::uint16_t>() const;
  template bool variant::can_be<std::uint32_t>() const;
  template bool variant::can_be<std::uint64_t>() const;
  template<> bool variant::can_be<float>() const
  {
    return (this->type() == variant_type::floating_point || this->type() == variant_type::signed_integer || this->type() == variant_type::unsigned_integer);
  }
  template<> bool variant::can_be<double>() const
  {
    return (this->type() == variant_type::floating_point || this->type() == variant_type::signed_integer || this->type() == variant_type::unsigned_integer);
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  template<>
  const std::nullptr_t& variant::get<std::nullptr_t>() const { return const_null; }
  template<>
  const bool& variant::get<bool>() const
  {
    return (this->type_ == variant_type::boolean ? this->data_.boolean_ : const_bool);
  }
  template<>
  const std::int64_t& variant::get<std::int64_t>() const
  {
    return (this->type_ == variant_type::signed_integer ?  this->data_.signed_integer_ : const_int64);
  }
  template<>
  const std::uint64_t& variant::get<std::uint64_t>() const
  {
    return (this->type_ == variant_type::unsigned_integer ? this->data_.unsigned_integer_ : const_uint64);
  }
  template<>
  const double& variant::get<double>() const
  {
    return (this->type_ == variant_type::floating_point ? this->data_.floating_point_ : const_double);
  }
  template<>
  const std::string& variant::get<std::string>() const
  {
    return (this->type_ == variant_type::string ? this->data_.string_ : const_string);
  }
  template<>
  const goodform::binary& variant::get<goodform::binary>() const
  {
    return (this->type_ == variant_type::binary ? this->data_.binary_ : const_binary);
  }
  template<>
  const goodform::array& variant::get<goodform::array>() const
  {
    return (this->type_ == variant_type::array ? this->data_.array_ : const_array);
  }
  template<>
  const goodform::object& variant::get<goodform::object>() const
  {
    return (this->type_ == variant_type::object ? this->data_.object_ : const_object);
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  template<typename T>
  bool variant::get(T& dest) const
  {
    bool ret =  this->can_be<T>();

    if (ret)
    {
      if (this->type() == variant_type::signed_integer)
        dest = static_cast<T>(this->data_.signed_integer_);
      else if (this->type() == variant_type::unsigned_integer)
        dest = static_cast<T>(this->data_.unsigned_integer_);
      else
        assert(!"This should never happen");
    }

    return ret;
  }

  template bool variant::get<std::int8_t>(std::int8_t& dest) const;
  template bool variant::get<std::int16_t>(std::int16_t& dest) const;
  template bool variant::get<std::int32_t>(std::int32_t& dest) const;
  template bool variant::get<std::int64_t>(std::int64_t& dest) const;
  template bool variant::get<std::uint8_t>(std::uint8_t& dest) const;
  template bool variant::get<std::uint16_t>(std::uint16_t& dest) const;
  template bool variant::get<std::uint32_t>(std::uint32_t& dest) const;
  template bool variant::get<std::uint64_t>(std::uint64_t& dest) const;

  template<>
  bool variant::get(bool& dest) const
  {
    bool ret = (this->type_ == variant_type::boolean);
    if (ret)
      dest = this->data_.boolean_;
    return ret;
  }

  template<>
  bool variant::get(float& dest) const
  {
    bool ret = this->can_be<float>();
    if (ret)
    {
      if (this->type() == variant_type::signed_integer)
        dest = static_cast<float>(this->data_.signed_integer_);
      else if (this->type() == variant_type::unsigned_integer)
        dest = static_cast<float>(this->data_.unsigned_integer_);
      else if (this->type() == variant_type::floating_point)
        dest = static_cast<float>(this->data_.floating_point_);
      else
        assert(!"This should never happen");
    }

    return ret;
  }

  template<>
  bool variant::get(double& dest) const
  {
    bool ret = this->can_be<double>();
    if (ret)
    {
      if (this->type() == variant_type::signed_integer)
        dest = static_cast<float>(this->data_.signed_integer_);
      else if (this->type() == variant_type::unsigned_integer)
        dest = static_cast<float>(this->data_.unsigned_integer_);
      else if (this->type() == variant_type::floating_point)
        dest = this->data_.floating_point_;
      else
        assert(!"This should never happen");
    }
    return ret;
  }

  template<>
  bool variant::get(std::string& dest) const
  {
    bool ret = (this->type_ == variant_type::string);
    if (ret)
      dest = this->data_.string_;
    return ret;
  }

  template<>
  bool variant::get(goodform::binary& dest) const
  {
    bool ret = (this->type_ == variant_type::binary);
    if (ret)
      dest = this->data_.binary_;
    return ret;
  }

  template<>
  bool variant::get(goodform::array& dest) const
  {
    bool ret = (this->type_ == variant_type::array);
    if (ret)
      dest = this->data_.array_;
    return ret;
  }

  template<>
  bool variant::get(goodform::object& dest) const
  {
    bool ret = (this->type_ == variant_type::object);
    if (ret)
      dest = this->data_.object_;
    return ret;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant& variant::operator[](size_t index)
  {
    if (this->type_ != variant_type::array)
    {
      this->destroy();
      this->type_ = variant_type::array;
      new (&this->data_.array_) goodform::array();
    }
    while (index >= this->data_.array_.size())
      this->data_.array_.push_back(variant());
    return this->data_.array_[index];
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  variant& variant::operator[](const std::string& index)
  {
    if (this->type_ != variant_type::object)
    {
      this->destroy();
      this->type_ = variant_type::object;
      new (&this->data_.object_) goodform::object();
    }
    return this->data_.object_[index];
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  const variant& variant::operator[](size_t index) const
  {
    if (this->type_ == variant_type::array && index < this->data_.array_.size())
      return this->data_.array_[index];
    else
      return null_variant;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  const variant& variant::operator[](const std::string& index) const
  {
    if (this->type_ != variant_type::object)
      return null_variant;
    else
    {
      auto it =  this->data_.object_.find(index);
      if (it != this->data_.object_.end())
        return it->second;
      else
        return null_variant;
    }
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  size_t variant::size() const
  {
    size_t ret = 0;
    if (this->type_ == variant_type::object)
      ret = this->data_.object_.size();
    else if (this->type_ == variant_type::array)
      ret = this->data_.array_.size();
    else if (this->type_ == variant_type::binary)
      ret = this->data_.binary_.size();
    else if (this->type_ == variant_type::string)
      ret = this->data_.string_.size();
    return ret;
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  void variant::push(const variant& value)
  {
    if (this->type_ != variant_type::array)
    {
      this->destroy();
      this->type_ = variant_type::array;
      new (&this->data_.array_) goodform::array();
    }
    this->data_.array_.push_back(value);
  }
  //----------------------------------------------------------------------//

  //----------------------------------------------------------------------//
  void variant::push(variant&& value)
  {
    if (this->type_ != variant_type::array)
    {
      this->destroy();
      this->type_ = variant_type::array;
      new (&this->data_.array_) goodform::array();
    }
    this->data_.array_.push_back(std::move(value));
  }
  //----------------------------------------------------------------------//

#if !defined(GOOODFORM_NO_CAST_OPERATOR_OVERLOADS)
  //---------------------------------------------------------------------//
  variant::operator bool() const
  {
    return this->get<bool>();
  }
  //---------------------------------------------------------------------//

  //---------------------------------------------------------------------//
  variant::operator std::int64_t() const
  {
    return this->get<std::int64_t>();
  }
  //---------------------------------------------------------------------//

  //---------------------------------------------------------------------//
  variant::operator std::uint64_t() const
  {
    return this->get<std::uint64_t>();
  }
  //---------------------------------------------------------------------//

  //---------------------------------------------------------------------//
  variant::operator double() const
  {
    return this->get<double>();
  }
  //---------------------------------------------------------------------//

  //---------------------------------------------------------------------//
  variant::operator const std::string&() const
  {
    return this->get<std::string>();
  }
  //---------------------------------------------------------------------//

  //---------------------------------------------------------------------//
  variant::operator const goodform::binary&() const
  {
    return this->get<binary>();
  }
  //---------------------------------------------------------------------//

  //---------------------------------------------------------------------//
  variant::operator const goodform::array&() const
  {
    return this->get<array>();
  }
  //---------------------------------------------------------------------//

  //---------------------------------------------------------------------//
  variant::operator const goodform::object&() const
  {
    return this->get<object>();
  }
  //----------------------------------------------------------------------//
#endif
}
//######################################################################//
