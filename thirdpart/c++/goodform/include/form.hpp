#ifndef GOODFORM_FORM_HPP
#define GOODFORM_FORM_HPP

#include "variant.hpp"

#include <functional>
#include <regex>

//######################################################################//
namespace goodform
{

  class error_message
  {
  private:
    std::string message_;
  public:
    error_message(const std::string& message) { this->message_ = message; }
    bool empty() const { return this->message_.empty(); }
    const std::string& message() const; // { return this->message_; }
  };

  //======================================================================//
  class boolean_validator
  {
  private:
    const bool value_;
  public:
    boolean_validator(bool value);

    /**
     * @return Returns boolean value.
     */
    bool val() const;
  };
  //======================================================================//

  //======================================================================//
  template <typename N>
  class number_validator
  {
  private:
    error_message& error_;
    const N value_;
  public:
    number_validator(N value, error_message& error_message);
    number_validator(const number_validator& source);

    /**
     * @return Returns numeric value of type N.
     */
    N val() const;

    /**
     * Validates that value is greater than parameter.
     *
     * @param greater_than Numeric value to check against.
     * @return Returns number_validator object for further validation.
     */
    number_validator gt(N greater_than);

    /**
     * Validates that value is less than than parameter.
     *
     * @param less_than Numeric value to check against.
     * @return Returns number_validator object for further validation.
     */
    number_validator lt(N less_than);

    /**
     * Validates that value is greater than or equal to parameter.
     *
     * @param greater_or_equal_to Numeric value to check against.
     * @return Returns number_validator object for further validation.
     */
    number_validator gte(N greater_or_equal_to);

    /**
     * Validates that value is less than or equal to parameter.
     *
     * @param less_or_equal_to Numeric value to check against.
     * @return Returns number_validator object for further validation.
     */
    number_validator lte(N less_or_equal_to);
  };
  //======================================================================//

  //======================================================================//
  class string_validator
  {
  private:
    error_message& error_;
    const std::string& value_;
  public:
    string_validator(const std::string& value, error_message& error_message);

    /**
     * Matches string value against regular expression.
     *
     * @param expression Regular expression to match against.
     * @return Returns string_validator object for further validation.
     */
    string_validator match(std::regex expression);

    /**
    * @return Returns string value.
    */
    const std::string& val() const;
  };
  //======================================================================//

  class sub_form;

  //======================================================================//
  class array_validator
  {
  private:
    error_message& error_;
    const std::vector<variant>& value_;
  public:
    array_validator(const std::vector<variant>& value, error_message& error_message);

    /**
     * Retrieves variant at array index or sets error if index does not exist.
     *
     * @param index Position in array.
     * @return Returns sub_form object for further validation.
     */
    sub_form at(size_t index);

    /**
     * Retrieves variant at array index or default_variant if index does not exist.
     *
     * @param index Position in array.
     * @param default_variant Variant to use if index does not exist.
     * @return Returns sub_form object for further validation.
     */
    sub_form at(size_t index, const variant& default_variant);

    /**
     * Iterates elements in array.
     *
     * @param fn Function used to validate each array element.
     */
    void for_each(const std::function<void(sub_form& element, size_t index)>& fn);

    /**
     * @return Returns array value.
     */
    const std::vector<variant>& val() const;
  };
  //======================================================================//

  //======================================================================//
  class object_validator
  {
  private:
    error_message& error_;
    const std::map<std::string, variant>& value_;
  public:
    object_validator(const std::map<std::string, variant>& value, error_message& error_message);

    /**
     * Retrieves variant at key or sets error if key does not exist.
     *
     * @param key Key in key-value map.
     * @return Returns sub_form object for further validation.
     */
    sub_form at(const std::string& key);

    /**
     * Retrieves variant at key or default_variant if index does not exist.
     *
     * @param key Key in key-value map.
     * @param default_variant Variant to use if key does not exist.
     * @return Returns sub_form object for further validation.
     */
    sub_form at(const std::string& key, const variant& default_variant);

