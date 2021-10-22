libbson
=======

A library for converting from and to BSON. Also contains a JSON parser/generator based on rapidjson.

It is small in size, the shared object has less than 64KB.

There are no dependencies.

Its only export is a single class: BSON::Value. This class can hold any value you want to express in bson.
You can assign and reference its content either by operator= or by the index operator.

A typical example would be this:


    BSON::Value doc = BSON::Object{
    	{"undefined",BSON::Value{}},
    	{"int32",(BSON::int32)1},
    	{"int64",(BSON::int64)1},
    	{"double",3.14},
    	{"true",true},
    	{"false",false},
    	{"string","foobar"},
    	{"datetime",std::chrono::milliseconds{123}},
    	{"object",BSON::Object{{"foo","bar"}}},
    	{"array",BSON::Array{1,2,3}}
    }; 

    doc["double"] = 42.23;

    for(auto it : doc){
        std::cout<<it.first<<std::endl;
    }

    std::string str = doc.toBSON();
    BSON::Value reDoc = BSON::Value::fromBSON(str);

    std::cout<<doc.toJSON()<<std::endl;
    std::cout<<reDoc.toJSON()<<std::endl;

It is not complete, i.e. it doesnt contain all the MongoDB specific stuff (like datatypes for ObjectID, Timestamp, or js code)

Beside that it has a very intuitive interface for document building.
