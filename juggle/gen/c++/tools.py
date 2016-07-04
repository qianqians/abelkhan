# 2016-7-1
# build by qianqians
# tools

def gentypetocpp(typestr):
    if typestr == 'int':
        return 'int64_t'
    elif typestr == 'string':
        return 'std::string'
    elif typestr == 'array':
        return 'std::vector<boost::any>'
    elif typestr == 'float':
        return 'double'
    elif typestr == 'bool':
        return 'bool'
    elif typestr == 'table':
        return 'std::unordered_map<std::string, boost::any>'

	