    /**
     * @return Returns object value.
     */
    const std::map<std::string, variant>& val() const;
  };
  //======================================================================//

  //======================================================================//
  class sub_form
  {
  private:
    error_message& error_;
    //static double convert_to_double(const variant& v);
  protected:
    const variant& variant_;
  public:
    sub_form(const variant& v, error_message& error_message);
    sub_form(const sub_form& source);

    /**
     * Verifies that the current variant has a boolean value and otherwise sets an error.
     *
     * @return boolean_validator object.
     */
    boolean_validator boolean();

    /**
     * Verifies that the current variant has a boolean value and otherwise uses default passed as parameter.
     *
     * @param default_value The value to use if variant is not boolean.
     * @return boolean_validator object.
     */
    boolean_validator boolean(bool default_value);

    /**
     * Verifies that the current variant's value can be represented as a signed 8-bit integer and otherwise sets an error.
     *
     * @return A number_validator object is returned for further validation of the value.
     */
    number_validator<std::int8_t> int8();

    /**
     * Verifies that the current variant's value can be represented as a signed 8-bit integer and otherwise uses default passed as parameter.
     *
     * @param default_value The value to use if validation fails.
     * @return A number_validator object is returned for further validation of the value.
     */
    number_validator<std::int8_t> int8(std::int8_t default_value);

    /**
     * Verifies that the current variant's value can be represented as a signed 16-bit integer and otherwise sets an error.
     *
     * @return A number_validator object is returned for further validation of the value.
     */
    number_validator<std::int16_t> int16();

    /**
     * Verifies that the current variant's value can be represented as a signed 16-bit integer and otherwise uses default passed as parameter.
     *
     * @param default_value The value to use if validation fails.
     * @return A number_validator object is returned for further validation of the value.
     */
    number_validator<std::int16_t> int16(std::int16_t default_value);

    /**
     * Verifies that the current variant's value can be represented as a signed 32-bit integer and otherwise sets an error.
     *
     * @return A number_validator object is returned for further validation of the value.
     */
    number_validator<std::int32_t> int32();

    /**
     * Verifies that the current variant's value can be represented as a signed 32-bit integer and otherwise uses default passed as parameter.
     *
     * @param default_value The value to use if validation fails.
     * @return A number_validator object is returned for further validation of the value.
     */
    number_validator<std::int32_t> int32(std::int32_t default_value);

    /**
     * Verifies that the current variant's value can be represented as a signed 64-bit integer and otherwise sets an error.
     *
     * @return A number_validator object is returned for further validation of the value.
     */
    number_validator<std::int64_t> int64();

    /**
     * Verifies that the current variant's value can be represented as a signed 8-bit integer and otherwise uses default passed as parameter.
     *
     * @param default_value The value to use if validation fails.
     * @return A number_validator object is returned for further validation of the value.
     */
    number_validator<std::int64_t> int64(std::int64_t default_value);

    /**
     * Verifies that the current variant's value can be represented as a unsigned 8-bit integer and otherwise sets an error.
     *
     * @return A number_validator object is returned for further validation of the value.
     */
    number_validator<std::uint8_t> uint8();

    /**
     * Verifies that the current variant's value can be represented as a unsigned 8-bit integer and otherwise uses default passed as parameter.
     *
     * @param default_value The value to use if validation fails.
     * @return A number_validator object is returned for further validation of the value.
     */
    number_validator<std::uint8_t> uint8(std::uint8_t default_value);

    /**
     * Verifies that the current variant's value can be represented as a unsigned 16-bit integer and otherwise sets an error.
     *
     * @return A number_validator object is returned for further validation of the value.
     */
    number_validator<std::uint16_t> uint16();

    /**
    * Verifies that the current variant's value can be represented as a unsigned 16-bit integer and otherwise uses default passed as parameter.
    *
    * @param default_value The value to use if validation fails.
    * @return A number_validator object is returned for further validation of the value.
    */
    number_validator<std::uint16_t> uint16(std::uint16_t default_value);

    /**
     * Verifies that the current variant's value can be represented as a unsigned 32-bit integer and otherwise sets an error.
     *
     * @return A number_validator object is returned for further validation of the value.
     */
    number_validator<std::uint32_t> uint32();

    /**
    * Verifies that the current variant's value can be represented as a unsigned 32-bit integer and otherwise uses default passed as parameter.
    *
    * @param default_value The value to use if validation fails.
    * @return A number_validator object is returned for further validation of the value.
    */
    number_validator<std::uint32_t> uint32(std::uint32_t default_value);

    /**
     * Verifies that the current variant's value can be represented as a unsigned 64-bit integer and otherwise sets an error.
     *
     * @return A number_validator object is returned for further validation of the value.
     */
    number_validator<std::uint64_t> uint64();

    /**
    * Verifies that the current variant's value can be represented as a unsigned 64-bit integer and otherwise uses default passed as parameter.
    *
    * @param default_value The value to use if validation fails.
    * @return A number_validator object is returned for further validation of the value.
    */
    number_validator<std::uint64_t> uint64(std::uint64_t default_value);

    /**
     * Verifies that the current variant's value can be represented as a float and otherwise sets an error.
     *
     * @return A number_validator object is returned for further validation of the value.
     */
    number_validator<float> float32();

    /**
    * Verifies that the current variant's value can be represented as a float and otherwise uses default passed as parameter.
    *
    * @param default_value The value to use if validation fails.
    * @return A number_validator object is returned for further validation of the value.
    */
    number_validator<float> float32(float default_value);

    /**
     * Verifies that the current variant's value can be represented as a double and otherwise sets an error.
     *
     * @return A number_validator object is returned for further validation of the value.
     */
    number_validator<double> float64();

    /**
    * Verifies that the current variant's value can be represented as a double and otherwise uses default passed as parameter.
    *
    * @param default_value The value to use if validation fails.
    * @return A number_validator object is returned for further validation of the value.
    */
    number_validator<double> float64(double default_value);

    /**
     * Verifies that the current variant has a string value and otherwise sets an error.
     *
     * @return A string_validator object is returned for further validation of the value.
     */
    string_validator string();

    /**
    * Verifies that the current variant has a string value and otherwise uses default passed as parameter.
    *
    * @param default_value The value to use if validation fails.
    * @return A string_validator object is returned for further validation of the value.
    */
    string_validator string(const std::string& default_value);

    /**
     * Verifies that the current variant has an array value and otherwise sets an error.
     *
     * @return An array_validator object is returned for further validation of the value.
     */
    array_validator array();

    /**
     * Verifies that the current variant has an array value and otherwise sets an error.
     *
     * @param default_value The value to use if validation fails.
     * @return An array_validator object is returned for further validation of the value.
     */
    array_validator array(const std::vector<variant>& default_value);

    /**
     * Verifies that the current variant has a key-value map value and otherwise sets an error.
     *
     * @return An object_validator object is returned for further validation of the value.
     */
    object_validator object();

    /**
     * Verifies that the current variant has a key-value map value and otherwise sets an error.
     *
     * @param default_value The value to use if validation fails.
     * @return An object_validator object is returned for further validation of the value.
     */
    object_validator object(const std::map<std::string, variant>& default_value);


    /**
     * Shortcut method for array_validator::at. An error is set if the current variant is not an array or the index does not exist.
     *
     * @param index The current variant's array position to access.
     * @return A sub_form object is returned representing the variant at the index passed.
     */
    sub_form at(size_t index);

    /**
     * Shortcut method for array_validator::at. The default_variant is used if the current variant is not an array or the index does not exist.
     *
     * @param index The current variant's array position to access.
     * @param default_variant The variant to use if validation fails.
     * @return A sub_form object is returned representing the variant at the index passed.
     */
    sub_form at(size_t index, const variant& default_variant);

    /**
     * Shortcut method for array_validator::for_each. An error is set if the current variant is not an array or the index does not exist.
     *
     * @param fn A function used to validate each array element.
     */
    void for_each(const std::function<void(sub_form& element, size_t index)>& fn);

    /**
     * Shortcut method for object_validator::at. An error is set if the current variant is not an object or the key does not exist.
     *
     * @param key The current variant's key-value map key to access.
     * @return A sub_form object is returned representing the variant at the key passed.
     */
    sub_form at(const std::string& key);

    /**
     * Shortcut method for object_validator::at. The default_variant is used if the current variant is not an object or the key does not exist.
     *
     * @param key The current variant's key-value map key to access.
     * @param default_variant The variant to use if validation fails.
     * @return A sub_form object is returned representing the variant at the key passed.
     */
    sub_form at(const std::string& key, const variant& default_variant);
  };
  //======================================================================//

  //======================================================================//
  class form : public sub_form
  {
  private:
    error_message error_;
  public:
    form(const variant& v);
    bool is_good() const;
  };
  //======================================================================//





  //======================================================================//
  boolean_validator::boolean_validator(bool value)
    : value_(value)
  {
  }

  bool boolean_validator::val() const
  {
    return this->value_;
  }
  //======================================================================//

  //======================================================================//
  template <typename N>
  number_validator<N>::number_validator(N value, error_message& errorMessage)
    : value_(value), error_(errorMessage)
  {

  }

  template <typename N>
  number_validator<N>::number_validator(const number_validator& source)
    : value_(source.value_), error_(source.error_)
  {

  }

  template <typename N>
  N number_validator<N>::val() const
  {
    return this->value_;
  }

  template <typename N>
  number_validator<N> number_validator<N>::gt(N greater_than)
  {
    if (this->value_ <= greater_than && this->error_.empty())
      this->error_ = "VALUE MUST BE GREATER THAN " + std::to_string(greater_than);

    number_validator<N> ret(*this);
    return ret;
  }

  template <typename N>
  number_validator<N> number_validator<N>::lt(N less_than)
  {
    if (this->value_ >= less_than && this->error_.empty())
      this->error_ = "VALUE MUST BE LESS THAN " + std::to_string(less_than);
    number_validator<N> ret(*this);
    return ret;
  }

  template <typename N>
  number_validator<N> number_validator<N>::gte(N greater_or_equal_to)
  {
    if (this->value_ < greater_or_equal_to && this->error_.empty())
      this->error_ = "VALUE MUST BE GREATER THAN OR EQUAL TO " + std::to_string(greater_or_equal_to);
    number_validator<N> ret(*this);
    return ret;
  }

  template <typename N>
  number_validator<N> number_validator<N>::lte(N less_or_equal_to)
  {
    if (this->value_ > less_or_equal_to && this->error_.empty())
      this->error_ = "VALUE MUST BE LESS THAN OR EQUAL TO " + std::to_string(less_or_equal_to);
    number_validator<N> ret(*this);
    return ret;
  }
  //======================================================================//

  //======================================================================//
  string_validator::string_validator(const std::string& value, error_message& errorMessage) : value_(value), error_(errorMessage)
  {

  }

  string_validator string_validator::match(std::regex expression)
  {
    if (!std::regex_match(this->value_, expression))
      this->error_ = error_message("STRING DOES NOT MATCH EXPRESSION");

    string_validator ret(*this);
    return ret;
  }

  const std::string& string_validator::val() const
  {
    return this->value_;
  }
  //======================================================================//

  //======================================================================//
  array_validator::array_validator(const std::vector<variant>& value, error_message& errorMessage) : value_(value), error_(errorMessage)
  {

  }


  sub_form array_validator::at(size_t index)
  {
    if (this->value_.size() > index)
    {
      sub_form ret(this->value_[index], this->error_);
      return ret;
    }
    else
    {
      sub_form ret(variant().get<std::nullptr_t>(), this->error_);
      if (this->error_.empty())
        this->error_ = error_message("INDEX (" + std::to_string(index) + ") OUT OF RANGE");
      return ret;
    }
  }

  sub_form array_validator::at(size_t index, const variant& default_variant)
  {
    sub_form ret(this->value_.size() > index ? this->value_[index] : default_variant, this->error_);
    return ret;
  }

  void array_validator::for_each(const std::function<void(sub_form& element, size_t index)>& fn)
  {
    for (size_t i = 0; i < this->value_.size(); ++i)
    {
      sub_form tmp(this->value_[i], this->error_);
      fn(tmp, i);
    }
  }

  const std::vector<variant>& array_validator::val() const
  {
    return this->value_;
  }
  //======================================================================//

  //======================================================================//
  object_validator::object_validator(const std::map<std::string,variant>& value, error_message& errorMessage) : value_(value), error_(errorMessage)
  {

  }

  sub_form object_validator::at(const std::string& key)
  {
    auto it = this->value_.find(key);
    if (it != this->value_.end())
    {
      sub_form ret(sub_form(it->second, this->error_));
      return ret;
    }
    else
    {
      sub_form ret(variant().get<std::nullptr_t>(), this->error_);
      if (this->error_.empty())
        this->error_ = error_message("KEY (" + key + ") DOES NOT EXIST");
      return ret;
    }
  }

  sub_form object_validator::at(const std::string& key, const variant& default_variant)
  {
    auto it = this->value_.find(key);
    sub_form ret(it != this->value_.end() ? it->second : default_variant, this->error_);
    return ret;
  }

  const std::map<std::string,variant>& object_validator::val() const
  {
    return this->value_;
  }
  //======================================================================//

  //======================================================================//
  sub_form::sub_form(const variant& v, error_message& err_msg) : variant_(v), error_(err_msg)
  {

  }


  sub_form::sub_form(const sub_form& source) : variant_(source.variant_), error_(source.error_)
  {

  }
  
  boolean_validator sub_form::boolean()
  {
    if (!this->variant_.is<bool>())
      this->error_ = error_message("TYPE NOT BOOLEAN");

    boolean_validator ret(this->variant_.get<bool>());
    return ret;
  }
  
  boolean_validator sub_form::boolean(bool default_value)
  {
    boolean_validator ret(this->variant_.is<bool>() ? this->variant_.get<bool>() : default_value);
    return ret;
  }

//  bool sub_form::can_be_int8(const variant& v)
//  {
//    return (v.type() == variant_type::signed_integer && v.get<std::int64_t>() >= std::numeric_limits<std::int8_t>::min() && v.get<std::int64_t>() <= std::numeric_limits<std::int8_t>::max())
//     || (v.type() == variant_type::unsigned_integer && v.get<std::uint64_t>() <= std::numeric_limits<std::int8_t>::max());
//  }
//
//  bool sub_form::can_be_int16(const variant& v)
//  {
//    return (v.type() == variant_type::signed_integer && v.get<std::int64_t>() >= std::numeric_limits<std::int16_t>::min() && v.get<std::int64_t>() <= std::numeric_limits<std::int16_t>::max())
//      || (v.type() == variant_type::unsigned_integer && v.get<std::uint64_t>() <= std::numeric_limits<std::int16_t>::max());
//  }
//
//  bool sub_form::can_be_int32(const variant& v)
//  {
//    return (v.type() == variant_type::signed_integer && v.get<std::int64_t>() >= std::numeric_limits<std::int32_t>::min() && v.get<std::int64_t>() <= std::numeric_limits<std::int32_t>::max())
//      || (v.type() == variant_type::unsigned_integer && v.get{std::uint64_t>() <= std::numeric_limits<std::int32_t>::max());
//  }
//
//  bool sub_form::can_be_int64(const variant& v)
//  {
//    return (v.type() == variant_type::signed_integer && v.get<std::int64_t>() >= std::numeric_limits<std::int64_t>::min() && v.get<std::int64_t>() <= std::numeric_limits<std::int64_t>::max())
//      || (v.type() == variant_type::unsigned_integer && v.get<std::uint64_t>() <= std::numeric_limits<std::int64_t>::max());
//  }


//  double sub_form::convert_to_double(const variant& v)
//  {
//    double ret = 0;
//
//    if (v.is<std::int8_t>())    ret = (double)(v.get<std::int8_t>());
//    if (v.is<std::int16_t>())   ret = (double)(v.get<std::int16_t>());
//    if (v.is<std::int32_t>())   ret = (double)(v.get<std::int32_t>());
//    if (v.is<std::int64_t>())   ret = (double)(v.get<std::int64_t>());
//    if (v.is<std::uint8_t>())   ret = (double)(v.get<std::uint8_t>());
//    if (v.is<std::uint16_t>())  ret = (double)(v.get<std::uint16_t>());
//    if (v.is<std::uint32_t>())  ret = (double)(v.get<std::uint32_t>());
//    if (v.is<std::uint64_t>())  ret = (double)(v.get<std::uint64_t>());
//    if (v.is<float>())          ret = (double)(v.get<float>());
//    if (v.is<double>())         ret = v.get<double>();
//
//    return ret;
//  }

  number_validator<std::int8_t> sub_form::int8()
  {
    std::int8_t val;
    if (!this->variant_.get(val))
      this->error_ = error_message("NOT A int8_t");
    return number_validator<std::int8_t>(val, this->error_);
  }

  number_validator<std::int8_t> sub_form::int8(std::int8_t default_value)
  {
    std::int8_t val;
    if (this->variant_.get(val))
      return number_validator<std::int8_t>(val, this->error_);
    else
      return number_validator<std::int8_t>(default_value, this->error_);
  }

  number_validator<std::int16_t> sub_form::int16()
  {
    std::int16_t val;
    if (!this->variant_.get(val))
      this->error_ = error_message("NOT A int16_t");
    return number_validator<std::int16_t>(val, this->error_);
  }

  number_validator<std::int16_t> sub_form::int16(std::int16_t default_value)
  {
    std::int16_t val;
    if (this->variant_.get(val))
      return number_validator<std::int16_t>(val, this->error_);
    else
      return number_validator<std::int16_t>(default_value, this->error_);
  }

  number_validator<std::int32_t> sub_form::int32()
  {
    std::int32_t val;
    if (!this->variant_.get(val))
      this->error_ = error_message("NOT A int32_t");
    return number_validator<std::int32_t>(val, this->error_);
  }

  number_validator<std::int32_t> sub_form::int32(std::int32_t default_value)
  {
    std::int8_t val;
    if (this->variant_.get(val))
      return number_validator<std::int32_t>(val, this->error_);
    else
      return number_validator<std::int32_t>(default_value, this->error_);
  }

  number_validator<std::int64_t> sub_form::int64()
  {
    std::int64_t val;
    if (!this->variant_.get(val))
      this->error_ = error_message("NOT A int64_t");
    return number_validator<std::int64_t>(val, this->error_);
  }

  number_validator<std::int64_t> sub_form::int64(std::int64_t default_value)
  {
    std::int64_t val;
    if (this->variant_.get(val))
      return number_validator<std::int64_t>(val, this->error_);
    else
      return number_validator<std::int64_t>(default_value, this->error_);
  }

  number_validator<std::uint8_t> sub_form::uint8()
  {
    std::uint8_t val;
    if (!this->variant_.get(val))
      this->error_ = error_message("NOT A uint8_t");
    return number_validator<std::uint8_t>(val, this->error_);
  }

  number_validator<std::uint8_t> sub_form::uint8(std::uint8_t default_value)
  {
    std::uint8_t val;
    if (this->variant_.get(val))
      return number_validator<std::uint8_t>(val, this->error_);
    else
      return number_validator<std::uint8_t>(default_value, this->error_);
  }

  number_validator<std::uint16_t> sub_form::uint16()
  {
    std::uint16_t val;
    if (!this->variant_.get(val))
      this->error_ = error_message("NOT A uint16_t");
    return number_validator<std::uint16_t>(val, this->error_);
  }

  number_validator<std::uint16_t> sub_form::uint16(std::uint16_t default_value)
  {
    std::uint16_t val;
    if (this->variant_.get(val))
      return number_validator<std::uint16_t>(val, this->error_);
    else
      return number_validator<std::uint16_t>(default_value, this->error_);
  }

  number_validator<std::uint32_t> sub_form::uint32()
  {
    std::uint32_t val;
    if (!this->variant_.get(val))
      this->error_ = error_message("NOT A uint32_t");
    return number_validator<std::uint32_t>(val, this->error_);
  }

  number_validator<std::uint32_t> sub_form::uint32(std::uint32_t default_value)
  {
    std::uint32_t val;
    if (this->variant_.get(val))
      return number_validator<std::uint32_t>(val, this->error_);
    else
      return number_validator<std::uint32_t>(default_value, this->error_);
  }

  number_validator<std::uint64_t> sub_form::uint64()
  {
    std::uint64_t val;
    if (!this->variant_.get(val))
      this->error_ = error_message("NOT A uint64_t");
    return number_validator<std::uint64_t>(val, this->error_);
  }

  number_validator<std::uint64_t> sub_form::uint64(std::uint64_t default_value)
  {
    std::uint64_t val;
    if (this->variant_.get(val))
      return number_validator<std::uint64_t>(val, this->error_);
    else
      return number_validator<std::uint64_t>(default_value, this->error_);
  }

  number_validator<float> sub_form::float32()
  {
    float val;
    if (!this->variant_.get(val))
      this->error_ = error_message("NOT A float32");
    return number_validator<float>(val, this->error_);
  }

  number_validator<float> sub_form::float32(float default_value)
  {
    float val;
    if (this->variant_.get(val))
      return number_validator<float>(val, this->error_);
    else
      return number_validator<float>(default_value, this->error_);
  }

  number_validator<double> sub_form::float64()
  {
    double val;
    if (!this->variant_.get(val))
      this->error_ = error_message("NOT A float64");
    return number_validator<double>(val, this->error_);
  }

  number_validator<double> sub_form::float64(double default_value)
  {
    double val;
    if (this->variant_.get(val))
      return number_validator<double>(val, this->error_);
    else
      return number_validator<double>(default_value, this->error_);
  }

//  number_validator<std::int64_t> sub_form::integer()
//  {
//    return this->integer(error_message("NOT A NUMBER"));
//  }
//
//  number_validator<std::int64_t> sub_form::integer(const error_message& customerror_message)
//  {
//    if (!this->variant_.isNumber())
//      this->error_ = customerror_message;
//    return this->int64Types_.back();
//  }
//
//  number_validator<std::int64_t> sub_form::integer(std::int64_t default_value)
//  {
//    if (!this->variant_.isNumber())
//      this->int64Types_.push_back(number_validator<int64_t>(default_value, this->error_));
//    else
//      this->int64Types_.push_back(number_validator<int64_t>(this->variant_.integer(), this->error_));
//    return this->int64Types_.back();
//  }
//
//  number_validator<std::uint64_t> sub_form::unsignedInteger()
//  {
//    return this->unsignedInteger(error_message("NOT A NUMBER"));
//  }
//
//  number_validator<std::uint64_t> sub_form::unsignedInteger(const error_message& customerror_message)
//  {
//    this->uint64Types_.push_back(number_validator<uint64_t>(this->variant_.unsignedInteger(), this->error_));
//    if (!this->variant_.isNumber() || this->variant_.isSignedNumber())
//      this->error_ = customerror_message;
//    return this->uint64Types_.back();
//  }
//
//  number_validator<std::uint64_t> sub_form::unsignedInteger(std::uint64_t default_value)
//  {
//    if (!this->variant_.isNumber() || this->variant_.isSignedNumber())
//      this->uint64Types_.push_back(number_validator<uint64_t>(default_value, this->error_));
//    else
//      this->uint64Types_.push_back(number_validator<uint64_t>(this->variant_.unsignedInteger(), this->error_));
//    return this->uint64Types_.back();
//  }
//
//  number_validator<double> sub_form::floatingPoint()
//  {
//    return this->floatingPoint(error_message("NOT A NUMBER"));
//  }
//
//  number_validator<double> sub_form::floatingPoint(const error_message& customerror_message)
//  {
//    this->realTypes_.push_back(number_validator<double>(this->variant_.floatingPoint(), this->error_));
//    if (!this->variant_.isNumber())
//      this->error_ = customerror_message;
//    return this->realTypes_.back();
//  }
//
//  number_validator<double> sub_form::floatingPoint(double default_value)
//  {
//    if (!this->variant_.isNumber())
//      this->realTypes_.push_back(number_validator<double>(default_value, this->error_));
//    else
//      this->realTypes_.push_back(number_validator<double>(this->variant_.floatingPoint(), this->error_));
//    return this->realTypes_.back();
//  }

  string_validator sub_form::string()
  {
    if (!this->variant_.is<std::string>())
      this->error_ = error_message("TYPE NOT STRING");

    string_validator ret(this->variant_.get<std::string>(), this->error_);
    return ret;
  }

  string_validator sub_form::string(const std::string& default_value)
  {
    string_validator ret(this->variant_.is<std::string>() ? this->variant_.get<std::string>() : default_value, this->error_);
    return ret;
  }

  array_validator sub_form::array()
  {
    array_validator ret(this->variant_.get<goodform::array>(), this->error_);
    if (!this->variant_.is<goodform::array>())
      this->error_ = error_message("NOT AN ARRAY");
    return ret;
  }

  array_validator sub_form::array(const std::vector<variant>& default_value)
  {
    array_validator ret(this->variant_.is<goodform::array>() ? this->variant_.get<goodform::array>() : default_value, this->error_);
    return ret;
  }

  object_validator sub_form::object()
  {
    object_validator ret(this->variant_.get<goodform::object>(), this->error_);
    if (!this->variant_.is<goodform::object>())
      this->error_ = error_message("NOT AN OBJECT");
    return ret;
  }

  object_validator sub_form::object(const std::map<std::string,variant>& default_value)
  {
    object_validator ret(this->variant_.is<goodform::object>() ? this->variant_.get<goodform::object>() : default_value, this->error_);
    return ret;
  }

  sub_form sub_form::at(size_t index)
  {
    return this->array().at(index);
  }

  sub_form sub_form::at(size_t index, const variant& default_variant)
  {
    return this->array(std::vector<variant>()).at(index, default_variant);
  }

  void sub_form::for_each(const std::function<void(sub_form& element, size_t index)>& fn)
  {
    this->array().for_each(fn);
  }

  sub_form sub_form::at(const std::string& key)
  {
    return this->object().at(key);
  }

  sub_form sub_form::at(const std::string& key, const variant& default_variant)
  {
    return this->object(std::map<std::string,variant>()).at(key, default_variant);
  }
  //======================================================================//

  //======================================================================//
  form::form(const variant& v) : error_(""), sub_form(v, error_)
  {

  }

  bool form::is_good() const
  {
    return this->error_.empty();
  }
  //======================================================================//
}
//######################################################################//
#endif //GOODFORM_FORM_HPP